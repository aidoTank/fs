using UnityEngine;
//using XLua;
using System.IO;
using System.Text;


namespace Roma
{
    public class EGame
    {
        public static string m_version;
        public const string m_gameName = "石器时代之疯狂原始人";
        public static string m_openid = "0";
        public static long m_uin;
        public static string m_strPassword;
        public static int m_createRoleTime;
        public static bool m_bSendEnter = false; 
    }

    public class FSPParam
    {
        public const int clientFrameInterval = 30;        //客户端帧率
        
        public const float clientFrameScTime = 0.033f;        //客户端帧率时间(秒)
        //public const long clientFrameScLongTime = (long)((double)clientFrameScTime * FixedMath.One);        //客户端帧率时间(秒)
        public const int clientFrameMsTime = 33;        //客户端帧率时间(毫秒)
        public const int serverTimeout = 15000;          //服务器判断客户端的超时
        public const int clientFrameRateMultiple = 1;   //客户端和服务器端的帧率倍数
        public const bool enableSpeedUp = true;         //  是否可以加速
        public const int defaultSpeed = 1;  //默认的数量
        public const int frameBufferSize = 0; //需要缓存的帧数
        public const bool enableAutoBuffer = true;

        public const int maxFrameId = 1800; //最大的帧率数
        public const bool useLocal = false;  //是否使用本地数据
    }
}