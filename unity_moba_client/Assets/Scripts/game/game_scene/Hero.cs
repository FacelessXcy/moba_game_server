using System;
using System.Collections;
using System.Collections.Generic;
using gprotocol;
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
public class Hero : MonoBehaviour
{

    public bool isGhost=false;//是否为别人控制的角色
    public float speed = 8.0f;//移动速度
    
    private CharacterController _characterController;
    private Animation _animation;
    private CharacterState _animState = CharacterState.idle;
    private Vector3 _cameraOffset;//摄像机对于主角的相对距离

    private int _stickX = 0;
    private int _stickY = 0;
    private CharacterState _logicState = CharacterState.idle;
    private Vector3 _logicPosition;//保存当前的逻辑帧位置

    public int seatid = -1;
    public int side = -1;
    
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
            if (this.side==1)
            {//sideB
                Camera.main.transform.localPosition =
                    new Vector3(262, 82, 112);
                Camera.main.transform.localEulerAngles =
                    new Vector3(50, 225, 0);
            }
            else
            {
                Camera.main.transform.localPosition =
                    new Vector3(32, 82, 86);
                Camera.main.transform.localEulerAngles =
                    new Vector3(50, 45, 0);
            }
            this._cameraOffset = Camera.main.transform.position - this
                                     .transform.position;
            
        }

        this._animation.Play("idle");
    }

    private void Update()
    {
        OnJoystickAnimUpdate();
    }

    public void OnAttacked(int attackValue)
    {
        Debug.Log("hero :" + this.gameObject.name + " was attacked: " +
                  attackValue);
    }

    public void LoginInit(Vector3 logicPos)
    {
        this._stickX = 0;
        this._stickY = 0;
        this._logicPosition = logicPos;
        this._logicState = CharacterState.idle;
    }

    /// <summary>
    /// 处理摇杆动画
    /// </summary>
    private void OnJoystickAnimUpdate()
    {
        if (this._logicState!=CharacterState.idle&&
            this._logicState!=CharacterState.walk)
        {
            return;
        }

        if (this._stickX==0&&this._stickY==0)
        {
            if (this._animState==CharacterState.walk)
            {
                this._animation.CrossFade("idle");
                this._animState = CharacterState.idle;
            }
            return;
        }
        if (this._animState==CharacterState.idle)
        {
            this._animation.CrossFade("walk");
            this._animState = CharacterState.walk;
        }
        DoJoystickEvent(Time.deltaTime);
        if (!this.isGhost)
        {
            Camera.main.transform.position =
                transform.position + _cameraOffset;
        }
    }

    /// <summary>
    /// 处理摇杆移动
    /// </summary>
    /// <param name="deltaTime"></param>
    private void DoJoystickEvent(float deltaTime)
    {
        if (this._stickX==0&&this._stickY==0)
        {
            this._logicState = CharacterState.idle;
            return;
        }
        this._logicState = CharacterState.walk;
        
        float dirX = (float) this._stickX / (float) (1 << 16);
        float diry = (float) this._stickY / (float) (1 << 16);
        float dir = Mathf.Atan2(diry,dirX);
        
        float s = this.speed * deltaTime;
        float offset = (this.side == 0)
            ? (-Mathf.PI * 0.25f)
            : (Mathf.PI * 0.75f);
        float sx = s * Mathf.Cos(dir + offset);
        float sy = s * Mathf.Sin(dir + offset);
        this._characterController.Move(new Vector3(sx, 0, sy));
        offset = (this.side == 0) ? 45 : -135;
        float degree =360-dir*Mathf.Rad2Deg+90.0f+offset;
        this.transform.localEulerAngles = new Vector3(0, degree, 0);

    }

    /// <summary>
    /// 用来更新本地动画与位置(非服务器同步位置)
    /// </summary>
    /// <param name="opt"></param>
    private void HandleJoyStickEvent(OptionEvent opt)
    {
        this._stickX = opt.x;
        this._stickY = opt.y;
        if (this._stickX==0&&this._stickY==0)
        {
            this._logicState = CharacterState.idle;
        }
        else
        {
            this._logicState = CharacterState.walk;
        }
    }

    private void JumpJoystickEvent(OptionEvent opt)
    {
        SyncLastJoystickEvent(opt);
    }

    /// <summary>
    /// 角色处理帧事件
    /// </summary>
    public void OnHandlerFrameEvent(OptionEvent opt)
    {
        switch (opt.opt_type)
        {
            case (int)OptType.JoyStick:
                HandleJoyStickEvent(opt);
                break;
        }
    }

    /// <summary>
    /// 根据上一帧逻辑位置，计算出这一帧到达时所在的位置(服务器同步位置)
    /// </summary>
    /// <param name="opt"></param>
    private void SyncLastJoystickEvent(OptionEvent opt)
    {
        //上一个逻辑位置--->deltaTime为服务器的逻辑帧间隔
        this._stickX = opt.x;
        this._stickY = opt.y;
        this.transform.position = this._logicPosition;
        DoJoystickEvent((float) GameZygote.LOGIC_FRAME_TIME / 1000.0f);
        this._logicPosition = this.transform.position;
    }
    
    
    /// <summary>
    /// 跳帧处理
    /// </summary>
    /// <param name="opt"></param>
    public void OnJumpToNextFrame(OptionEvent opt)
    {
        switch (opt.opt_type)
        {
            case (int)OptType.JoyStick:
                JumpJoystickEvent(opt);
                break;
        }
    }

    public void OnSyncLastLogicFrame(OptionEvent opt)
    {
        switch (opt.opt_type)
        {
            case (int)OptType.JoyStick:
                SyncLastJoystickEvent(opt);
                break;
        }
    }
}
