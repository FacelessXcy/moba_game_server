local Response=require("Response");
local Stype=require("Stype");
local Cmd=require("Cmd");

local mysql_game=require("database/mysql_game");
local login_bonues=require("system_server/login_bonues");
local redis_game=require("database/redis_game");


--{stype,ctype,utag,[{message} or jsonStr]}
--登录时，获取信息
function get_ugame_info( s,req )
    local uid=req[3];
    mysql_game.get_ugame_info(uid,
    function ( err,ugame_info )
        if err then--告诉客户端某个错误信息
            local msg={Stype.System,Cmd.eGetUgameInfoRes,uid,{
                status=Response.SystemErr,
            }};
            Session.send_msg(s,msg);
            return;
        end

        if ugame_info == nil then--没有查到游戏信息
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
        if ugame_info.ustatus ~= 0 then--账号被查封
            local msg={Stype.System,Cmd.eGetUgameInfoRes,uid,{
                status=Response.UserIsFreeze,
            }};
            Session.send_msg(s,msg);
            return;
        end

        --更新redis数据库
        redis_game.set_ugame_info_inredis(uid,ugame_info);

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
                    uchip=ugame_info.uchip,
                    uexp=ugame_info.uexp,
                    uvip=ugame_info.uvip,
                    uchip2=ugame_info.uchip2,
                    uchip3=ugame_info.uchip3,
                    udata1=ugame_info.udata1,
                    udata2=ugame_info.udata2,
                    udata3=ugame_info.udata3,

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