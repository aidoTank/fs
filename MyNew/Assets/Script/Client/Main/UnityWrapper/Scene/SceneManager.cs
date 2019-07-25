using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Roma
{
    public class LoadProcess
    {
        public void ResetLoadProcess()
        {
            fPercent = 0.0f;
            strCurInfo = string.Empty;
            m_bDone = false;
        }

        public float fPercent;         //当前下载资源的进度
        public string strCurInfo;
        public bool m_bDone;
    }


    public class SceneManager : Singleton
    {
        private LoadProcess m_mapLoadProcess = new LoadProcess();
        public LoadProcess GetMapLoadProcess()
        {
            return m_mapLoadProcess;
        }

        public static SceneManager Inst;
        public SceneManager() : base(true)
        {
        }

        private SceneCsvData m_sceneInfo;
        // 场景自身需要实例化的资源
        public Entity m_ent;
        public Entity m_colliderEnt;
        private SceneDataResource m_staticDynamic;
        public SoundEntity m_soundEnt;

        public bool m_bModelLoaded;
        private bool m_bSceneDataLoaded;
        private bool m_bBgmLoaded;

        private const float MODEL_PCT = 0.99F;
        private const float SCENE_DATA_PCT = 0.00F;
        private const float BGM_PCT = 0.01F;

        private Action m_sceneLoaded;

        private int m_width;
        private int m_hight;

        public SceneCsvData GetSceneData()
        {
            return m_sceneInfo;
        }

        public void LoadScene(int sceneID, Action loadEnd)
        {
            m_sceneLoaded = loadEnd;
            SceneCsv csvScene = CsvManager.Inst.GetCsv<SceneCsv>((int)eAllCSV.eAC_Scene);
            m_sceneInfo = csvScene.GetData(sceneID);
            if (m_sceneInfo == null)
            {
                Debug.LogError("地图表缺少配置：" + sceneID);
                return;
            }
            OnLoadModel();
        }

        public void UnLoadScene()
        {
            // 销毁场景模型
            if (m_ent != null)
            {
                EntityManager.Inst.RemoveEntity(m_ent.m_hid);
            }
            if (m_colliderEnt != null)
            {
                EntityManager.Inst.RemoveEntity(m_colliderEnt.m_hid);
            }
            // 销毁地图动态障碍数据
            if (m_staticDynamic != null)
            {
                ResourceFactory.Inst.UnLoadResource(m_staticDynamic);
                m_staticDynamic = null;
            }
            // 销毁声音
            if (m_soundEnt != null)
            {
                EntityManager.Inst.RemoveEntity(m_soundEnt.m_hid);
                m_soundEnt = null;
            }

            m_sceneInfo = null;
            m_bModelLoaded = false;
            m_bSceneDataLoaded = false;

            SceneManager.Inst.GetMapLoadProcess().fPercent = 0f;

            // 清除实体缓存
            EntityManager.Inst.ClearCache();
        }

        public bool IsLoaded()
        {
            return m_bModelLoaded && m_bSceneDataLoaded && m_bBgmLoaded;
        }

        private void OnLoadModel()
        {
            EntityBaseInfo info = new EntityBaseInfo();
            info.m_resID = (int)m_sceneInfo.resId;
            info.m_ilayer = (int)LusuoLayer.eEL_Static;
            int handleId = EntityManager.Inst.CreateEntity(eEntityType.eSceneEntity, info, OnLoadCollider);
            m_ent = EntityManager.Inst.GetEnity(handleId);
        }

        public void OnLoadCollider(Entity ent)
        {
            EntityBaseInfo info = new EntityBaseInfo();
            info.m_resID = (int)m_sceneInfo.collider;
            info.m_ilayer = (int)LusuoLayer.eEL_Static;
            int handleId = EntityManager.Inst.CreateEntity(eEntityType.eSceneEntity, info, OnLoadStaticData);
            m_colliderEnt = EntityManager.Inst.GetEnity(handleId);
        }

        private void OnLoadStaticData(Entity ent)
        {
            m_bModelLoaded = true;
            if (m_sceneInfo.staticData == 0)
            {
                OnLoadBgm();
                return;
            }
            m_staticDynamic = (SceneDataResource)ResourceFactory.Inst.LoadResource(m_sceneInfo.staticData, (res) =>
            {
                OnLoadBgm();
            });
        }

        private void OnLoadBgm()
        {
            m_bSceneDataLoaded = true;
            if (m_sceneInfo.bgm == 0)
            {
                m_bBgmLoaded = true;
                OnLoadEnd();
                return;
            }
            int sHid = SoundManager.Inst.PlaySound(m_sceneInfo.bgm, (ent) =>
            {
                //SoundEntity sEnt = ent as SoundEntity;
                //sEnt.Stop(true);

                m_bBgmLoaded = true;
                OnLoadEnd();
            });
            m_soundEnt = EntityManager.Inst.GetEnity(sHid) as SoundEntity;
        }

        private void OnLoadEnd()
        {
            InitStaticData();
            m_bSceneDataLoaded = true;

            if (IsLoaded())
            {
                SceneManager.Inst.GetMapLoadProcess().fPercent = 1.0f;
                if (m_sceneLoaded != null)
                    m_sceneLoaded();
            }
        }

        public override void Update(float fTime, float fDTime)
        {
            if (m_sceneInfo == null)
                return;

            // 给上层提供场景加载进度
            if (!IsLoaded())
            {
                // 合计资源总量
                float cur = 0f;

                // 场景模型
                if (m_bModelLoaded)
                {
                    cur += MODEL_PCT;
                }
                else
                {
                    if (m_ent != null)
                        cur += m_ent.GetLoadProcess() * MODEL_PCT;
                }

                if (m_bSceneDataLoaded)
                {
                    cur += SCENE_DATA_PCT;
                }
                else
                {
                    if (m_staticDynamic != null)
                        cur += m_staticDynamic.GetDownLoadProcess() * SCENE_DATA_PCT;
                }

                // 背景音乐
                if (m_bBgmLoaded)
                {
                    cur += BGM_PCT;
                }
                else
                {
                    if (m_soundEnt != null)
                    {
                        //Debug.Log("sound========================" + cur);
                        cur += m_soundEnt.GetLoadProcess() * BGM_PCT;
                    }
                }
                SceneManager.Inst.GetMapLoadProcess().fPercent = cur;
            }
        }

        public byte[] m_staticData;
        private void InitStaticData()
        {
            if (m_staticDynamic == null)
                return;
            byte[] data = m_staticDynamic.GetData();
            LusuoStream ls = new LusuoStream(data);
            m_width = ls.ReadInt();
            m_hight = ls.ReadInt();
            m_staticData = new byte[m_width * m_hight];
            ls.Read(ref m_staticData);
        }

        public void SetBlock(int x, int y, bool block)
        {
            if ((x >= 0 && x < m_width) && (y >= 0 && y < m_hight))
            {
                if (block)
                {
                    m_staticData[x * m_width + y] = 1;
                }
                else
                {
                    m_staticData[x * m_width + y] = 0;
                }
            }
        }

        public bool Isblock(int x, int y)
        {
            if ((x >= 0 && x < m_width) && (y >= 0 && y < m_hight))
            {
                return m_staticData[x * m_width + y] != 0;
            }
            return false;
        }

        public bool IsblockNotAirWal(int x, int y)
        {
            if ((x >= 0 && x < m_width) && (y >= 0 && y < m_hight))
            {
                return m_staticData[x * m_width + y] == 1;
            }
            return false;
        }

    }
}
