using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamMatch : MonoBehaviour
{


    public void OnBeginMatchClick()
    {
        int zid = UGame.Instance.zid;
        Debug.Log(zid);
        LogicServiceProxy.Instance.EnterZone(zid);
    }

}
