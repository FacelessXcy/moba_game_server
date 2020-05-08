using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIBloodInfo
{
    public int Blood;
    public int MaxBlood;
}

public class UIExpInfo
{
    public int Exp;
    public int Total;
}

public class GameUI : MonoBehaviour
{
    public Image bloodProcess;
    public Image expProcess;
    public Text bloodLabel;
    public Text expLabel;

    private void Start()
    {
        //exp_ui_sync  blood_ui_sync
        EventManager.Instance.AddEventListener("exp_ui_sync",
            OnExpUISync);
        EventManager.Instance.AddEventListener("blood_ui_sync",
            OnBloodUISync);
    }

    private void OnExpUISync(string eventName,object udata)
    {
        UIExpInfo info = (UIExpInfo) udata;
        this.expProcess.fillAmount =
            (float) info.Exp / (float) info.Total;
        this.expLabel.text = info.Exp + " / " + info.Total;
    }

    private void OnBloodUISync(string eventName,object udata)
    {
        UIBloodInfo info = (UIBloodInfo) udata;
        this.bloodProcess.fillAmount =
            (float) info.Blood / (float) info.MaxBlood;
        this.bloodLabel.text = info.Blood + " / " + info.MaxBlood;
    }

    private void OnDestroy()
    {
        EventManager.Instance.RemoveEventListener("exp_ui_sync",
            OnExpUISync);
        EventManager.Instance.RemoveEventListener("blood_ui_sync",
            OnBloodUISync);
    }
}
