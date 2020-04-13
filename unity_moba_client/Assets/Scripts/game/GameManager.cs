using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : UnitySingleton<GameManager>
{
    private void Start()
    {
        EventManager.Instance.Init();
        UserLogin.Instance.Init();   
    }
}
