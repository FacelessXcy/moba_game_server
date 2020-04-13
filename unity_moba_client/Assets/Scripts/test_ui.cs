using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test_ui : MonoBehaviour
{
    public AudioClip clip;
    private void Start()
    {
        EventManager.Instance.AddEventListener("coin",test);
        AudioManager.Instance.PlayMusic(clip);
    }

    private void test(string name,object udata)
    {
        int coin = (int)udata;
        Debug.Log("coin="+coin);
        AudioManager.Instance.EnableMusic(false);
    }

    private void OnDestroy()
    {
        EventManager.Instance.RemoveEventListener("coin",test);
    }
}
