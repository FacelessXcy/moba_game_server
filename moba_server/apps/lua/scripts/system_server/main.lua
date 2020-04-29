--初始化日志模块
Logger.init("logger/system_server/","system",true);
--end

--连接到mysql_game数据库
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
Netbus.tcp_listen(servers[Stype.System].port);
print("System Server Start at "..servers[Stype.System].port);
-- Netbus.udp_listen(8002);
--end

local system_service=require("system_server/system_service");

local ret= Service.register(Stype.System,system_service);
if ret then
  print("register System service:["..Stype.System.."] success!!!")
else
  print("register System service:["..Stype.System.."] failed!!!")
end 