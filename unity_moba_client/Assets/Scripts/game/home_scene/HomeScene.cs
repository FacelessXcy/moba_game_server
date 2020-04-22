using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HomeScene : MonoBehaviour
{
    public Text unick;
    public Image header;
    public Sprite[] ufaceImg;

    public GameObject uinfoDlgPrefab;
    
    private void Start()
    {
        EventManager.Instance.AddEventListener("sync_uinfo",SyncUInfo);
        SyncUInfo("sync_uinfo",null);
    }

    private void OnDestroy()
    {
        EventManager.Instance.RemoveEventListener("sync_uinfo",SyncUInfo);
    }
    
    //负责同步主信息
    private void SyncUInfo(string name,object udata)
    {
        if (this.unick!=null)
        {
            this.unick.text = UGame.Instance.uNick;
        }

        if (this.header!=null)
        {
            this.header.sprite = ufaceImg[UGame.Instance.uFace - 1];
        }
    }

    public void OnUInfoClick()
    {
        GameObject uinfoDlg =
            GameObject.Instantiate(this.uinfoDlgPrefab);
        uinfoDlg.transform.SetParent(this.transform,false);
    }

}
