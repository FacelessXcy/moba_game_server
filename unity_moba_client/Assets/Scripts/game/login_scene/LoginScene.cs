using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoginScene : MonoBehaviour
{
    
    
    private void Start()
    {
        
    }

    public void OnGuestLoginClick()
    {
        UserLogin.Instance.GuestLogin();
    }

}
