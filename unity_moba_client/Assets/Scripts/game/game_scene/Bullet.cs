using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum BulletType
{
    Normal=1,
    Main=2,
}

public class Bullet : MonoBehaviour
{
    protected int type;
    protected int side;//0:sideA  1:sideB

    protected BulletConfig config;
    
    /// <summary>
    /// 初始化子弹属性
    /// </summary>
    /// <param name="side">0:sideA  1:sideB</param>
    /// <param name="type">TowerType</param>
    public virtual void Init(int side,int type)
    {
        this.side = side;
        this.type = type;
        switch (type)
        {
            case (int)TowerType.Normal:
                this.config = GameConfig.MainBulletConfig;
                break;
            case (int)TowerType.Main:
                this.config = GameConfig.NormalBulletConfig;
                break;
        }
    }

    public virtual void OnLogicUpdate(int deltaTime)
    {
        
        
    }
}
