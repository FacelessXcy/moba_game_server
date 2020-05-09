using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TowerType
{
    Main=1,
    Normal=2,
}

public class Tower : MonoBehaviour
{
    protected int type;
    protected int side;//0:sideA  1:sideB

    protected TowerConfig config;
    
    public UIShowBlood uiBlood;


    public virtual void Update()
    {
        UIBloodUpdate();
    }

    /// <summary>
    /// 初始化塔属性
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
                this.config = GameConfig.NormalTowerConfig;
                break;
            case (int)TowerType.Main:
                this.config = GameConfig.MainTowerConfig;
                break;
        }
    }

    public virtual void OnLogicUpdate(int deltaTime)
    {
        
    }
    
    private void UIBloodUpdate()//感觉有问题
    {
        Vector2 pos2D =
            Camera.main.WorldToScreenPoint(this.transform.position);
        this.uiBlood.transform.position = pos2D + 
                                          new Vector2(this.uiBlood.xOffset,
                                              this.uiBlood.yOffset);
        if (pos2D.x>Screen.width||pos2D.x<0||
            pos2D.y>Screen.height||pos2D.y<0)
        {
            this.uiBlood.gameObject.SetActive(false);
        }
        else
        {
            this.uiBlood.gameObject.SetActive(true);
        }
    }
}
