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

    private void OnEnterZoneReturn(cmd_msg msg)
    {
        EnterZoneRes res = proto_man
            .protobuf_deserialize<EnterZoneRes>(msg.body);
        if (res==null)
        {
            return;
        }
        if (res.status!=Response.OK)
        {
            Debug.Log("Enter Zone status:"+res.status);
            return;
        }
        Debug.Log("enter zone success!!!");
    }

    private void OnEnterMatchReturn(cmd_msg msg)
    {
        EnterMatch res = proto_man
            .protobuf_deserialize<EnterMatch>(msg.body);
        if (res==null)
        {
            return;
        }
        Debug.Log("enter success zid "+res.zid+" match id: "+res.matchid);
    }

    private void OnUserArrivedReturn(cmd_msg msg)
    {
        UserArrived res = proto_man
            .protobuf_deserialize<UserArrived>(msg.body);
        if (res==null)
        {
            return;
        }
        Debug.Log(res.unick+" user arrived!");
    }

    void OnLogicServerReturn(cmd_msg msg)
    {
        //Debug.Log(msg.ctype.ToString());
        switch (msg.ctype)
        {
            case (int)Cmd.eLoginLogicRes:
                OnLoginLogicServerReturn(msg);
                break;
            case (int)Cmd.eEnterZoneRes:
                OnEnterZoneReturn(msg);
                break;
            case (int)Cmd.eEnterMatch:
                OnEnterMatchReturn(msg);
                break;
            case (int)Cmd.eUserArrived:
                OnUserArrivedReturn(msg);
                break;
        }
    }

    public void LoginLogicServer()
    {
        network.Instance.send_protobuf_cmd((int)Stype.Logic,(int)Cmd
            .eLoginLogicReq,null);
    }

    public void EnterZone(int zid)
    {
        if (zid<Zone.SGYD||zid>Zone.ASSY)
        {
            return;
        }
        EnterZoneReq req=new EnterZoneReq();
        req.zid = zid;
        network.Instance.send_protobuf_cmd((int)Stype.Logic,(int)Cmd
        .eEnterZoneReq,req);
    }


}
