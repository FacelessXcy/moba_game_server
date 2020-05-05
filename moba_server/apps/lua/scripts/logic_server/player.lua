local Response = require("Response")
local Stype = require("Stype")
local Cmd = require("Cmd")
local mysql_game = require("database/mysql_game")
local redis_game = require("database/redis_game")
local redis_center = require("database/redis_center")
local State = require("logic_server/State")
local Zone = require("logic_server/Zone")

local player = {}

function player:new(instant) 
	if not instant then 
		instant = {} --类的实例
	end

	setmetatable(instant, {__index = self}) 
	return instant
end


function player:init(uid, s, ret_handler)
	self.session = s
	self.uid = uid
	self.zid = -1 -- 玩家所在的空间, -1,不在任何游戏场
	self.matchid = -1 -- 玩家所在的比赛房间的id
	self.seatid = -1 -- 玩家在比赛中的序列号
	self.side = -1 -- 玩家在游戏里面所在的边, 0(lhs), 1(rhs) 
	self.heroid = -1 -- 玩家的英雄号 [1, 5]
	self.state = State.InView -- 玩家当前处于旁观状态
	self.is_robot = false -- 玩家是否为机器人

	self.client_ip = nil -- 玩家对应客户端的 udp的ip地址
    self.client_udp_port = 0 -- 玩家对应的客户端udp 的port
    self.sync_frameid=0;--玩家当前同步到哪一帧

	-- 数据库理面读取玩家的基本信息;
	mysql_game.get_ugame_info(uid, function (err, ugame_info)
		if err then
			if ret_handler then 
				ret_handler(Response.SystemErr) 
			end
			return
		end
		-- self可能会有隐患，后续验证
		self.ugame_info = ugame_info

		redis_center.get_uinfo_inredis(uid, function (err, uinfo)
			if err then 
				if ret_handler then 
					ret_handler(Response.SystemErr) 
				end
				return
			end

			self.uinfo = uinfo
			if ret_handler then 
				ret_handler(Response.OK) 
			end
		end)
		
	end)
	-- end

	-- ...其他信息后面再读
	-- end 
end

function player:get_user_arrived()
	local body = {
		unick = self.uinfo.unick,
		uface = self.uinfo.uface,
		usex = self.uinfo.usex,
		seatid = self.seatid,
		side = self.side,
	}

	--print(body.unick .. " " .. body.uface .. body.usex)
	return body
end

function player:set_udp_addr(ip, port)
	self.client_ip = ip
	self.client_udp_port = port
end

function player:set_session(s)
	self.session = s
end

function player:send_cmd(stype, ctype, body)
	if not self.session or self.is_robot then 
		return
	end

	local msg = {stype, ctype, self.uid, body}
	Session.send_msg(self.session, msg)
end

function player:udp_send_cmd(stype, ctype, body) 
	if not self.session or self.is_robot then --玩家已经断线或是机器人
		return
	end

	if not self.client_ip or self.client_udp_port == 0 then 
		return
	end
	local msg = {stype, ctype, 0, body}
	Session.udp_send_msg(self.client_ip, self.client_udp_port, msg)
end

return player

