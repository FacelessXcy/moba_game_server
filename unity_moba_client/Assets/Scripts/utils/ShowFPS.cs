using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowFPS : MonoBehaviour
{
    private float _timeDelta = 0.5f;//固定时间间隔
    private float prevTime=0.0f;//上一次统计FPS的时间
    private float _fps = 0.0f;
    private int iFrames = 0;//累计刷新帧数

    private GUIStyle _guiStyle;

    private void Awake()
    {
        Application.targetFrameRate = 60;
    }

    private void Start()
    {
        this.prevTime = Time.realtimeSinceStartup;
        _guiStyle=new GUIStyle();
        this._guiStyle.fontSize = 15;
        this._guiStyle.normal.textColor = Color.white;
    }
    
    private void Update()
    {
        this.iFrames++;
        if (Time.realtimeSinceStartup>=this.prevTime+this._timeDelta)
        {
            this._fps = this.iFrames /
                        (Time.realtimeSinceStartup - this.prevTime);
            this.prevTime = Time.realtimeSinceStartup;
            this.iFrames = 0;
        }
    }

    private void OnGUI()
    {
        GUI.Label(new Rect(0, Screen.height-20, 200, 200), 
            "FPS:" + this._fps.ToString("f2"), 
            this._guiStyle);
    }
}
