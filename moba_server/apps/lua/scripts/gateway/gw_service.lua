
--{stype,ctype,utag,[{message} or jsonStr]}
function on_gw_recv_cmd( s,msg )
    
end

function on_gw_session_disconnect( s )

end 

local gw_service={
    on_session_recv_cmd=on_gw_recv_cmd,
    on_session_disconnect=on_gw_session_disconnect,
};

return gw_service;