--初始化日志模块
Logger.init("logger/logic_server/","logic",true);
--end

--连接到gamer数据库
require("database/mysql_game")

--初始化协议模块
local proto_type={
  PROTO_JSON=0,
  PROTO_BUF=1,
}
ProtoMan.init(proto_type.PROTO_BUF);
--如果是protobuf协议，还要注册映射表
if ProtoMan.proto_type() == proto_type.PROTO_BUF then
  local cmd_name_map=require("cmd_name_map");
  if cmd_name_map then
    ProtoMan.register_protobuf_cmd_map(cmd_name_map);
  end
end
--end

local game_config=require("game_config");
local servers=game_config.servers;
local Stype=require("Stype");

--开启网关端口监听
Netbus.tcp_listen(servers[Stype.Logic].port);
print("Logic Server Start at "..servers[Stype.Logic].port);
Netbus.udp_listen(game_config.logic_udp.port);
--end

local logic_service=require("logic_server/logic_service");

local ret= Service.register(Stype.Logic,logic_service);
if ret then
  print("register Logic_service:["..Stype.Logic.."] success!!!")
else
  print("register Logic_service:["..Stype.Logic.."] failed!!!")
end 