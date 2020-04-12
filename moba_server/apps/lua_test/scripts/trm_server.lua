--保存所有客户端的集合
local session_set={}
--广播
function broadcast_except( msg,except_session )
    for i=1,#session_set do
        if except_session ~= session_set[i] then
            Session.send_msg(session_set[i],msg);
        end 
    end
end


function on_recv_login_cmd(s)
    --当前是否在集合内，如果是，返回已经在聊天室的提示
    for i=1,#session_set do
        if s==session_set[i] then--返回状态-1
            local msg={1,2,0,{status=-1}};
            Session.send_msg(s,msg);
            return;
        end 
    end
    --加入到当前集合,发送数据给客户端
    table.insert(session_set,s);
    local msg={1,2,0,{status=1}};--返回状态1：表示登陆成功
    Session.send_msg(s,msg);

    local s_ip,s_port=Session.get_address(s);
    msg={1,7,0,{ip=s_ip,port=s_port}};
    broadcast_except(msg,s);

end 

function on_recv_exit_cmd(s)
    for i=1,#session_set do
        if s==session_set[i] then--当前在聊天室内，返回状态1
            table.remove(session_set,i);
            local msg={1,4,0,{status=1}};--返回状态1：表示退出成功
            Session.send_msg(s,msg);

            local s_ip,s_port=Session.get_address(s);
            msg={1,8,0,{ip=s_ip,port=s_port}};
            broadcast_except(msg,s);
            return;
        end 
    end
    --当前不在聊天室内
    local msg={1,4,0,{status=-1}};--返回-1
    Session.send_msg(s,msg);
end 

function on_recv_send_msg_cmd(s,str)
    for i=1,#session_set do
        if s==session_set[i] then--当前在聊天室内，返回状态1
            local msg={1,6,0,{status=1}};--返回状态1：表示发送成功
            Session.send_msg(s,msg);
            local s_ip,s_port=Session.get_address(s);
            msg={1,9,0,{ip=s_ip,port=s_port,content=str}};
            broadcast_except(msg,s);
            return;
        end 
    end
    local msg={1,6,0,{status=-1}};--返回状态-1：表示发送失败
    Session.send_msg(s,msg);
end 


--{stype,ctype,utag,[{message} or jsonStr]}
function on_trm_recv_cmd( s,msg )
    local ctype=msg[2];
    local body=msg[4];
    if ctype==1 then
        on_recv_login_cmd(s);
    elseif ctype==3 then
        on_recv_exit_cmd(s);
    elseif ctype==5 then
        on_recv_send_msg_cmd(s,body.content);
    end
end

function on_trm_session_disconnect( s )
    local ip,port=Session.get_address(s);
    --当前是否在集合内，如果是，返回已经在聊天室的提示
    for i=1,#session_set do
        if s==session_set[i] then--返回状态-1
            print("remove form talk room "..ip..":"..port);
            table.remove(session_set,i);

            local s_ip,s_port=Session.get_address(s);
            local msg={1,8,0,{ip=s_ip,port=s_port}};
            broadcast_except(msg,s);
            return;
        end 
    end
    
end

local trm_service={
    on_session_recv_cmd=on_trm_recv_cmd,
 on_session_disconnect=on_trm_session_disconnect,
};

local trm_server={
    stype=1,
    service=trm_service,
}

return trm_server;