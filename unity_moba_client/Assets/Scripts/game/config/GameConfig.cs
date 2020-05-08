using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerConfig
{
    public int Hp;
    public int AttackR;//攻击范围
    public int ShootLogicFps;//攻击间隔
}

public class BulletConfig
{
    public int Attack;//攻击力
    public int Speed;//子弹移动速度
    public int MaxDistance;//最大有效范围
}
//多个英雄，每个英雄[15]个等级
public class HeroLevelConfig
{
    public int Defense;//防御力
    public int Attack;//攻击力
    public int MaxBlood;//当前等级最大血量
    public int AddBlood;//升级时，加多少血量
    public int Exp;
}

public class GameConfig
{
    public static TowerConfig MainTowerConfig=new TowerConfig()
    {
        Hp = 10,
        AttackR = 10,
        ShootLogicFps = 3,//1秒5次攻击
    };
    
    public static TowerConfig NormalTowerConfig=new TowerConfig()
    {
        Hp = 10,
        AttackR = 10,
        ShootLogicFps = 5,//1秒3次攻击
    };
    
    public static BulletConfig NormalBulletConfig=new BulletConfig()
    {
        Attack = 10,
        Speed = 20,
        MaxDistance = 20,
    };
    
    public static BulletConfig MainBulletConfig=new BulletConfig()
    {
        Attack = 10,
        Speed = 20,
        MaxDistance = 20,
    };

    public static HeroLevelConfig[] NormalHeroLevelConfigs = new[]
    {
        new HeroLevelConfig()
        {
            Defense = 1,
            Attack = 1,
            MaxBlood = 1,
            AddBlood = 1,
            Exp = 1,
        },
        new HeroLevelConfig()
        {
            Defense = 1,
            Attack = 1,
            MaxBlood = 1,
            AddBlood = 1,
            Exp = 1,
        },
        new HeroLevelConfig()
        {
            Defense = 1,
            Attack = 1,
            MaxBlood = 1,
            AddBlood = 1,
            Exp = 1,
        },
    };

    public static int NormalHeroExpK=(1<<15);//定点数，表示小数
}
