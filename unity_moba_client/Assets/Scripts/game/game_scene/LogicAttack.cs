using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AttackType
{
    InvalidType=-1,
    P2PAttack=0,
    AreaAttack=1,
}

//攻击时间，计算伤害的时间。
//攻击过程中，不能进行其他操作
//用户，需要一个回调函数-->attackLogic，当攻击完成后，计算伤害
//攻击动画结束之后，可以进入到其他操作
public class LogicAttack : MonoBehaviour
{
    private bool _isRunning = false;
    private object _target=null;
    private int _attackValue;
    private int _attackFPS;
    private int _endFPS;
    private int _nowFps;
    private AttackType _attackType = AttackType.InvalidType;
    
    public delegate void OnAttackEnd();

    
    private OnAttackEnd _onEnd=null;

    public void AddListener(OnAttackEnd onAttackEnd)
    {
        this._onEnd = onAttackEnd;
    }



    /// <summary>
    /// 攻击效果判定(指向性)
    /// </summary>
    /// <param name="target">攻击目标</param>
    /// <param name="attackValue">攻击伤害</param>
    /// <param name="attackFps">攻击有效的那一帧</param>
    /// <param name="endFps">攻击结束帧</param>
    /// <returns>是否攻击成功</returns>
    public bool AttackTo(object target,int attackValue,int attackFps,
                        int endFps)
    {
        if (this._isRunning)
        {
            return false;
        }

        this._attackType = AttackType.P2PAttack;
        this._target = target;
        this._attackValue = attackValue;
        this._attackFPS = attackFps;
        this._endFPS = endFps;
        this._isRunning = true;
        this._nowFps = 0;
        return true;
    }
    
    /// <summary>
    /// 攻击效果判定(范围性)
    /// </summary>
    /// <param name="target">攻击目标</param>
    /// <param name="attackValue">攻击伤害</param>
    /// <param name="attackFps">攻击有效的那一帧</param>
    /// <param name="endFps">攻击结束帧</param>
    /// <returns>是否攻击成功</returns>
    public bool AttackAll(object target,int attackValue,int attackFps,
        int endFps)
    {
        if (this._isRunning)
        {
            return false;
        }

        this._attackType = AttackType.AreaAttack;
        this._target = target;
        this._attackValue = attackValue;
        this._attackFPS = attackFps;
        this._endFPS = endFps;
        this._isRunning = true;
        this._nowFps = 0;
        return true;
    }

    private void DoP2PKillWound()
    {
        if (this._target==null)
        {
            return;
        }
        GameObject obj=this._target as GameObject;
        DoAttackObject(obj);
    }

    private void DoAttackObject(GameObject obj)
    {
        if (obj==null)
        {
            return;
        }

        switch (obj.layer)
        {
            case (int)ObjectType.Hero:
                obj.GetComponent<Hero>().OnAttacked(this._attackValue);
                break;
            case (int)ObjectType.Tower:
                
                break;
            case (int)ObjectType.Monster:
                
                break;
        }
    }

    private void DoAreaKillWound()
    {
        if (this._target==null)
        {
            return;
        }
        List<GameObject> objs = _target as List<GameObject>;
        for (int i = 0; i < objs.Count; i++)
        {
            GameObject obj = objs[i];
            DoAttackObject(obj);
        }
    }

    public void OnLogicUpdate()
    {
        if (!this._isRunning)
        {
            return;
        }

        this._nowFps++;
        if (this._nowFps==this._attackFPS)
        {//计算伤害
            if (this._attackType==AttackType.P2PAttack)
            {
                DoP2PKillWound();
            }else if (this._attackType == AttackType.AreaAttack)
            {
                DoAreaKillWound();
            }
        }
        
        if (this._nowFps>=_endFPS)
        {//攻击动画结束
            this._isRunning = false;
            this._onEnd?.Invoke();
        }
    }

}
