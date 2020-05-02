local Response=require("Response");
local Stype=require("Stype");
local Cmd=require("Cmd");

local mysql_game=require("database/mysql_game");
local redis_game=require("database/redis_game");

local player={};


function player:new( instant )
    if not instant then
        instant={};
    end
    setmetatable(instant, {__index=self});
    return instant;
end


function player:init( uid,s,ret_handler )
    self.session=s;
    self.uid=uid;
    self.zid=-1;--表明玩家所在空间 -1：不在任何游戏场中

    --从数据库中读取玩家基本信息；
    mysql_game.get_ugame_info(uid,
    function ( err,ugame_info )
        if err then
            ret_handler(Response.SystemErr);
            return;
        end

        self.ugame_info=ugame_info;
        ret_handler(Response.OK);
    end)

    --。。其他信息后面在读
end

function player:set_session(s)
    self.session=s;
end


return player 