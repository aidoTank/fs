using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Runtime.InteropServices;

namespace Roma
{
    /// <summary>
    /// 在编辑器下的资源平台类型
    /// </summary>
    public enum eResPlatform
    {
        Android,
        Ios
    }

    public partial class Client : MonoBehaviour
    {
        public static Client m_client;

        public string s_gameServerIP;
        public string s_gameServerPort;

        public string s_fileServerIP;
        public string s_fileServerPort;

        public string s_serverListIP;
        public string s_serverListPort;   // 获取服务器列表的端口

        public bool m_bDebug = true;
        /// <summary>
        /// 1.在VSCODE中写代码
        /// 2.写完后右键alllua.lua使用菜单打包HF_LUA
        /// 3.勾选编辑器下game测试LUA的开关
        /// 4.增加宏HOTFIX_ENABLE;INJECT_WITHOUT_TOOL
        /// 5.点击菜单XLUA-Clear...
        /// 6.点击菜单XLUA-Generate Code，生成中间代码
        /// 7.点击菜单XLUA-HotFix inject in editor，注入到编辑器
        /// </summary>
        public bool m_bLuaHotfix = false;
        public bool m_bDrawGrid = false;
        public float m_timeScale = 1;


        public int m_mobaCameraDis = 30;
        public int m_mobaCameraDir = 50;
        public int m_mobaCameraFov = 20;
        public float m_mobaCameraZOffset = 1f;

        public bool m_bSkillCamera = true;
        public float m_skillStartTime = 0.2f;
        public float m_skillEndTime = 30f;


        public GameInit m_gameInit = null;
        private GameUpdate m_gameUpdate = null;
        private GameQuit m_gameQuit = null;
        public UIPanelResInit m_uiResInit;
        public UIPanelResInitDialog m_uiResInitDialog;
        private GameObject m_gameTips;


        public static Client Inst()
        {

            //Debug.Log(Mathf.Cos(30 * Mathf.Deg2Rad));

            if (m_client == null)
            {
                Caching.CleanCache();
                m_client = GameObject.Find("game").GetComponent<Client>();
                m_client.InitIp();
                m_client.InitUI();
                //m_client.gameObject.AddComponent<GameInfo>();
                //m_client.gameObject.AddComponent<GameLog>();
                //GameObject.Find("fps").AddComponent<ShowFps>();
                Time.timeScale = 1;

                if (Application.isEditor)
                {
                    Debug.logger.logEnabled = m_client.m_bDebug;
                    Application.runInBackground = true;
                    QualitySettings.vSyncCount = 0;
                    Application.targetFrameRate = 120;
                }
                else if (Application.platform == RuntimePlatform.Android)
                {
                    Screen.sleepTimeout = SleepTimeout.NeverSleep;
                    // 真机开启ERROR级别的日志
                    Debug.logger.logEnabled = true;
                    //Debug.logger.filterLogType = LogType.Error;
                    Application.runInBackground = true;
                    //QualitySettings.vSyncCount = 2;
                    Application.targetFrameRate = 30;
                }
                else if (Application.platform == RuntimePlatform.IPhonePlayer)
                {
                    Screen.sleepTimeout = SleepTimeout.NeverSleep;
                    Debug.logger.logEnabled = m_client.m_bDebug;
                    Application.runInBackground = true;
                    //QualitySettings.vSyncCount = 1;
                    Application.targetFrameRate = 30;
                }
                // 在SHELL中卸载了Game关联的图集资源，就不用再使用masterTextureLimit，会发生贴图丢失
                //QualitySettings.antiAliasing = 0;        // 抗锯齿0248越大越好 手机上一般用不着
                //QualitySettings.masterTextureLimit = 0;  // 1为变成1/2贴图，2为变成1/4贴图
            }
            return m_client;
        }

        private void InitIp()
        {
            GlobleConfig.s_gameServerIP = s_gameServerIP;
            GlobleConfig.s_gameServerPort = s_gameServerPort;
            GlobleConfig.s_fileServerIP = s_fileServerIP;
            GlobleConfig.s_fileServerPort = s_fileServerPort;
            GlobleConfig.s_serverListIP = s_serverListIP;
            GlobleConfig.s_serverListPort = s_serverListPort;
        }

        private void InitUI()
        {
            m_uiResInit = GameObject.Find("panel_init").AddComponent<UIPanelResInit>();
            m_uiResInit.Init();
            m_uiResInitDialog = GameObject.Find("panel_res_init_dialog").AddComponent<UIPanelResInitDialog>();
            m_uiResInitDialog.Init();
        }

        private void Start()
        {
            //Handheld.PlayFullScreenMovie("first_movie.mov", Color.black, FullScreenMovieControlMode.CancelOnInput);
            Inst();
            Init();
        }

        private void Init()
        {
            m_gameInit = new GameInit();
            m_gameUpdate = new GameUpdate();
            m_gameQuit = new GameQuit();
            m_gameInit.Init();

            GameManager.Inst = new GameManager();
            GameManager.Inst.Init();
        }

        private void Update()
        {
            if (Application.isEditor)
            {
                Time.timeScale = m_timeScale;
            }
            if (m_gameInit != null && m_gameUpdate != null)
            {
                m_gameInit.Update();
                m_gameUpdate.Update(RealTime.time, Time.deltaTime);
            }
        }

        private void LateUpdate()
        {
            if (m_gameUpdate != null)
            {
                m_gameUpdate.LateUpdate(RealTime.time, Time.deltaTime);
            }

            if(Input.GetKeyDown(KeyCode.F1))
            {
                //CPlayerMgr.GetMaster().GoTo(216, 226, eControlMode.eCM_mouse, 0);
            }
        }


        private void FixedUpdate()
        {
            GameManager.Inst.FixedUpdate();
        }

        private void OnApplicationQuit()
        {
            if (m_gameQuit != null)
                m_gameQuit.Init();
        }

        /// <summary>
        /// 是否切到后台
        /// </summary>
        /// <param name="bPause"></param>
        private void OnApplicationPause(bool bPause)
        {
            //Debug.Log("OnApplicationPause:" + bPause);
            if (bPause)
            {
                // 切到后台
            }
            else
            {
                // 返回游戏
            }
        }


        /// <summary>
        /// 封装U3D的协程
        /// </summary>
        public void OnStartCoroutine(IEnumerator routine)
        {
            StartCoroutine(routine);
        }

#if UNITY_EDITOR
        //void OnDrawGizmos()
        //{
        //    Camera.main.depthTextureMode |= DepthTextureMode.Depth;
        //}
#endif
    }
}
