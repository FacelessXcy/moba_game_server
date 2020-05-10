using System.Collections;
using System.Collections.Generic;
using System.Security.AccessControl;
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

public class KillWound
{
    public int Value;
    public float ValidR;
}

//多个英雄，每个英雄[15]个等级
public class HeroLevelConfig
{
    public int Defense;//防御力
    public KillWound Attack;//普通攻击
    public KillWound Skill;//技能攻击
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
            Attack = new KillWound()
            {
                Value = 20,
                ValidR = 3.0f,
            },
            Skill = new KillWound()
            {
                Value = 20,
                ValidR = 3.5f,
            },
            MaxBlood = 100,
            AddBlood = 0,
            Exp = 0,//需要获得的经验  0 +100-->100 +200-->300
        },
        new HeroLevelConfig()
        {
            Defense = 2,
            Attack = new KillWound()
            {
                Value = 20,
                ValidR = 3.0f,
            },
            Skill = new KillWound()
            {
                Value = 20,
                ValidR = 3.5f,
            },
            MaxBlood = 200,
            AddBlood = 50,
            Exp = 100,
        },
        new HeroLevelConfig()
        {
            Defense = 3,
            Attack = new KillWound()
            {
                Value = 20,
                ValidR = 3.0f,
            },
            Skill = new KillWound()
            {
                Value = 20,
                ValidR = 3.5f,
            },
            MaxBlood = 300,
            AddBlood = 100,
            Exp = 200,
        },
    };

    public static int AddExpPerLogic=1;//每个逻辑帧成长1点

    public static int GenMonsterFrames = 15*33;//15*33
    public static int Exp2Level(HeroLevelConfig[] configs,int exp)
    //当前所有的exp
    {
        int level = 0;//从第0级开始
        while (level+1<configs.Length&&
               exp>=configs[level+1].Exp)
        {
            exp -= configs[level+1].Exp;
            level++;
            //todo
        }

        return level;
    }

    public static void ExpUpgradeLevelInfo(HeroLevelConfig[] configs,
    int exp,ref int now,ref int total)
    {
        int level = 0;//从第0级开始
        
        while (level+1<configs.Length&&
               exp>=configs[level+1].Exp)
        {
            exp -= configs[level+1].Exp;
            level++;
            //todo
        }

        if (level+1>=configs.Length)
        {
            now = total = configs[level].Exp;
        }
        else
        {
            now = exp;
            total = configs[level + 1].Exp;
        }
        
        
    }
}
