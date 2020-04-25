using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using gprotocol;
using UnityEngine.PlayerLoop;

public class SystemServiceProxy:Singleton<SystemServiceProxy>
{

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

    void OnSystemServerReturn(cmd_msg msg)
    {
        switch (msg.ctype)
        {
            case (int)Cmd.eGetUgameInfoRes:
                OnGetUgameInfoReturn(msg);
                break;
        }
    }

    public void LoadUserUGameInfo()
    {
        network.Instance.send_protobuf_cmd((int)Stype.System,(int)Cmd
            .eGetUgameInfoReq,null);
    }

}
