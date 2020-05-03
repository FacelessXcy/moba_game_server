local game_config=require("game_config");
local redis_conn=nil;

local function is_connected(  )
    if not redis_conn then
        return false;
    end
    return true;
end


function redis_connect_to_game(  )
    local host=game_config.game_redis.host;
    local port=game_config.game_redis.port;
    local db_index=game_config.game_redis.db_index;

    Redis.connect(host,port,
    function ( err,conn )
        if err ~=nil then
            Logger.error(err);
            Scheduler.once(redis_connect_to_game,5000);
            return;
        end
        redis_conn=conn;
        Logger.debug("connect to Game_Redis db success!");
        Redis.query(redis_conn,"select "..db_index,function ( err,ret ) end)

    end)
end


redis_connect_to_game();

function set_ugame_info_inredis( uid,ugame_info )
    if redis_conn == nil then
        Logger.error("game_redis disconnected");
        return;
    end

	local redis_cmd = "hmset moba_ugame_user_uid_" .. uid ..
	                  " uchip " .. ugame_info.uchip ..
	                  " uexp " .. ugame_info.uexp .. 
	                  " uvip " .. ugame_info.uvip 
                      
    Redis.query(redis_conn,redis_cmd,
    function ( err,ret )
        if err then
			return 
		end
    end)
end

--ret_handler(err,uinfo)
function get_ugame_info_inredis( uid,ret_handler )
    if redis_conn == nil then
        Logger.error("game_redis disconnected");
        return;
    end

    local redis_cmd = "hgetall moba_ugame_user_uid_" .. uid;
    Redis.query(redis_conn,redis_cmd,
    function ( err,ret )
        if err then
            if ret_handler then
                ret_handler(err,nil);
            end
            return;
        end 

		local ugame_info = {}
		ugame_info.uid = uid
		ugame_info.uchip = tonumber(ret[2])
		ugame_info.uexp = tonumber(ret[4])
		ugame_info.uvip = tonumber(ret[6])
        ret_handler(nil,ugame_info);
    end)

end

function add_chip_inredis( uid,add_chip )
    if redis_conn == nil then
        Logger.error("game_redis disconnected");
        return;
    end
    
    get_ugame_info_inredis(uid,
    function ( err,ugame_info )
        if err then
            Logger.error("get ugame_info in redis error");
            return
        end

        ugame_info.uchip = ugame_info.uchip + add_chip;

        set_ugame_info_inredis(uid,ugame_info);
    end)
end

local redis_game={
    set_ugame_info_inredis=set_ugame_info_inredis,
    get_ugame_info_inredis=get_ugame_info_inredis,
    add_chip_inredis=add_chip_inredis,
    is_connected=is_connected,
}

return redis_game;