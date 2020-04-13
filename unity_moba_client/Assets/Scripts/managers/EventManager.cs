using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : Singleton<EventManager>
{
    private Dictionary<string, OnEventHandler> _eventListeners=new Dictionary<string, OnEventHandler>();
    public delegate void OnEventHandler(string name,object udata);
     
    
    //订阅者
    public void AddEventListener(string name,OnEventHandler handler)
    {
        if (this._eventListeners.ContainsKey(name))
        {
            this._eventListeners[name] += handler;
        }
        else
        {
            this._eventListeners.Add(name,handler);
        }
    }

    public void RemoveEventListener(string name,OnEventHandler handler)
    {
        if (!this._eventListeners.ContainsKey(name))
        {
            return;
        }
        this._eventListeners[name] -= handler;
        if (this._eventListeners[name]==null)
        {
            this._eventListeners.Remove(name);
        }
    }
    
    //发布者
    public void DispatchEvent(string name, object udata)
    {
        if (!this._eventListeners.ContainsKey(name))
        {
            return;
        }
        this._eventListeners[name]?.Invoke(name,udata);
        
    }
}
