using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class HomeScene : MonoBehaviour
{
    public Text unick;
    public Text uchipLabel;
    public Text diamondLabel;
    
    
    public Image header;
    public Sprite[] ufaceImg;

    public GameObject uinfoDlgPrefab;
    
    private void Start()
    {
        EventManager.Instance.AddEventListener("sync_uinfo",SyncUInfo);
        EventManager.Instance.AddEventListener("sync_ugame_info",SyncUGameInfo);
        EventManager.Instance.AddEventListener("login_out",OnUserLoginOut);
        SyncUInfo("sync_uinfo",null);
        SyncUGameInfo("sync_ugame_info",null);
    }

    private void OnDestroy()
    {
        EventManager.Instance.RemoveEventListener("sync_uinfo",
            SyncUInfo);
        EventManager.Instance.RemoveEventListener("login_out",
            OnUserLoginOut);
    }
    
    private void OnUserLoginOut(string name,object udata)
    {
        SceneManager.LoadScene("Scenes/login");
    }
    
    //负责同步游戏信息
    private void SyncUGameInfo(string name,object udata)
    {
        if (this.uchipLabel!=null)
        {
            this.uchipLabel.text =
                UGame.Instance.uGameInfo.uchip.ToString();
        }
        if (this.diamondLabel!=null)
        {
            this.diamondLabel.text = UGame.Instance.uGameInfo.uchip2
            .ToString();
        }
        
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
