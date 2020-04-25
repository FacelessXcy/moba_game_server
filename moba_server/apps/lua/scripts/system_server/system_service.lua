local Stype=require("Stype");
local Cmd=require("Cmd");


local system_service_handlers={}



--{stype,ctype,utag,[{message} or jsonStr]}
function on_system_recv_cmd( s,msg )
    print(msg[1],msg[2],msg[3])

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