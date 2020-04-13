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