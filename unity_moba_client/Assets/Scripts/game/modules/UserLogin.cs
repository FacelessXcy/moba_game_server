using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using gprotocol;
public class UserLogin:Singleton<UserLogin>
{

    public void Init()
    {
        network.Instance.add_service_listeners((int)Stype.Auth,OnAuthServerReturn);
    }

    void OnAuthServerReturn(cmd_msg msg)
    {
        
        
    }

    public void GuestLogin()
    {
        string g_key = utils.RandStr(32);
        g_key = "to9OveCec00GdBnaTp0mFRashRUsnQJu";
        Debug.Log(g_key);
        GuestLoginReq req=new GuestLoginReq();
        req.guest_key = g_key;
        network.Instance.send_protobuf_cmd((int)Stype.Auth,(int)Cmd
        .eGuestLoginReq,req);
        
    }

}
