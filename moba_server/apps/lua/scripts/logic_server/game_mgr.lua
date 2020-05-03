local Response=require("Response");
local Stype=require("Stype");
local Cmd=require("Cmd");
local mysql_game=require("database/mysql_game");
local redis_game=require("database/redis_game");
local player=require("logic_server/player");
local Zone=require("logic_server/Zone");
local match_mgr=require("logic_server/match_mgr");
local State=require("logic_server/State")

--uid player 映射表
local logic_server_players={};
local online_player_num=0;
local zone_wait_list={};--zone_wait_list[Zone.SGYD]={}-->uid-->p
local zone_match_list={}--比赛列表
zone_match_list[Zone.SGYD]={};
zone_match_list[Zone.ASSY]={};

function send_status(s,stype,ctype,uid,status)
    local msg={stype,ctype,uid,{
        status=status,
    }};
    Session.send_msg(s,msg);
end

function search_inview_match_mgr( zid )
    local match_list=zone_match_list[zid];

    --查找一个空闲房间
    for k,v in pairs(match_list) do 
        if v.state == State.InView then
            return v;
        end
    end

    local match=match_mgr:new();
    table.insert(match_list,match);
    match:init(zid);

    return match
end

function do_match_players(  )
    local zid,wait_list;
    for zid,wait_list in pairs(zone_wait_list) do
        local k,v;
        for k,v in pairs(wait_list) do
            local match=search_inview_match_mgr(zid);
            if match then
                if not match:enter_player(v) then
                    Logger.error("match system error: player state ",v.state);
                else
                    wait_list[k]=nil;--将玩家从等待列表中移出
                end
            end
        end
    end
end

Scheduler.schedule(do_match_players,1000,-1,5000);


function login_logic_server( s,req )
    local uid=req[3];
    local stype=req[1];

    local p=logic_server_players[uid];--玩家player对象
    if p then--玩家对象已经存在，更新session即可
        p:set_session(s);
        send_status(s,stype,Cmd.eLoginLogicRes,uid,Response.OK);
        return;
    end

    p=player:new();
    p:init(uid,s,
    function ( status )
        if status == Response.OK then
            logic_server_players[uid]=p;
            online_player_num=online_player_num+1;
        end
        send_status(s,stype,Cmd.eLoginLogicRes,uid,status);
    end);
end

--玩家断线
function on_player_disconnect( s,req )
    local uid=req[3];
    local p = logic_server_players[uid];
    if not p then
        return
    end
    --游戏中的玩家掉线后面考虑
    if p.zid ~= -1 then
        --玩家在等待列表中,将玩家移出等待列表
        if zone_wait_list[p.zid][p.uid] then
            zone_wait_list[p.zid][p.uid]=nil;
            p.zid=-1;
            print("remove from wait list")
        end
    end 

    --玩家断线离开
    if p then
        print("player uid "..uid.." disconnect.");
        logic_server_players[uid]=nil;
        online_player_num=online_player_num-1;
    end 

end

function on_gateway_connect( s )
    local k,v;
    for k,v in pairs(logic_server_players) do
        v:set_session(s);
    end
end

function on_gateway_disconnect(s)
    local k,v;
    for k,v in pairs(logic_server_players) do
        v:set_session(nil);
    end
end


function enter_zone( s,req )
    local stype=req[1];
    local uid=req[3];

    local p=logic_server_players[uid];
    if not p or p.zid ~= -1 then 
        send_status(s,stype,Cmd.eEnterZoneRes,uid,Response.InvalidOpt);
        return;
    end 

    local zid=req[4].zid;
    if zid ~= Zone.SGYD and zid ~= Zone.ASSY then
        send_status(s,stype,Cmd.eEnterZoneRes,uid,Response.InvalidParams);
        return;
    end

    if not zone_wait_list[zid] then
        zone_wait_list[zid]={};
    end

    zone_wait_list[zid][uid]=p;
    p.zid=zid;
    send_status(s,stype,Cmd.eEnterZoneRes,uid,Response.OK);

end


local game_mgr={
    login_logic_server=login_logic_server,
    on_player_disconnect=on_player_disconnect,
    on_gateway_disconnect=on_gateway_disconnect,
    on_gateway_connect=on_gateway_connect,

    enter_zone=enter_zone,
}

return game_mgr;
-- local Response = require("Response")
-- local Stype = require("Stype")
-- local Cmd = require("Cmd")
-- local mysql_game = require("database/mysql_game")
-- local redis_game = require("database/redis_game")
-- local player = require("logic_server/player")
-- local Zone = require("logic_server/Zone")
-- local State = require("logic_server/State")
-- local match_mgr = require("logic_server/match_mgr")

