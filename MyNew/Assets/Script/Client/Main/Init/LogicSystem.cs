using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Roma;

namespace Roma
{
    /// <summary>
    /// 针对所有管理器的管理类
    /// 底层管理类+逻辑管理类
    /// </summary>
    public partial class LogicSystem
    {
        public LogicSystem()
        {
        }

        public void InitModule()
        {
            CsvManager.Inst = new CsvManager();
            SingletonManager.Inst.AddSingleton(SingleName.m_CSV, CsvManager.Inst);

            ResourceFactory.Inst = new ResourceFactory();
            SingletonManager.Inst.AddSingleton(SingleName.m_ResFac, ResourceFactory.Inst);

            ResourceManager.Inst = new ResourceManager();
            SingletonManager.Inst.AddSingleton(SingleName.m_ResMgr, ResourceManager.Inst);

            DPResourceManager.Inst = new DPResourceManager();
            SingletonManager.Inst.AddSingleton(SingleName.m_ResDpMgr, DPResourceManager.Inst);

            EntityManager.Inst = new EntityManager();
            SingletonManager.Inst.AddSingleton(SingleName.m_Entity, EntityManager.Inst);



            NetRunTime.Inst = new NetRunTime();
            SingletonManager.Inst.AddSingleton(SingleName.m_netRun, NetRunTime.Inst);
            NetManager.Inst = new NetManager();
            SingletonManager.Inst.AddSingleton(SingleName.m_netMgr, NetManager.Inst);

            GUIManager.Inst = new GUIManager();
            SingletonManager.Inst.AddSingleton("gui", GUIManager.Inst);
            LayoutMgr.Inst = new LayoutMgr();
            SingletonManager.Inst.AddSingleton("layout", LayoutMgr.Inst);

            SceneManager.Inst = new SceneManager();
            SingletonManager.Inst.AddSingleton(SingleName.m_Scene, SceneManager.Inst);


            CameraMgr.Inst = new CameraMgr();
            SingletonManager.Inst.AddSingleton("cam", CameraMgr.Inst);

            KeyMgr.Inst = new KeyMgr();
            SingletonManager.Inst.AddSingleton("KeyMgr", KeyMgr.Inst);

            TimeMgr.Inst = new TimeMgr();
            SingletonManager.Inst.AddSingleton("timeMgr", TimeMgr.Inst);

            SingletonManager.Inst.Init();
        }


        public void UpdateModule(float fTime, float fDTime)
        {
            Lua_Update(fTime, fDTime);
            SingletonManager.Inst.Update(fTime, fDTime);
            CEffectMgr.Update(fTime, fDTime);
            VObjectMgr.Update(fTime, fDTime);
        }

        public void LateUpdateModule(float fTime, float fDTime)
        {
            SingletonManager.Inst.LateUpdate(fTime, fDTime);
        }

        public void UnInitModule()
        {
            Lua_Destroy();

            SingletonManager.Inst.Destroy();
            SingletonManager.Inst.RemoveSingleton(SingleName.m_CSV);
            CsvManager.Inst = null;

            SingletonManager.Inst.RemoveSingleton(SingleName.m_ResMgr);
            ResourceManager.Inst = null;
            SingletonManager.Inst.RemoveSingleton(SingleName.m_ResDpMgr);
            DPResourceManager.Inst = null;
            SingletonManager.Inst.RemoveSingleton(SingleName.m_ResFac);
            ResourceFactory.Inst.GC();
            ResourceFactory.Inst = null;
            SingletonManager.Inst.RemoveSingleton(SingleName.m_Entity);
            EntityManager.Inst = null;


            SingletonManager.Inst.RemoveSingleton(SingleName.m_netRun);
            NetRunTime.Inst = null;
            SingletonManager.Inst.RemoveSingleton(SingleName.m_netMgr);
            NetManager.Inst = null;

            SingletonManager.Inst.RemoveSingleton(SingleName.m_Scene);
            SceneManager.Inst = null;

            SingletonManager.Inst.RemoveSingleton(SingleName.m_sound);

            SingletonManager.Inst = null;
            Inst = null;
        }

        public static LogicSystem Inst = null;

        private LoadProcess m_mapLoadProcess = new LoadProcess();

        public LoadProcess GetMapLoadProcess()
        {
            return m_mapLoadProcess;
        }
    }

    public class LoadProcess
    {
        public void ResetLoadProcess()
        {
            fPercent = 0.0f;
            strCurInfo = string.Empty;
            m_bDone = false;
        }

        public float fPercent = 0.0f;         //当前下载资源的进度
        public string strCurInfo;
        public bool m_bDone = false;
    }
}