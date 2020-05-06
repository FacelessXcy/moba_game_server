using System;
using System.Collections;
using System.Collections.Generic;
using gprotocol;
using UnityEngine;

public enum OptType
{
    JoyStick=1,
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
    private FrameOpts _lastFrameOpts;
    private Hero _testHero;
    
    public const int LOGIC_FRAME_TIME = 66;//逻辑帧间隔时间

    private void Start()
    {
        EventManager.Instance.AddEventListener("on_logic_update",
        OnLogicUpdate);
        //UGame.Instance.uSex = 1;
        GameObject hero = Instantiate(this.heroCharacters[UGame.Instance.uSex]);
        hero.transform.SetParent(this.transform,false);
        hero.transform.position = this.entryA.transform.position;
        Hero ctrl = hero.AddComponent<Hero>();
        ctrl.isGhost = false;//自己控制
        ctrl.LoginInit(hero.transform.position);//逻辑数据初始化
        this._testHero = ctrl;
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
        optStick.opt_type = (int)OptType.JoyStick;
        optStick.x = (int)(this.stick.TouchDir.x * (1 << 16));//定点数
        optStick.y = (int)(this.stick.TouchDir.y * (1 << 16));//定点数
        nextFrameOpts.opts.Add(optStick);

        //攻击
        
        //发送给服务器
        LogicServiceProxy.Instance.SendNextFrameOpts(nextFrameOpts);
    }

    /// <summary>
    /// 处理上一帧操作
    /// </summary>
    /// <param name="frameOpts"></param>
    private void OnHandlerFrameEvent(FrameOpts frameOpts)
    {
        //把所有玩家的操作带入处理
        for (int i = 0; i < frameOpts.opts.Count; i++)
        {
            if (frameOpts.opts[i].seatid==UGame.Instance.selfSeatid)
            {
                this._testHero.OnHandlerFrameEvent(frameOpts.opts[i]);
            }
        }
        
        //怪物AI根据操作产生相应
        
    }

    private void OnSyncLastLogicFrame(FrameOpts frameOpts)
    {
        //把所有玩家的操作进行数据同步，同步的时间间隔(逻辑帧的时间间隔)
        for (int i = 0; i < frameOpts.opts.Count; i++)
        {
            if (frameOpts.opts[i].seatid==UGame.Instance.selfSeatid)
            {
                this._testHero.OnSyncLastLogicFrame(frameOpts.opts[i]);
            }
        }
        
    }

    private void OnJumpToNextFrame(FrameOpts frameOpts)
    {
        //把所有玩家的操作带入处理
        for (int i = 0; i < frameOpts.opts.Count; i++)
        {
            if (frameOpts.opts[i].seatid==UGame.Instance.selfSeatid)
            {
                this._testHero.OnJumpToNextFrame(frameOpts.opts[i]);
            }
        }
        //怪物AI根据操作产生相应
        
        
    }

    private void OnLogicUpdate(string eventName,object udata)
    {
        LogicFrame frame = (LogicFrame) udata;
        if (frame.frameid<this._syncFrameID)//收到以前发来的操作帧，直接忽略
        {
            return;
        }
        

        //同步自己客户端上一帧的逻辑操作，调整位置到真实位置:
        //调整完以后，客户端同步到syncFrameID
        if (this._lastFrameOpts!=null)
        {
            OnSyncLastLogicFrame(this._lastFrameOpts);
            //此时，所有玩家的位置都同步到了正确的位置。
        }
        
        //从syncFrameID+1开始-->frame.frameid-1
        //同步丢失的帧，快速同步到当前帧
        //所有客户端的数据都被同步到frame.frameid-1
        for (int i = 0; i < frame.unsync_frames.Count; i++)
        {
            if (this._syncFrameID>=frame.unsync_frames[i].frameid)
            {
                continue;
            }
            if (frame.unsync_frames[i].frameid>=frame.frameid)
            {
                break;
            }

            OnJumpToNextFrame(frame.unsync_frames[i]);
        }
        
        
        //获取最后一个操作 frame.frameid的操作
        //根据这个操作来处理，播放动画，产生的位移为“假位移”
        this._syncFrameID = frame.frameid;//同步到的事件帧ID
        if (frame.unsync_frames.Count>0)
        {
            this._lastFrameOpts = frame.unsync_frames[frame.unsync_frames.Count - 1];
            OnHandlerFrameEvent(this._lastFrameOpts);
        }
        else
        {
            _lastFrameOpts = null;
        }
        //采集下一个帧的事件，发送给服务器
        CapturePlayerOpts();
    }
    
    
}
