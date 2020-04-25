using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ULevel : Singleton<ULevel>
{
    private int[] _levelExp;
    public void Init()
    {
        //test 从配置文件中读取
        this._levelExp=new int[]
        {
            0,
            1000,
            2000,
            3000,
            4000,
            5000,
            6000,
            7000,
            8000,
            9000,
            10000,
            20000,
            30000
        };
        int now_exp;
        int next_level_exp;
        int level =
            ULevel.Instance.GetLevelInfo(2500,out now_exp, 
                out next_level_exp);
        Debug.Log("level = "+level+" now_exp = "+now_exp+" next_level_exp = "+next_level_exp);
    }

    public int GetLevelInfo(int uexp,out int now_exp,out int next_level_exp)
    {
        now_exp = 0;
        next_level_exp = 0;

        int level = 0;
        //升级后剩余经验
        int last_exp = uexp;

        while (level+1<=this._levelExp.Length-1&&
               last_exp>=this._levelExp[level+1])
        {
            last_exp -= this._levelExp[level+1];
            level++;
        }
        now_exp = last_exp;
        //升到最高等级
        if (level==this._levelExp.Length-1)
        {
            next_level_exp = now_exp;
        }
        else
        {
            next_level_exp = this._levelExp[level+1];
        }
        return level;
    }
}
