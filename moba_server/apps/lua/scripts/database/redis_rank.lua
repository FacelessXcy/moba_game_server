local game_config=require("game_config");
local redis_conn=nil;

local function is_connected(  )
    if not redis_conn then
        return false;
    end
    return true;
end

function redis_connect_to_rank(  )
    local host=game_config.rank_redis.host;
    local port=game_config.rank_redis.port;
    local db_index=game_config.rank_redis.db_index;

    Redis.connect(host,port,
    function ( err,conn )
        if err ~=nil then
            Logger.error(err);
            Scheduler.once(redis_connect_to_rank,5000);
            return;
        end
        redis_conn=conn;
        Logger.debug("connect to Rank_Redis db success!");
        Redis.query(redis_conn,"select "..db_index,function ( err,ret ) end)

    end)
end


redis_connect_to_rank();


--redis有序集合:key WOLD_CHIP_RANK用来做世界排行的有序集合
--好友排行，每个人都有一个好友的FRIEND_CHIP_RANK_UID()  有序集合
local WOLD_CHIP_RANK="WOLD_CHIP_RANK";

function flush_world_rank_with_uchip_inredis( uid,uchip )
    if redis_conn == nil then
        Logger.error("rank_redis disconnected");
        return;
    end

    local redis_cmd = "zadd WOLD_CHIP_RANK " .. uchip.." "..uid;
    Redis.query(redis_conn,redis_cmd,
    function ( err,ret )
        if err then

            return;
        end 

    end)
end

--n:排行榜中条目的数目  ret_handler:回调函数
function get_world_rank_with_uchip_inredis( n,ret_handler )
    if redis_conn == nil then
        Logger.error("rank_redis disconnected");
        return;
    end
    --zrange 由小到大排列  
    --zrevrange 由小到大排列
    --local redis_cmd ="zrange WOLD_CHIP_RANK 0 "..n;
    local redis_cmd ="zrevrange WOLD_CHIP_RANK 0 "..n;
    Redis.query(redis_conn,redis_cmd,
    function ( err,ret )
        if err then
            if ret_handler then
                ret_handler("zrange WOLD_CHIP_RANK in redis error",nil);
            end
            return;
        end 
        --  排行榜没有任何数据
        if ret == nil or #ret <=0 then
            ret_handler(nil,nil);
        end

        --获取到排行榜数据
        local rank_info={};
        local k,v;

        for k,v in pairs(ret) do
            rank_info[k]=tonumber(v);
        end
        
        if ret_handler then
            ret_handler(nil,rank_info);
        end

    end)
end

local redis_rank={
    flush_world_rank_with_uchip_inredis=flush_world_rank_with_uchip_inredis,
    get_world_rank_with_uchip_inredis=get_world_rank_with_uchip_inredis,
    is_connected=is_connected,
}

return redis_rank;