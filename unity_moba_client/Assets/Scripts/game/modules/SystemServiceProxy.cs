using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using gprotocol;
using UnityEngine.PlayerLoop;

public class SystemServiceProxy:Singleton<SystemServiceProxy>
{
    private int _verNum = 0;
    private List<string> _sysMsgs=null;
    public void Init()
    {
        network.Instance.add_service_listeners((int) Stype.System,
            OnSystemServerReturn);
    }

    private void OnGetUgameInfoReturn(cmd_msg msg)
    {
        GetUgameInfoRes res = proto_man
            .protobuf_deserialize<GetUgameInfoRes>(msg.body);
        if (res==null)
        {
            return;
        }
        if (res.status!=Response.OK)
        {
            Debug.Log("Get Uname Info status:"+res.status);
            return;
        }
        UserGameInfo uinfo = res.uinfo;
        UGame.Instance.SaveUGameInfo(uinfo);
        
        EventManager.Instance.DispatchEvent("get_ugame_info_success",null);
        EventManager.Instance.DispatchEvent("sync_ugame_info",null);
        
    }

    public void OnRecvLoginBonuesReturn(cmd_msg msg)
    {
        RecvLoginBonuesRes res = proto_man
            .protobuf_deserialize<RecvLoginBonuesRes>(msg.body);
        if (res==null)
        {
            return;
        }
        if (res.status!=Response.OK)
        {
            Debug.Log("recv login bonues status:"+res.status);
            return;
        }

        UGame.Instance.uGameInfo.uchip +=
            UGame.Instance.uGameInfo.bonues;
        UGame.Instance.uGameInfo.bonues_status = 1;
        
        EventManager.Instance.DispatchEvent("sync_ugame_info",null);
    }

    private void OnGetWorldUChipRankInfoReturn(cmd_msg msg)
    {
        GetWorldRankUchipRes res = proto_man
            .protobuf_deserialize<GetWorldRankUchipRes>(msg.body);
        if (res==null)
        {
            return;
        }
        if (res.status!=Response.OK)
        {
            Debug.Log("recv World Rank status:"+res.status);
            return;
        }
        
        //获取得到排行榜数据
        
        EventManager.Instance.DispatchEvent("get_rank_list",res.rank_info);
        
    }

    private void OnGetSysMsgReturn(cmd_msg msg)
    {
        GetSysMsgRes res = proto_man
            .protobuf_deserialize<GetSysMsgRes>(msg.body);
        if (res==null)
        {
            return;
        }
        if (res.status!=Response.OK)
        {
            Debug.Log("get sys msg status:"+res.status);
            return;
        }
        Debug.Log("get sys msg success!!");
        
        if (this._verNum==res.ver_num)
        {//本地版本号和服务器相同，使用本地数据
            Debug.Log("use local data");
        }
        else
        {
            this._verNum = res.ver_num;
            this._sysMsgs = res.sys_msgs;
            Debug.Log("sync server data");
        }

        if (this._sysMsgs!=null)
        {
            for (int i = 0; i < this._sysMsgs.Count; i++)
            {
                Debug.Log(this._sysMsgs[i]);
            }
        }
        
        EventManager.Instance.DispatchEvent("get_sys_email",
        this._sysMsgs);
    }

    void OnSystemServerReturn(cmd_msg msg)
    {
        switch (msg.ctype)
        {
            case (int)Cmd.eGetUgameInfoRes:
                OnGetUgameInfoReturn(msg);
                break;
            case (int)Cmd.eRecvLoginBonuesRes:
                OnRecvLoginBonuesReturn(msg);
                break;
            case (int)Cmd.eGetWorldRankUchipRes:
                OnGetWorldUChipRankInfoReturn(msg);
                break;
            case (int)Cmd.eGetSysMsgRes:
                OnGetSysMsgReturn(msg);
                break;
        }
    }

    public void LoadUserUGameInfo()
    {
        network.Instance.send_protobuf_cmd((int)Stype.System,(int)Cmd
            .eGetUgameInfoReq,null);
    }

    public void RecvLoginBonues()
    {
        network.Instance.send_protobuf_cmd((int)Stype.System,(int)Cmd
        .eRecvLoginBonuesReq,null);
    }

    public void GetWorldUChipRankInfo()
    {
        network.Instance.send_protobuf_cmd((int)Stype.System,(int)Cmd
        .eGetWorldRankUchipReq,null);
    }

    public void GetSysMsg()
    {
        GetSysMsgReq req=new GetSysMsgReq();
        req.ver_num = this._verNum;
        
        network.Instance.send_protobuf_cmd((int)Stype.System,
            (int)Cmd.eGetSysMsgReq,req);
    }

}
