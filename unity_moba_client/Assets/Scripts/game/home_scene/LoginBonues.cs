using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoginBonues : MonoBehaviour
{
    public GameObject[] fingerPrint;
    
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
    }

    public void OnRecvLoginBonuesClick()
    {
        this.gameObject.SetActive(false);
        SystemServiceProxy.Instance.RecvLoginBonues();
    }

    public void OnCloseLoginBonues()
    {
        this.gameObject.SetActive(false);
    }
}
