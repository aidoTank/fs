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
        public const int clientFrameRateMultiple = 4;   //客户端和服务器端的帧率倍数
        public const bool enableSpeedUp = true;         //  是否可以加速
        public const int defaultSpeed = 1;  //默认的数量
        public const int frameBufferSize = 0; //需要缓存的帧数
        public const bool enableAutoBuffer = true;

        public const int maxFrameId = 1800; //最大的帧率数
        public const bool useLocal = false;  //是否使用本地数据
    }

    public static class OverrideFunction
    {
        public static void SetActiveNew(this GameObject go, bool bShow)
        {
            if (bShow)
            {
                if (!go.activeSelf)
                    go.SetActive(true);
                if (go.transform.localScale != Vector3.one)
                {
                    go.transform.localScale = Vector3.one;
                }
            }
            else
            {
                if (go.transform.localScale != Vector3.zero)
                {
                    go.transform.localScale = Vector3.zero;
                }
            }
        }

        public static bool ActiveSelfNew(this GameObject go)
        {
            //if (go.transform.localScale == Vector3.one)
            //    return true;
            if (go.transform.localScale.y > 0.9f)//屏幕分辨率变化后 localScale != Vector3.one
                return true;
            return false;
        }

        //public static bool ActiveSelfNew(this Behaviour go)
        //{
        //    if (go.transform.localScale == Vector3.one)
        //        return true;
        //    return false;
        //}

        public static Vector3 ToVector3(this Vector2 vec)
        {
            return new Vector3(vec.x, 0, vec.y);
        }

        public static Vector2 ToVector2(this Vector3 vec)
        {
            return new Vector2(vec.x, vec.z);
        }

        public static Vector3 ToVector3(this Vector2d vec)
        {
            return new Vector3(vec.x.value, 0, vec.y.value);
        }

        public static Vector2d ToVector2d(this Vector3 vec)
        {
            return new Vector2d(vec.x, vec.z);
        }

        public static Vector2 ToVector2(this Vector2d vec)
        {
            return new Vector2(vec.x.value, vec.y.value);
        }

        public static Vector2d ToVector2d(this Vector2 vec)
        {
            return new Vector2d(vec.x, vec.y);
        }


        public static void SetActiveNew(this Transform component, bool bShow)
        {
            if (null != component)
                component.gameObject.SetActiveNew(bShow);
        }

        public static void AppQuit()
        {
            if (Application.isEditor)
            {
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#endif
            }
            else if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                //IOSInfo.OnExitApp();
            }
            else if (Application.platform == RuntimePlatform.Android ||
                Application.platform == RuntimePlatform.WindowsPlayer)
            {
                Application.Quit();
            }
        }


    }
}