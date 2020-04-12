using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class data_viewer
{
    //ushort=2字节
    public static void write_ushort_le(byte[] buf, int offset,ushort value)
    {
        //value ---> byte[];
        byte[] byte_value = BitConverter.GetBytes(value);
        //小端，还是大端？BitConverter 取决于系统
        if (!BitConverter.IsLittleEndian)
        {
            Array.Reverse(byte_value);
        }
        Array.Copy(byte_value,0,buf,offset,byte_value.Length);
    }

    public static void write_uint_le(byte[] buf, int offset, uint value)
    {
        //value ---> byte[];
        byte[] byte_value = BitConverter.GetBytes(value);
        //小端，还是大端？BitConverter 取决于系统
        if (!BitConverter.IsLittleEndian)
        {
            Array.Reverse(byte_value);
        }
        Array.Copy(byte_value,0,buf,offset,byte_value.Length);

    }
    //写入内存
    public static void write_bytes(byte[] dst, int offset, byte[] value)
    {
        Array.Copy(value,0,dst,offset,value.Length);
    }

    public static ushort read_ushort_le(byte[] data, int offset)
    {
        int ret = (data[offset] | data[offset + 1] << 8);
        return (ushort) ret;
    }

}
