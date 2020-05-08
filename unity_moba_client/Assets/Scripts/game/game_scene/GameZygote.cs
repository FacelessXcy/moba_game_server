using System;
using System.Collections;
using System.Collections.Generic;
using gprotocol;
using TMPro;
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

public enum SideType
{
    SideA=0,
    SideB=1,
}

public enum ObjectType
{
    Bullet=12,
    Hero=13,
    Tower=14,
}

public class GameZygote : UnitySingleton<GameZygote>
{
    //test
    public Joystick stick;
    //end
    
    public GameObject[] heroCharacters;//男、女
    
    public GameObject entryA;//SideA 出生点
    public GameObject entryB;//SideB 出生点

    private int _syncFrameID = 0;
    private FrameOpts _lastFrameOpts;
    
    private List<Hero> _heroes = new List<Hero>();
    
    
    private List<Tower> _ATowers=new List<Tower>();
    private List<Tower> _BTowers=new List<Tower>();
    
    public GameObject[] ATowerObjects;//[主塔,left,right,front]
    public GameObject[] BTowerObjects;//[主塔,left,right,front]

    public GameObject normalBulletPrefab;//普通塔子弹
    public GameObject mainBulletPrefab;//主塔子弹
    
    public const int LOGIC_FRAME_TIME = 66;//逻辑帧间隔时间

    private List<Bullet> _towerBullets=new List<Bullet>();//bullet集合


    public override void Awake()
    {
        _destoryOnLoad = true;
        base.Awake();
    }

    private void Start()
    {
        EventManager.Instance.AddEventListener("on_logic_update",
            OnLogicUpdate);
        //UGame.Instance.uSex = 1;
        
        //创建英雄
        PlaceHeroes();
        
        //创建防御塔
        PlaceTowers();
    }

    private void OnDestroy()
    {
        EventManager.Instance.RemoveEventListener("on_logic_update",
            OnLogicUpdate);
    }

    public Bullet AllocBullet(int side,int type)
    {
        GameObject obj = null;
        Bullet bullet=null;
        switch (type)
        {
            case (int)BulletType.Normal:
                obj = GameObject.Instantiate(this.normalBulletPrefab);
                obj.transform.SetParent(this.transform,false);
                bullet = obj.AddComponent<NormalBullet>();
                bullet.Init(side,type);
                break;
            case (int)BulletType.Main:
                obj = GameObject.Instantiate(this.mainBulletPrefab);
                obj.transform.SetParent(this.transform,false);
                bullet = obj.AddComponent<MainBullet>();
                bullet.Init(side,type);
                break;
        }
        if (bullet!=null)
        {
            this._towerBullets.Add(bullet);
        }
        return bullet;
    }

    public void RemoveBullet(Bullet bullet)
    {
        if (bullet!=null)
        {
            this._towerBullets.Remove(bullet);
        }
        GameObject.Destroy(bullet.gameObject);
    }

    public List<Hero> GetHeroes()
    {
        return this._heroes;
    }

    private void PlaceTowers()
    {
        //sideA
        Tower t;
        t = this.ATowerObjects[0].AddComponent<MainTower>();
        t.Init((int)SideType.SideA,(int)TowerType.Main);
        this._ATowers.Add(t);//主塔
        t.gameObject.name = "A_main_tower";
        
        t = this.ATowerObjects[1].AddComponent<NormalTower>();
        t.Init((int)SideType.SideA,(int)TowerType.Main);
        this._ATowers.Add(t);//left
        t.gameObject.name = "A_left_tower";
        
        t = this.ATowerObjects[2].AddComponent<NormalTower>();
        t.Init((int)SideType.SideA,(int)TowerType.Main);
        this._ATowers.Add(t);//right
        t.gameObject.name = "A_right_tower";
        
        t = this.ATowerObjects[3].AddComponent<NormalTower>();
        t.Init((int)SideType.SideA,(int)TowerType.Main);
        this._ATowers.Add(t);//front
        t.gameObject.name = "A_front_tower";
        
        //sideB
        t = this.BTowerObjects[0].AddComponent<MainTower>();
        t.Init((int)SideType.SideB,(int)TowerType.Main);
        this._BTowers.Add(t);//主塔
        t.gameObject.name = "B_main_tower";
        
        t = this.BTowerObjects[1].AddComponent<NormalTower>();
        t.Init((int)SideType.SideB,(int)TowerType.Main);
        this._BTowers.Add(t);//left
        t.gameObject.name = "B_left_tower";
        
        t = this.BTowerObjects[2].AddComponent<NormalTower>();
        t.Init((int)SideType.SideB,(int)TowerType.Main);
        this._BTowers.Add(t);//right
        t.gameObject.name = "B_right_tower";
        
        t = this.BTowerObjects[3].AddComponent<NormalTower>();
        t.Init((int)SideType.SideB,(int)TowerType.Main);
        this._BTowers.Add(t);//front
        t.gameObject.name = "B_front_tower";
    }

