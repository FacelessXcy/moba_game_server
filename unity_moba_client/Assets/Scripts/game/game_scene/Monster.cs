using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum State
{
    Idle=1,
    Walk=2,
    Attack=3,
    Dead=4,
}

public class Monster : MonoBehaviour
{
    private int _type;
    public int side;
    private Vector3[] _roadData;
    private int _state=(int)State.Idle;
    private Animation _anim;

    private Vector3 _logicPos;
    private int _nextStep;
    
    
    //怪物的参数，每种类型-->小兵配置文件
    private float _speed=5.0f;
    private MonsterMove _localMove;

    public UIShowBlood uiBlood;
    
    private void Awake()
    {
        this._localMove = GetComponent<MonsterMove>();
    }

    private void Start()
    {
        this._anim = GetComponent<Animation>();
        if (this._state==(int)State.Idle)
        {
            this._anim.Play("free");
        }
        else if(this._state==(int)State.Walk)
        {
            this._anim.Play("walk");
        }
        
    }

    private void Update()
    {
        UIBloodUpdate();
    }

    public void Init(int type,int side,Vector3[] roadData)
    {
        this._type = type;
        this.side = side;
        this._roadData = roadData;
        if (this._roadData.Length<2)
        {
            this._state = (int) State.Idle;
        }
        else
        {
            this._state = (int) State.Walk;
        }

        this._logicPos = this._roadData[0];
        this.transform.position = this._logicPos;
        this._nextStep = 1;
    }

    private void UIBloodUpdate()//感觉有问题
    {
        Vector2 pos2D =
            Camera.main.WorldToScreenPoint(this.transform.position);
        this.uiBlood.transform.position = pos2D + 
                                          new Vector2(this.uiBlood.xOffset,
                                              this.uiBlood.yOffset);
        if (pos2D.x>Screen.width||pos2D.x<0||
            pos2D.y>Screen.height||pos2D.y<0)
        {
            this.uiBlood.gameObject.SetActive(false);
        }
        else
        {
            this.uiBlood.gameObject.SetActive(true);
        }
    }
    //15FPS
    private void OnLogicWalkUpdate(float dtMs)
    {
        //当前逻辑位置，dt下来迭代
        this.transform.position = this._logicPos;
        
        Vector3 src = this.transform.position;
        Vector3 dst = this._roadData[this._nextStep];
        Vector3 dir = dst - src;
        float len = dir.magnitude;
        if (len<=0)
        {
            this._nextStep++;
            OnLogicWalkUpdate(dtMs);
            return;
        }

        bool isArrived = false;
        float time = len / this._speed;
        float dt = dtMs / (float) 1000.0f;
        if (time<dt)
        {//防止走过目标点，如果所需要的时间小于每帧的deltaTime，则设置deltaTime
            dt = time;
            isArrived = true;
        }
        this.transform.LookAt(dst);
        this.transform.position +=
            (this.transform.forward * dt * this._speed);
        this._logicPos = this.transform.position;

        if (isArrived)
        {
            this._nextStep++;
            if (this._nextStep>=this._roadData.Length)
            {
                this._state = (int) State.Idle;
                this._anim.Play("free");
                return;
            }
        }
        this._localMove.WalkToDst(this._roadData[this._nextStep]);
    }

    public void OnLogicUpdate(float dtMs)
    {
        if (this._state==(int)State.Walk)
        {
            OnLogicWalkUpdate(dtMs);
        }
    }

    public void DoAI(float dtMs)
    {
        
    }
}
