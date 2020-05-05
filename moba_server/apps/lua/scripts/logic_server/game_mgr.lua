local Response = require("Response")
local Stype = require("Stype")
local Cmd = require("Cmd")
local mysql_game = require("database/mysql_game")
local redis_game = require("database/redis_game")
local mysql_center = require("database/mysql_auth_center")
local redis_center = require("database/redis_center")

local player = require("logic_server/player")
local robot_player = require("logic_server/robot_player")
local Zone = require("logic_server/Zone")
local State = require("logic_server/State")
local match_mgr = require("logic_server/match_mgr")

-- uid --> player
local logic_server_players = {}
local online_player_num = 0
local zone_wait_list = {} -- zone_wait_list[Zone.SGYD] = {} --> uid --> p;
local zone_match_list = {} -- 当前开的比赛的列表
zone_match_list[Zone.SGYD] = {}
zone_match_list[Zone.ASSY] = {}
local zone_robot_list = {} -- 存放我们当前所在区间的机器人
zone_robot_list[Zone.SGYD] = {}
zone_robot_list[Zone.ASSY] = {}
--end

function do_new_robot_players(robots)
	if #robots <= 0 then 
		return
	end

	local half_len = #robots
	local i = 1
	half_len = math.floor(half_len * 0.5)

	-- 前半部分放一个分区
	for i = 1, half_len do 
		local v = robots[i] 
		local r_player = robot_player:new()
		r_player:init(v.uid, nil, nil)
		r_player.zid = Zone.SGYD
		zone_robot_list[Zone.SGYD][v.uid] = r_player
	end

	-- 下半部分放一个分区
	for i = half_len + 1, #robots do 
		local v = robots[i] 
		local r_player = robot_player:new()
		r_player:init(v.uid, nil, nil)
		r_player.zid = Zone.ASSY
		zone_robot_list[Zone.ASSY][v.uid] = r_player
	end
end

function do_load_robot_uinfo(now_index, robots)
	mysql_center.get_uinfo_by_uid(robots[now_index].uid, function (err, uinfo)
		if err or not uinfo then
			return 
		end

		redis_center.set_uinfo_inredis(robots[now_index].uid, uinfo)
		-- Logger.debug("uid " .. robots[now_index].uid .. " load to center reids!!!")
		now_index = now_index + 1
		if now_index > #robots then
			do_new_robot_players(robots)
			return 
		end


		do_load_robot_uinfo(now_index, robots)
	end)
end

function do_load_robot_ugame_info()
	mysql_game.get_robots_ugame_info(function(err, ret) 
		if err then
			return 
		end

		if not ret or #ret <= 0 then 
			return
		end

		local k, v
		for k, v in pairs(ret) do
			redis_game.set_ugame_info_inredis(v.uid, v)
		end

		do_load_robot_uinfo(1, ret)
	end) 
end


function load_robots()
	if not mysql_game.is_connected() or 
	   not mysql_center.is_connected() or 
	   not redis_center.is_connected() or 
	   not redis_game.is_connected() then 
	   Scheduler.once(load_robots, 5000)
	   return
	end

	do_load_robot_ugame_info()
end

Scheduler.once(load_robots, 5000)


function send_status(s, stype, ctype, uid, status) 
	local msg = {stype, ctype, uid, {
		status = status,
	}}

	Session.send_msg(s, msg)
end

function search_inview_match_mgr(zid)

	local match_list = zone_match_list[zid]
	
	for k, v in pairs(match_list) do 
		if v.state == State.InView then 
			return v
		end
	end


	local match = match_mgr:new()
	--table.insert(match_list, match)
	match:init(zid)
    match_list[match.matchid]=match;

	return match
end

function do_match_players()

	local zid, wait_list
	for zid, wait_list in pairs(zone_wait_list) do 
		local k, v
		for k, v in pairs(wait_list) do 
			local match = search_inview_match_mgr(zid)
			if match then
				 if not match:enter_player(v) then 
				 	Logger.error("match system error : player state: ", v.state)
                 else
                    --print("select zid",zid)
				 	wait_list[k] = nil
				 end
			end
		end
	end
end

Scheduler.schedule(do_match_players, 1000, -1, 5000)

function search_idle_robot(zid)
	local robots = zone_robot_list[zid]
	local k, v 
	for k, v in pairs(robots) do 
		if v.matchid == -1 then
			return v 
		end
	end

	return nil
end

