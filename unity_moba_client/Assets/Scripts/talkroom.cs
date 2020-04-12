using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using gprotocol;

public class talkroom : MonoBehaviour
{
    public ScrollRect scroll_view;
    public GameObject status_prefab;
    public GameObject talkinfo_prefab;
    public GameObject selftalk_prefab;
    public InputField input;

    private string _sendMsg = null;
    
    private static int index = 1;
    private void Start()
    {
        network.Instance.add_service_listeners(1,on_trm_server_return);

    }

    void on_login_return(byte[] body)
    {
        LoginRes res = proto_man.protobuf_deserialize<LoginRes>(body);
        if (res.status==1)
        {
            add_status_option("您已成功进入聊天室！");
        }else if (res.status==-1)
        {
            add_status_option("您已在聊天室中！");
        }
    }
    
    void on_exit_return(byte[] body)
    {
        ExitRes res = proto_man.protobuf_deserialize<ExitRes>(body);
        if (res.status==1)
        {
            add_status_option("您已成功退出聊天室！");
        }else if (res.status==-1)
        {
            add_status_option("您已不在聊天室中！");
        }
    }
    void on_send_msg_return(byte[] body)
    {
        SendMsgRes res = proto_man.protobuf_deserialize<SendMsgRes>(body);
        if (res.status==1)
        {
            add_self_option(this._sendMsg);
        }else if (res.status==-1)
        {
            add_status_option("您不在聊天室,无法发送消息！");
        }
    }
    void on_other_user_enter(byte[] body)
    {
        OnUserLogin res = proto_man.protobuf_deserialize<OnUserLogin>(body);
        add_status_option(res.ip+":"+res.port+"进入聊天室");
    }
    void on_other_user_exit(byte[] body)
    {
        OnUserExit res = proto_man.protobuf_deserialize<OnUserExit>
        (body);
        add_status_option(res.ip+":"+res.port+"退出聊天室");
    }
    void on_other_user_sendMsg(byte[] body)
    {
        OnSendMsg res = proto_man.protobuf_deserialize<OnSendMsg>
            (body);
        add_talk_option(res.ip,res.port,res.content);
    }

    void on_trm_server_return(cmd_msg msg)
    {
        switch (msg.ctype)
        {
            case (int)Cmd.eLoginRes:
                on_login_return(msg.body);    
                break;
            case (int)Cmd.eExitRes:
                on_exit_return(msg.body);
                break;
            case (int)Cmd.eSendMsgRes:
                on_send_msg_return(msg.body);
                break;
            case (int)Cmd.eOnUserLogin:
                on_other_user_enter(msg.body);
                break;
            case (int)Cmd.eOnUserExit:
                on_other_user_exit(msg.body);
                break;
            case (int)Cmd.eOnSendMsg:
                on_other_user_sendMsg(msg.body);
                break;
        }
    }

    void add_status_option(string status_info)
    {
        GameObject opt = GameObject.Instantiate(this.status_prefab);
        opt.transform.SetParent(this.scroll_view.content,false);

        Vector2 content_size = this.scroll_view.content.sizeDelta;
        content_size.y += 120;
        this.scroll_view.content.sizeDelta = content_size;

        opt.transform.Find("src").GetComponent<Text>().text =
            status_info;
        Vector3 local_pos = this.scroll_view.content.localPosition;
        local_pos.y += 120;
        this.scroll_view.content.localPosition = local_pos;
    }

    void add_talk_option(string ip,int port,string body)
    {
        GameObject opt = GameObject.Instantiate(this.talkinfo_prefab);
        opt.transform.SetParent(this.scroll_view.content,false);

        Vector2 content_size = this.scroll_view.content.sizeDelta;
        content_size.y += 120;
        this.scroll_view.content.sizeDelta = content_size;
        
        opt.transform.Find("src").GetComponent<Text>().text =
            body;
        opt.transform.Find("ip_port").GetComponent<Text>().text =
            ip +"："+ port;
        Vector3 local_pos = this.scroll_view.content.localPosition;
        local_pos.y += 120;
        this.scroll_view.content.localPosition = local_pos;
    }
    
    void add_self_option(string body)
    {
        GameObject opt = GameObject.Instantiate(this.selftalk_prefab);
        opt.transform.SetParent(this.scroll_view.content,false);

        Vector2 content_size = this.scroll_view.content.sizeDelta;
        content_size.y += 120;
        this.scroll_view.content.sizeDelta = content_size;
        
        opt.transform.Find("src").GetComponent<Text>().text =
            body;
        Vector3 local_pos = this.scroll_view.content.localPosition;
        local_pos.y += 120;
        this.scroll_view.content.localPosition = local_pos;
    }

    public void on_enter_talkroom()
    {
        network.Instance.send_protobuf_cmd(1,
            (int)Cmd.eLoginReq,null);
    }

    public void on_exit_talkroom()
    {
        network.Instance.send_protobuf_cmd(1,
            (int)Cmd.eExitReq,null);
    }

    public void on_send_msg()
    {
        if (this.input.text.Length<=0)
        {
            return;
        }
        SendMsgReq req=new SendMsgReq();
        req.content = this.input.text;
        this._sendMsg = this.input.text;
        network.Instance.send_protobuf_cmd(1, (int) Cmd.eSendMsgReq,
            req);
    }
}
