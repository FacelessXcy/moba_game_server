local Response=require("Response");
local Stype=require("Stype");
local Cmd=require("Cmd");
local mysql_game=require("database/mysql_game");

local sys_msg_data={}--存放msg
local sys_msg_version=0;--什么时候加载的，设置一个时间戳，作为版本号

function load_sys_msg(  )
    mysql_game.get_sys_msg(
    function ( err,ret )
        if err then
            Scheduler.once(load_sys_msg,5000);
            return;
        end

        sys_msg_version=Utils.timestamp();

        if ret ==nil or #ret <= 0 then--数据库中没有任何系统消息
            sys_msg_data={}
            return;
        end

        sys_msg_data=ret;
        
        --过了晚上12点，再更新一次
        local tomorrow=Utils.timestamp_today()+25*60*60;
        Scheduler.once(load_sys_msg,(tomorrow-sys_msg_version)*1000);
        -- local k,v;
        -- for k,v in pairs(sys_msg_data) do
        --     print(v);
        -- end

    end)
end

Scheduler.once(load_sys_msg,5000);

--{stype,ctype,utag,[{message} or jsonStr]}
function get_sys_msg( s,req )
    local uid=req[3];
    local body=req[4];

    if body.ver_num == sys_msg_version then--版本号相同
        local msg={Stype.System,Cmd.eGetSysMsgRes,uid,{
            status=Response.OK,
            ver_num=sys_msg_version,
        }};
        Session.send_msg(s,msg);
    end

    local msg={Stype.System,Cmd.eGetSysMsgRes,uid,{
        status=Response.OK,
        ver_num=sys_msg_version,
        sys_msgs=sys_msg_data,
    }};
    Session.send_msg(s,msg);

end

local sys_msg={
    get_sys_msg=get_sys_msg,
}

return sys_msg;