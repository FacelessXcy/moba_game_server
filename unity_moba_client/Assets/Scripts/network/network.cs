using System;
using System.Net;
using System.Net.Sockets;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using gprotocol;
using TMPro;
using UnityEngine;

public class network : MonoBehaviour
{
    public string serverIP;
    public int port;
    
    
    
    private Socket _clientSocket;
    private bool _isConnect = false;

    private Thread _recvThread = null;

    private const int RECV_LEN = 8192;
    private byte[] _recvBuf=new byte[RECV_LEN];
    private int _recved;
    private byte[] _longPkg = null;
    private int _longPkgSize = 0;

    //event queue
    private Queue<cmd_msg> _netEvent=new Queue<cmd_msg>();
    //event listener  stype->>监听者
    public delegate void net_message_handler(cmd_msg msg);
    //事件与监听的映射表
    private Dictionary<int, net_message_handler> _eventListeners =
        new Dictionary<int, net_message_handler>();
    
    
    
    private static network _instance;
    public static network Instance => _instance;

    void Awake()
    {
        _instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        connect_to_server();
        
        //test
        
        
    }

    private void Update()
    {
        lock (this._netEvent)
        {
            while (this._netEvent.Count>0)
            {
                cmd_msg msg = this._netEvent.Dequeue();
                //收到了一个命令包
                if (this._eventListeners.ContainsKey(msg.stype))
                {
                    this._eventListeners[msg.stype]?.Invoke(msg);
                }
            }
        }
    }

    private void OnDestroy()
    {
        //Debug.Log("net work onDestory");
        this.close();
    }

    private void OnApplicationQuit()
    {
        this.close();
    }



    void on_connect_timeout()
    {
        
    }
    void on_connect_error(string error)
    {
        
    }
    private void connect_to_server()
    {
        try
        {
            this._clientSocket=new Socket(AddressFamily.InterNetwork,
            SocketType.Stream,ProtocolType.Tcp);
            
            IPAddress ipAddress = IPAddress.Parse((this.serverIP));
            IPEndPoint ipEndPoint=new IPEndPoint(ipAddress,port);
            
            IAsyncResult result = this._clientSocket.BeginConnect
            (ipEndPoint,on_connected,
                _clientSocket);

            bool success = result.AsyncWaitHandle.WaitOne(5000, true);
            if (!success)//超时
            {
                on_connect_timeout();
            }
        }
        catch (Exception e)
        {
            Debug.Log(e.ToString());
            on_connect_error(e.ToString());
        }
    }
    
    private void on_recv_tcp_cmd(byte[] data,int rawDataStart,int rawDataLen)
    {
        cmd_msg msg;
        proto_man.unpack_cmd_msg(data, rawDataStart, rawDataLen, out msg);
        
        if (msg!=null)
        {
            //test
//            gprotocol.LoginRes res = proto_man
//                .protobuf_deserialize<gprotocol.LoginRes>(msg.body);
//            Debug.Log("res= "+res.status);
            lock (this._netEvent)//收数据线程
            {
                this._netEvent.Enqueue(msg);
            }
        }
    }

    private void on_recv_tcp_data()
    {
        byte[] pkgData = (this._longPkg != null)
            ? this._longPkg
            : this._recvBuf;
        while (this._recved>0)
        {
            int pkgSize = 0;
            int headSize = 0;
            //读取包头，获取包长
            if (!tcp_packer.read_header(pkgData,this._recved,out pkgSize,out headSize))
            {
                break;
            }
            if (this._recved<pkgSize)
            {
                break;
            }
            int rawDataStart = headSize;
            int rawDataLen = pkgSize - headSize;
            on_recv_tcp_cmd(pkgData,rawDataStart,rawDataLen);
            if (this._recved>pkgSize)
            {
                this._recvBuf = new byte[RECV_LEN];
                Array.Copy(pkgData,pkgSize,this._recvBuf,0,this
                ._recved-pkgSize);
                pkgData = this._recvBuf;
            }

            this._recved -= pkgSize;
            if (this._recved==0&&this._longPkg!=null)
            {
                this._longPkg = null;
                this._longPkgSize = 0;
            }
        }
    }

