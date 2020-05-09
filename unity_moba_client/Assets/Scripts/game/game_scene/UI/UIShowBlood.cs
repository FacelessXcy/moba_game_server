using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIShowBlood : MonoBehaviour
{
    public Sprite sideABlood;
    public Sprite sideBBlood;

    public Image blood=null;
    public Image blue=null;
    public Text expLevel=null;

    public float xOffset;
    public float yOffset;
    
    private int _side = 0;

    private void Start()
    {
        blood = transform.Find("blood").GetComponent<Image>();
        blue = transform.Find("blue")?.GetComponent<Image>();
        expLevel = transform.Find("exp_level")?.GetComponent<Text>();
        if (this._side==(int)SideType.SideA)
        {
            this.blood.sprite = this.sideABlood;
        }
        else
        {
            this.blood.sprite = this.sideBBlood;
        }
    }

    public void Init(int side)
    {
        this._side = side;
    }

    public void SetBlood(float per)
    {
        if (this.blood)
        {
            this.blood.fillAmount = per;
        }
    }
    
    public void SetBlue(float per)
    {
        if (this.blue)
        {
            this.blue.fillAmount = per;
        }
    }
    
    public void SetLevel(int level)
    {
        if (this.expLevel)
        {
            this.expLevel.text = level.ToString();
        }
    }
    
    
}