-- -- uid --> player
-- local logic_server_players = {}
-- local online_player_num = 0
-- local zone_wait_list = {} -- zone_wait_list[Zone.SGYD] = {} --> uid --> p;
-- local zone_match_list = {} -- 当前开的比赛的列表
-- zone_match_list[Zone.SGYD] = {}
-- zone_match_list[Zone.ASSY] = {}
-- --end

-- function send_status(s, stype, ctype, uid, status) 
-- 	local msg = {stype, ctype, uid, {
-- 		status = status,
-- 	}}

-- 	Session.send_msg(s, msg)
-- end

-- function search_inview_match_mgr(zid)

-- 	local match_list = zone_match_list[zid]
	
-- 	for k, v in pairs(match_list) do 
-- 		if v.state == State.InView then 
-- 			return v
-- 		end
-- 	end


-- 	local match = match_mgr:new()
-- 	table.insert(match_list, match)
-- 	match:init(zid)

-- 	return match
-- end

-- function do_match_players()

-- 	local zid, wait_list
-- 	for zid, wait_list in pairs(zone_wait_list) do 
-- 		local k, v
-- 		for k, v in pairs(wait_list) do 
-- 			local match = search_inview_match_mgr(zid)
-- 			if match then
-- 				 if not match:enter_player(v) then 
-- 				 	Logger.error("match system error : player state: ", v.state)
-- 				 else
-- 				 	wait_list[k] = nil
-- 				 end
-- 			end
-- 		end
-- 	end
-- end

-- Scheduler.schedule(do_match_players, 1000, -1, 5000)

-- -- {stype, ctype, utag, body}
-- function login_logic_server(s, req)
-- 	local uid = req[3]
-- 	local stype = req[1]

-- 	local p = logic_server_players[uid] -- player对象
-- 	if p then -- 玩家对象已经存在了，更新一下session就可以了; 
-- 		p:set_session(s)
-- 		send_status(s, stype, Cmd.eLoginLogicRes, uid, Response.OK)
-- 		return
-- 	end

-- 	p = player:new()
-- 	p:init(uid, s, function(status)
-- 		if status == Response.OK then
-- 			logic_server_players[uid] = p
-- 			online_player_num = online_player_num + 1
-- 		end
-- 		send_status(s, stype, Cmd.eLoginLogicRes, uid, status)
-- 	end)
-- end

-- -- 玩家离开了
-- function on_player_disconnect(s, req)
-- 	local uid = req[3]
-- 	local p = logic_server_players[uid]
-- 	if not p then
-- 		return 
-- 	end

-- 	-- 游戏中的玩家我们后续考虑
-- 	if p.zid ~= -1 then
-- 		-- 玩家在等待列表里面
-- 		if zone_wait_list[p.zid][p.uid] then
-- 			zone_wait_list[p.zid][p.uid] = nil
-- 			p.zid = -1
-- 			print("remove from wait list")
-- 		end
-- 		--end 
-- 	end
-- 	-- end 

-- 	-- 玩家断线离开
-- 	if p then
-- 		print("player uid " .. uid .. " disconnect!")
-- 		logic_server_players[uid] = nil
-- 		online_player_num = online_player_num - 1
-- 	end
-- 	-- end
-- end

-- function on_gateway_connect(s)
-- 	local k, v

-- 	for k, v in pairs(logic_server_players) do 
-- 		v:set_session(s)
-- 	end
-- end

-- function on_gateway_disconnect(s) 
-- 	local k, v

-- 	for k, v in pairs(logic_server_players) do 
-- 		v:set_session(nil)
-- 	end
-- end

-- -- {stype, ctype, utag, body}
-- function  enter_zone(s, req)
-- 	local stype = req[1]
-- 	local uid = req[3]

-- 	local p = logic_server_players[uid]
-- 	if not p or p.zid ~= -1 then
-- 		send_status(s, stype, Cmd.eEnterZoneRes, uid, Response.InvalidOpt)
-- 		return
-- 	end


-- 	local zid = req[4].zid;
-- 	if zid ~= Zone.SGYD and zid ~= Zone.ASSY then
-- 		send_status(s, stype, Cmd.eEnterZoneRes, uid, Response.InvalidParams)
-- 		return
-- 	end

-- 	if not zone_wait_list[zid] then 
-- 		zone_wait_list[zid] = {}
-- 	end

-- 	zone_wait_list[zid][uid] = p
-- 	p.zid = zid
-- 	send_status(s, stype, Cmd.eEnterZoneRes, uid, Response.OK)
-- end

-- local game_mgr = {
-- 	login_logic_server = login_logic_server,
-- 	on_player_disconnect = on_player_disconnect,
-- 	on_gateway_disconnect = on_gateway_disconnect,
-- 	on_gateway_connect = on_gateway_connect,

-- 	enter_zone = enter_zone,
-- }

-- return game_mgr

