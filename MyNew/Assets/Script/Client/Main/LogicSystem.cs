using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Roma;

namespace Roma
{
    public partial class LogicSystem
    {
        public LogicSystem()
        {
        }

        public void InitModule()
        {
            NetRunTime.Inst = new NetRunTime();
            SingletonManager.Inst.AddSingleton(SingleName.m_netRun, NetRunTime.Inst);
            NetManager.Inst = new NetManager();
            SingletonManager.Inst.AddSingleton(SingleName.m_netMgr, NetManager.Inst);

            CsvManager.Inst = new CsvManager();
            SingletonManager.Inst.AddSingleton(SingleName.m_CSV, CsvManager.Inst);

            ResourceFactory.Inst = new ResourceFactory();
            SingletonManager.Inst.AddSingleton(SingleName.m_ResFac, ResourceFactory.Inst);

            ResourceManager.Inst = new ResourceManager();
            SingletonManager.Inst.AddSingleton(SingleName.m_ResMgr, ResourceManager.Inst);

            DPResourceManager.Inst = new DPResourceManager();
            SingletonManager.Inst.AddSingleton(SingleName.m_ResDpMgr, DPResourceManager.Inst);


            GUIManager.Inst = new GUIManager();
            SingletonManager.Inst.AddSingleton("gui", GUIManager.Inst);
            LayoutMgr.Inst = new LayoutMgr();
            SingletonManager.Inst.AddSingleton("layout", LayoutMgr.Inst);

            SceneManager.Inst = new SceneManager();
            SingletonManager.Inst.AddSingleton(SingleName.m_Scene, SceneManager.Inst);
            EntityManager.Inst = new EntityManager();
            SingletonManager.Inst.AddSingleton(SingleName.m_Entity, EntityManager.Inst);
            SoundManager.Inst = new SoundManager();
            SingletonManager.Inst.AddSingleton(SingleName.m_sound, SoundManager.Inst);
        
            SingletonManager.Inst.Init();

            //// 初始化csv配置
            //InitCsv(ref CsvManager.Inst);
        }

        public void InitData()      // 所有配置下载完成时，初始化客户端本地数据
        {
            InitMapData();
        }

       
        private void InitMapData()
        {
            MapCsv mapCsv = CsvManager.Inst.GetCsv<MapCsv>((int)eAllCSV.eAC_Map);
            foreach (KeyValuePair<uint, MapData> item in mapCsv.m_mapDataDic)
            {
                SceneManager.Inst.SetSceneData(item.Value);
            }
        }



        public int GetMapId()
        {
            return (int)SceneManager.Inst.GetMap().GetMapID();
        }

        public void LoadMap(uint mapId, SceneLoaded loaded)
        {
            if(mapId != SceneManager.Inst.GetMap().GetMapID())
            {
                m_mapLoadFinshed = loaded;
                // 逻辑相关
                SceneManager.Inst.LoadMap(mapId, ref m_mapLoadProcess, OnMapLoaded);
                // 打开进度条
                CSharpCallLua.OpenLoading(true);
            }
            else
            {
                Debug.LogWarning("同一场景无需加载" + mapId);
            }
        }

        private void OnMapLoaded()
        {
            // 关闭进度条
            CSharpCallLua.OpenLoading(false);
            if (m_mapLoadFinshed != null)
            {
                m_mapLoadFinshed();
            }
        }

        /// <summary>
        /// 0到3，高到低
        /// </summary>
        /// <param name="shadow"></param>
        public void SetQualityLevel(int level)
        {
            QualitySettings.masterTextureLimit = level;
        }

        public void UpdateModule(float fTime, float fDTime)
        {
            Lua_Update(fTime, fDTime);
            SingletonManager.Inst.Update(fTime, fDTime);
            if (SceneManager.Inst != null && SceneManager.Inst.IsLoaded())
            {
                CPointMgr.Update(fTime, fDTime);
                CPlayerMgr.Update(fTime, fDTime);
                CCameraMgr.Update();
            }
        }

        public void LateUpdateModule(float fTime, float fDTime)
        {
            //Profiler.BeginSample("LateUpdateModule");
            //if (SceneManager.Inst.IsLoaded())
            //{

            //}
            //Profiler.EndSample();
        }

        private void QuitGame()
        {

        }

        public void UnInitModule()
        {
            Lua_Destroy();
            QuitGame();

            SingletonManager.Inst.Destroy();
            SingletonManager.Inst.RemoveSingleton(SingleName.m_netRun);
            NetRunTime.Inst = null;
            SingletonManager.Inst.RemoveSingleton(SingleName.m_netMgr);
            NetManager.Inst = null;

            SingletonManager.Inst.RemoveSingleton(SingleName.m_CSV);
            CsvManager.Inst = null;

            SingletonManager.Inst.RemoveSingleton(SingleName.m_Scene);
            SceneManager.Inst = null;
            SingletonManager.Inst.RemoveSingleton(SingleName.m_Entity);
            EntityManager.Inst = null;

            SingletonManager.Inst.RemoveSingleton(SingleName.m_sound);
            SoundManager.Inst = null;

            SingletonManager.Inst.RemoveSingleton(SingleName.m_ResMgr);
            ResourceManager.Inst = null;
            SingletonManager.Inst.RemoveSingleton(SingleName.m_ResDpMgr);
            DPResourceManager.Inst = null;
            SingletonManager.Inst.RemoveSingleton(SingleName.m_ResFac);
            ResourceFactory.Inst.GC();
            ResourceFactory.Inst = null;

            SingletonManager.Inst = null;
            Inst = null;
        }

        public static LogicSystem Inst = null;

        private LoadProcess m_mapLoadProcess = new LoadProcess();
        public SceneLoaded m_mapLoadFinshed;
        public SceneLoaded m_mapCreatureLoadFinshed;
        public bool m_fristLoadScene = true;
        //private QuadTree m_tree;
        private uint m_fightSoundHandle;

        public LoadProcess GetMapLoadProcess()
        {
            return m_mapLoadProcess;
        }

    }
}