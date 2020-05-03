local player=require("logic_server/player");
local robot_player=player:new();

function robot_player:new(  )
    local instant={};
    setmetatable(instant, {__index=self});
    return instant;
end

function robot_player:init( uid,s,ret_handler )
    player.init(self,uid,s,ret_handler);
    self.is_robot=true;
end

return robot_player;