using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

public class utils
{
    public static string RandStr(int len)
    {
        byte[] b = new byte[4];
        new System.Security.Cryptography.RNGCryptoServiceProvider().GetBytes(b);
        System.Random r = new System.Random(System.BitConverter.ToInt32(b, 0));

        string str = null;
        str += "0123456789";
        str += "abcdefghijklmnopqrstuvwxyz";
        str += "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

        string s = null;

        for (int i = 0; i < len; i++)
        {
            s += str.Substring(r.Next(0, str.Length - 1), 1);
        }
        return s;
    }
    
    //获取md5值
    public static string md5(string str) {
        string cl = str;
        StringBuilder md5_builder = new StringBuilder();
        MD5 md5 = MD5.Create();//实例化一个md5对像
        // 加密后是一个字节类型的数组，这里要注意编码UTF8/Unicode等的选择　
        byte[] s = md5.ComputeHash(Encoding.UTF8.GetBytes(cl));
        // 通过使用循环，将字节类型的数组转换为字符串，此字符串是常规字符格式化所得
        for (int i = 0; i < s.Length; i++)
        {
            // 将得到的字符串使用十六进制类型格式。格式后的字符是小写的字母，如果使用大写（X）则格式后的字符是大写字符
            md5_builder.Append(s[i].ToString("X2"));
            //pwd = pwd + s[i].ToString("X");
        }
        return md5_builder.ToString();
    }


}
