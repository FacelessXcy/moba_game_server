local Response = require("Response")
local Stype = require("Stype")
local Cmd = require("Cmd")
local mysql_game = require("database/mysql_game")
local redis_game = require("database/redis_game")
local State = require("logic_server/State")
local Zone = require("logic_server/Zone")
local player = require("logic_server/player")

local match_mgr = {}
local sg_matchid = 1
local PLAYER_NUM = 2 -- 3v3

function match_mgr:new(instant) 
	if not instant then 
		instant = {} --类的实例
	end

	setmetatable(instant, {__index = self}) 
	return instant
end

function match_mgr:init(zid)
	self.zid = zid
	self.matchid = sg_matchid
	sg_matchid = sg_matchid + 1
	self.state = State.InView
	self.frameid = 0

	self.inview_players = {} -- 旁观玩家的列表
	self.lhs_players = {} -- 左右两边的玩家
	self.rhs_players = {} -- 左右两边的玩家

	self.match_frames = {} -- 保存的是游戏开始依赖所有的帧操作;
	self.next_frame_opt = {} -- 当前的帧操作;
end

function match_mgr:broadcast_cmd_inview_players(stype, ctype, body, not_to_player)
	local k, v

	for k, v in pairs(self.inview_players) do 
		if v ~= not_to_player then 
			v:send_cmd(stype, ctype, body)
		end
	end
end

function match_mgr:exit_player(p)
	local body = {
		seatid = p.seatid
	}
	self:broadcast_cmd_inview_players(Stype.Logic, Cmd.eUserExitMatch, body, p)

	self.inview_players[p.seatid] = nil -- 等待列表里面来移除掉我们的player
	p.zid = -1
	p.matchid = -1
	p.seatid = -1
	p.side = -1

	body = {status = Response.OK}
	p:send_cmd(Stype.Logic, Cmd.eExitMatchRes, body)
end

function match_mgr:game_start()
	local players_match_info = {}
	for i = 1, PLAYER_NUM * 2 do
		local p = self.inview_players[i]
		local info = {
			heroid = p.heroid,
			seatid = p.seatid,
			side = p.side,
		}
		table.insert(players_match_info, info)
	end

	local body = {
		players_match_info = players_match_info,
	}
	self:broadcast_cmd_inview_players(Stype.Logic, Cmd.eGameStart, body, nil)

	self.state = State.Playing
	self:update_players_state(State.Playing)

	-- 5秒以后 开始第一个帧事件, 1000 --> 20 FPS ---> 50
	self.frameid = 1
	self.match_frames = {} -- 保存的是游戏开始依赖所有的帧操作;
	self.next_frame_opt = {frameid = self.frameid, opts = {}} -- 当前的帧玩家操作;

	self.frame_timer = Scheduler.schedule(function() 
		self:on_logic_frame()
	end, 5000, -1, 50)
	-- end
end

function match_mgr:on_next_frame_event(next_frame_opts)
	local seatid = next_frame_opts.seatid
	--print(seatid, next_frame_opts.frameid, #next_frame_opts.opts)

	local p = self.inview_players[seatid]
	if not p then 
		return
	end


	-- 当前客户端已经同步了哪个
	if p.sync_frameid < next_frame_opts.frameid - 1 then
		p.sync_frameid = next_frame_opts.frameid - 1
	end

	
	-- end
	if (next_frame_opts.frameid ~= self.frameid) then
		return
	end


	for i = 1, #next_frame_opts.opts do 
		table.insert(self.next_frame_opt.opts, next_frame_opts.opts[i])
	end

end

function match_mgr:send_unsync_frames(p)
	local opt_frams = {}
	--print("^^^^", p.sync_frameid, #self.match_frames)

	for i = (p.sync_frameid + 1), #self.match_frames do 
		table.insert(opt_frams, self.match_frames[i])
	end


	local body = { frameid = self.frameid,  unsync_frames = opt_frams}
	p:udp_send_cmd(Stype.Logic, Cmd.eLogicFrame, body)
end

function match_mgr:on_logic_frame()
	table.insert(self.match_frames, self.next_frame_opt)

	for i = 1, PLAYER_NUM * 2 do 
		local p = self.inview_players[i]
		if p then
			self:send_unsync_frames(p)
		end 
	end

	self.frameid = self.frameid + 1
	self.next_frame_opt = {frameid = self.frameid, opts = {}} -- 当前的帧玩家操作;
end

function match_mgr:update_players_state(state)
	for i = 1, PLAYER_NUM * 2 do 
		self.inview_players[i].state = state
	end
end

function match_mgr:enter_player(p)
	local i
	if self.state ~= State.InView or p.state ~= State.InView then 
		return false
	end

	p.matchid = self.matchid
	p.sync_frameid = 0
	--将玩家加入到集结列表里面，
	for i = 1, PLAYER_NUM * 2 do
		if not self.inview_players[i] then 
			self.inview_players[i] = p
			p.seatid = i
			p.side = 0
			if i > PLAYER_NUM then 
				p.side = 1
			end
			break
		end 
	end
	-- end

	-- 发送命令，告诉客户端，你进入了一个比赛, zid, matchid
	local body = { 
		zid = self.zid,
		matchid = self.matchid,
		seatid = p.seatid,
		side = p.side,
	}
	p:send_cmd(Stype.Logic, Cmd.eEnterMatch, body)

	-- 将用户进来的消息发送给房间里面的其他玩家
	body = p:get_user_arrived()
	self:broadcast_cmd_inview_players(Stype.Logic, Cmd.eUserArrived, body, p)
	-- end

	-- 玩家还有收到 其他在我们这个等待列表里面的玩家
	-- for i = 1, #self.inview_players do 
	for i = 1, PLAYER_NUM * 2 do
		if self.inview_players[i] and self.inview_players[i] ~= p then 
			body = self.inview_players[i]:get_user_arrived()
			p:send_cmd(Stype.Logic, Cmd.eUserArrived, body)
		end
	end
	-- end 

	-- 判断我们当前是否集结玩家结束了
	if #self.inview_players >= PLAYER_NUM * 2 then 
		self.state = State.Ready
		self:update_players_state(State.Ready)
		-- 开始游戏数据发送给客户端
		-- 进入到一个选英雄的这个界面，知道所有的玩家选好英雄这样一个状态;
		-- 在游戏主页里面，自己设置你用的英雄，然后你自己再用大厅那里设置的英雄;
		-- 服务器随机生成英雄的id[1, 5];
		for i = 1, PLAYER_NUM * 2 do 
			self.inview_players[i].heroid = math.floor(math.random() * 5 + 1) -- [1, 5] 
		end
		-- end
		
		self:game_start()
		-- end 
	end

	return true
end

return match_mgr
