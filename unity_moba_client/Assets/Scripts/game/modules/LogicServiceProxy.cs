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

    void OnLogicServerReturn(cmd_msg msg)
    {
        switch (msg.ctype)
        {
            
        }
    }
    

}
