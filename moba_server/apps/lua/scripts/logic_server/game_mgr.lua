local Response=require("Response");
local Stype=require("Stype");
local Cmd=require("Cmd");
local mysql_game=require("database/mysql_game");
local redis_game=require("database/redis_game");
local player=require("logic_server/player");

function send_status(s,stype,ctype,uid,status)
    local msg={stype,ctype,uid,{
        status=status,
    }};
    Session.send_msg(s,msg);
end


--uid player 映射表
local logic_server_players={};
local online_player_num=0;

function login_logic_server( s,req )
    local uid=req[3];
    local p=logic_server_players[uid];--玩家player对象
    if p then--玩家对象已经存在，更新session即可
        p:set_session(s);
        send_status(s,Stype.Logic,Cmd.eLoginLogicRes,uid,Response.OK);
        return;
    end

    p=player:new();
    p:init(uid,s,
    function ( status )
        if status == Response.OK then
            logic_server_players[uid]=p;
            online_player_num=online_player_num+1;
        end
        send_status(s,Stype.Logic,Cmd.eLoginLogicRes,uid,status);
    end);
end

--玩家断线
function on_player_disconnect( s,req )
    local uid=req[3];
    
    --游戏中的玩家后面考虑


    --玩家断线离开
    if logic_server_players[uid] then
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


local game_mgr={
    login_logic_server=login_logic_server,
    on_player_disconnect=on_player_disconnect,
    on_gateway_disconnect=on_gateway_disconnect,
    on_gateway_connect=on_gateway_connect,
}

return game_mgr;
