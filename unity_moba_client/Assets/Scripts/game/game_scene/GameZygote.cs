using System;
using System.Collections;
using System.Collections.Generic;
using gprotocol;
using UnityEngine;

enum OptType
{
    JotStick=1,
    Attack1=2,
    Attack2=3,
    Attack3=4,
    Skill1=5,
    Skill2=6,
    Skill3=7,
}
public class GameZygote : MonoBehaviour
{
    //test
    public Joystick stick;
    //end
    
    public GameObject[] heroCharacters;//男、女
    
    public GameObject entryA;

    private int _syncFrameID = 0;
    //private FrameOpts _lastFrameOpts;

    private void Start()
    {
        EventManager.Instance.AddEventListener("on_logic_update",
        OnLogicUpdate);
        //UGame.Instance.uSex = 1;
        GameObject hero = Instantiate(this.heroCharacters[UGame.Instance.uSex]);
        hero.transform.SetParent(this.transform,false);
        hero.transform.position = this.entryA.transform.position;
        CharacterCtrl ctrl = hero.AddComponent<CharacterCtrl>();
        ctrl.isGhost = false;//自己控制
        ctrl.stick = this.stick;//测试
        
    }

    private void CapturePlayerOpts()
    {
        NextFrameOpts nextFrameOpts=new NextFrameOpts();
        nextFrameOpts.frameid = this._syncFrameID + 1;
        nextFrameOpts.zid = UGame.Instance.zid;
        nextFrameOpts.matchid = UGame.Instance.matchid;
        nextFrameOpts.seatid = UGame.Instance.selfSeatid;
        //摇杆操作收集
        OptionEvent optStick=new OptionEvent();
        optStick.seatid = UGame.Instance.selfSeatid;
        optStick.opt_type = (int)OptType.JotStick;
        optStick.x = (int)(this.stick.TouchDir.x * (1 << 16));//定点数
        optStick.y = (int)(this.stick.TouchDir.y * (1 << 16));//定点数
        nextFrameOpts.opts.Add(optStick);

        //攻击
        
        //发送给服务器
        LogicServiceProxy.Instance.SendNextFrameOpts(nextFrameOpts);
    }

    private void OnLogicUpdate(string eventName,object udata)
    {
        LogicFrame frame = (LogicFrame) udata;
        if (frame.frameid<this._syncFrameID)//收到以前发来的操作帧，直接忽略
        {
            return;
        }
        
        Debug.Log(frame.unsync_frames.Count);
        for (int i = 0; i < frame.unsync_frames.Count; i++) 
        {
            for (int j = 0; j < frame.unsync_frames[i].opts.Count; j++)
            {
                Debug.Log(frame.unsync_frames[i].opts[j].x + ":" + frame.unsync_frames[i].opts[j].y);
            }
        }
        //同步自己客户端上一帧的逻辑操作，调整位置:调整完以后，
        //客户端同步到syncFrameID
        
        
        //从syncFrameID+1开始-->frame.frameid-1
        //同步丢失的帧，所有客户端的数据都被同步到frame.frameid-1
        
        
        //获取最后一个操作 frame.frameid的操作
        //根据这个操作来处理，播放动画
        
        
        //采集下一个帧的事件，发送给服务器
        this._syncFrameID = frame.frameid;//同步到的事件帧ID
//        this._lastFrameOpts = frame.unsync_frames[frame.unsync_frames
//                                                      .Count - 1];
        CapturePlayerOpts();
    }
}
