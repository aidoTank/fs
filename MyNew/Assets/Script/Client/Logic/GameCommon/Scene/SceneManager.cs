using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Roma
{
    public class SceneManager : Singleton
    {
		public static SceneManager Inst;
        public SceneManager() : base(true)
        {
        }

        private SceneCsvData m_sceneInfo;
        // 场景自身需要实例化的资源
        public Entity m_ent;
        private SceneDataResource m_staticDynamic;

        public bool m_bModelLoaded;
        private bool m_bSceneDataLoaded;

        private Action m_sceneLoaded;


        public SceneCsvData GetSceneData()
        {
            return m_sceneInfo;
        }

        public void LoadScene(int sceneID, Action loadEnd)
        {
            m_sceneLoaded = loadEnd;
            UnLoadScene();
            SceneCsv csvScene = CsvManager.Inst.GetCsv<SceneCsv> ((int)eAllCSV.eAC_Scene);
            m_sceneInfo = csvScene.GetData(sceneID);

            OnLoadModel();
        }

        public void UnLoadScene()
        {
            // 销毁场景模型
            if (m_ent != null)
            {
                EntityManager.Inst.RemoveEntity(m_ent.m_hid);
            }

            ResourceFactory.Inst.UnLoadResource(m_staticDynamic);
            m_staticDynamic = null;

            m_sceneInfo = null;
            m_bModelLoaded = false;
            m_bSceneDataLoaded = false;

            LogicSystem.Inst.GetMapLoadProcess().fPercent = 0f;

            // 清除实体缓存
            EntityManager.Inst.ClearCache();
        }

        public bool IsLoaded()
        {
            return m_bModelLoaded && m_bSceneDataLoaded;
        }

        private void OnLoadModel()
        {
            EntityBaseInfo info = new EntityBaseInfo();
            info.m_resID = (int)m_sceneInfo.resId;
            info.m_ilayer = (int)LusuoLayer.eEL_Static;
            int handleId = EntityManager.Inst.CreateEntity(eEntityType.eSceneEntity, info, OnLoadStaticData);
            m_ent = EntityManager.Inst.GetEnity(handleId);
        }

        private void OnLoadStaticData(Entity ent)
        {
            m_bModelLoaded = true;
            if(m_sceneInfo.staticData == 0)
            {
                OnLoadEnd();
                return;
            }
            m_staticDynamic = (SceneDataResource)ResourceFactory.Inst.LoadResource(m_sceneInfo.staticData, (res) =>
            {
                Debug.Log("静态障碍加载完成");
                OnLoadEnd();
            });
        }

        private void OnLoadEnd()
        {
            InitStaticData();
            m_bSceneDataLoaded = true;

            if (IsLoaded())
            {
                LogicSystem.Inst.GetMapLoadProcess().fPercent = 1.0f;
                if (m_sceneLoaded != null)
                    m_sceneLoaded();
            }
        }

		public override void Update(float fTime, float fDTime)
		{
			if(m_sceneInfo == null)
				return;

            // 给上层提供场景加载进度
            if(!IsLoaded())
            {
                // 合计资源总量
                float cur = 0f;

                // 场景模型
                if (m_bModelLoaded)
                {
                    cur += 1;
                }
                else
                {
                    if(m_ent != null)
                        cur += m_ent.GetLoadProcess();
                }

                Debug.Log("end =======================" + cur);
                LogicSystem.Inst.GetMapLoadProcess().fPercent = cur;
            }
		}

        private byte[] m_staticData;        
        private void InitStaticData()
        {
            if(m_staticDynamic == null)
                return;
            byte[] data = m_staticDynamic.GetData();
            LusuoStream ls = new LusuoStream(data);
            int w = ls.ReadInt();
            int h = ls.ReadInt();
            m_staticData = new byte[w * h];
            ls.Read(ref m_staticData);
        }

        public bool Isblock(int x, int y)
        {
            if ((x >= 0 && x < 64) && (y >= 0 && y < 64))
            {
                return m_staticData[x * 64 + y] > 0;
            }
            return false;
        }

    }
}
