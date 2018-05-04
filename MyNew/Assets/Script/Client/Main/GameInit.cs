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
        public void Init()
        {
            // 逻辑初始化 //
            LogicSystem.Inst = new LogicSystem();
            LogicSystem.Inst.InitModule();

            InitBaseConfig();
            //OnCheckFirst();
        }

        public void InitBaseConfig()
        {
            Client.Inst().m_uiResInit.SetText("开始加载游戏");
            Debug.Log("开始初始化配置");

            GlobleConfig.m_gameState = eGameState.Game;
            GlobleConfig.m_downLoadType = eDownLoadType.None;
            ResInfo rInfo = new ResInfo();
            rInfo.m_bDepend = false;
            rInfo.strName = ExportDefine.m_prefix;
            rInfo.strUrl = ExportDefine.m_prefix;
            rInfo.iType = ResType.ManifestResource;
            ResourceFactory.Inst.LoadResource(rInfo, OnManifestLoaded);

            LogicSystem.Inst.GetMapLoadProcess().strCurInfo = "正在加载基本配置：";
            LogicSystem.Inst.GetMapLoadProcess().fPercent = 0.1f;
        }

        private void OnManifestLoaded(Resource res)
        {
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
            ResourceFactory.Inst.GC();

            ResInfo resInfo = new ResInfo();
            resInfo.m_bDepend = false;
            resInfo.strName = "allcsvinfo";
            resInfo.strUrl = "config/allcsvinfo";
            resInfo.iType = ResType.CsvListResource;
            ResourceFactory.Inst.LoadResource(resInfo, OnCsvInfoLoaded);

            LogicSystem.Inst.GetMapLoadProcess().strCurInfo = "正在加载基本配置：";
            LogicSystem.Inst.GetMapLoadProcess().fPercent = 0.2f;
        }

        private void OnCsvInfoLoaded(Resource res)
        {
            ResourceFactory.Inst.UnLoadResource(res, true);

            LogicSystem.Inst.InitData();
            ResourceFactory.Inst.LoadResource(2, OnLuaInfoLoaded);
        }

        static Resource r1;
        static Resource r2;
        static Resource r3;

        
        public static void LoadRestTest()
        {
            r1 = ResourceFactory.Inst.LoadResource(30001, (bRes) =>
            {
                bRes.InstantiateGameObject();
            });
            r2 = ResourceFactory.Inst.LoadResource(30002, (bRes) =>
            {
                bRes.InstantiateGameObject();
            });
            r3 = ResourceFactory.Inst.LoadResource(30002, (bRes) =>
            {
                bRes.InstantiateGameObject();
            });
        }

        public static void UnRestTest()
        {
            ResourceFactory.Inst.UnLoadResource(r1, false);
            ResourceFactory.Inst.UnLoadResource(r2, false);
            ResourceFactory.Inst.UnLoadResource(r3, false);
        }

        private void OnLuaInfoLoaded(Resource res)
        {
            //string name = res.GetResInfo().strName;
            TextAsset[] luas = res.m_assertBundle.LoadAllAssets<TextAsset>();
            if (luas != null)
            {
                LogicSystem.Inst.LuaInit(luas);
            }

            Client.Inst().m_uiResInit.OpenPanel(false);

            LogicSystem.Inst.LoadMap(1, () =>
            {

                CPlayer p = CPlayerMgr.CreateMaster(1);
                p.InitConfigure();
                p.SetPos(60, 27);
            });
        }

        private void OnLoadingEnd()
        {
            NetRunTime.Inst.ConServer(() => {
                    
            });
        }


        public void Update()
        {
            OnUpdateUI();
        }


        /// <summary>
        /// 第一次loading初始化
        /// </summary>
        public bool m_firstInitEnd = true;
        private Resource m_uiRes;

        /// <summary>
        /// 当前要下载的所占比例，资源表和CSV暂用0.3,其余占用总和0.7
        /// 这个值+当前进度 等于当前要加载资源的结束百分比
        /// </summary>
        private float m_curItmPct;
        public int m_maxItemNum;
        public int m_curItemNum;
    }
}