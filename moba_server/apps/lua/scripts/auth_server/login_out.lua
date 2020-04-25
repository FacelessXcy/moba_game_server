local mysql_center=require("database/mysql_auth_center");
local redis_center=require("database/redis_center")
local Response=require("Response");
local Stype=require("Stype");
local Cmd=require("Cmd");


--{stype,ctype,utag,body}
function do_login_out( s,req )
    local uid=req[3];
    Logger.debug("user "..uid.." login out!");

    local msg={Stype.Auth,Cmd.eLoginOutRes,uid,{
        status=Response.OK,
    }};
    Session.send_msg(s,msg);
    return;
end 

local login_out={
    do_login_out=do_login_out,
}

return login_out;