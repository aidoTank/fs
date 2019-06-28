﻿using System.Runtime.CompilerServices;
using UnityEngine;
using System.Collections;

namespace Roma
{
    /// <summary>
    /// 平台标识
    /// 1.目录名和主资源名一样
    /// 2.编辑器下，用于打包不同平台的资源，例如：是安卓平台就打包到安卓路径目录
    /// 3.编辑器下，用于读取不同平台的资源，例如：是安卓平台就读取安卓路径资源
    /// </summary>
    public class ExportDefine
    {
#if UNITY_IOS
        public const string m_prefix = "iOS";
#elif UNITY_ANDROID
        public const string m_prefix = "android";
#elif UNITY_WEBPLAYER
        public const string m_prefix = "web";
#elif UNITY_WEBGL
        public const string m_prefix = "webgl";
#else
        public const string m_prefix = "pc";
#endif
    }

    public enum eGameState
    {
        First,
        Login,
        Update,
        Game,
    }

    public class GlobleConfig
    {
        public static eGameState m_gameState = eGameState.Update;
        public static eDownLoadType m_downLoadType = eDownLoadType.WWW;

        public static string s_gameServerIP = "";
        public static int s_gameServerPort;

        public static string s_frameServerIP = "";
        public static int s_frameServerPort = 2234;

        public static string s_fileServerIP = "";
        public static string s_fileServerPort = "";   //获取资源的端口

        public static string s_serverListIP = "192.168.1.140";
        public static string s_serverListPort = "6028";   // 获取服务器列表的端口


        public static string s_androidStreamingAssetsPath = Application.streamingAssetsPath + "/android/";
        public static string s_androidPersistentDataPath = "file://" + Application.persistentDataPath + "/android/";

        public static string s_iosStreamingAssetsPath = "file://" + Application.dataPath + "/Raw/ios/";
        public static string s_iosPersistentDataPath = "file://" + Application.persistentDataPath + "/ios/";

        public static string s_fileServerBasePath;


        public static string GetGameServerPath()
        {
            return s_gameServerIP + ":" + s_gameServerPort;
        }

        /// <summary>
        /// 区服导航地址
        /// </summary>
        public static string GetServerListPath()
        {
            if (Application.isEditor)
            {
                return "http://" + s_serverListIP + ":" + s_serverListPort + "/tree";
            }
            switch (Application.platform)
            {
                case RuntimePlatform.Android:
                    return "http://" + s_serverListIP + ":" + s_serverListPort + "/tree";
                case RuntimePlatform.IPhonePlayer:
                    return "https://" + s_serverListIP + ":" + s_serverListPort + "/tree";
            }
            return "";
        }

        /// <summary>
        /// 网络地址
        /// pc:
        /// 1.pc上网络地址就是stream目录，是最新资源
        /// 2.新建一个目录，用于模拟移到平台的沙盒目录
        /// 3.首次进入游戏将主配置文件解压到沙盒
        /// 4.下载网络主配置文件，加载沙盒主配置文件，进行版本对比，然后更新资源
        /// 5.进入游戏，优先获取沙盒资源，如果没有去stram目录获取
        /// </summary>
        /// <returns></returns>
        public static string GetFileServerPath()
        {
            if (string.IsNullOrEmpty(s_fileServerBasePath))
            {
                s_fileServerBasePath = "http://" + s_fileServerIP + ":" + s_fileServerPort + "/" + ExportDefine.m_prefix + "/";
            }
            return s_fileServerBasePath;
        }


        /// <summary>
        /// 获取包内文件
        /// </summary>
        public static string GetStreamingPath()
        {
            switch (Application.platform)
            {
                case RuntimePlatform.Android:   //安卓
                    return s_androidStreamingAssetsPath;
                case RuntimePlatform.IPhonePlayer:  //Iphone
                    return s_iosStreamingAssetsPath;
                case RuntimePlatform.WindowsPlayer:
                    return "file://" + string.Concat(Application.dataPath, "/StreamingAssets/" + ExportDefine.m_prefix + "/");
                case RuntimePlatform.WindowsEditor:
                    return "file://" + string.Concat(Application.dataPath, "/StreamingAssets/" + ExportDefine.m_prefix + "/");

            }
            return "";
        }


