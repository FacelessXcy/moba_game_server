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

function get_bonues_info( uid,ret_handler )
    if mysql_conn == nil then
        if ret_handler then
            ret_handler("mysql_game is not connected",nil);
        end
        return;
    end

    local sql="select bonues, status, bonues_time, days from login_bonues where uid = %d limit 1";
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
        local bonuse_info={};
        bonuse_info.bonues=tonumber(result[1]);
        bonuse_info.status=tonumber(result[2]);
        bonuse_info.bonues_time=tonumber(result[3]);
        bonuse_info.days=tonumber(result[4]);

        ret_handler(nil,bonuse_info);
    end)

end

function insert_bonues_info( uid,ret_handler )
    if mysql_conn == nil then
        if ret_handler then
            ret_handler("mysql_game is not connected",nil);
        end
        return;
    end
    local sql = "insert into login_bonues(`uid`, `bonues_time`, `status`) values(%d, %d, 1)"
    local sql_cmd=string.format(sql,uid,Utils.timestamp());
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

function update_login_bonues( uid,bonues_info,ret_handler )
    if mysql_conn == nil then
        if ret_handler then
            ret_handler("mysql_game is not connected",nil);
        end
        return;
    end

    local sql="update login_bonues set status = 0, bonues = %d, bonues_time = %d,days = %d  where uid = %d"
    local sql_cmd=string.format(sql,bonues_info.bonues,
                                    bonues_info.bonues_time,
                                    bonues_info.days,
                                    uid);

    Mysql.query(mysql_conn,sql_cmd,
    function ( err,ret )
        if err then
            if ret_handler ~=nil then
                ret_handler(err,nil);
            end
            return
        end

        if  ret_handler then
            ret_handler(nil,nil);
        end
    end)
end

function update_login_bonues_status( uid,ret_handler )
    if mysql_conn == nil then
        if ret_handler then
            ret_handler("mysql_game is not connected",nil);
        end
        return;
    end

    local sql="update login_bonues set status = 1 where uid = %d"
    local sql_cmd=string.format(sql,uid);

    Mysql.query(mysql_conn,sql_cmd,
    function ( err,ret )
        if err then
            if ret_handler ~=nil then
                ret_handler(err,nil);
            end
            return
        end

        if  ret_handler then
            ret_handler(nil,nil);
        end
    end)
end

function add_chip( uid,chip,ret_handler )
    if mysql_conn == nil then
        if ret_handler then
            ret_handler("mysql_game is not connected",nil);
        end
        return;
    end

    local sql="update ugame set uchip = uchip + %d where uid = %d"
    local sql_cmd=string.format(sql,chip,uid);

    Mysql.query(mysql_conn,sql_cmd,
    function ( err,ret )
        if err then
            if ret_handler ~=nil then
                ret_handler(err,nil);
            end
            return
        end

        if  ret_handler then
            ret_handler(nil,nil);
        end
    end)
end

function get_sys_msg( ret_handler )
    if mysql_conn == nil then
        if ret_handler then
            ret_handler("mysql_game is not connected",nil);
        end
        return;
    end

    local sql="select msg from sys_msg";
    local sql_cmd=sql;

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

        --{{"msg内容"},{}}
        local result={};
        local k,v;
        for k,v in pairs(ret) do
            result[k]=v[1];
        end

        ret_handler(nil,result);
    end)
end

local mysql_game={
    get_ugame_info=get_ugame_info,
    insert_ugame_info=insert_ugame_info,
    get_bonues_info=get_bonues_info,
    insert_bonues_info=insert_bonues_info,
    update_login_bonues=update_login_bonues,
    update_login_bonues_status=update_login_bonues_status,
    add_chip=add_chip,
    get_sys_msg=get_sys_msg,
}

return mysql_game;