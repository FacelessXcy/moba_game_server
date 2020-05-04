using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum CharacterState
{
    walk=1,
    free=2,
    idle=3,
    attack=4,
    attack2=5,
    attack3=6,
    skill=7,
    skill2=8,
    death=9
}

//主角角色控制
public class CharacterCtrl : MonoBehaviour
{
    //test
    public Joystick stick;
    //end
    
    public bool isGhost=false;//是否为别人控制的角色
    public float speed = 8.0f;//移动速度
    
    private CharacterController _characterController;
    private Animation _animation;
    private CharacterState _state = CharacterState.idle;
    private Vector3 _cameraOffset;//摄像机对于主角的相对距离
    private void Start()
    {
        GameObject ring =
            Resources.Load<GameObject>("effect/other/guangquan_fanwei");
        _characterController = GetComponent<CharacterController>();
        _animation = GetComponent<Animation>();
        if (!this.isGhost)//玩家控制的角色
        {
            GameObject r = Instantiate(ring);
            r.transform.SetParent(this.transform,false);
            r.transform.localPosition = Vector3.zero;
            r.transform.localScale=new Vector3(2,1,2);
            this._cameraOffset = Camera.main.transform.position - this
                                     .transform.position;
        }

        this._animation.Play("idle");
    }

    private void Update()
    {
        if (this._state!=CharacterState.idle&&this._state!=CharacterState.walk)
        {
            return;
        }

        if (this.stick.TouchDir==Vector2.zero)
        {
            if (this._state==CharacterState.walk)
            {
                this._animation.CrossFade("idle");
                this._state = CharacterState.idle;
            }
            return;
        }
        if (this._state==CharacterState.idle)
        {
            this._animation.CrossFade("walk");
            this._state = CharacterState.walk;
        }

        float s = this.speed * Time.deltaTime;
        this._characterController.Move(
            new Vector3(this.stick.TouchDir.x, 
                0,
                this.stick.TouchDir.y) 
            * s);
        float dir = Mathf.Atan2(this.stick.TouchDir.y,this.stick
        .TouchDir.x);
        float degree =360-dir*Mathf.Rad2Deg+90.0f;
        this.transform.localEulerAngles = new Vector3(0, degree, 0);

        if (!this.isGhost)
        {
            Camera.main.transform.position =
                transform.position + _cameraOffset;
        }
    }
}
