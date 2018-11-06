using UnityEngine;
using System.IO;
using System.Text;
using System.Collections.Generic;

/// pc:
/// 1.pc上网络地址就是stream目录，是最新资源
/// 2.新建一个目录，用于模拟移到平台的沙盒目录
/// 3.首次进入游戏将主配置文件解压到沙盒
/// 4.下载网络主配置文件，加载沙盒主配置文件，进行版本对比，然后更新资源
/// 5.进入游戏，优先获取沙盒资源，如果没有去stram目录获取
namespace Roma
{
    public partial class GameInit
    {
        public static bool isFirstLoading = false;
        private UIPanelResInit m_panelInit;
        private UIPanelResInitDialog m_panelDialog;

        public bool Init()
        {
            if (Client.Inst() == null)
                return false;

            LogicSystem.Inst = new LogicSystem();
            LogicSystem.Inst.InitModule();

            m_panelInit = Client.Inst().m_uiResInit;
            m_panelDialog = Client.Inst().m_uiResInitDialog;

            m_panelInit.OpenPanel(true);
            m_panelInit.SetText("获取版本信息");
            m_panelInit.SetVersion("AppVersion:" + Application.version);


            if (Application.isEditor)
            {
                //InitIos();
                InitAndCheckUpdate();
            }
            else
            {
                if (Client.Inst().isCheckAppVersion)
                {
                    //if (Application.platform == RuntimePlatform.Android)
                    //{
                    //    InitAndroid();
                    //}
                    //else if (Application.platform == RuntimePlatform.IPhonePlayer)
                    //{
                    //    InitIos();
                    //}
                    //InitIos();
                }
                else
                {
                    InitAndCheckUpdate();
                }
            }
            return true;
        }

        public void InitAndCheckUpdate()
        {
            if (Client.Inst().isUpdate)
            {
                OnCheckFirst();
            }
            else
            {
                InitBaseConfig();
            }
        }

        public void InitBaseConfig()
        {
            isFirstLoading = true;


            Debug.Log("开始初始化配置");

            GlobleConfig.m_gameState = eGameState.Game;
            if(Application.isEditor)
            {
                GlobleConfig.m_downLoadType = Client.Inst().editorResPath;
            }
            else
            {
                GlobleConfig.m_downLoadType = eDownLoadType.None;
            }

            if(GlobleConfig.m_downLoadType  == eDownLoadType.LocalResource)
            {
                ResInfo rInfo = new ResInfo();
                rInfo.m_bDepend = false;
                rInfo.strName = "allresinfo";
                rInfo.strUrl = "config/allresinfo";
                rInfo.iType = ResType.ResInfosResource;
                ResourceFactory.Inst.LoadResource(rInfo, OnResInfoLoaded);
                return;
            }
            
            ResInfo pInfo = new ResInfo();
            pInfo.m_bDepend = false;
            pInfo.strName = ExportDefine.m_prefix;
            pInfo.strUrl = ExportDefine.m_prefix;
            pInfo.iType = ResType.ManifestResource;
            ResourceFactory.Inst.LoadResource(pInfo, OnManifestLoaded);

            LogicSystem.Inst.GetMapLoadProcess().strCurInfo = "正在加载主配置：";
            LogicSystem.Inst.GetMapLoadProcess().fPercent = 0.1f;
        }

        private void OnManifestLoaded(Resource res)
        {
            if(res.GetState() == eResourceState.eRS_NoFile)
            {
                ResourceFactory.Inst.UnLoadResource(res, true);
                UIPanelResInitDialog.OpenNoConnNet();
                return;
            }
            // 主配置不用销毁，因为要把配置给下载器使用
            ResourceManager.ABManifest = ((ManifestResource)res).GetManifest();    //获取总的Manifest

            ResInfo rInfo = new ResInfo();
            rInfo.m_bDepend = false;
            rInfo.strName = "allresinfo";
            rInfo.strUrl = "config/allresinfo";
            rInfo.iType = ResType.ResInfosResource;
            ResourceFactory.Inst.LoadResource(rInfo, OnResInfoLoaded);
        }

        private void OnResInfoLoaded(Resource res)
        {
            ResourceFactory.Inst.UnLoadResource(res, true);

            // 读取CSV之前，初始化CSV管理器,把CSV信息传给DLL，进行CSV构建
            LogicSystem.Inst.InitCsv(ref CsvManager.Inst);
            CsvListResource.SetCsvList(CsvManager.Inst.m_mapCSV);
            ResInfo resInfo = new ResInfo();
            resInfo.m_bDepend = false;
            resInfo.strName = "allcsvinfo";
            resInfo.strUrl = "config/allcsvinfo";
            resInfo.iType = ResType.CsvListResource;
            ResourceFactory.Inst.LoadResource(resInfo, OnCsvInfoLoaded);

            LogicSystem.Inst.GetMapLoadProcess().strCurInfo = "正在加载配置表：";
            LogicSystem.Inst.GetMapLoadProcess().fPercent = 0.2f;
        }

