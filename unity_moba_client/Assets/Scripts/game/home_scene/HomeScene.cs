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
    public GameObject loginBonues;
    public GameObject rankListPrefab;
    public GameObject emailListPrefab;

    public Text ulevelLabel;
    public Text expressLabel;
    public Image expressProcess;
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
        //计算等级信息，并显示
        int now_exp,next_level_exp;
        int level = ULevel.Instance.GetLevelInfo(UGame.Instance
            .uGameInfo.uexp,out now_exp,out next_level_exp);
        if (this.ulevelLabel!=null)
        {
            this.ulevelLabel.text = "LV\n" + level;
        }

        if (this.expressLabel!=null)
        {
            this.expressLabel.text = now_exp + " / " + next_level_exp;
        }

        if (this.expressProcess!=null)
        {
            this.expressProcess.fillAmount =
                (float) now_exp / next_level_exp;
        }
        
        //同步登录奖励信息
        if (UGame.Instance.uGameInfo.bonues_status==0)//有登录奖励可以领取
        {
            loginBonues.SetActive(true);
            loginBonues.GetComponent<LoginBonues>().ShowLoginBonues
            (UGame.Instance.uGameInfo.days);
        }
        else
        {
            loginBonues.SetActive(false);
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

    public void OnGetRankClick()
    {
        GameObject rankList =
            GameObject.Instantiate(this.rankListPrefab);
        rankList.transform.SetParent(this.transform,false);
    }

    public void OnGetSysMsgClick()
    {
        //SystemServiceProxy.Instance.GetSysMsg();
        GameObject sysEmail = Instantiate(this.emailListPrefab);
        sysEmail.transform.SetParent(this.transform,false);
    }

}
