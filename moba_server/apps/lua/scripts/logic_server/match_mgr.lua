local Response=require("Response");
local Stype=require("Stype");
local Cmd=require("Cmd");
local mysql_game=require("database/mysql_game");
local redis_game=require("database/redis_game");
local State=require("logic_server/State")
local player=require("logic_server/player");
local Zone=require("logic_server/Zone");



local match_mgr={};
local sg_matchid=1;
local PLAYER_NUM=3;--3v3

function match_mgr:new( instant )
    if not instant then
        instant={};
    end
    setmetatable(instant, {__index=self});
    return instant;
end

function match_mgr:init( zid )
    self.zid=zid;
    self.matchid=sg_matchid;
    sg_matchid = sg_matchid + 1;
    self.state=State.InView;

    --旁观玩家列表
    self.inview_players={}--旁观玩家列表
    self.lhs_players={};--左右两边玩家列表
    self.rhs_players={};
end

function match_mgr:broadcast_cmd_inview_players( stype,ctype,body,not_to_player )
    local k,v;
    
	for k,v in pairs(self.inview_players) do
		if v ~= not_to_player then 
			v:send_cmd(stype, ctype, body)
		end
	end
end

function match_mgr:exit_player( p )
    local body={
        seatid=p.seatid,
    }
    self:broadcast_cmd_inview_players(Stype.Logic,Cmd.eUserExitMatch,body,p);
    --从等待列表内，移除player
    self.inview_players[p.seatid]=nil;
    p.zid=-1;
    p.matchid=-1;
    p.seatid=-1;

    local body={status=Response.OK};
    p:send_cmd(Stype.Logic,Cmd.eExitMatchRes,body);
    --广播给其他玩家，该玩家已经离开
    
end

function match_mgr:enter_player( p )
    local i;
    if self.state ~= State.InView or p.state ~= State.InView then
        return false;
    end
    p.matchid=self.matchid;
    --将玩家加入到匹配列表中
    for i=1,PLAYER_NUM*2 do
        if not self.inview_players[i] then
            self.inview_players[i]=p;
            p.seatid=i;
            break;
        end
    end
    
    --通知客户端，已进入游戏房间，matchid,zid
    local body={
        zid=self.zid,
        matchid=self.matchid,
        seatid=1,
        side=1,
    }
    p:send_cmd(Stype.Logic,Cmd.eEnterMatch,body);
    --将用户进入的消息发送给房间里的其他玩家
    body=p:get_user_arrived();
    self:broadcast_cmd_inview_players(Stype.Logic,Cmd.eUserArrived,body,p);

    --玩家还要收到，其他在等待列表中的玩家信息。
    for i=1,#self.inview_players do
        if self.inview_players[i] ~= p  then
            body=self.inview_players[i]:get_user_arrived();
            p:send_cmd(Stype.Logic,Cmd.eUserArrived,body);
        end
    end

    --判断是否匹配完成
    if #self.inview_players >= PLAYER_NUM*2 then
        self.state=State.Ready;
        for i = 1, #self.inview_players do 
            self.inview_players[i].state=State.Ready;
        end 
    end

    return true;
end



return match_mgr;
