using System.Collections;
using System.IO;
using System.Collections.Generic;
using System.Text;
	public class StringHelper 
	{
        public const int s_IntSize = 4;
        public const int s_UIntSize = 4;
        public const int s_LongSize = 8;
        public const int s_FloatSize = 4;
        public const int s_Float2Size = 8;
        public const int s_Float3Size = 12;
        public const int s_Float4Size = 16;
        public const int s_DoubleSize = 8;
        public const int s_CharSize = 1;
        public const int s_ByteSize = 1;
        public const int s_BoolSize = 1;
        public const int s_ShortSize = 2;
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

        public static string getUnicodeStringByBytes(ref byte[] by)
        {
            return Encoding.Unicode.GetString(by);
        }

	}
