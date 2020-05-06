using System;
using System.Net;
using System.Net.Sockets;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using gprotocol;
using TMPro;
using UnityEngine;

public class network : UnitySingleton<network>
{
    #region TCP
    
    private string _serverIP="127.0.0.1";
    private int _port=6080;
    private Socket _clientSocket;
    private bool _isConnect = false;
    private Thread _recvThread = null;
    private const int RECV_LEN = 8192;
    private byte[] _recvBuf=new byte[RECV_LEN];
    private int _recved;
    private byte[] _longPkg = null;
    private int _longPkgSize = 0;
    
    #endregion

    #region UDP

    private string _udpServerIP = "127.0.0.1";
    private int _udpPort = 8800;
    private IPEndPoint _udpRemotePoint;
    private Socket _udpSocket;
    private byte[] _udpRecvBuf=new byte[60*1024];
    private Thread _udpRecvThread;
    public int localUdpPort = 8888;
    
    #endregion
    
    
    //event queue
    private Queue<cmd_msg> _netEvent=new Queue<cmd_msg>();
    //event listener  stype->>监听者
    public delegate void net_message_handler(cmd_msg msg);
    //事件与监听的映射表
    private Dictionary<int, net_message_handler> _eventListeners =
        new Dictionary<int, net_message_handler>();
    
    
    private void Start()
    {
        ConnectToServer();
        UdpSocketInit();
        //test udp data
        //this.InvokeRepeating(nameof(TestUdp),5,5);
    }

    private void TestUdp()
    {
        Debug.Log("Udp Send");
        LogicServiceProxy.Instance.SendUdpTest("HelloWorld!!");
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
        this.Close();
        
    }

    private void OnApplicationQuit()
    {
        this.Close();
        this.UdpClose();
    }
    
    void OnConnectTimeout()
    {
        
    }
    void OnConnectError(string error)
    {
        
    }
    private void ConnectToServer()
    {
        try
        {
            this._clientSocket=new Socket(AddressFamily.InterNetwork,
            SocketType.Stream,ProtocolType.Tcp);
            
            IPAddress ipAddress = IPAddress.Parse((this._serverIP));
            IPEndPoint ipEndPoint=new IPEndPoint(ipAddress,_port);
            
            IAsyncResult result = this._clientSocket.BeginConnect
            (ipEndPoint,OnConnected,
                _clientSocket);

            bool success = result.AsyncWaitHandle.WaitOne(5000, true);
            if (!success)//超时
            {
                OnConnectTimeout();
            }
        }
        catch (Exception e)
        {
            Debug.Log(e.ToString());
            OnConnectError(e.ToString());
        }
    }
    
    private void OnRecvCmd(byte[] data,int rawDataStart,int rawDataLen)
    {
        cmd_msg msg;
        proto_man.unpack_cmd_msg(data, rawDataStart, rawDataLen, out msg);
        
        if (msg!=null)
        {
            lock (this._netEvent)//收数据线程
            {
                this._netEvent.Enqueue(msg);
            }
        }
    }

    private void OnRecvTcpData()
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
            OnRecvCmd(pkgData,rawDataStart,rawDataLen);
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

    void ThreadRecvWorker()
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
                    this.OnRecvTcpData();
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

    void OnConnected(IAsyncResult iar)
    {
        try
        {
            Socket client = (Socket)iar.AsyncState;
            client.EndConnect(iar);

            _isConnect = true;
            _recvThread=new Thread(ThreadRecvWorker);
            _recvThread.Start();
            Debug.Log("connect to server success!!"+_serverIP+":"+_port+"!");
        }
        catch (Exception e)
        {
            Debug.Log(e.ToString());
            OnConnectError(e.ToString());
            _isConnect = false;
        }   
    }
    
    //关闭线程与socket
    void Close()
    {
        if (!_isConnect)
        {
            return;
        }

        this._isConnect = false;
        //中断接收线程
        if (_recvThread!=null)
        {
            _recvThread.Interrupt();
            _recvThread.Abort();
            _recvThread = null;
        }

        if (_clientSocket!=null&&_clientSocket.Connected)
        {
            _clientSocket.Close();
            _clientSocket = null;
        }
    }
    
    private void OnSendData(IAsyncResult iar)
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
    
    public void SendProtobufCmd(int stype, int ctype, 
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
            SocketFlags.None, OnSendData, _clientSocket);

    }

    public void SendJsonCmd(int stype, int ctype, string json_body)
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
            SocketFlags.None, OnSendData, _clientSocket);
    }

    public void AddServiceListeners(int stype,net_message_handler handler)
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

    public void RemoveServiceListener(int stype,
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

    private void UdpThreadRecvWorker()
    {
        while (true)
        {
            EndPoint remote=(EndPoint)new IPEndPoint(IPAddress.Parse
            (this._udpServerIP),this._udpPort);
            //Debug.Log("Begin Receive");
            int recved = this._udpSocket.ReceiveFrom(_udpRecvBuf,ref remote);
            //Debug.Log("End Receive");
            this.OnRecvCmd(this._udpRecvBuf,0,recved);
        }
    }
    
    private void UdpClose()
    {
        
        if (_udpRecvThread!=null)
        {
            _udpRecvThread.Interrupt();
            _udpRecvThread.Abort();
            _udpRecvThread = null;
        }

        if (this._udpSocket!=null)
        {
            _udpSocket.Close();
            _udpSocket = null;
        }
    }

    private void UdpSocketInit()
    {
        //配置远程传输端口
        this._udpRemotePoint = new IPEndPoint(IPAddress.Parse(this
            ._udpServerIP), this._udpPort);
        //创建udp的sokcet
        this._udpSocket = new Socket(AddressFamily.InterNetwork,
            SocketType.Dgram, ProtocolType.Udp);
        //接收数据,使用另外一个线程，如果不绑定，无法接收
        IPEndPoint localPoint = new IPEndPoint(IPAddress.Parse(
            "127.0.0.1"), this.localUdpPort);
        //Debug.Log(_udpSocket==null);
        this._udpSocket.Bind(localPoint);
        
        //在特定的端口上，启动接收线程
        this._udpRecvThread=new Thread(UdpThreadRecvWorker);
        this._udpRecvThread.Start();
    }
    
    private void OnUdpSendData(IAsyncResult iar)
    {
        try
        {
            Socket client = (Socket)iar.AsyncState;
            client.EndSendTo(iar);
        }
        catch (Exception e)
        {
            Debug.Log(e.ToString());
        }
    }
    
    public void UdpSendProtobufCmd(int stype, int ctype, 
        ProtoBuf.IExtensible body)
    {
        byte[] cmd_data =
            proto_man.pack_protobuf_cmd(stype, ctype, body);
        if (cmd_data==null)
        {
            return;
        }
        //打包完成
        _udpSocket.BeginSendTo(cmd_data,0,cmd_data.Length,
        SocketFlags.None,this._udpRemotePoint,OnUdpSendData,this._udpSocket);

    }

}
