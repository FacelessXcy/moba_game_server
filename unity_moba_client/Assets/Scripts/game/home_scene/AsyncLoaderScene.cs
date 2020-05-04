using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AsyncLoaderScene : MonoBehaviour
{
    public string sceneName;
    public Image process;//进度条

    private AsyncOperation _ao;
    

    private void Start()
    {
        this.process.fillAmount = 0.0f;
        StartCoroutine(AsyncLoadScene());
    }

    private void Update()
    {
        float per = this._ao.progress;
        if (per>=0.9f)
        {
            this._ao.allowSceneActivation = true;
        }
        process.fillAmount = per/0.9f;

    }


    IEnumerator AsyncLoadScene()
    {
        _ao = SceneManager.LoadSceneAsync(sceneName);
        _ao.allowSceneActivation = false;
        yield return this._ao;
        
    }
}
