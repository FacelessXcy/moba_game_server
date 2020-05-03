using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using gprotocol;
using UnityEngine.PlayerLoop;

public class AuthServiceProxy:Singleton<AuthServiceProxy>
{
    private string g_key = null;
    private bool _isSaveGKey = false;

    private EditProfileReq _tempReq = null;
    public void Init()
    {
        network.Instance.add_service_listeners((int)Stype.Auth,OnAuthServerReturn);
    }

    private void OnGuestAccountUpgradeReturn(cmd_msg msg)
    {
        AccountUpgradeRes res = proto_man
            .protobuf_deserialize<AccountUpgradeRes>(msg.body);
        if (res.status==Response.OK)
        {
            UGame.Instance.isGuest = false;
        }

        EventManager.Instance.DispatchEvent("upgrade_account_return",
            res.status);
        //本地保存的guest_key清空
        PlayerPrefs.SetString("xcy_moba_guest_key","");
        
    }

    private void OnUnameLoginReturn(cmd_msg msg)
    {
        UnameLoginRes res = proto_man
            .protobuf_deserialize<UnameLoginRes>(msg.body);
        if (res==null)
        {
            return;
        }

        if (res.status!=Response.OK)
        {
            Debug.Log("Uname Login status:"+res.status);
            return;
        }
        UserCenterInfo uinfo = res.uinfo;
        UGame.Instance.SaveUInfo(uinfo,false);
        
        EventManager.Instance.DispatchEvent("login_success",null);
        EventManager.Instance.DispatchEvent("sync_uinfo",null);
    }

    private void OnGuestLoginReturn(cmd_msg msg)
    {
        GuestLoginRes res = proto_man
            .protobuf_deserialize<GuestLoginRes>(msg.body);
        if (res==null)
        {
            return;
        }

        if (res.status!=Response.OK)
        {
            Debug.Log("Guest Login status:"+res.status);
            return;
        }

        UserCenterInfo uinfo = res.uinfo;
        UGame.Instance.SaveUInfo(uinfo,true,this.g_key);
        
        //保存游客Key到本地
        if (this._isSaveGKey)
        {
            this._isSaveGKey = false;
            PlayerPrefs.SetString("xcy_moba_guest_key",this.g_key);
        }
        
        EventManager.Instance.DispatchEvent("login_success",null);
        EventManager.Instance.DispatchEvent("sync_uinfo",null);
    }

    private void OnEditProfileReturn(cmd_msg msg)
    {
        EditProfileRes res = proto_man
            .protobuf_deserialize<EditProfileRes>(msg.body);
        if (res==null)
        {
            return;
        }
        if (res.status!=Response.OK)
        {
            Debug.Log("Edit Profile status:"+res.status);
            return;
        }
        UGame.Instance.SaveEditProfile(this._tempReq.unick,this
        ._tempReq.uface,this._tempReq.usex);
        this._tempReq = null;
        EventManager.Instance.DispatchEvent("sync_uinfo",null);

    }

    private void OnUserLoginOutReturn(cmd_msg msg)
    {
        LoginOutRes res = proto_man
            .protobuf_deserialize<LoginOutRes>(msg.body);
        if (res==null)
        {
            return;
        }
        if (res.status!=Response.OK)
        {
            Debug.Log("User Login Out status:"+res.status);
            return;
        }
        
        //注销完成
        UGame.Instance.UserLoginOut();
        EventManager.Instance.DispatchEvent("login_out",null);
    }

    void OnAuthServerReturn(cmd_msg msg)
    {
        switch (msg.ctype)
        {
            case (int)Cmd.eGuestLoginRes:
                OnGuestLoginReturn(msg);
                break;
            case (int)Cmd.eEditProfileRes:
                OnEditProfileReturn(msg);
                break;
            case (int)Cmd.eAccountUpgradeRes:
                OnGuestAccountUpgradeReturn(msg);
                break;
            case (int)Cmd.eUnameLoginRes:
                OnUnameLoginReturn(msg);
                break;
            case (int)Cmd.eLoginOutRes:
                OnUserLoginOutReturn(msg);
                break;
        }
        
    }

    public void GuestLogin()
    {
        this.g_key = PlayerPrefs.GetString("xcy_moba_guest_key");
        //this.g_key = null;
        this._isSaveGKey = false;
        if (this.g_key==null||this.g_key.Length!=32)
        {
            this.g_key = utils.RandStr(32);
            //this.g_key = "to9OveCec00GdBnaTp0mFRashRUsnQJu";
            this._isSaveGKey = true;
            Debug.Log("random key:"+this.g_key);
        }

//        this.g_key = "Hello";
        GuestLoginReq req=new GuestLoginReq();
        req.guest_key = this.g_key;
        network.Instance.send_protobuf_cmd((int)Stype.Auth,(int)Cmd
        .eGuestLoginReq,req);
    }

    public void UnameLogin(string uname,string upwd)
    {
        string upwd_md5 = utils.md5(upwd);
        
        Debug.Log(uname+" "+upwd_md5);
        UnameLoginReq req=new UnameLoginReq();
        req.uname = uname;
        req.upwd = upwd_md5;
        network.Instance.send_protobuf_cmd((int)Stype.Auth,(int)Cmd
        .eUnameLoginReq,req);
    }

    public void DoAccountUpgrade(string uname,string upwd_md5)
    {
        AccountUpgradeReq req=new AccountUpgradeReq();
        req.uname = uname;
        req.upwd_md5 = upwd_md5;
        network.Instance.send_protobuf_cmd((int)Stype.Auth,(int)Cmd
        .eAccountUpgradeReq,req);
    }
    
    public void EditProfile(string unick,int uface,int usex)
    {
        if (unick.Length<=0)
        {
            return;
        }

        if (uface<=0||uface>9)
        {
            return;
        }
        if (usex!=0&&usex!=1)
        {
            return;
        }
        //提交修改资料的请求
        EditProfileReq req=new EditProfileReq();
        req.unick = unick;
        req.uface = uface;
        req.usex = usex;
        this._tempReq = req;
        network.Instance.send_protobuf_cmd((int)Stype.Auth,(int)Cmd
        .eEditProfileReq,req);
        
    }

    public void UserLoginOut()
    {
        network.Instance.send_protobuf_cmd((int)Stype.Auth,(int)Cmd
            .eLoginOutReq,null);
    }

}
