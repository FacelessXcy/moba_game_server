using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public delegate void OnAttackTo(object target, int attackValue);
    public delegate void OnAttackEnd();

    private OnAttackTo _onAttack=null;
    private OnAttackEnd _onEnd=null;

    public void AddListener(OnAttackTo onAttackTo,OnAttackEnd onAttackEnd)
    {
        this._onAttack = onAttackTo;
        this._onEnd = onAttackEnd;
    }



    /// <summary>
    /// 攻击效果判定
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

        this._target = target;
        this._attackValue = attackValue;
        this._attackFPS = attackFps;
        this._endFPS = endFps;
        this._isRunning = true;
        this._nowFps = 0;
        return true;
    }

    public void OnLogicUpdate()
    {
        if (!this._isRunning)
        {
            return;
        }

        this._nowFps++;
        if (this._nowFps>=this._attackFPS)
        {//计算伤害
            this._onAttack?.Invoke(this._target,this._attackValue);
        }
        
        if (this._nowFps>=_endFPS)
        {//攻击动画结束
            this._isRunning = false;
            this._onEnd?.Invoke();
        }
    }

}
