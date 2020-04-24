local mysql_center=require("database/mysql_auth_center");
local redis_center=require("database/redis_center")
local Response=require("Response");
local Stype=require("Stype");
local Cmd=require("Cmd");

function login( s,req )
    local g_key = req[4].guest_key
    local utag=req[3];
    --print(req[1],req[2],req[3],req[4].guest_key);
    --判断guestkey的合法性，并且长度为32
    if type(g_key) ~="string" or string.len(g_key) ~= 32 then--如果不是返回错误信息
        local msg={Stype.Auth,Cmd.eGuestLoginRes,utag,{
            status=Response.InvalidParams,
        }};
        Session.send_msg(s,msg);
        return;
    end

    mysql_center.get_guest_uinfo(g_key,--获取gkey的资料
    function ( err,uinfo )
        if err then--告诉客户端某个错误信息
            local msg={Stype.Auth,Cmd.eGuestLoginRes,utag,{
                status=Response.SystemErr,
            }};
            Session.send_msg(s,msg);
            return;
        end
        if uinfo == nil then--没有查到对应的g_key的信息
            mysql_center.insert_guest_user(g_key,----插入gkey的资料
            function ( err,ret )
                if err then--告诉客户端某个错误信息
                    local msg={Stype.Auth,Cmd.eGuestLoginRes,utag,{
                        status=Response.SystemErr,
                    }};
                    Session.send_msg(s,msg);
                    return;
                end
                login(s,req);
            end)     
            return  
        end
        --找到了gkey所对应的游客数据
        if uinfo.status ~= 0 then--账号被查封
            local msg={Stype.Auth,Cmd.eGuestLoginRes,utag,{
                status=Response.UserIsFreeze,
            }};
            Session.send_msg(s,msg);
            return;
        end
        if uinfo.is_guest ~=1 then--已经不是游客账号了
            local msg={Stype.Auth,Cmd.eGuestLoginRes,utag,{
                status=Response.UserIsNotGuest,
            }};
            Session.send_msg(s,msg);
            return;
        end

        print(uinfo.uid,uinfo.unick);--登陆成功，返回给客户端

        redis_center.set_uinfo_inredis(uinfo.uid,uinfo);
        local msg={Stype.Auth,Cmd.eGuestLoginRes,utag,{
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

local guest={
    login=login,
}

return guest