        /// <summary>
        /// 获取沙盒
        /// </summary>
        public static string GetPersistentPath()
        {
            switch (Application.platform)
            {
                case RuntimePlatform.Android:   
                    return s_androidPersistentDataPath;
                case RuntimePlatform.IPhonePlayer:  
                    return s_iosPersistentDataPath;
                case RuntimePlatform.WindowsPlayer:
                    return "file://" + string.Concat(Application.dataPath, "/../AssetsResources/" + ExportDefine.m_prefix + "/");
                case RuntimePlatform.WindowsEditor:
                    return "file://" + string.Concat(Application.dataPath, "/../AssetsResources/"+ ExportDefine.m_prefix + "/");

            }
            return "";
        }

        /// <summary>
        /// 获取沙盒的SD卡的文件路径
        /// </summary>
        public static string GetPersistentPath_File()
        {
            switch (Application.platform)
            {
                case RuntimePlatform.Android:
                    return Application.persistentDataPath + "/android/";
                case RuntimePlatform.IPhonePlayer:
                    return Application.persistentDataPath + "/ios/";
                case RuntimePlatform.WindowsPlayer:
                    return string.Concat(Application.dataPath, "/../AssetsResources/" + ExportDefine.m_prefix + "/");
                case RuntimePlatform.WindowsEditor:
                    return string.Concat(Application.dataPath, "/../AssetsResources/" + ExportDefine.m_prefix + "/");

            }
            return "";
        }

    }


    public enum eDownLoadType
    {
        None,
        WWW,
        StreamingAssetsPath,
        PersistentDataPath,

        LocalResource,
    }

    public enum LusuoLayer
    {
        eEL_Default = 0,
        eEL_Transparent = 1,
        eEL_NotPick = 2,
        eEL_Water = 4,
        eEL_UI = 5,

        eEL_Static = 8,
        eEL_Dynamic = 9,
        eEL_StaticNoRcShadow = 10,
        eEL_StaticCollision = 11,
        eEL_StaticNoCulling = 12,
        eEL_StaticNoWalking = 13,

        eEL_Photo = 21,
        eEL_Sound = 22,
        eEL_Hide = 23,
        eEL_UIEffect = 24,

        eEL_Grass = 29,
        eEL_Terrain = 30,
        eEL_NavMesh = 31,
    }

    // 最大长度32位的二进制，1表示检测该层，可以做&，|，~位运算
    public enum LusuoLayerMask
    {
        eEL_Default = 1 << 0,
        eEL_Transparent = 1 << 1,
        eEL_NotPick = 1 << 2,
        eEL_Water = 1 << 4,
        eEL_UI = 1 << 5,

        eEL_Static = 1 << 8,    // 01 0000 0000
        eEL_Dynamic = 1 << 9,   // 10 0000 0000
        eEL_StaticNoRcShadow = 1 << 10,
        eEL_StaticCollision = 1 << 11,
        eEL_StaticNoCulling = 1 << 12,
        eEL_StaticNoWalking = 1 << 13,      // 静态不能走，就是不能点击移动,也用于遮挡模型透明。

        eEL_Photo = 1 << 21,
        eEL_Sound = 1 << 22,
        eEL_Hide = 1 << 23,
        eEL_UIEffect = 1 << 24,

        eEL_Grass = 1 << 29,
        eEL_Terrain = 1 << 30,
        eEL_NavMesh = 1 << 31,
    }

    public static class OverrideFunction
    {
        public static void SetActiveNew(this GameObject go, bool bShow)
        {
            if (bShow)
            {
                if (!go.activeSelf)
                    go.SetActive(true);
                if(go.transform.localScale != Vector3.one)
                {
                    go.transform.localScale = Vector3.one;
                }
            }
            else
            {
                if(go.transform.localScale != Vector3.zero)
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

        public static void SetActiveNew(this Component component, bool bShow)
        {
            if (null != component)
                component.gameObject.SetActiveNew(bShow);
        }

        public static void AppQuit()
        {
            if(Application.isEditor)
            {
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#endif
            }
            else if(Application.platform == RuntimePlatform.IPhonePlayer)
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