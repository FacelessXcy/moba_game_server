using System;
using System.Collections;
using System.Collections.Generic;
using gprotocol;
using UnityEngine;
using UnityEngine.UI;

public class TeamMatch : MonoBehaviour
{
    public ScrollRect scrollView;
    public GameObject optPrefab;
    public Sprite[] ufaceImg;
    private int _memberCount;

    private void Start()
    {
        EventManager.Instance.AddEventListener("user_arrived",OnUserArrived);
        EventManager.Instance.AddEventListener("exit_match", OnSelfExitMatch);
    }

    private void OnDestroy()
    {
        EventManager.Instance.RemoveEventListener("user_arrived",OnUserArrived);
        EventManager.Instance.RemoveEventListener("exit_match", OnSelfExitMatch);
    }

    public void OnBeginMatchClick()
    {
        int zid = UGame.Instance.zid;
        LogicServiceProxy.Instance.EnterZone(zid);
    }

    public void OnExitMatchClick()
    {
        LogicServiceProxy.Instance.ExitMatch();

    }

    private void OnSelfExitMatch(string eventName,object udata)
    {
        UGame.Instance.zid = -1;
        GameObject.Destroy(this.gameObject);
    }

    private void OnUserArrived(string eventName,object udata)
    {
        UserArrived userInfo = (UserArrived) udata;
        this._memberCount++;
        GameObject user = Instantiate(this.optPrefab);
        user.transform.SetParent(this.scrollView.content,false);
        this.scrollView.content.sizeDelta=new Vector2(0, 
            this._memberCount*106);
        user.transform.Find("name").GetComponent<Text>()
            .text = userInfo.unick;
        user.transform.Find("header/avator").GetComponent<Image>()
            .sprite = ufaceImg[userInfo.uface-1];
        user.transform.Find("sex").GetComponent<Text>()
            .text = userInfo.usex==1?"女":"男";
    }

}
