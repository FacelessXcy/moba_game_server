using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalTower : Tower
{
    public override void Init(int side, int type)
    {
        base.Init(side, type);
        
        //Debug.Log("NormalTower Init");
    }

    public override void OnLogicUpdate(int deltaTime)
    {
        
        //Debug.Log("NormalTower Update");
    }
}