-- function do_exit_robot( match,robot )
-- 	Scheduler.once(function (  )
-- 		match:exit_player(robot);
-- 	end,5000)
-- end

function do_push_robot_to_match()
	local zid, match_list
	local k, match  
	for zid, match_list in pairs(zone_match_list) do 
		for k, match in pairs(match_list) do
			if match.state == State.InView then  -- 找到了一个空闲的match
				local robot = search_idle_robot(zid)
				if robot then
					Logger.debug("[".. robot.uid .."]" .. " enter match!")
					match:enter_player(robot) 

					--test
					--do_exit_robot(match,robot);
				end
			end 
		end
	end
end
Scheduler.schedule(do_push_robot_to_match, 1000, -1, 1000)

-- {stype, ctype, utag, body}
function login_logic_server(s, req)
	local uid = req[3]
	local stype = req[1]

	local p = logic_server_players[uid] -- player对象
	if p then -- 玩家对象已经存在了，更新一下session就可以了; 
		p:set_session(s)
		send_status(s, stype, Cmd.eLoginLogicRes, uid, Response.OK)
		return
	end

	p = player:new()
	p:init(uid, s, function(status)
		if status == Response.OK then
			logic_server_players[uid] = p
			online_player_num = online_player_num + 1
		end
		send_status(s, stype, Cmd.eLoginLogicRes, uid, status)
	end)
end

-- 玩家离开了
function on_player_disconnect(s, req)
	local uid = req[3]
	local p = logic_server_players[uid]
	if not p then
		return 
	end

	-- 游戏中的玩家我们后续考虑
	if p.zid ~= -1 then
		-- 玩家在等待列表里面
		if zone_wait_list[p.zid][p.uid] then
			zone_wait_list[p.zid][p.uid] = nil
			p.zid = -1
			print("remove from wait list")
		end
		--end 
	end
	-- end 

	-- 玩家断线离开
	if p then
		print("player uid " .. uid .. " disconnect!")
		logic_server_players[uid] = nil
		online_player_num = online_player_num - 1
	end
	-- end
end

function on_gateway_connect(s)
	local k, v

	for k, v in pairs(logic_server_players) do 
		v:set_session(s)
	end
end

function on_gateway_disconnect(s) 
	local k, v

	for k, v in pairs(logic_server_players) do 
		v:set_session(nil)
	end
end

-- {stype, ctype, utag, body}
function  enter_zone(s, req)
	local stype = req[1]
	local uid = req[3]

	local p = logic_server_players[uid]
	if not p or p.zid ~= -1 then
		send_status(s, stype, Cmd.eEnterZoneRes, uid, Response.InvalidOpt)
		return
	end


	local zid = req[4].zid;
	if zid ~= Zone.SGYD and zid ~= Zone.ASSY then
		send_status(s, stype, Cmd.eEnterZoneRes, uid, Response.InvalidParams)
		return
	end

	if not zone_wait_list[zid] then 
		zone_wait_list[zid] = {}
	end

	zone_wait_list[zid][uid] = p
	p.zid = zid
	send_status(s, stype, Cmd.eEnterZoneRes, uid, Response.OK)
end

function do_exit_match( s,req )
    local uid = req[3]
	local p = logic_server_players[uid]
    if not p then
        send_status(req[1],Cmd.eExitMatchRes,uid,Response.InvalidOpt);
		return 
    end
    
	if p.state ~= State.InView or 
		p.zid ==-1 or 
		p.matchid == -1 or 
		p.seatid == -1 then
        send_status(req[1],Cmd.eExitMatchRes,uid,Response.InvalidOpt);
        return
    end

    local match=zone_match_list[p.zid][p.matchid];
    if not match or match.state ~= State.InView then
        send_status(req[1],Cmd.eExitMatchRes,uid,Response.InvalidOpt);
        return
    end

    match:exit_player(p);

end

function do_udp_test( s,req )
	local stype=req[1];
	local ctype=req[2];
	local body=req[4];

	print(body.content);
	local msg = {stype, ctype, 0, {
		content = body.content,
	}}

	Session.send_msg(s, msg)
end


local game_mgr = {
	login_logic_server = login_logic_server,
	on_player_disconnect = on_player_disconnect,
	on_gateway_disconnect = on_gateway_disconnect,
	on_gateway_connect = on_gateway_connect,

    enter_zone = enter_zone,
	do_exit_match=do_exit_match,
	do_udp_test=do_udp_test,
}

return game_mgr

