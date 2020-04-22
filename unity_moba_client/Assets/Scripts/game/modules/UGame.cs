using System.Collections;
using System.Collections.Generic;
using gprotocol;
using TMPro;
using UnityEngine;

//玩家信息保存类
public class UGame : Singleton<UGame>
{
    public string uNick = "";
    public int uFace = 1;
    public int uSex = 0;
    public int uVip = 0;
    public bool isGuest = false;
    public void SaveUInfo(UserCenterInfo uinfo,bool is_guest)
    {
        this.uNick = uinfo.unick;
        this.uFace = uinfo.uface;
        this.uSex = uinfo.usex;
        this.uVip = uinfo.uvip;
        this.isGuest = is_guest;
    }

    public void SaveEditProfile(string unick,int uface,int usex)
    {
        this.uNick = unick;
        this.uFace = uface;
        this.uSex = usex;
        
    }

}