        private void OnCsvInfoLoaded(Resource res)
        {
            ResourceFactory.Inst.UnLoadResource(res, true);

            // 加载LUA
            if(Client.Inst()!= null && Client.Inst().isLua)
            {
                ResourceFactory.Inst.LoadResource(2, OnLuaInfoLoaded);
            }
            else
            {
                LogicSystem.Inst.GetMapLoadProcess().strCurInfo = "正在加载常用界面：";
                LogicSystem.Inst.GetMapLoadProcess().fPercent = 0.3f;
                m_curItmPct = 0.6f;
                LayoutMgr.Inst.OnLoadFristUI(OnInitFristUIEnd);
            }
        }

        private void OnLuaInfoLoaded(Resource res)
        {
            TextAsset[] luas = res.m_assertBundle.LoadAllAssets<TextAsset>();
            if (luas != null)
            {
#if UNITY_EDITOR
                luas = GetLuaListInEditor();
#endif
                LogicSystem.Inst.LuaInit(luas);
            }

            LogicSystem.Inst.GetMapLoadProcess().strCurInfo = "正在加载常用界面：";
            LogicSystem.Inst.GetMapLoadProcess().fPercent = 0.3f;
            m_curItmPct = 0.6f;
            LayoutMgr.Inst.OnLoadFristUI(OnInitFristUIEnd);
        }

        private void OnInitFristUIEnd(int cur, int max)
        {
            m_curItemNum = cur;
            m_maxItemNum = max;
            if (cur == max)
            {
                LayoutMgr.Inst.m_listLoading.Clear();
                LayoutMgr.Inst.m_listLoading = null;
                //InitLoadingShader();
                OnInitShaderEnd(0,0);
            }
        }

        private void InitLoadingShader()
        {
            // ShaderManager.Inst.LoadAllShader(OnInitShaderEnd);

            // LogicSystem.Inst.GetMapLoadProcess().strCurInfo = "正在加载其他资源：";
            // LogicSystem.Inst.GetMapLoadProcess().fPercent = 0.9f;
            // m_curItmPct = 0.1f;
        }

        private void OnInitShaderEnd(int cur, int max)
        {
            m_curItemNum = cur;
            m_maxItemNum = max;
            if (cur == max)
            {
                OnLoadingEnd();
            }
        }

        private void OnLoadingEnd()
        {
            isFirstLoading = false;
            Client.Inst().m_uiResInit.OpenPanel(false);
            LayoutMgr.Inst.InitLayout();
            //LayoutMgr.Inst.OpenUIBgm(LayoutMgr.UIBgmType.Main);
            LoginModule login = (LoginModule)LayoutMgr.Inst.GetLogicModule(LogicModuleIndex.eLM_PanelLogin);
            login.SetVisible(true);
        }

        public bool Update(float fTime, float fElsp)
        {
            // 资源更新的心跳
            // if (MtSkill_Base.m_skillEditer)
            //     return true;


            if (GlobleConfig.m_gameState == eGameState.Update)
            {
                // 更新进度
                OnUpdateUI();
            }
            if (isFirstLoading)
            {
                // 加载进度
                float pro = LogicSystem.Inst.GetMapLoadProcess().fPercent;
                if (m_maxItemNum != 0 && m_curItemNum != m_maxItemNum)
                {
                    pro = LogicSystem.Inst.GetMapLoadProcess().fPercent + ((float)m_curItemNum / (float)m_maxItemNum) * m_curItmPct;
                }
                m_panelInit.SetProgress(pro);
                m_panelInit.SetText(LogicSystem.Inst.GetMapLoadProcess().strCurInfo + (pro * 100).ToString("F2") + "%");
            }
            return true;
        }

        /// <summary>
        /// 当前要下载的所占比例，资源表和CSV暂用0.3,其余占用总和0.7
        /// 这个值+当前进度 等于当前要加载资源的结束百分比
        /// </summary>
        private float m_curItmPct;
        public int m_maxItemNum;
        public int m_curItemNum;




#if UNITY_EDITOR
        private string m_luaScriptPath = Application.dataPath + "/script/Lua/LuaScript";
        public TextAsset[] GetLuaListInEditor()
        {
            List<FileInfo> fileList = new List<FileInfo>();
            DirectoryInfo folder = new DirectoryInfo(m_luaScriptPath);
            GetAllFile(folder, "*.txt", ref fileList);
            GetAllFile(folder, "*.bytes", ref fileList);

            TextAsset[] tList = new TextAsset[fileList.Count];
            for(int i = 0; i < fileList.Count; i ++)
            {
                FileInfo file = fileList[i];
                string assetPath = file.FullName.Substring(file.FullName.IndexOf("Assets"));
                TextAsset myCfg = (TextAsset)UnityEditor.AssetDatabase.LoadAssetAtPath(assetPath, typeof(TextAsset));
                tList[i] = myCfg;
            }
            return tList;
        }

        private void GetAllFile(DirectoryInfo folder, string searchPattern, ref List<FileInfo> list)
        {
            DirectoryInfo[] dirInfo = folder.GetDirectories();
            foreach (DirectoryInfo item in dirInfo)
            {
                GetAllFile(item, searchPattern, ref list);
            }

            foreach (FileInfo file in folder.GetFiles(searchPattern))
            {
                list.Add(file);
            }
        }
#endif
    }
}