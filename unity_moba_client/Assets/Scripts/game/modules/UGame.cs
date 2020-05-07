using System.Collections;
using System.Collections.Generic;
using gprotocol;
using TMPro;
using UnityEngine;


public class UserInfo
{
    public string unick;
    public int usex;
    public int uface;
}

//玩家信息保存类
public class UGame : Singleton<UGame>
{
    public string uNick = "";
    public int uFace = 1;
    public int uSex = 0;
    public int uVip = 0;
    public bool isGuest = false;
    public string guestKey;
    public int zid = -1;
    public int matchid=-1;
    public int selfSeatid=-1;
    public int selfSide = -1;
    

    public UserGameInfo uGameInfo;

    public List<UserArrived> otherUsers=new List<UserArrived>();
    public List<PlayerMatchInfo> playersMatchInfo = null;//当局比赛玩家的比赛信息

    /// <summary>
    /// 获取玩家用户信息
    /// </summary>
    /// <param name="seatid"></param>
    /// <returns></returns>
    public UserInfo getUserInfo(int seatid)
    {
        UserInfo uinfo=new UserInfo();
        if (seatid==this.selfSeatid)
        {
            uinfo.unick = this.uNick;
            uinfo.uface = this.uFace;
            uinfo.usex = this.uSex;
            return uinfo;
        }

        for (int i = 0; i < this.otherUsers.Count; i++)
        {
            if (this.otherUsers[i].seatid==seatid)
            {
                uinfo.unick = this.otherUsers[i].unick;
                uinfo.uface = this.otherUsers[i].uface;
                uinfo.usex = this.otherUsers[i].usex;
                return uinfo;
            }
        }
    
        return null;
    }
    /// <summary>
    /// 获取玩家游戏信息
    /// </summary>
    /// <param name="seatid"></param>
    /// <returns></returns>
    public PlayerMatchInfo GetPlayerMatchInfo(int seatid)
    {
        for (int i = 0; i < this.playersMatchInfo.Count; i++)
        {
            if (this.playersMatchInfo[i].seatid==seatid)
            {
                return this.playersMatchInfo[i];
            }
        }

        return null;
    }

    public void SaveUGameInfo(UserGameInfo ugame_info)
    {
        this.uGameInfo = ugame_info;
    }
    public void SaveUInfo(UserCenterInfo uinfo,bool is_guest,string 
    guest_key=null)
    {
        this.uNick = uinfo.unick;
        this.uFace = uinfo.uface;
        this.uSex = uinfo.usex;
        this.uVip = uinfo.uvip;
        this.isGuest = is_guest;
        this.guestKey = guest_key;
    }

    public void SaveEditProfile(string unick,int uface,int usex)
    {
        this.uNick = unick;
        this.uFace = uface;
        this.uSex = usex;
    }

    public void UserLoginOut()
    {
        
    }

}