    void thread_recv_worker()
    {
        if (!_isConnect)
        {
            return;
        }
        while (true)
        {
            if (!_clientSocket.Connected)
            {
                break;
            }

            try
            {
                int recvLen = 0;
                if (this._recved < RECV_LEN)
                {
                    recvLen = this._clientSocket.Receive(this._recvBuf,
                        this._recved,
                        RECV_LEN-this._recved,SocketFlags.None);
                }
                else
                {
                    if (this._longPkg==null)
                    {
                        int pkgSize;
                        int headSize;
                        tcp_packer.read_header(this._recvBuf, this
                            ._recved, out pkgSize, out headSize);
                        this._longPkgSize = pkgSize;
                        this._longPkg=new byte[pkgSize];
                        Array.Copy(this._recvBuf,0,
                            this._longPkg,0,
                            this._recved);
                    }
                    recvLen = this._clientSocket.Receive(this._longPkg,
                        this._recved, this._longPkgSize - this._recved,
                        SocketFlags.None);
                }

                if (recvLen>0)
                {
                    this._recved += recvLen;
                    this.on_recv_tcp_data();
                }
                
            }
            catch (Exception e)
            {
                Debug.Log(e.ToString());
                _clientSocket.Disconnect(true);
                _clientSocket.Shutdown(SocketShutdown.Both);
                _clientSocket.Close();
                _isConnect = false;
                break;
            }
        }
    }

    void on_connected(IAsyncResult iar)
    {
        try
        {
            Socket client = (Socket)iar.AsyncState;
            client.EndConnect(iar);

            _isConnect = true;
            _recvThread=new Thread(thread_recv_worker);
            _recvThread.Start();
            Debug.Log("connect to server success!!"+serverIP+":"+port+"!");
        }
        catch (Exception e)
        {
            Debug.Log(e.ToString());
            on_connect_error(e.ToString());
            _isConnect = false;
        }   
    }
    
    //关闭线程与socket
    void close()
    {
        if (!_isConnect)
        {
            return;
        }
        //中断接收线程
        if (_recvThread!=null)
        {
            _recvThread.Abort();
        }

        if (_clientSocket!=null&&_clientSocket.Connected)
        {
            _clientSocket.Close();
        }
    }
    
    private void on_send_data(IAsyncResult iar)
    {
        try
        {
            Socket client = (Socket)iar.AsyncState;
            client.EndSend(iar);
        }
        catch (Exception e)
        {
            Debug.Log(e.ToString());
        }
    }
    
    public void send_protobuf_cmd(int stype, int ctype, 
        ProtoBuf.IExtensible body)
    {
        byte[] cmd_data =
            proto_man.pack_protobuf_cmd(stype, ctype, body);
        if (cmd_data==null)
        {
            return;
        }
        byte[] tcp_pkg = tcp_packer.pack(cmd_data);
        //打包完成
        _clientSocket.BeginSend(tcp_pkg, 0, tcp_pkg.Length, 
            SocketFlags.None, on_send_data, _clientSocket);

    }

    public void send_json_cmd(int stype, int ctype, string json_body)
    {
        byte[] cmd_data =
            proto_man.pack_json_cmd(stype, ctype, json_body);
        if (cmd_data==null)
        {
            return;
        }
        byte[] tcp_pkg = tcp_packer.pack(cmd_data);
        //打包完成
        _clientSocket.BeginSend(tcp_pkg, 0, tcp_pkg.Length, 
            SocketFlags.None, on_send_data, _clientSocket);
    }

    public void add_service_listeners(int stype,net_message_handler handler)
    {
        if (this._eventListeners.ContainsKey(stype))
        {
            this._eventListeners[stype] += handler;
        }
        else
        {
            this._eventListeners.Add(stype,handler);
        }
        
    }

    public void remove_service_listener(int stype,
        net_message_handler handler)
    {
        if (!this._eventListeners.ContainsKey(stype))
        {
            return;
        }
        this._eventListeners[stype] -= handler;
        if (this._eventListeners[stype]==null)
        {
            this._eventListeners.Remove(stype);
        }
    }

}
