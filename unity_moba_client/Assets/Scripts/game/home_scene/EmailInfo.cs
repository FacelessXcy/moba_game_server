using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EmailInfo : MonoBehaviour
{
    public GameObject optPrefab;
    
    public ScrollRect scrollView;


    private void Start()
    {
        //监听拉取系统消息事件
        EventManager.Instance.AddEventListener("get_sys_email",
            OnGetSysEmailData);
        SystemServiceProxy.Instance.GetSysMsg();
    }

    private void OnDestroy()
    {
        EventManager.Instance.RemoveEventListener("get_sys_email",
        OnGetSysEmailData);
    }

    private void OnGetSysEmailData(string name, object udata)
    {
        //显示系统邮件列表
        List<string> sysMsgs= (List<string>)udata;
        if (sysMsgs==null||sysMsgs.Count<=0)
        {
            return;
        }
        
        scrollView.content.sizeDelta=new Vector2(0,sysMsgs.Count*160);
        for (int i = 0; i < sysMsgs.Count; i++)
        {
            GameObject opt = Instantiate(optPrefab);
            opt.transform.SetParent(scrollView.content,false);
            opt.transform.Find("order").GetComponent<Text>().text =
                (i + 1).ToString();
            opt.transform.Find("msg_content").GetComponent<Text>()
                .text = sysMsgs[i];
        }
    }

    public void OnCloseClick()
    {
        GameObject.Destroy(this.gameObject);
    }

}
