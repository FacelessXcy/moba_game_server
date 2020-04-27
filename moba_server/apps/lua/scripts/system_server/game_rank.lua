local Response=require("Response");
local Stype=require("Stype");
local Cmd=require("Cmd");

local mysql_game=require("database/mysql_game");
local login_bonues=require("system_server/login_bonues");
local redis_game=require("database/redis_game");
local redis_rank=require("database/redis_rank");

--{stype,ctype,utag,[{message} or jsonStr]}
function get_world_uchip_rank_info( s,req )
    
end

local game_rank={
    get_world_uchip_rank_info=get_world_uchip_rank_info,

}

return game_rank; 