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
            //SoundManager.Inst = new SoundManager();
            //SingletonManager.Inst.AddSingleton(SingleName.m_sound, SoundManager.Inst);


            CameraMgr.Inst = new CameraMgr();
            SingletonManager.Inst.AddSingleton("cam", CameraMgr.Inst);

            KeyMgr.Inst = new KeyMgr();
            SingletonManager.Inst.AddSingleton("KeyMgr", KeyMgr.Inst);

            SingletonManager.Inst.Init();



            //// 初始化csv配置
            //InitCsv(ref CsvManager.Inst);
        }

        public void InitData()      // 所有配置下载完成时，初始化客户端本地数据
        {
            //InitMapData();
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
            CEffectMgr.Update(fTime, fDTime);
            //if (SceneManager.Inst != null && SceneManager.Inst.IsLoaded())
            //{
            //    CPointMgr.Update(fTime, fDTime);
            //    CPlayerMgr.Update(fTime, fDTime);
            //    CCameraMgr.Update();
            //}
        }

        public void LateUpdateModule(float fTime, float fDTime)
        {
            SingletonManager.Inst.LateUpdate(fTime, fDTime);
            //Profiler.BeginSample("LateUpdateModule");
            //if (SceneManager.Inst.IsLoaded())
            //{

            //}
            //Profiler.EndSample();
        }

        private void QuitGame()
        {
            MsgExit msg = (MsgExit)NetManager.Inst.GetMessage(eNetMessageID.MsgExit);
            NetRunTime.Inst.SendMessage(msg);
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
            //SoundManager.Inst = null;

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
        //public SceneLoaded m_mapLoadFinshed;
        //public SceneLoaded m_mapCreatureLoadFinshed;
        public bool m_fristLoadScene = true;
        //private QuadTree m_tree;
        private uint m_fightSoundHandle;

        public LoadProcess GetMapLoadProcess()
        {
            return m_mapLoadProcess;
        }

    }
}