using System.Collections;
using System.IO;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Roma
{ 
	public class StringHelper 
	{
        public static int s_IntSize = 4;
        public static int s_UIntSize = 4;
        public static int s_LongSize = 8;
        public static int s_FloatSize = 4;
        public static int s_Float2Size = 8;
        public static int s_Float3Size = 12;
        public static int s_Float4Size = 16;
        public static int s_DoubleSize = 8;
        public static int s_CharSize = 1;
        public static int s_ByteSize = 1;
        public static int s_BoolSize = 1;
        public static int s_ShortSize = 2;
		//去除一个字符串里面的所有/n/t/r空格
		//返回一定清除了几个
		//参数2 是否删除 /r
		//参数3 是否删除 /t
		//参数4 是否删除 空格
		//参数5 是否删除 /n
		public static void Strip(ref string strOut, bool bR, bool bT, bool bSpace, bool bN, bool bLK, bool bRK, bool bC)
		{
			if(bR) 
			{
				strOut.Replace("\r", "");
			}
			if(bT)
			{
				strOut.Replace("\t", "");
			}
			if(bSpace)
			{
				strOut.Replace(" ", "");
			}
			if(bN)
			{
				strOut.Replace("\n", "");
			}
			if(bLK)
			{
				strOut.Replace("<", "");
			}
			if(bRK)
			{
				strOut.Replace(">", "");
			}
			if(bC)
			{
				strOut.Replace("\"", "");
			}
		}
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		public static void StandardPath(ref string strPath)
		{
			if(strPath == null && strPath.Length == 0)
			{
				return;
			}
			strPath.Replace("//", "\\");
			strPath.Replace("/", "\\");		
			//如果最后没有\\同时又没有.那么这个路径还不完整在拼接一个
			if (strPath[(strPath.Length - 1)] != '\\')
			{
				int iLastDot = strPath.LastIndexOf('.');
				//找不到点或者点是第一个。
				if (iLastDot <= 0)
				{
					//找不到点肯定不是全路径了
					strPath += "\\";
				}
			}
			strPath.ToLower();
		}

        public static string GetUnicodeStringByBytes(ref byte[] by)
        {
            return Encoding.Unicode.GetString(by);
        }

        public static string GetUTF8StringByBytes(ref byte[] byArray)
        {
            string str = Encoding.UTF8.GetString(byArray);
            int nLen = str.IndexOf('\0');
            if (nLen != -1)
            {
                str = str.Substring(0, nLen);
            }
            return str;
        }

        public static string GetString(string str)
        {
            int nLen = str.IndexOf('\0');
            if (nLen != -1)
            {
                str = str.Substring(0, nLen);
            }
            return str;
        }

        public static string GetUTF8StringByBytes(byte[] byArray)
        {
            string str = Encoding.UTF8.GetString(byArray);
            int nLen = str.IndexOf('\0');
            if (nLen != -1)
            {
                str = str.Substring(0, nLen);
            }
            return str;
        }

        public static byte[] GetBytesByStr(string str)
        {
            if(string.IsNullOrEmpty(str))
            {
                return Encoding.UTF8.GetBytes("");
            }
            return Encoding.UTF8.GetBytes(str.Trim());
        }


        public static int GetLength(string str)
        {
            if (str == null || str.Length == 0) { return 0; }

            int l = str.Length;
            int realLen = l;

            #region 计算长度
            int clen = 0;//当前长度
            while (clen < l)
            {
                //每遇到一个中文，则将实际长度加一。
                if ((int)str[clen] > 128) { realLen++; }
                clen++;
            }
            #endregion

            return realLen;
        }

        public static string GetSize(int size)
        {
            if (size / 1024.0f / 1024.0f >= 0.5f)
            {
                return (size / 1024.0f / 1024.0f).ToString("f1") + "MB";
            }
            else
            {
                return (size / 1024.0f).ToString("f1") + "KB";
            }
        }

        public static Vector3 GetVector3(string pos)
        {
            string[] posList = pos.Split('_');
            if (posList.Length != 3)
            {
                Debug.LogError("位置解析错误。" + pos);
                return Vector3.zero;
            }
            float x, y, z;
            float.TryParse(posList[0], out x);
            float.TryParse(posList[1], out y);
            float.TryParse(posList[2], out z);
            return new Vector3(x, y, z);
        }

        public static string GetVector3Str(Vector3 pos)
        {
            return pos.x + "_" + pos.y + "_" + pos.z;
        }

        public static Vector2 GetVector2(string pos)
        {
            string[] posList = pos.Split('_');
            if (posList.Length != 2)
            {
                Debug.LogError("位置解析错误。" + pos);
                return Vector3.zero;
            }
            float x, y;
            float.TryParse(posList[0], out x);
            float.TryParse(posList[1], out y);
            return new Vector2(x, y);
        }

        public static Color GetColor(string pos)
        {
            string[] colorList = pos.Split('_');
            if (colorList.Length != 3)
            {
                Debug.LogError("颜色解析错误。");
                return Color.white;
            }
            float x, y, z;
            float.TryParse(colorList[0], out x);
            float.TryParse(colorList[1], out y);
            float.TryParse(colorList[2], out z);
            return new Color(x, y, z);
        }

        public static int[] GetIntList(string info)
        {
            string[] infoList = info.Split('_');
            int[] rInfo = new int[infoList.Length];
            for(int i = 0; i < infoList.Length; i ++)
            {
                int val = 0;
                int.TryParse(infoList[i], out val);
                rInfo[i] = val;
            }
            return rInfo;
        }

    }
}