local game_config=require("game_config");
local redis_conn=nil;

local function is_connected(  )
    if not redis_conn then
        return false;
    end
    return true;
end


function redis_connect_to_center(  )
    local host=game_config.center_redis.host;
    local port=game_config.center_redis.port;
    local db_index=game_config.center_redis.db_index;

    Redis.connect(host,port,
    function ( err,conn )
        if err ~=nil then
            Logger.error(err);
            Scheduler.once(redis_connect_to_center,5000);
            return;
        end
        redis_conn=conn;
        Logger.debug("connect to Redis center db success!");
        Redis.query(redis_conn,"select "..db_index,function ( err,ret ) end)

    end)
end


redis_connect_to_center();

function set_uinfo_inredis( uid,uinfo )
    if redis_conn == nil then
        Logger.error("redis center disconnected");
        return;
    end
    print("set_uinfo_inredis "..uinfo.uvip);
	local redis_cmd = "hmset moba_auth_center_user_uid_" .. uid ..
	                  " unick " .. uinfo.unick ..
	                  " usex " .. uinfo.usex .. 
	                  " uface " .. uinfo.uface ..
	                  " uvip " .. uinfo.uvip ..
                      " is_guest " .. uinfo.is_guest
                      
    Redis.query(redis_conn,redis_cmd,
    function ( err,ret )
        if err then
			return 
		end
    end)
end

--ret_handler(err,uinfo)
function get_uinfo_inredis( uid,ret_handler )
    local redis_cmd = "hgetall moba_auth_center_user_uid_" .. uid;
    Redis.query(redis_conn,redis_cmd,
    function ( err,ret )
        if err then
            if ret_handler then
                ret_handler(err,nil);
            end
            return;
        end 

		local uinfo = {}
		uinfo.uid = uid
		uinfo.unick = ret[2]
		uinfo.usex = tonumber(ret[4])
		uinfo.uface = tonumber(ret[6])
		uinfo.uvip = tonumber(ret[8])
		uinfo.is_guest = tonumber(ret[10])
        print("get_uinfo_inredis "..uinfo.uvip);
        ret_handler(nil,uinfo);
    end)

end

function edit_profile( uid,unick,uface,usex )
    get_uinfo_inredis(uid,
    function ( err,uinfo )
        if err then
            Logger.error("get uinfo in redis error");
            return

        end

        uinfo.unick = unick
		uinfo.usex = usex
		uinfo.uface = uface

        set_uinfo_inredis(uid,uinfo);
    end)
end

local redis_center={
    set_uinfo_inredis=set_uinfo_inredis,
    get_uinfo_inredis=get_uinfo_inredis,
    edit_profile=edit_profile,
    is_connected=is_connected,
}

return redis_center;