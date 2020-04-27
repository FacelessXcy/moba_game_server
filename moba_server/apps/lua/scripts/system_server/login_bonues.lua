local Response=require("Response");
local Stype=require("Stype");
local Cmd=require("Cmd");
local mysql_game=require("database/mysql_game");
local moba_game_config=require("moba_game_config");
local redis_game=require("database/redis_game");

function send_bonues_to_user( uid,bonues_info,ret_handler )
    --要更新发放奖励
    if bonues_info.bonues_time  < Utils.timestamp_today() then
        if bonues_info.bonues_time >= Utils.timestamp_yesterday() then--连续登录
            bonues_info.days=bonues_info.days+1;
        else--重新开始计算登陆天数
            bonues_info.days=1;
        end
        --连续登录了5天，重新开始计算
        if bonues_info.days> #moba_game_config.login_bonues then
            bonues_info.days=1;
        end

        bonues_info.status=0;
        bonues_info.bonues_time=Utils.timestamp();
        bonues_info.bonues=moba_game_config.login_bonues[bonues_info.days];
        mysql_game.update_login_bonues(uid,bonues_info,
        function ( err,ret )
            if err then
                ret_handler(err,nil);
                return;
            end

            ret_handler(nil,bonues_info);
        end)
        return;
    end
    --把登录奖励信息返回ugame
    ret_handler(nil,bonues_info);

end

--ret_handler(err,bonues_info)
function check_login_bonues( uid,ret_handler )
    mysql_game.get_bonues_info(uid,
    function ( err,bonues_info )
        if err then
            ret_handler(err,nil);
            return
        end
        --用户第一次登录
        if bonues_info == nil then
            mysql_game.insert_bonues_info(uid,
            function ( err,ret )
                if err then
                    ret_handler(err,nil);
                end

                check_login_bonues(uid,ret_handler);
            end);
            return;
        end

        send_bonues_to_user(uid,bonues_info,ret_handler);
    end)
end

--{stype,ctype,utag,[{message} or jsonStr]}
function recv_login_bonues( s,req )
    local uid=req[3];

    mysql_game.get_bonues_info(uid,
    function ( err,bonues_info )
        if err then
            local msg={Stype.System,Cmd.eRecvLoginBonuesRes,uid,{
                status=Response.SystemErr,
            }};
            Session.send_msg(s,msg);
            return;
        end

        --用户第一次登录或者已经领取过了
        if bonues_info == nil or bonues_info.status ~= 0 then
            local msg={Stype.System,Cmd.eRecvLoginBonuesRes,uid,{
                status=Response.InvalidOpt,
            }};
            Session.send_msg(s,msg);
            return;
        end

        --有奖励可领取
        mysql_game.update_login_bonues_status(uid,
        function ( err,ret )
            if err then
                local msg={Stype.System,Cmd.eRecvLoginBonuesRes,uid,{
                    status=Response.SystemErr,
                }};
                Session.send_msg(s,msg);
                return;
            end

            --更新数据库的金币
            mysql_game.add_chip(uid,bonues_info.bonues,nil);
            
            --更新redis的uchip
            redis_game.add_chip_inredis(uid,bonues_info.bonues);

            local msg={Stype.System,Cmd.eRecvLoginBonuesRes,uid,{
                status=Response.OK,
            }};
            Session.send_msg(s,msg);

        end)
    end)

end

local login_bonues={
    check_login_bonues=check_login_bonues,
    recv_login_bonues=recv_login_bonues,
}

return login_bonues;