using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using gprotocol;
using UnityEngine.PlayerLoop;

public class LogicServiceProxy:Singleton<LogicServiceProxy>
{

    public void Init()
    {
        network.Instance.add_service_listeners((int)Stype.Logic,OnLogicServerReturn);
    }

    private void OnLoginLogicServerReturn(cmd_msg msg)
    {
        //Debug.Log("OnLoginLogicServerReturn");
        LoginLogicRes res = proto_man
            .protobuf_deserialize<LoginLogicRes>(msg.body);
        if (res==null)
        {
            return;
        }
        if (res.status!=Response.OK)
        {
            Debug.Log("Login Logic Server status:"+res.status);
            return;
        }
        //Debug.Log("Login Logic Server success!!");
        //登录成功
        EventManager.Instance.DispatchEvent("login_logic_server",null);
        
    }

    void OnLogicServerReturn(cmd_msg msg)
    {
        //Debug.Log(msg.ctype.ToString());
        switch (msg.ctype)
        {
            case (int)Cmd.eLoginLogicRes:
                OnLoginLogicServerReturn(msg);
                break;
        }
    }

    public void LoginLogicServer()
    {
        network.Instance.send_protobuf_cmd((int)Stype.Logic,(int)Cmd
            .eLoginLogicReq,null);
    }




}
