using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test_ui : MonoBehaviour
{
    private void Start()
    {
        EventManager.Instance.AddEventListener("coin",test);
    }

    private void test(string name,object udata)
    {
        int coin = (int)udata;
        Debug.Log("coin="+coin);
        
    }

    private void OnDestroy()
    {
        EventManager.Instance.RemoveEventListener("coin",test);
    }
}
