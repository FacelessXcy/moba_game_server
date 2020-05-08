using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum BulletType
{
    Normal=1,
    Main=2,
}

//子弹帧同步原理：
//1.动画本地直接播放
//2.打到哪个目标，由逻辑帧控制
//3.生命周期由逻辑帧控制
public class Bullet : MonoBehaviour
{
    protected int type;
    protected int side;//0:sideA  1:sideB

    protected BulletConfig config;
    
    private int _activeTime;
    private float _passedTime;
    private bool _isRunning;

    //已经经过的逻辑时间
    private int _logicPassedTime = 0;
    private Vector3 _logicPos;
    
    
    private void Update()
    {
        if (!this._isRunning)
        {
            return;
        }

        float total = (float)(this._activeTime) / 1000.0f;
        float deltaTime = Time.deltaTime;
        this._passedTime += deltaTime;
        if (this._passedTime>total)
        {
            deltaTime -= (this._passedTime - total);
        }
        
        //更新子弹位置
        Vector3 offset = this.transform.forward * this.config.Speed *
                         deltaTime;
        this.transform.position += offset;

        if (this._passedTime>=total)
        {
            this._isRunning = false;
        }
    }
    
    public void ShootTo(Vector3 worldTarget)
    {
        transform.LookAt(worldTarget);//修改朝向
        Vector3 dir = worldTarget - this.transform.position;
        float len = dir.magnitude;
        this._activeTime = (int)((len*1000) / this.config.Speed);
        this._passedTime = 0;
        this._logicPassedTime = 0;
        this._isRunning = true;

        this._logicPos = this.transform.position;
    }
    
    
    /// <summary>
    /// 初始化子弹属性
    /// </summary>
    /// <param name="side">0:sideA  1:sideB</param>
    /// <param name="type">TowerType</param>
    public virtual void Init(int side,int type)
    {
        this.side = side;
        this.type = type;
        switch (type)
        {
            case (int)TowerType.Normal:
                this.config = GameConfig.MainBulletConfig;
                break;
            case (int)TowerType.Main:
                this.config = GameConfig.NormalBulletConfig;
                break;
        }
    }

    private bool HitTest(Vector3 startPos,float distance)
    {
        //发射射线，startPos 前方-->distance的射线 ，撞到哪些物体
        RaycastHit[] hits = Physics.RaycastAll(startPos, this.transform
            .forward, distance);
        if (hits!=null&&hits.Length>0)
        {
            for (int i = 0; i < hits.Length; i++)
            {
                RaycastHit hit = hits[i];
                if (hit.collider.gameObject.layer==(int)ObjectType.Hero)
                {
                    Hero h = hit.collider.GetComponent<Hero>();
                    if (h.side==this.side)
                    {
                        continue;
                    }

                    h.OnAttacked(this.config.Attack);
                }
            }
        }
        return false;
    }

    public virtual void OnLogicUpdate(int deltaTime)
    {
        this._logicPassedTime += deltaTime;
        if (this._logicPassedTime>this._activeTime)
        {
            deltaTime -= (this._logicPassedTime - this._activeTime);
        }
        //更新子弹逻辑位置
        float dt = (float)(deltaTime / 1000.0f);
        Vector3 offset = this.transform.forward * this.config.Speed * dt;
        
        
        //子弹击中人物逻辑
        if (HitTest(this._logicPos,offset.magnitude))//子弹攻击到了物体
        {
            return;
        }
        this._logicPos += offset;
        
        if (_logicPassedTime>=this._activeTime)
        {
            GameZygote.Instance.RemoveBullet(this);
        }

    }
}
