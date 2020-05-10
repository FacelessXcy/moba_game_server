using System;
using System.Collections;
using System.Collections.Generic;
using gprotocol;
using SWS;
using TMPro;
using UnityEngine;

public enum OptType
{
    Invalid=-1,
    JoyStick=1,
    Attack=2,
    Skill1=3,
    Skill2=4,
    Skill3=5,
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
    Monster=15,
}

struct RoadData
{
    public Vector3[] pathSideA;
    public Vector3[] pathSideB;
}

//小兵类型
public enum MonsterType
{
    Monster1=0,
    Monster2=1,
    Monster3=2,
}

public class GameZygote : UnitySingleton<GameZygote>
{
    
    public Joystick stick;
    public UIAttack uiAttack;
    
    
    public GameObject[] heroCharacters;//男、女
    
    public GameObject entryA;//SideA 出生点
    public GameObject entryB;//SideB 出生点

    private int _syncFrameID = 0;
    private FrameOpts _lastFrameOpts;
    
    private List<Hero> _heroes = new List<Hero>();
    private List<Tower> _towers=new List<Tower>();

    public GameObject[] ATowerObjects;//[主塔,left,right,front]
    public GameObject[] BTowerObjects;//[主塔,left,right,front]

    public GameObject normalBulletPrefab;//普通塔子弹
    public GameObject mainBulletPrefab;//主塔子弹
    
    //小兵路径
    public PathManager[] monsterRoads;
    public GameObject[] monsterPrefabs;
    private RoadData[] _roadDataSet;//所有小兵路径数据集合
    
    //血条管理对象
    public UIBloodManager UiBloodManager;
    
    public const int LOGIC_FRAME_TIME = 66;//逻辑帧间隔时间

    private List<Bullet> _towerBullets=new List<Bullet>();//bullet集合
    private List<Monster> _monsters=new List<Monster>();//所有小兵集合
    private int _nowGenMonsterFrame = GameConfig.GenMonsterFrames;

    public override void Awake()
    {
        _destoryOnLoad = true;
        base.Awake();
    }

