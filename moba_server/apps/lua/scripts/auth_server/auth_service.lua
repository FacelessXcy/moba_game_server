
--{stype,ctype,utag,[{message} or jsonStr]}
function on_auth_recv_cmd( s,msg )

end
function on_auth_session_disconnect( s )

end 

local auth_service={
    on_session_recv_cmd=on_auth_recv_cmd,
    on_session_disconnect=on_auth_session_disconnect,
};



return auth_service;