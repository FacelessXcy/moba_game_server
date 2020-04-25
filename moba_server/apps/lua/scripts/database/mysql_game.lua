local game_config=require("game_config");
local moba_game_config=require("moba_game_config");

local mysql_conn=nil;

function mysql_connect_to_moba_game( )
    local conf=game_config.game_mysql;
    Mysql.connect(conf.host,conf.port,
                    conf.db_name,conf.uname,
                    conf.upwd,
                    function ( err,conn )
                        if err then
                            Logger.error(err);
                            Scheduler.once(mysql_connect_to_moba_game,5000);
                            return
                        end
                        Logger.debug("connect to GameSQL db success!");
                        mysql_conn=conn;
                    end);
end

mysql_connect_to_moba_game();

function get_ugame_info( uid,ret_handler )
    if mysql_conn == nil then
        if ret_handler then
            ret_handler("mysql_game is not connected",nil);
        end
        return;
    end

    local sql="select uchip, uchip2, uchip3, uvip, uvip_endtime, udata1, udata2, udata3, uexp, ustatus from ugame where uid = %d limit 1";
    local sql_cmd=string.format(sql,uid);

    Mysql.query(mysql_conn,sql_cmd,
    function ( err,ret )
        if err then
            if ret_handler ~=nil then
                ret_handler(err,nil);
            end
            return
        end
        --没有此条记录
        if ret == nil or #ret<=0 then
            if ret_handler ~=nil then
                ret_handler(nil,nil);
            end
            return;
        end

        local result=ret[1];
        local uinfo={};
        uinfo.uchip=tonumber(result[1]);
        uinfo.uchip2=tonumber(result[2]);
        uinfo.uchip3=tonumber(result[3]);
        uinfo.uvip=tonumber(result[4]);
        uinfo.uvip_endtime=tonumber(result[5]);
        uinfo.udata1=tonumber(result[6]);
        uinfo.udata2=tonumber(result[7]);
        uinfo.udata3=tonumber(result[8]);
        uinfo.uexp=tonumber(result[9]);
        uinfo.ustatus=tonumber(result[10]);
        ret_handler(nil,uinfo);
    end)

end

function insert_ugame_info( uid,ret_handler )
    if mysql_conn == nil then
        if ret_handler then
            ret_handler("mysql_game is not connected",nil);
        end
        return;
    end
    local sql = "insert into ugame(`uid`, `uchip`, `uvip`, `uexp`) values(%d, %d, %d, %d)"
    local sql_cmd=string.format(sql,uid,
                                moba_game_config.ugame.uchip,
                                moba_game_config.ugame.uvip,
                                moba_game_config.ugame.uexp);
    Mysql.query(mysql_conn,sql_cmd,
    function ( err,ret )
        if err then
            ret_handler(err,nil);
            return;
        else
            ret_handler(nil,nil);
        end
    end)
end 

local mysql_game={
    get_ugame_info=get_ugame_info,
    insert_ugame_info=insert_ugame_info,
}

return mysql_game;