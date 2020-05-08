using System.Collections;
using System.Collections.Generic;
using SWS;
using UnityEngine;

public class MonsterMove : MonoBehaviour
{

    private Vector3[] _roadDatas = null;

    public Vector3[] RoadDatas
    {
        get => _roadDatas;
        set => _roadDatas = value;
    }

    private int _nextStep = 0;
    private float _speed = 5;
    private float _walkTime = 0.0f;
    private float _passTime = 0.0f;
    private bool _isWalking = false;

    private Animation _anim;
    
    private void Start()
    {
        _anim = GetComponent<Animation>();
        this._anim.CrossFade("free");
        WalkOnRoad();//Test

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
            this._nextStep++;
            WalkToNext();
        }
    }

    private void StopWalk()
    {
        if (this._isWalking)
        {
            this._anim.CrossFade("free");
        }
        this._isWalking = false;
    }

    private void GoAhead()
    {
        WalkToNext();
    }


    private void WalkOnRoad()
    {
        if (this._roadDatas.Length<2)
        {
            return;
        }

        this.transform.position = this._roadDatas[0];
        this._nextStep = 1;
        WalkToNext();
    }

    private void WalkToNext()
    {
        if (this._nextStep>=this._roadDatas.Length)
        {
            this._isWalking = false;
            this._anim.CrossFade("free");
            return;
        }

        Vector3 src = this.transform.position;
        Vector3 dst = this._roadDatas[this._nextStep];
        Vector3 dir = dst - src;
        float len = dir.magnitude;
        if (len<=0)
        {
            this._nextStep++;
            this.WalkToNext();
            return;
        }

        if (!this._isWalking)
        {
            this._anim.CrossFade("walk");
        }
        this._isWalking = true;
        this._walkTime = len / this._speed;
        _passTime = 0;
        
        //调整角色位置，对准下一个点
        this.transform.LookAt(dst);
    }

}
