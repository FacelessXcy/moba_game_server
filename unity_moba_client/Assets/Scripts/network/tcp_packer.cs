using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tcp_packer
{
    private const int HEADER_SIZE = 2;
    //打包tcp包
    public static byte[] pack(byte[] cmd_data)
    {
        int len = cmd_data.Length;
        if (len>65535-2)//如果包长过大，返回null  65535为2字节最大整数(ushort)
        {
            return null;
        }
        int cmd_len = len + HEADER_SIZE;
        byte[] cmd=new byte[cmd_len];
        data_viewer.write_ushort_le(cmd,0,(ushort)cmd_len);
        data_viewer.write_bytes(cmd,HEADER_SIZE,cmd_data);
        
        return cmd;
    }

    public static bool read_header(byte[] data, int data_len, out int
        pkg_size, out int head_size)
    {
        pkg_size = 0;
        head_size = 0;
        if (data_len<2)
        {
            return false;
        }

        head_size = 2;
        pkg_size = data_viewer.read_ushort_le(data,0);
        
        
        return true;
    }

}
