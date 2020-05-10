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
        EventManager.Instance.AddEventListener("login_success",
            OnLoginSuccess);
        EventManager.Instance.AddEventListener("get_ugame_info_success",
            OnGetUGameInfoSuccess);
        EventManager.Instance.AddEventListener("login_logic_server",
            OnLoginLogicServerSuccess);
    }

    private void OnDestroy()
    {
        EventManager.Instance.RemoveEventListener("login_success",
            OnLoginSuccess);
        EventManager.Instance.RemoveEventListener("get_ugame_info_success",
            OnGetUGameInfoSuccess);
        EventManager.Instance.RemoveEventListener("login_logic_server",
            OnLoginLogicServerSuccess);
    }

    private void OnLoginLogicServerSuccess(string name, object udata)
    {
        SceneManager.LoadScene("Scenes/home_scene");
    }

    private void OnGetUGameInfoSuccess(string name,object udata)
    {
        //SceneManager.LoadScene("Scenes/home_scene");
        //Debug.Log("get_ugame_info_success");
        LogicServiceProxy.Instance.LoginLogicServer();
    }

    private void OnLoginSuccess(string name,object udata)
    {
        //SceneManager.LoadScene("Scenes/home_scene");
        Debug.Log("load game data...");
        SystemServiceProxy.Instance.LoadUserUGameInfo();
    }

    public void OnGuestLoginClick()
    {
        AuthServiceProxy.Instance.GuestLogin();
    }

    public void OnUnameLoginClick()
    {
        if (this.unameInput.text.Length<=0||this.upwdInput.text.Length<=0)
        {
            return;
        }
        AuthServiceProxy.Instance.UnameLogin(this.unameInput.text,this.upwdInput
        .text);
    }

}
