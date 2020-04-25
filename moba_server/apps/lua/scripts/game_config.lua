local Stype=require("Stype");

local remote_servers={}

--配置服务所部署的服务器IP地址和端口
remote_servers[Stype.Auth]={
    stype=Stype.Auth,
    ip="127.0.0.1",
    port=8000,
    desic="Auth server"
}
remote_servers[Stype.System]={
    stype=Stype.System,
    ip="127.0.0.1",
    port=8001,
    desic="System server"
}

local game_config={
    gateway_tcp_ip="127.0.0.1",
    gateway_tcp_port=6080,

    gateway_ws_ip="127.0.0.1",
    gateway_ws_port=6081,

    servers=remote_servers,

    auth_mysql={
        host="127.0.0.1",--数据host
        port=3306,--数据库端口
        db_name="auth_center",--数据库名
        uname="root",--数据库登录账号
        upwd="xcy19990419"--数据库登录密码
    },
    center_redis={
        host="127.0.0.1",--数据库host
        port=6379,
        db_index=1,
    },
    game_mysql={
        host="127.0.0.1",--数据host
        port=3306,--数据库端口
        db_name="moba_game",--数据库名
        uname="root",--数据库登录账号
        upwd="xcy19990419"--数据库登录密码
    },
}

return game_config;