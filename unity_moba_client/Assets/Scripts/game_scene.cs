using System;
using System.Collections;
using System.Collections.Generic;
using gprotocol;
using UnityEngine;

public class game_scene : MonoBehaviour
{
    private void Start()
    {
        Invoke(nameof(test),5.0f);
        
        network.Instance.add_service_listeners(1,on_service_event);
        
    }

    void on_service_event(cmd_msg msg)
    {
        switch (msg.ctype)
        {
            case 2:
                gprotocol.LoginRes res = proto_man
                    .protobuf_deserialize<gprotocol.LoginRes>(msg.body);
                Debug.Log("res= "+res.status);
                break;
        }
    }

    private void OnDestroy()
    {
        if (network.Instance)
        {
            network.Instance.remove_service_listener(1,this.on_service_event);
        }
    }

    void test()
    {
        LoginReq req=new LoginReq();
        req.name = "xcy";
        req.email = "4176@qq.com";
        req.age = 21;
        req.int_set = 8;
        network.Instance.send_protobuf_cmd(1,1,req);
    }
    
    
}
