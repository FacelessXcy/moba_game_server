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

    private void OnGuestLoginReturn(cmd_msg msg)
    {
        GuestLoginRes res = proto_man
            .protobuf_deserialize<GuestLoginRes>(msg.body);
        if (res==null)
        {
            return;
        }

        if (res.status!=Response.OK)
        {
            Debug.Log("Guest Login status:"+res.status);
            return;
        }

        UserCenterInfo uinfo = res.uinfo;
        Debug.Log(uinfo.unick+"   "+uinfo.usex);
        EventManager.Instance.DispatchEvent("login_success",null);
    }

    void OnAuthServerReturn(cmd_msg msg)
    {
        switch (msg.ctype)
        {
            case (int)Cmd.eGuestLoginRes:
                OnGuestLoginReturn(msg);
                break;;
                
        }
        
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
