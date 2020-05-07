using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalTower : Tower
{
    private int _nowFps;

    private void Start()
    {
        _nowFps = this.config.ShootLogicFps;
    }

    public override void Init(int side, int type)
    {
        base.Init(side, type);
        
        
    }

    private void ShootAt(Vector3 pos)
    {
        NormalBullet bullet = GameZygote.Instance.AllocBullet(this.side,
            (int) BulletType.Normal) as NormalBullet;
        bullet.transform.position =
            this.transform.Find("point").position;
        bullet.ShootTo(pos);
        

    }

    private void DoShoot()
    {
        List<Hero> heroes = GameZygote.Instance.GetHeroes();
        Hero target=null;
        float minLen = this.config.AttackR+1;
        for (int i = 0; i < heroes.Count; i++)
        {
            Hero h = heroes[i];
            if (h.side==this.side)
            {
                continue;
            }
            Vector3 dir =
                h.transform.position - this.transform.position;
            float len = dir.magnitude;
            if (len>this.config.AttackR)
            {
                continue;
            }
            
            //在攻击范围之内,判断是否是最近的
            if (len<minLen)
            {
                minLen = len;
                target = h;
            }
        }

        if (target!=null)
        {//发射一发子弹
            ShootAt(target.transform.position);
        }
    }

    public override void OnLogicUpdate(int deltaTime)
    {
        this._nowFps++;
        if (this._nowFps>=this.config.ShootLogicFps)
        {
            this._nowFps = 0;
            DoShoot();
        }

    }
}
