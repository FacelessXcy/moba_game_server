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
    
    
}
