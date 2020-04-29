local Response=require("Response");
local Stype=require("Stype");
local Cmd=require("Cmd");

local mysql_game=require("database/mysql_game");
local login_bonues=require("system_server/login_bonues");
local redis_game=require("database/redis_game");
local redis_rank=require("database/redis_rank");
local redis_center=require("database/redis_center");

function get_rank_user_center_info( index, rank_uid, success_func, failed_func )
    redis_center.get_uinfo_inredis(rank_uid,function ( err,uinfo )
        if err or uinfo == nil then
            if failed_func then
                failed_func();
            end
            return
        end

        success_func(index,uinfo);
    end)
end

function get_rank_user_ugame_info( index, rank_uid, success_func, failed_func )
    redis_game.get_ugame_info_inredis(rank_uid,function ( err,ugame_info )
        if err or ugame_info == nil then
            if failed_func then
                failed_func();
            end
            return
        end

        success_func(index,ugame_info);
    end)
end

function send_rank_info_to_client(s, uid, rank_uids, rank_uinfo, rank_gameinfo)
    local rank_info_body={};
    local i;
    for  i=1,#rank_uids do
        local user_rank_info={
            unick=rank_uinfo[i].unick,
            uface=rank_uinfo[i].uface,
            usex=rank_uinfo[i].usex,
            
            uvip=rank_gameinfo[i].uvip,
            uchip=rank_gameinfo[i].uchip,
        };
        rank_info_body[i]=user_rank_info;
    end

    local msg={Stype.System,Cmd.eGetWorldRankUchipRes,uid,{
        status=Response.OK,
        rank_info=rank_info_body,
    }};
    Session.send_msg(s,msg);
    return

end

function get_rank_ugame_info(s, uid, rank_uids, rank_uinfo)

    local rank_ugame_info={};
    local failed_func=function (  )
        local msg={Stype.System,Cmd.eGetWorldRankUchipRes,uid,{
            status=Response.SystemErr,
        }};
        Session.send_msg(s,msg);
        return
    end

    local success_func=function ( index,ugame_info )
        rank_ugame_info[index]=ugame_info;
        if index == #rank_uids then--排行榜信息全部收集完，要返回给客户端
            send_rank_info_to_client(s,uid,rank_uids,rank_uinfo,rank_ugame_info);
        else
            index = index + 1;
            get_rank_user_ugame_info(index,rank_uids[index],success_func,failed_func);
        end
    end
    get_rank_user_ugame_info(1,rank_uids[1],success_func,failed_func)

end

--{stype,ctype,utag,[{message} or jsonStr]}
function get_world_uchip_rank( s,req )
    local uid=req[3];

    --拉去排行榜的前30个
    redis_rank.get_world_rank_with_uchip_inredis(30,
    function ( err,rank_uids )
        if err or rank_uids == nil then
            local msg={Stype.System,Cmd.eGetWorldRankUchipRes,uid,{
                status=Response.SystemErr,
            }};
            Session.send_msg(s,msg);
            return
        end

        if #rank_uids <= 0  then
            local msg={Stype.System,Cmd.eGetWorldRankUchipRes,uid,{
                status=Response.OK,
            }};
            Session.send_msg(s,msg);
            return
        end

        --获得了前面的UID集合{first_uid,second_uid,....}
        local rank_user_info={};
        local failed_func=
        function (  )
            local msg={Stype.System,Cmd.eGetWorldRankUchipRes,uid,{
                status=Response.SystemErr,
            }};
            Session.send_msg(s,msg);
            return
        end

        local success_func=
        function ( index,user_info )
            rank_user_info[index]=user_info;
            if index == #rank_uids then--排行榜中所有uinfo都获取了
                get_rank_ugame_info(s,uid,rank_uids,rank_user_info);
            else
                index = index + 1;
                get_rank_user_center_info(index,rank_uids[index],success_func,failed_func);
            end
        end

        get_rank_user_center_info(1,rank_uids[1],success_func,failed_func);

    end)

end

local game_rank={
    get_world_uchip_rank=get_world_uchip_rank,

}

return game_rank; 