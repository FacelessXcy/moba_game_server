local mysql_center=require("database/mysql_auth_center");
local redis_center=require("database/redis_center")
local Response=require("Response");
local Stype=require("Stype");
local Cmd=require("Cmd");

--{stype,ctype,utag,body}
local function login( s,req )
    local utag=req[3];
    local uname_login_req=req[4];

    if string.len(uname_login_req.uname) <= 0 or 
    string.len(uname_login_req.upwd) ~= 32 then
        local msg={Stype.Auth,Cmd.eUnameLoginRes,utag,{
            status=Response.InvalidParams,
        }};
        Session.send_msg(s,msg);
        return;
    end

    --检查用户名密码是否正确
    mysql_center.get_uinfo_by_uname_upwd(uname_login_req.uname,
                                            uname_login_req.upwd,--获取gkey的资料
    function ( err,uinfo )
        if err then--告诉客户端某个错误信息
            local msg={Stype.Auth,Cmd.eUnameLoginRes,utag,{
                status=Response.SystemErr,
            }};
            Session.send_msg(s,msg);
            return;
        end

        if uinfo == nil then--没有查到对应的用户，返回不存在用户或密码错误的信息
            local msg={Stype.Auth,Cmd.eUnameLoginRes,utag,{
                status=Response.UnameOrUpwdError,
            }};
            Session.send_msg(s,msg);
            return;
        end

        --找到了用户所对应的游客数据
        if uinfo.status ~= 0 then--账号被查封
            local msg={Stype.Auth,Cmd.eUnameLoginRes,utag,{
                status=Response.UserIsFreeze,
            }};
            Session.send_msg(s,msg);
            return;
        end

        print(uinfo.uid,uinfo.unick);--登陆成功，返回给客户端

        redis_center.set_uinfo_inredis(uinfo.uid,uinfo);
        local msg={Stype.Auth,Cmd.eUnameLoginRes,utag,{
            status=Response.OK,
            uinfo={
                unick=uinfo.unick,
                uface=uinfo.uface,
                usex=uinfo.usex,
                uvip=uinfo.uvip,
                uid=uinfo.uid,
            }
        }}
        Session.send_msg(s,msg);
    end)

end

local uname_login={
    login=login,
}

return uname_login;