    private void PlaceHeroes()
    {
        //放置英雄
        Hero h;
        h = PlaceHeroAt(UGame.Instance.playersMatchInfo[0],0);//sideA的0号位
        this._heroes.Add(h);
        h = PlaceHeroAt(UGame.Instance.playersMatchInfo[1],1);//sideA的1号位
        this._heroes.Add(h);
        h = PlaceHeroAt(UGame.Instance.playersMatchInfo[2],0);//sideB的0号位
        this._heroes.Add(h);
        h = PlaceHeroAt(UGame.Instance.playersMatchInfo[3],1);//sideB的1号位
        this._heroes.Add(h);
    }

    public Hero GetHeroBySeatID(int seatid)
    {
        for (int i = 0; i < this._heroes.Count; i++)
        {
            if (this._heroes[i].seatid==seatid)
            {
                return this._heroes[i];
            }
        }
        return null;
    }

    private Hero PlaceHeroAt(PlayerMatchInfo matchInfo,int index)
    {
        int side = matchInfo.side;
        UserInfo uinfo = UGame.Instance.getUserInfo(matchInfo.seatid);
        GameObject heroObject = Instantiate(this.heroCharacters[uinfo
        .usex]);
        heroObject.name = uinfo.unick;
        heroObject.transform.SetParent(this.transform,false);
        Vector3 centerPos;
        if (side==0)
        {
            centerPos = this.entryA.transform.position;
        }
        else
        {
            centerPos = this.entryB.transform.position;
        }

        if (index==0)
        {
            centerPos.z -= 3.0f;
        }
        else
        {
            centerPos.z += 3.0f;
        }

        heroObject.transform.position = centerPos;
        Hero ctrl = heroObject.AddComponent<Hero>();
        ctrl.isGhost = (matchInfo.seatid == UGame.Instance.selfSeatid)
            ? false
            : true;
        ctrl.LoginInit(heroObject.transform.position);//逻辑数据初始化
        ctrl.seatid = matchInfo.seatid;
        ctrl.side = side;
        return ctrl;
    }

    private void OnFrameHandleTowerLogic()
    {
        for (int i = 0; i < _ATowers.Count; i++)
        {
            this._ATowers[i].OnLogicUpdate(LOGIC_FRAME_TIME);
        }

        for (int i = 0; i < _BTowers.Count; i++)
        {
            this._BTowers[i].OnLogicUpdate(LOGIC_FRAME_TIME);
        }
    }
    
    private void OnFrameHandleTowerBulletLogic()
    {
        for (int i = 0; i < this._towerBullets.Count; i++)
        {
            this._towerBullets[i].OnLogicUpdate(LOGIC_FRAME_TIME);
        }
        
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
            int seatid = frameOpts.opts[i].seatid;
            Hero h = GetHeroBySeatID(seatid);
            if (h==null)
            {
                Debug.Log("cannot find hero by seatid: "+seatid);
                continue;
            }
            h.OnHandlerFrameEvent(frameOpts.opts[i]);
        }

        OnFrameHandleTowerBulletLogic();
        
        //塔和怪物AI根据操作产生相应
        OnFrameHandleTowerLogic();
    }

    private void OnSyncLastLogicFrame(FrameOpts frameOpts)
    {
        //把所有玩家的操作进行数据同步，同步的时间间隔(逻辑帧的时间间隔)
        for (int i = 0; i < frameOpts.opts.Count; i++)
        {
            int seatid = frameOpts.opts[i].seatid;
            Hero h = GetHeroBySeatID(seatid);
            if (h==null)
            {
                Debug.Log("cannot find hero by seatid: "+seatid);
                continue;
            }
            h.OnSyncLastLogicFrame(frameOpts.opts[i]);
        }
        
    }

    private void OnJumpToNextFrame(FrameOpts frameOpts)
    {
        //把所有玩家的操作带入处理
        for (int i = 0; i < frameOpts.opts.Count; i++)
        {
            int seatid = frameOpts.opts[i].seatid;
            Hero h = GetHeroBySeatID(seatid);
            if (h==null)
            {
                Debug.Log("cannot find hero by seatid: "+seatid);
                continue;
            }
            h.OnJumpToNextFrame(frameOpts.opts[i]);
        }
        OnFrameHandleTowerBulletLogic();
        //塔和怪物AI根据操作产生相应
        OnFrameHandleTowerLogic();
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
