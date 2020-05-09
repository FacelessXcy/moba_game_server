using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIAttack : MonoBehaviour
{

    public int attackType=(int)OptType.Invalid;

    public void OnAttackDown()
    {
        this.attackType = (int) OptType.Attack;
    }

    public void OnKeyUp()
    {
        this.attackType = (int) OptType.Invalid;
    }
    
    public void OnSkill1Down()
    {
        this.attackType = (int) OptType.Skill1;
    }
}
