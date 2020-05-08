using System.Collections;
using System.Collections.Generic;
using SWS;
using UnityEngine;


/// <summary>
/// 动画播放
/// </summary>
public class MonsterMove : MonoBehaviour
{
    private float _speed = 5;
    private float _walkTime = 0.0f;
    private float _passTime = 0.0f;
    private bool _isWalking = false;
    
    
    private void Start()
    {

    }

    private void Update()
    {
        if (!_isWalking)
        {
            return;
        }

        float dt = Time.deltaTime;
        this._passTime += dt;
        if (this._passTime>=this._walkTime)
        {
            dt -= (this._passTime - this._walkTime);
        }
        
        float s = this._speed * dt;
        this.transform.Translate(this.transform.forward*s,Space.World);
        if (this._passTime>=this._walkTime)
        {
            this._isWalking = false;
        }
    }
    

    public void WalkToDst(Vector3 dst)
    {
        Vector3 src = this.transform.position;
        Vector3 dir = dst - src;
        float len = dir.magnitude;
        if (len<=0)
        {
            return;
        }
        
        this._isWalking = true;
        this._walkTime = len / this._speed;
        this._passTime = 0;
        
        //调整角色位置，对准下一个点
        this.transform.LookAt(dst);
    }

}
