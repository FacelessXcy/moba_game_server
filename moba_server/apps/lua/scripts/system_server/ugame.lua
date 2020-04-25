local Response=require("Response");
local Stype=require("Stype");
local Cmd=require("Cmd");

local mysql_game=require("database/mysql_game");
local login_bonues=require("system_server/login_bonues");


--{stype,ctype,utag,[{message} or jsonStr]}
--登录时，获取信息
function get_ugame_info( s,req )
    local uid=req[3];
    mysql_game.get_ugame_info(uid,
    function ( err,uinfo )
        if err then--告诉客户端某个错误信息
            local msg={Stype.System,Cmd.eGetUgameInfoRes,uid,{
                status=Response.SystemErr,
            }};
            Session.send_msg(s,msg);
            return;
        end

        if uinfo == nil then--没有查到游戏信息
            mysql_game.insert_ugame_info(uid,----插入gkey的资料
            function ( err,ret )
                if err then--告诉客户端某个错误信息
                    local msg={Stype.System,Cmd.eGetUgameInfoRes,uid,{
                        status=Response.SystemErr,
                    }};
                    Session.send_msg(s,msg);
                    return;
                end
                get_ugame_info(s,req);
            end)     
            return  
        end

        --读取到了
        if uinfo.ustatus ~= 0 then--账号被查封
            local msg={Stype.System,Cmd.eGetUgameInfoRes,uid,{
                status=Response.UserIsFreeze,
            }};
            Session.send_msg(s,msg);
            return;
        end

        --检查登录奖励
        login_bonues.check_login_bonues(uid,
        function ( err,bonues_info )
            if err then
                local msg={Stype.System,Cmd.eGetUgameInfoRes,uid,{
                    status=Response.SystemErr,
                }};
                Session.send_msg(s,msg);
                return;
            end
            --成功读取，返回给客户端
            local msg={Stype.System,Cmd.eGetUgameInfoRes,uid,{
                status=Response.OK,
                uinfo={
                    uchip=uinfo.uchip,
                    uexp=uinfo.uexp,
                    uvip=uinfo.uvip,
                    uchip2=uinfo.uchip2,
                    uchip3=uinfo.uchip3,
                    udata1=uinfo.udata1,
                    udata2=uinfo.udata2,
                    udata3=uinfo.udata3,

                    bonues_status=bonues_info.status,
                    bonues=bonues_info.bonues,
                    days=bonues_info.days,
                }
            }};
            Session.send_msg(s,msg);
        end)


    end)
end



local ugame={
    get_ugame_info=get_ugame_info,
}


return ugame;