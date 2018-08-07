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
        public bool m_bModelLoaded;

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
            m_sceneInfo = null;
            LogicSystem.Inst.GetMapLoadProcess().fPercent = 0f;

            // 清除实体缓存
            EntityManager.Inst.ClearCache();
        }

        public bool IsLoaded()
        {
            return m_bModelLoaded;
        }

        private void OnLoadModel()
        {
            EntityBaseInfo info = new EntityBaseInfo();
            info.m_resID = (int)m_sceneInfo.resId;
            info.m_ilayer = (int)LusuoLayer.eEL_Dynamic;
            int handleId = EntityManager.Inst.CreateEntity(eEntityType.eSceneEntity, info, OnLoadEnd);
            m_ent = EntityManager.Inst.GetEnity(handleId);
        }

      
        private void OnLoadEnd(Entity ent)
        {
            m_bModelLoaded = true;
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


    }
}
