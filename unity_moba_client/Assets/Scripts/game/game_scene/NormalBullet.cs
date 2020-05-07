using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalBullet : Bullet
{
    private float _activeTime;
    private float _passedTime;
    private bool _isRunning;


    private void Update()
    {
        if (!this._isRunning)
        {
            return;
        }

        float deltaTime = Time.deltaTime;
        this._passedTime += deltaTime;
        if (this._passedTime>this._activeTime)
        {
            deltaTime -= (this._passedTime - this._activeTime);
        }
        
        //更新子弹位置
        Vector3 offset = this.transform.forward * this.config.Speed *
                         deltaTime;
        this.transform.position += offset;

        if (this._passedTime>=this._activeTime)
        {
            this._isRunning = false;
            GameZygote.Instance.RemoveBullet(this);
        }
    }

    public override void Init(int side, int type)
    {
        base.Init(side, type);
        
        
    }

    public override void OnLogicUpdate(int deltaTime)
    {
        
    }

    public void ShootTo(Vector3 worldTarget)
    {
        transform.LookAt(worldTarget);//修改朝向
        Vector3 dir = worldTarget - this.transform.position;
        float len = dir.magnitude;
        this._activeTime = len / this.config.Speed;
        this._passedTime = 0;
        this._isRunning = true;

    }
}
