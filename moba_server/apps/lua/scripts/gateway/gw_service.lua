--{stype,ctype,utag,}
function on_gw_recv_raw_cmd( s,raw_cmd )
    
end

function on_gw_session_disconnect( s )

end 

local gw_service={
    on_session_recv_raw_cmd=on_gw_recv_raw_cmd,
    on_session_disconnect=on_gw_session_disconnect,
};

return gw_service;