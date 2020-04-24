local mysql_center=require("database/mysql_auth_center");
local redis_center=require("database/redis_center")
local Response=require("Response");
local Stype=require("Stype");
local Cmd=require("Cmd");

function _do_account_upgrade( s,req,uid,uname,upwd_md5 )
    
    mysql_center.do_guest_account_upgrade(uid,uname,upwd_md5,
    function ( err,ret )
        if err then
            local msg={Stype.Auth,Cmd.eAccountUpgradeRes,uid,{
                status=Response.SystemErr,
            }};
            Session.send_msg(s,msg);
            return;
        end 

        local msg={Stype.Auth,Cmd.eAccountUpgradeRes,uid,{
            status=Response.OK,
        }};
        Session.send_msg(s,msg);

    end)
end

function _check_is_guest( s,req,uid,uname,upwd_md5 )
    mysql_center.get_uinfo_by_uid(uid,
    function ( err,uinfo )
        if err then
            local msg={Stype.Auth,Cmd.eAccountUpgradeRes,uid,{
                status=Response.SystemErr,
            }};
            Session.send_msg(s,msg);
            return;
        end

        if uinfo.is_guest ~= 1 then--不是游客账号
            local msg={Stype.Auth,Cmd.eAccountUpgradeRes,uid,{
                status=Response.UserIsNotGuest,
            }};
            Session.send_msg(s,msg);
            return;
        end

        _do_account_upgrade(s,req,uid,uname,upwd_md5);

    end)
end

function do_upgrade( s,req )
    local uid = req[3]
    local account_upgrade_req = req[4]
    
    local uname=account_upgrade_req.uname;
    local upwd_md5=account_upgrade_req.upwd_md5;

    if string.len(uname)<=0 or string.len(upwd_md5) ~= 32 then
        local msg = {Stype.Auth, Cmd.eAccountUpgradeRes, uid, {
			status = Response.InvalidParams,
		}}

        Session.send_msg(s, msg)
        return;
    end
    --检查uanme是否存在
    mysql_center.check_uname_exit(uname,
    function ( err,ret )
        if err then
            local msg={Stype.Auth,Cmd.eAccountUpgradeRes,uid,{
                status=Response.SystemErr,
            }};
            Session.send_msg(s,msg);
            return;
        end
        if ret then--uname被占用了
            local msg={Stype.Auth,Cmd.eAccountUpgradeRes,uid,{
                status=Response.UnameIsExist,
            }};
            Session.send_msg(s,msg);
            return;
        end
        _check_is_guest(s,req,uid,uname,upwd_md5);
    end)

end




local account_upgrade={
    do_upgrade=do_upgrade,
}
return account_upgrade;