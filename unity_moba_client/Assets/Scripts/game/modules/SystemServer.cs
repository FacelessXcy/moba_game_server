using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using gprotocol;
using UnityEngine.PlayerLoop;

public class SystemServer:Singleton<SystemServer>
{

    public void Init()
    {
        network.Instance.add_service_listeners((int) Stype.System,
            OnSystemServerReturn);
    }

    void OnSystemServerReturn(cmd_msg msg)
    {
        
    }

    public void LoadUserUGameInfo()
    {
        network.Instance.send_protobuf_cmd((int)Stype.System,(int)Cmd
            .eGetUgameInfoReq,null);
    }

}
