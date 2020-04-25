local Stype=require("Stype");
local Cmd=require("Cmd");
local guest=require("auth_server/guest");
local edit_profile=require("auth_server/edit_profile");
local account_upgrade=require("auth_server/account_upgrade")
local uname_login=require("auth_server/uname_login");

local auth_service_handler={}
auth_service_handler[Cmd.eGuestLoginReq]=guest.login;
auth_service_handler[Cmd.eEditProfileReq]=edit_profile.do_edit_profile;
auth_service_handler[Cmd.eAccountUpgradeReq]=account_upgrade.do_upgrade;
auth_service_handler[Cmd.eUnameLoginReq]=uname_login.login;


--{stype,ctype,utag,[{message} or jsonStr]}
function on_auth_recv_cmd( s,msg )
    
    if auth_service_handler[msg[2]]  then
        auth_service_handler[msg[2]](s,msg);
    end
end
function on_auth_session_disconnect( s,stype )

end 

local auth_service={
    on_session_recv_cmd=on_auth_recv_cmd,
    on_session_disconnect=on_auth_session_disconnect,
};



return auth_service;