    private void Start()
    {
        LoadRoadData();//加载路径数据
        EventManager.Instance.AddEventListener("on_logic_update",
            OnLogicUpdate);
        
            
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

    //英雄查找指向性目标攻击
    public GameObject FindHeroAttackTarget(int side,Vector3 center,
    float validR)
    {
        float maxR = validR;
        GameObject target = null;
        //寻找攻击的敌方目标英雄
        for (int i = 0; i < this._heroes.Count; i++)
        {
            if (side==this._heroes[i].side)
            {
                continue;
            }

            Vector3 dir = center - this._heroes[i]
                              .transform.position;
            float len = dir.magnitude;
            if (len<=maxR)
            {
                maxR = len;
                target = this._heroes[i].gameObject;
            }
        }
        if (target)
        {
            return target;
        }
        //寻找攻击的敌方目标小兵
        for (int i = 0; i < this._monsters.Count; i++)
        {
            if (side==this._monsters[i].side)
            {
                continue;
            }
            Vector3 dir = center - this._monsters[i]
                              .transform.position;
            float len = dir.magnitude;
            if (len<=maxR)
            {
                maxR = len;
                target = this._monsters[i].gameObject;
            }
        }
        if (target)
        {
            return target;
        }
        //寻找攻击的敌方目标防御塔
        for (int i = 0; i < this._towers.Count; i++)
        {
            if (side==this._towers[i].side)
            {
                continue;
            }
            Vector3 dir = center - this._towers[i]
                              .transform.position;
            float len = dir.magnitude;
            if (len<=maxR)
            {
                maxR = len;
                target = this._towers[i].gameObject;
            }
        }
        if (target)
        {
            return target;
        }
        return null;
    }

    public List<GameObject> FindTargets(int side, Vector3 center,
        float validR)
    {
        List<GameObject> targets=new List<GameObject>();
        float maxR = validR;
        //寻找攻击的敌方目标英雄
        for (int i = 0; i < this._heroes.Count; i++)
        {
            if (side==this._heroes[i].side)
            {
                continue;
            }

            Vector3 dir = center - this._heroes[i]
                              .transform.position;
            float len = dir.magnitude;
            if (len<=maxR)
            {
                maxR = len;
                targets.Add(this._heroes[i].gameObject); 
            }
        }
        //寻找攻击的敌方目标小兵
        for (int i = 0; i < this._monsters.Count; i++)
        {
            if (side==this._monsters[i].side)
            {
                continue;
            }
            Vector3 dir = center - this._monsters[i]
                              .transform.position;
            float len = dir.magnitude;
            if (len<=maxR)
            {
                maxR = len;
                targets.Add(this._monsters[i].gameObject); 
            }
        }

        //寻找攻击的敌方目标防御塔
        for (int i = 0; i < this._towers.Count; i++)
        {
            if (side==this._towers[i].side)
            {
                continue;
            }
            Vector3 dir = center - this._towers[i]
                              .transform.position;
            float len = dir.magnitude;
            if (len<=maxR)
            {
                maxR = len;
                targets.Add(this._towers[i].gameObject); 
            }
        }

        return targets;
    }

    private void GenThreeMonster()
    {
        //Test，放一个小兵
        PlaceMonster((int) MonsterType.Monster1, (int) SideType.SideA,
            0);
        PlaceMonster((int) MonsterType.Monster1, (int) SideType.SideA,
            1);
        PlaceMonster((int) MonsterType.Monster1, (int) SideType.SideA,
            2);
        //Test，放一个小兵
        PlaceMonster((int) MonsterType.Monster1, (int) SideType.SideB,
            0);
        PlaceMonster((int) MonsterType.Monster1, (int) SideType.SideB,
            1);
        PlaceMonster((int) MonsterType.Monster1, (int) SideType.SideB,
            2);
    }

    private void LoadRoadData()
    {
        this._roadDataSet=new RoadData[this.monsterRoads.Length];
        for (int i = 0; i < this._roadDataSet.Length; i++)
        {
            this._roadDataSet[i].pathSideA = WaypointManager.GetCurved(this
                .monsterRoads[i].GetPathPoints());
            //复制SideB的路径
            int len = this._roadDataSet[i].pathSideA.Length;
            this._roadDataSet[i].pathSideB=new Vector3[len];
            for (int j = 0; j <len; j++)
            {
                this._roadDataSet[i].pathSideB[len - 1 - j]=this
                    ._roadDataSet[i].pathSideA[j];
            }
        }
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

    private void PlaceMonster(int type,int side,int roadIndex)
    {
        if (type<(int)MonsterType.Monster1||
            type>(int)MonsterType.Monster3)
        {
            return;
        }
        if (side!=(int)SideType.SideA&&
            side!=(int)SideType.SideB)
        {
            return;
        }
        if (roadIndex<0||
            roadIndex>=this.monsterRoads.Length)
        {
            return;
        }
        if (type>=this.monsterPrefabs.Length)
        {//没有对应小怪的资源
            return;
        }
        //Debug.Log("放置小兵");
        GameObject m = Instantiate(this.monsterPrefabs[type]);
        m.transform.SetParent(this.transform,false);
        MonsterMove agent = m.AddComponent<MonsterMove>();
        Vector3[] roadData = null;
        if (side==(int)SideType.SideA)
        {   
            roadData = this._roadDataSet[roadIndex].pathSideA;
        }
        else
        {
            roadData = this._roadDataSet[roadIndex].pathSideB;
        }

        Monster monster = m.AddComponent<Monster>();
        monster.Init(type,side,roadData);
        this._monsters.Add(monster);
        
        //创建一个UI血条
        UIShowBlood uiBlood =
            this.UiBloodManager.PlaceUIBloodOnMonster(side);
        monster.uiBlood = uiBlood;
    }

    public void RemoveMonster(Monster monster)
    {
        this._monsters.Remove(monster);
        GameObject.Destroy(monster.gameObject);
    }

    private void PlaceTowers()
    {
        //sideA
        Tower t;
        t = this.ATowerObjects[0].AddComponent<MainTower>();
        t.Init((int)SideType.SideA,(int)TowerType.Main);
        this._towers.Add(t);//主塔
        t.gameObject.name = "A_main_tower";
        //创建一个UI血条
        UIShowBlood uiBlood =
            this.UiBloodManager.PlaceUIBloodOnTower((int)SideType.SideA);
        t.uiBlood = uiBlood;
        
        t = this.ATowerObjects[1].AddComponent<NormalTower>();
        t.Init((int)SideType.SideA,(int)TowerType.Main);
        this._towers.Add(t);//left
        t.gameObject.name = "A_left_tower";
        //创建一个UI血条
        uiBlood =
            this.UiBloodManager.PlaceUIBloodOnTower((int)SideType.SideA);
        t.uiBlood = uiBlood;
        
        t = this.ATowerObjects[2].AddComponent<NormalTower>();
        t.Init((int)SideType.SideA,(int)TowerType.Main);
        this._towers.Add(t);//right
        t.gameObject.name = "A_right_tower";
        //创建一个UI血条
        uiBlood =
            this.UiBloodManager.PlaceUIBloodOnTower((int)SideType.SideA);
        t.uiBlood = uiBlood;
        
        t = this.ATowerObjects[3].AddComponent<NormalTower>();
        t.Init((int)SideType.SideA,(int)TowerType.Main);
        this._towers.Add(t);//front
        t.gameObject.name = "A_front_tower";
        //创建一个UI血条
        uiBlood =
            this.UiBloodManager.PlaceUIBloodOnTower((int)SideType.SideA);
        t.uiBlood = uiBlood;
        
        //sideB
        t = this.BTowerObjects[0].AddComponent<MainTower>();
        t.Init((int)SideType.SideB,(int)TowerType.Main);
        this._towers.Add(t);//主塔
        t.gameObject.name = "B_main_tower";
        //创建一个UI血条
        uiBlood =
            this.UiBloodManager.PlaceUIBloodOnTower((int)SideType.SideB);
        t.uiBlood = uiBlood;
        
        t = this.BTowerObjects[1].AddComponent<NormalTower>();
        t.Init((int)SideType.SideB,(int)TowerType.Main);
        this._towers.Add(t);//left
        t.gameObject.name = "B_left_tower";
        //创建一个UI血条
        uiBlood =
            this.UiBloodManager.PlaceUIBloodOnTower((int)SideType.SideB);
        t.uiBlood = uiBlood;
        
        t = this.BTowerObjects[2].AddComponent<NormalTower>();
        t.Init((int)SideType.SideB,(int)TowerType.Main);
        this._towers.Add(t);//right
        t.gameObject.name = "B_right_tower";
        //创建一个UI血条
        uiBlood =
            this.UiBloodManager.PlaceUIBloodOnTower((int)SideType.SideB);
        t.uiBlood = uiBlood;
        
        t = this.BTowerObjects[3].AddComponent<NormalTower>();
        t.Init((int)SideType.SideB,(int)TowerType.Main);
        this._towers.Add(t);//front
        t.gameObject.name = "B_front_tower";
        //创建一个UI血条
        uiBlood =
            this.UiBloodManager.PlaceUIBloodOnTower((int)SideType.SideB);
        t.uiBlood = uiBlood;
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
        
        //创建一个UI血条
        UIShowBlood uiBlood =
            this.UiBloodManager.PlaceUIBloodOnHero(side);
        ctrl.uiBlood = uiBlood;
        return ctrl;
    }

    private void OnFrameHandleTowerLogic()
    {
//        for (int i = 0; i < _ATowers.Count; i++)
//        {
//            this._ATowers[i].OnLogicUpdate(LOGIC_FRAME_TIME);
//        }
//
//        for (int i = 0; i < _BTowers.Count; i++)
//        {
//            this._BTowers[i].OnLogicUpdate(LOGIC_FRAME_TIME);
//        }

        for (int i = 0; i < _towers.Count; i++)
        {
            this._towers[i].OnLogicUpdate(LOGIC_FRAME_TIME);
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
        
        if (this.uiAttack.attackType!=(int)OptType.Invalid)
        {//有攻击按钮按下了
            //攻击操作收集
            OptionEvent optAttack=new OptionEvent();
            optAttack.seatid = UGame.Instance.selfSeatid;
            optAttack.opt_type = this.uiAttack.attackType;
            nextFrameOpts.opts.Add(optAttack);
        }
        else
        {
            //摇杆操作收集
            OptionEvent optStick=new OptionEvent();
            optStick.seatid = UGame.Instance.selfSeatid;
            optStick.opt_type = (int)OptType.JoyStick;
            optStick.x = (int)(this.stick.TouchDir.x * (1 << 16));//定点数
            optStick.y = (int)(this.stick.TouchDir.y * (1 << 16));//定点数
            nextFrameOpts.opts.Add(optStick);
        }
        
        
        
        
        //发送给服务器
        LogicServiceProxy.Instance.SendNextFrameOpts(nextFrameOpts);
    }

    private void UpgradeExpByTime()
    {
        for (int i = 0; i < this._heroes.Count; i++)
        {
            this._heroes[i].AddExp(GameConfig.AddExpPerLogic);
        }
    }

    private void GenMonster()
    {
        this._nowGenMonsterFrame++;
        if (this._nowGenMonsterFrame>=GameConfig.GenMonsterFrames)
        {
            this._nowGenMonsterFrame = 0;
            GenThreeMonster();
        }
    }

    private void OnFrameHandleHeroLogic()
    {
        for (int i = 0; i < this._heroes.Count; i++)
        {
            this._heroes[i].OnLogicUpdate();
        }
    }

    /// <summary>
    /// 处理上一帧操作
    /// </summary>
    /// <param name="frameOpts"></param>
    private void OnHandlerFrameEvent(FrameOpts frameOpts)
    {
        //调用logicUpdate
        OnFrameHandleHeroLogic();
        
        //把所有玩家的操作带入处理
        for (int i = 0; i < frameOpts.opts.Count; i++)
        {
            int seatid = frameOpts.opts[i].seatid;
            Hero h = GetHeroBySeatID(seatid);
            if (h==null)
            {
                Debug.LogError("cannot find hero by seatid: "+seatid);
                continue;
            }
            h.OnHandlerFrameEvent(frameOpts.opts[i]);
        }
        
        //同步小兵位置
        OnFrameHandleMonsterLogic();
        //同步塔的子弹
        OnFrameHandleTowerBulletLogic();
        
        //塔和怪物AI根据操作产生相应
        OnFrameHandleTowerLogic();
        
        //同步小兵AI
        OnFrameHandleMonsterAI();
        
        //产生一波小兵
        GenMonster();
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
                Debug.LogError("cannot find hero by seatid: "+seatid);
                continue;
            }
            h.OnSyncLastLogicFrame(frameOpts.opts[i]);
        }
        
    }

    private void OnFrameHandleMonsterLogic()
    {
        for (int i = 0; i < this._monsters.Count; i++)
        {
            this._monsters[i].OnLogicUpdate(LOGIC_FRAME_TIME);
        }
    }


    private void OnFrameHandleMonsterAI()
    {
        for (int i = 0; i < this._monsters.Count; i++)
        {
            this._monsters[i].DoAI(LOGIC_FRAME_TIME);
        }
    }

    /// <summary>
    /// 帧同步调度主函数
    /// </summary>
    /// <param name="eventName"></param>
    /// <param name="udata"></param>
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

            OnHandlerFrameEvent(frame.unsync_frames[i]);//更新动画状态
            UpgradeExpByTime();
        }
        
        
        //获取最后一个操作 frame.frameid的操作
        //根据这个操作来处理，播放动画，产生的位移为“假位移”
        this._syncFrameID = frame.frameid;//同步到的事件帧ID
        if (frame.unsync_frames.Count>0)
        {
            this._lastFrameOpts = frame.unsync_frames[frame.unsync_frames.Count - 1];
            OnHandlerFrameEvent(this._lastFrameOpts);//更新动画状态
            UpgradeExpByTime();
        }
        else
        {
            _lastFrameOpts = null;
        }
        //采集下一个帧的事件，发送给服务器
        CapturePlayerOpts();
    }
    
    
}
