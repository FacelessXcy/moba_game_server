using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using gprotocol;
using UnityEngine.PlayerLoop;

public class LogicServiceProxy:Singleton<LogicServiceProxy>
{

    public void Init()
    {
        network.Instance.AddServiceListeners((int)Stype.Logic,OnLogicServerReturn);
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
        //Debug.Log("enter success zid "+res.zid+" match id: "+res.matchid);
        UGame.Instance.zid = res.zid;
        UGame.Instance.matchid = res.matchid;
        UGame.Instance.selfSeatid = res.seatid;
        UGame.Instance.selfSide = res.side;
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
        UGame.Instance.otherUsers.Add(res);
        EventManager.Instance.DispatchEvent("user_arrived",res);
    }

    private void OnUserExitReturn(cmd_msg msg)
    {
        ExitMatchRes res = proto_man
            .protobuf_deserialize<ExitMatchRes>(msg.body);
        if (res==null)
        {
            return;
        }
        if (res.status!=Response.OK)
        {
            Debug.Log("exit match status:"+res.status);
            return;
        }
        Debug.Log("exit match success!!!");
        EventManager.Instance.DispatchEvent("exit_match",null);
    }

    private void OnOtherUserExitReturn(cmd_msg msg)
    {
        UserExitMatch res = proto_man
            .protobuf_deserialize<UserExitMatch>(msg.body);
        if (res==null)
        {
            return;
        }

        for (int i = 0; i < UGame.Instance.otherUsers.Count; i++)
        {
            if (UGame.Instance.otherUsers[i].seatid==res.seatid)
            {
                EventManager.Instance.DispatchEvent
                ("other_user_exit",i);
                UGame.Instance.otherUsers.RemoveAt(i);
                return;
            }
        }
    }

    private void OnGameStart(cmd_msg msg)
    {
        GameStart res = proto_man
            .protobuf_deserialize<GameStart>(msg.body);
        if (res==null)
        {
            return;
        }

//        foreach (PlayerMatchInfo info in res.players_match_info)
//        {
//            Debug.Log(info.heroid);
//        }

        UGame.Instance.playersMatchInfo = res.players_match_info;
        
        EventManager.Instance.DispatchEvent("game_start",null);
    }

    private void OnUdpTest(cmd_msg msg)
    {
        UdpTest res = proto_man
            .protobuf_deserialize<UdpTest>(msg.body);
        if (res==null)
        {
            return;
        }
        Debug.Log("Server Udp Return "+res.content);
    }

    private void OnServerLogicFrame(cmd_msg msg)
    {
        LogicFrame res = proto_man
            .protobuf_deserialize<LogicFrame>(msg.body);
        if (res==null)
        {
            return;
        }
        //Debug.Log(res.frameid);//当前帧id，和当前玩家没有同步到的操作
        EventManager.Instance.DispatchEvent("on_logic_update",res);
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
            case (int)Cmd.eExitMatchRes:
                OnUserExitReturn(msg);
                break;
            case (int)Cmd.eUserExitMatch:
                OnOtherUserExitReturn(msg);
                break;
            case (int)Cmd.eGameStart:
                OnGameStart(msg);
                break;
            case (int)Cmd.eUdpTest:
                OnUdpTest(msg);
                break;
            case (int)Cmd.eLogicFrame:
                OnServerLogicFrame(msg);
                break;
        }
    }

    public void LoginLogicServer()
    {
        LoginLogicReq req=new LoginLogicReq();
        req.udp_ip = "127.0.0.1";
        req.udp_port = network.Instance.localUdpPort;
        network.Instance.SendProtobufCmd((int)Stype.Logic,(int)Cmd
            .eLoginLogicReq,req);
    }

    public void EnterZone(int zid)
    {
        if (zid<Zone.SGYD||zid>Zone.ASSY)
        {
            return;
        }
        EnterZoneReq req=new EnterZoneReq();
        req.zid = zid;
        network.Instance.SendProtobufCmd((int)Stype.Logic,(int)Cmd
        .eEnterZoneReq,req);
    }

    public void ExitMatch()
    {
        network.Instance.SendProtobufCmd((int)Stype.Logic,(int)Cmd
            .eExitMatchReq,null);
    }

    public void SendUdpTest(string content)
    {
        UdpTest req=new UdpTest();
        req.content = content;
        network.Instance.UdpSendProtobufCmd((int)Stype.Logic,(int)Cmd
        .eUdpTest,req);
    }

    public void SendNextFrameOpts(NextFrameOpts nextFrameOpts)
    {
        network.Instance.UdpSendProtobufCmd((int)Stype.Logic,(int)Cmd
            .eNextFrameOpts,nextFrameOpts);
    }
}
