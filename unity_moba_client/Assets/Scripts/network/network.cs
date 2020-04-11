using System;
using System.Net;
using System.Net.Sockets;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;

public class network : MonoBehaviour
{
    public string serverIP;
    public int port;
    private Socket _clientSocket;
    private bool _isConnect = false;

    private Thread _recvThread = null;
    private byte[] _recvBuffer=new byte[8192];
    void Awake()
    {
        DontDestroyOnLoad(gameObject);

    }

    private void Start()
    {
        connect_to_server();
        
        //test
        Invoke(nameof(close),5.0f);
        
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
            _clientSocket=new Socket(AddressFamily.InterNetwork,
            SocketType.Stream,ProtocolType.Tcp);
            
            IPAddress ipAddress = IPAddress.Parse((this.serverIP));
            IPEndPoint ipEndPoint=new IPEndPoint(ipAddress,port);
            
            IAsyncResult result = _clientSocket.BeginConnect
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

    void on_recv_data()
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
                int recvLen = _clientSocket.Receive(_recvBuffer);
                if (recvLen>0)//接收到了数据
                {
                    
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
            _recvThread=new Thread(on_recv_data);
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

}
