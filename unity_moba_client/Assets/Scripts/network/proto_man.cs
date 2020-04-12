using System;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using ProtoBuf;

public class cmd_msg
{
    public int stype;
    public int ctype;
    public byte[] body;//protobuf || utf8 jsonStr byte
}

public class proto_man
{
    private const int HEADER_SIZE = 8;//(2 stype 2 ctype 4 utag)8+msg.body
    
    private static byte[] protobuf_serializer(ProtoBuf.IExtensible data)
    {
        using (MemoryStream m = new MemoryStream())
        {
            byte[] buffer = null;
            Serializer.Serialize(m, data);
            m.Position = 0;
            int length = (int)m.Length;
            buffer = new byte[length];
            m.Read(buffer, 0, length);
            return buffer;
        }
    }

    //protobuf 打包成字节流(2 stype 2 ctype 4 utag)8+msg.body
    public static byte[] pack_protobuf_cmd(int stype, int ctype,
        ProtoBuf.IExtensible msg)
    {
        int cmd_len = HEADER_SIZE;
        byte[] cmd_body = null;
        if (msg!=null)
        {
            cmd_body = protobuf_serializer(msg);
            cmd_len += cmd_body.Length;
        }
        byte[] cmd=new byte[cmd_len];
        //stype ctype utag(4字节保留)，cmd_body
        data_viewer.write_ushort_le(cmd,0,(ushort)stype);
        data_viewer.write_ushort_le(cmd,2,(ushort)ctype);
        if (cmd_body!=null)
        {
            data_viewer.write_bytes(cmd,HEADER_SIZE,cmd_body);
        }
        
        return cmd;
    }

    public static byte[] pack_json_cmd(int stype, int ctype,
        string json_msg)
    {
        int cmd_len = HEADER_SIZE;
        byte[] cmd_body = null;
        if (json_msg.Length>0)//utf-8
        {
            cmd_body = Encoding.UTF8.GetBytes(json_msg);//string to bytes
            cmd_len += cmd_body.Length;
        }
        byte[] cmd=new byte[cmd_len];
        //stype ctype utag(4字节保留)，cmd_body
        data_viewer.write_ushort_le(cmd,0,(ushort)stype);
        data_viewer.write_ushort_le(cmd,2,(ushort)ctype);
        if (cmd_body!=null)
        {
            data_viewer.write_bytes(cmd,HEADER_SIZE,cmd_body);
        }
        return cmd;
    }
    
    //解包，解成(2 stype 2 ctype 4 utag)8+msg.body的对象
    public static bool unpack_cmd_msg(byte[] data,int start,int 
    cmdLen,out cmd_msg msg)
    {
        msg=new cmd_msg();
        msg.stype = data_viewer.read_ushort_le(data, start);
        msg.ctype = data_viewer.read_ushort_le(data, start + 2);
        int bodyLen = cmdLen - HEADER_SIZE;
        msg.body=new byte[bodyLen];
        Array.Copy(data,start+HEADER_SIZE,
            msg.body,0,bodyLen);
        return true;
    }
    
    //protobuf 解码函数
    public static T protobuf_deserialize<T>(byte[] _data)
    {
        using (MemoryStream m = new MemoryStream(_data))
        {
            return Serializer.Deserialize<T>(m);
        }
    }

    
}
