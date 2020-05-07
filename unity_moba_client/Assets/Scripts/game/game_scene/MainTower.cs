using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainTower : Tower
{
    public override void Init(int side, int type)
    {
        base.Init(side, type);
        
        //Debug.Log("MainTower Init");
    }

    public override void OnLogicUpdate(int deltaTime)
    {
        
        //Debug.Log("MainTower Update");
    }
}
