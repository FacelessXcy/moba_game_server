using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoginScene : MonoBehaviour
{
    
    
    private void Start()
    {
        EventManager.Instance.AddEventListener("login_success",OnLoginSuccess);
    }

    private void OnDestroy()
    {
        EventManager.Instance.RemoveEventListener("login_success",OnLoginSuccess);
    }

    public void OnLoginSuccess(string name,object udata)
    {
        SceneManager.LoadScene("Scenes/home_scene");
    }

    public void OnGuestLoginClick()
    {
        UserLogin.Instance.GuestLogin();
    }

}
