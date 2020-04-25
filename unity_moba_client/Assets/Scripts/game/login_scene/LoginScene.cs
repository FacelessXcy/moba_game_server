using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoginScene : MonoBehaviour
{
    public InputField unameInput;
    public InputField upwdInput;
    
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

    public void OnUnameLoginClick()
    {
        if (this.unameInput.text.Length<=0||this.upwdInput.text.Length<=0)
        {
            return;
        }
        
        UserLogin.Instance.UnameLogin(this.unameInput.text,this.upwdInput
        .text);
        
    }

}
