using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using gprotocol;

public class game_scene : MonoBehaviour
{
    
    private void Start()
    {
        network.Instance.add_service_listeners((int)Stype.Auth,
        on_auth_server_return);
        
        this.Invoke(nameof(test),3.0f);
        
    }

    private void test()
    {
        Debug.Log("Test");
        network.Instance.send_protobuf_cmd((int)Stype.Auth,(int)Cmd
        .eLoginReq,null);
    }

    void on_auth_server_return(cmd_msg msg)
    {
        switch (msg.ctype)
        {
            case (int)Cmd.eLoginRes:
                break;
        }
    }
    
}
