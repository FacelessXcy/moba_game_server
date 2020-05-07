using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//普通单例模板
public abstract class Singleton<T> where T:new()
{
    private static T _instance=default(T);
    private static object mutex=new object();

    public static T Instance
    {
        get
        {
            if (_instance==null)
            {
                lock (mutex)
                {
                    if (_instance==null)
                    {
                        _instance=new T();
                    }
                }
            }

            return _instance;
        }
    }
}

public class UnitySingleton<T> : MonoBehaviour
    where T : Component
{
    private static T _instance=null;
    protected bool _destoryOnLoad=false;
    public static T Instance
    {
        get
        {
            if (_instance==null)
            {
                _instance=FindObjectOfType(typeof(T)) as T;
                if (_instance==null)
                {
                    GameObject obj=new GameObject(typeof(T).Name);
                    _instance=obj.AddComponent(typeof(T)) as T;
                    obj.hideFlags = HideFlags.DontSave;
                }
            }
            return _instance;
        }
    }

    public virtual void Awake()
    {
        if (!_destoryOnLoad)
        {
            DontDestroyOnLoad(this.gameObject);
        }
        if (_instance==null)
        {
            _instance = this as T;
        }
        else
        {
            GameObject.Destroy(this.gameObject);
        }
    }
}