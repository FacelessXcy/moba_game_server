local Stype=require("Stype");
local Cmd=require("Cmd");
local ugame=require("system_server/ugame");
local login_bonues=require("system_server/login_bonues");
local system_service_handlers={}
system_service_handlers[Cmd.eGetUgameInfoReq]=ugame.get_ugame_info;
system_service_handlers[Cmd.eRecvLoginBonuesReq]=login_bonues.recv_login_bonues;

--{stype,ctype,utag,[{message} or jsonStr]}
function on_system_recv_cmd( s,msg )
    if system_service_handlers[msg[2]]  then
        system_service_handlers[msg[2]](s,msg);
    end
end
function on_system_session_disconnect( s,stype )

end 

local system_service={
    on_session_recv_cmd=on_system_recv_cmd,
    on_session_disconnect=on_system_session_disconnect,
};



return system_service;