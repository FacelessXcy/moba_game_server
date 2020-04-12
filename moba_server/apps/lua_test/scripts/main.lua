--初始化日志模块
logger.init("logger/gateway/","gateway",true);
--end

--初始化协议模块
local proto_type={
  PROTO_JSON=0,
  PROTO_BUF=1,
}
proto_man.init(proto_type.PROTO_BUF);
--如果是protobuf协议，还要注册映射表
if proto_man.proto_type() == proto_type.PROTO_BUF then
  local cmd_name_map=require("cmd_name_map");
  if cmd_name_map then
    proto_man.register_protobuf_cmd_map(cmd_name_map);
  end
end

--end

--开启网络服务
netbus.tcp_listen(6080);
netbus.ws_listen(8001);
netbus.udp_listen(8002);
--end

print("start service success!");

local echo_server=require("echo_server");
service.register(echo_server.stype,echo_server.service);
