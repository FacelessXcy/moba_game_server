local game_config=require("game_config");

--  stype--> session的映射表
local server_session_man={}
--当前正在连接的服务器
local do_connecting={}

--临时ukey-->client_session
local g_ukey=1;
local client_sessions_ukey={};
--uid-->client_session
local client_sessions_uid={};
local Stype=require("Stype");
local Cmd=require("Cmd");
local Response = require("Response")

--连接到服务器
function connect_to_server( stype,ip,port )
    Netbus.tcp_connect(ip,port,
    function ( error,session )
        do_connecting[stype]=false;
        if error ~=0 then
            Logger.error("connect error to server ["..game_config.servers[stype].desic.."]"..ip..":"..port)
            return;
        end
        server_session_man[stype]=session;
        print("connect success to server ..["..game_config.servers[stype].desic.."]"..ip..":"..port);
    end);
end

--检查服务器连接
function check_server_connect(  )
    for k,v in pairs(game_config.servers) do
        --如果该服务未连接,则重新尝试连接
        if server_session_man[v.stype]==nil and do_connecting[v.stype]==false then
            do_connecting[v.stype]=true;
            print("connecting to server ["..v.desic.."]"..v.ip..":"..v.port);
            connect_to_server(v.stype,v.ip,v.port);
        end
    end 
end


function gw_service_init(  )
    for k,v in pairs(game_config.servers) do
        server_session_man[v.stype]=nil;
        do_connecting[v.stype]=false;
    end 

    --启动一个定时器,检查连接
    Scheduler.schedule(check_server_connect,1000,-1,5000);
    --end
end

function is_login_return_cmd( ctype )
    if ctype == Cmd.eGuestLoginRes 
    or ctype == Cmd.eUnameLoginRes then
        return true;
    end
    return false;
end

function send_to_client( server_session,raw_cmd )
    local stype,ctype,utag=RawCmd.read_header(raw_cmd);
    local client_session=nil;

    if is_login_return_cmd(ctype) then
        client_session=client_sessions_ukey[utag];
        client_sessions_ukey[utag]=nil;
        if client_session == nil then
            return;
        end

        local body=RawCmd.read_body(raw_cmd);

        if body.status ~= Response.OK then
            RawCmd.set_utag(raw_cmd,0);
            Session.send_raw_cmd(client_session,raw_cmd);
            return
        end

        local uid=body.uinfo.uid;
        --判断是否有相同的session已经登录
        if client_sessions_uid[uid] and client_sessions_uid[uid]~= client_session then
            local relogin_cmd={Stype.Auth,Cmd.eRelogin,0,nil};
            Session.send_msg(client_sessions_uid[uid],relogin_cmd);
            Session.close(client_sessions_uid[uid]);
            --client_sessions_uid[uid]=nil;
        end

        client_sessions_uid[uid]=client_session;
        Session.set_uid(client_session,uid);
        body.uinfo.uid=0;
        local login_res={stype,ctype,0,body};
        Session.send_msg(client_session,login_res);
        return;
    end
    --很有可能是UId做key
    --区分命令是登录前还是登录后
    --只有命令的类型才知道是到uid里查，还是到ukey里查

    client_session=client_sessions_uid[utag];
    if client_session then
        RawCmd.set_utag(raw_cmd,0);
        Session.send_raw_cmd(client_session,raw_cmd);

        if ctype == Cmd.eLoginOutRes then--注销的消息转发给其他服务器
            Session.set_uid(client_session,0);
            client_sessions_uid[utag]=nil;
        end
    end
end

function is_login_request_cmd( ctype )
    if ctype == Cmd.eGuestLoginReq 
    or ctype == Cmd.eUnameLoginReq then
        return true;
    end
    return false;
end

function send_to_server( client_session,raw_cmd )
    local stype,ctype,utag=RawCmd.read_header(raw_cmd);
    --print(stype,ctype,utag);
    local server_session=server_session_man[stype];
    if server_session==nil then--系统错误
       return; 
    end

    if is_login_request_cmd(ctype) then
        utag=Session.get_utag(client_session);
        if utag==0 then--如果未设置过utag，则设置一个默认utag
            utag=g_ukey;
            g_ukey=g_ukey+1;
            Session.set_utag(client_session,utag);
        end
        client_sessions_ukey[utag]=client_session;
    elseif ctype==Cmd.eLoginLogicReq then
        local uid=Session.get_uid(client_session);
        utag=uid;
        if utag==0 then--该用户未登录,需要先登录
            return;
        end
        local tcp_ip,tcp_port=Session.get_address(client_session);
        local body=RawCmd.read_body(raw_cmd);
        body.udp_ip=tcp_ip;

        local login_logic_cmd={stype,ctype,utag,body};
        Session.send_msg(server_session,login_logic_cmd);
        return;
    else
        local uid=Session.get_uid(client_session);
        utag=uid;
        if utag==0 then--该用户未登录,需要先登录
            return;
        end
        --client_sessions_uid[uid]=client_session;
    end

    --给cmd打上utag，转发给服务器
    RawCmd.set_utag(raw_cmd,utag);
    Session.send_raw_cmd(server_session,raw_cmd);
end

--{stype,ctype,utag,}
function on_gw_recv_raw_cmd( s,raw_cmd )
    if Session.asclient(s)==0 then--数据由客户端传来，转发给服务器
        send_to_server(s,raw_cmd);
    else--数据由其他服务器传来，转发给客户端
        send_to_client(s,raw_cmd);
    end

end 

function on_gw_session_disconnect( s,stype )
    --连接到服务器的session断线了
    if Session.asclient(s)==1 then
        for k,v in pairs(server_session_man) do
            if v ==s then
                print("gateway disconnect: ["..game_config.servers[k].desic.."]");
                server_session_man[k]=nil;
                return;
            end
        end 
        return;
    end

    --连接到网关的客户端session断线了  
    --把客户端从临时映射表中删除
    local utag=Session.get_utag(s);
    if client_sessions_ukey[utag] ~=nil and client_sessions_ukey[utag] ==s then
        --lua的table在删除数组元素时，会自动保持数组的有序性，会把后面的元素向前移动
        client_sessions_ukey[utag]=nil;
        Session.set_utag(s,0);
    end
    --end
    --把客户端从uid映射表中移除
    local uid=Session.get_uid(s);
    if client_sessions_uid[uid] ~=nil and client_sessions_uid[uid]==s then
        client_sessions_uid[uid]=nil;
    end

    local server_session=server_session_man[stype];
    if server_session==nil then
       return; 
    end

    --客户端uid用户断开连接，转发事件给与网关连接Stype类型的服务器
    if uid ~= 0 then
        print(uid,"lost_connect")
        local user_lost={stype,Cmd.eUserLostConn,uid,nil};
        Session.send_msg(server_session,user_lost);
    end 

end 


gw_service_init();

local gw_service={
    on_session_recv_raw_cmd=on_gw_recv_raw_cmd,
    on_session_disconnect=on_gw_session_disconnect,
};

return gw_service;