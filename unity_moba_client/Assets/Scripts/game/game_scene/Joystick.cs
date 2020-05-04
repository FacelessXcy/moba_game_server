using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Joystick : MonoBehaviour
{
    public Canvas canvas;
    private float _maxRadius=70;
    private Transform _stick;

    private Vector2 _touchDir;
    public Vector2 TouchDir => _touchDir;

    private void Start()
    {
        this._stick = transform.Find("stick");
        this._touchDir = Vector2.zero;
        this._stick.localPosition = Vector2.zero;
    }

    public void OnStickDrag(BaseEventData baseEventData)
    {
        Vector2 pos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(this
                .transform as RectTransform, Input.mousePosition,
            this.canvas.worldCamera, out pos);
        float len = pos.magnitude;
        this._touchDir = pos.normalized;
        //Debug.Log(_touchDir);
        if (len>_maxRadius)
        {
            float ratio=_maxRadius/len;//短:长
            pos*=ratio;
        }
        this._stick.localPosition = pos;
    }

    public void OnStickEndDrag(BaseEventData baseEventData)
    {
        this._stick.localPosition = Vector2.zero;
        this._touchDir = Vector2.zero;
    }
}
