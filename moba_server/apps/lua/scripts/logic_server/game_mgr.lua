local Response=require("Response");
local Stype=require("Stype");
local Cmd=require("Cmd");
local mysql_game=require("database/mysql_game");
local redis_game=require("database/redis_game");
local player=require("logic_server/player");
local Zone=require("logic_server/Zone");

--uid player 映射表
local logic_server_players={};
local online_player_num=0;
local zone_wait_list={};--zone_wait_list[Zone.SGYD]={}-->uid-->p



function send_status(s,stype,ctype,uid,status)
    local msg={stype,ctype,uid,{
        status=status,
    }};
    Session.send_msg(s,msg);
end


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
