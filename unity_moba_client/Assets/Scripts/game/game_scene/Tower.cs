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
    private int _type;
    private int _side;//0:sideA  1:sideB
    
    /// <summary>
    /// 初始化塔属性
    /// </summary>
    /// <param name="side">0:sideA  1:sideB</param>
    /// <param name="type">TowerType</param>
    public virtual void Init(int side,int type)
    {
        this._side = side;
        this._type = type;
    }

    public virtual void OnLogicUpdate(int deltaTime)
    {
        //Debug.Log("BaseTower Update");
    }
}
