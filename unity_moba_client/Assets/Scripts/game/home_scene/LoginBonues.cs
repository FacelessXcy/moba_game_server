using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoginBonues : MonoBehaviour
{
    public GameObject[] fingerPrint;
    public GameObject recvButton;
    
    //显示指纹图标
    public void ShowLoginBonues(int days)
    {
        int i;
        for (i = 0; i < days; i++)
        {
            this.fingerPrint[i].SetActive(true);
        }

        for (; i < 5; i++)
        {
            this.fingerPrint[i].SetActive(false);
        }

        if (UGame.Instance.uGameInfo.bonues_status==0)
        {//有登录奖励可领取
            this.recvButton.SetActive(true);
        }
        else
        {
            this.recvButton.SetActive(false);
        }
    }

    public void OnRecvLoginBonuesClick()
    {
        SystemServiceProxy.Instance.RecvLoginBonues();
        GameObject.Destroy(this.gameObject);
    }

    public void OnCloseLoginBonues()
    {
        GameObject.Destroy(this.gameObject);
    }
}
