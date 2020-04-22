local game_config=require("game_config");
local mysql_conn=nil;

function mysql_connect_to_auth_center( )
    local auth_conf=game_config.auth_mysql;
    Mysql.connect(auth_conf.host,
                    auth_conf.port,
                    auth_conf.db_name,
                    auth_conf.uname,
                    auth_conf.upwd,
                    function ( err,conn )
                        if err then
                            Logger.error(err);
                            Scheduler.once(mysql_connect_to_auth_center,5000);
                            return
                        end
                        Logger.debug("connect to auth_center db success!");
                        mysql_conn=conn;
                    end);
end

mysql_connect_to_auth_center();

--ret_handler lua回调函数
function get_guest_uinfo( g_key,ret_handler )
    if mysql_conn == nil then
        if ret_handler then
            ret_handler("mysql is not connected",nil);
        end
        return;
    end
    local sql="select uid, unick, usex, uface, uvip, status, is_guest from uinfo where guest_key = \"%s\" limit 1";
    local sql_cmd=string.format(sql,g_key);
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
        uinfo.uid=tonumber(result[1]);
        uinfo.unick=result[2];
        uinfo.usex=tonumber(result[3]);
        uinfo.uface=tonumber(result[4]);
        uinfo.uvip=tonumber(result[5]);
        uinfo.status=tonumber(result[6]);
        uinfo.is_guest=tonumber(result[7]);
        ret_handler(nil,uinfo);
    end)

end

function insert_guest_user( g_key,ret_handler )
    if mysql_conn == nil then
        if ret_handler then
            ret_handler("mysql is not connected",nil);
        end
        return;
    end
    local unick="gst"..math.random( 100000,999999);
    local uface=math.random(1,9);
    local usex=math.random(0,1);
    local sql = "insert into uinfo(`guest_key`, `unick`, `uface`, `usex`, `is_guest`)values(\"%s\", \"%s\", %d, %d, 1)"
    local sql_cmd=string.format(sql,g_key,unick,uface,usex);
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

function edit_profile( uid,unick,uface,usex,ret_handler )
    if mysql_conn == nil then
        if ret_handler then
            ret_handler("mysql is not connected",nil);
        end
        return;
    end

    local sql="update uinfo set unick = \"%s\", usex = %d, uface = %d where uid = %d"
    local sql_cmd=string.format(sql,unick,usex,uface,uid);
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

local mysql_auth_center={
    get_guest_uinfo=get_guest_uinfo,
    insert_guest_user=insert_guest_user,
    edit_profile=edit_profile,

}

return mysql_auth_center;