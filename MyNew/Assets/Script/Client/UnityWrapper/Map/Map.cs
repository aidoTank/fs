using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Roma
{
    public enum eMapType
    {
        None = 0,
        MainScene = 1,
        Pve = 2,
        Battle = 3,
        Instance = 4,
    }

    public class CameraInfo
    {
        public Vector3 pos;
        public Vector3 rota;
        public float fov;
    }
    public class MapData
    {
        public uint id;
        public string name;
        public string size;
        public int sizeW;   // 宽
        public int sizeD;   // 深度
        public int sizeH;   // 高
        public int type;
        public int sceneCfgResID;   // 场景资源id
        public int terrainResID;    // 地形资源id = 光照贴图ID
        public float maxClimb;      // 最大爬坡高度(米)
        public float maxSlope;      // 最大坡度(米)
        public int skyBoxID;
        public int bgMusic;
        public int bgFightMusic;
        public string necessaryResIDs;
        public string iconName;

        public Vector3 vBirthPos;
        public Vector3 vBirthDir;
        /// <summary>
        /// 自由视角时的方向
        /// </summary>
        public Vector3 vBirthCamDir;
        /// <summary>
        /// 固定视角时的方向
        /// </summary>
        public Vector3 vFixBirthDir;

        public string cameraInfo;
        public List<CameraInfo> cameraInfoList = new List<CameraInfo>();

        public string mainLight;
        public Vector3 mainLightDir;
        public Color mainLightColor;
        public float mainLightIntensity;

        public string envLight;
        public Color envLightColor;
        public float envLightIntensity;

        public string fog;
        public Color fogColor;
        public int fogType;
        public float fogVal1;
        public float fogVal2;

        public string bloom;
        public float m_bloomIntensity;
        public Color m_bloomColorMix;
        public float m_bloomColorMixBlend;

        public string CCVintage;
        public int CCVintageFilter;
        public float CCVintageAmount;

        public string customParam;
        public string[] m_listCustomParam;

        public string fightParam;
        public int fightSceneId;
        public Vector3 fightPosOffset;
        public int sceneAnimaId;
        public bool isPK;
        public int serverDataId;

        public void MakeSize()
        {
            string[] strArr = size.Split('x');
            if (null == strArr)
            {
                Debug.LogError("map size invalid");
            }

            if (strArr.Length != 3)
            {
                Debug.LogError("map size invalid");
            }
            sizeW = int.Parse(strArr[0]);
            sizeD = int.Parse(strArr[1]);
            sizeH = int.Parse(strArr[2]);
        }
    }

    /// <summary>
    /// 包含所有模型，高度面，光照贴图的创建
    /// 他的上层可以再加一个地图管理层，包含一些地图信息和缓存
    /// </summary>
    public delegate void SceneLoaded();
    public class Map
    {
        private MapData m_mapData;
        private LoadProcess m_loadProcess;

        private SceneLoaded m_sceneLoaded;  // 场景加载回调
        public SceneCfgResource m_rescfg;
        private LightMapResource m_lightMapRes;
        private bool m_bSceneCfgLoaded = false;
        private bool m_bLightMapLoaded = false;
        private List<Entity> m_listStaticEnity = new List<Entity>();// 场景所有静态对象
        private List<QuadTreeEntity> m_listDynamicEntity = new List<QuadTreeEntity>();
        private int m_curStaticEntityNum = 0;
        private int m_maxStaticEntityNum = 0;

        /// <summary>
        /// NPC由上层逻辑计数
        /// </summary>
        public int m_curNpcCount;
        public int m_maxNpcCount;

        private uint m_bgSoundHandle;
        private bool m_bBgSoundLoaded = false;

        public CBHAStar m_aStar;

        public GameObject m_sceneRoot = null;
        private QuadTree m_tree;

        public Map()
        {
            m_sceneRoot = new GameObject("Scene");
            m_sceneRoot.transform.localPosition = Vector3.zero;
            m_sceneRoot.isStatic = true;
        }

        // 传入地图信息，高度面信息
        public bool LoadMap(MapData mapData, ref LoadProcess loadPro, SceneLoaded loaded)
        {
            if (m_mapData != null && m_mapData.id == mapData.id)
            {
                //Debug.Log("同一个场景，无需重复加载");
                return false;
            }
            UnLoadMap();
            m_mapData = mapData;
            m_loadProcess = loadPro;
            m_sceneLoaded = null;
            m_sceneLoaded = loaded;
            _CreateSceneCfg();
            return true;
        }

        public void OnRegisterSceneEnd(SceneLoaded loaded)
        {
            m_sceneLoaded += loaded;
        }

        public bool IsLoaded()
        {
            return m_bSceneCfgLoaded && m_bBgSoundLoaded && m_bLightMapLoaded && m_maxStaticEntityNum == m_curStaticEntityNum && m_curNpcCount == m_maxNpcCount;
        }

        private void _CreateSceneCfg()
        {
            m_rescfg = (SceneCfgResource)ResourceFactory.Inst.LoadResource(m_mapData.sceneCfgResID, OnSceneCfgLoaded);
            m_loadProcess.strCurInfo = "加载地图配置";
        }

        private void OnSceneCfgLoaded(Resource res)
        {
            m_rescfg = (SceneCfgResource)res;
            m_bSceneCfgLoaded = true;

            CreateAStar();
            //CreateLightMap();
            CreateBgSound();
        }

        private void CreateAStar()
        {
            m_tree = new QuadTree(new Rect(0, 0, m_mapData.sizeW, m_mapData.sizeD));
            m_tree.EnableDebugLines = Debug.logger.logEnabled;

            m_aStar = new CBHAStar(m_mapData.sizeW, m_mapData.sizeH);
            m_aStar.InitLists(0, 0);
        }

        public bool GetPath(ref Vector2 startPos, ref Vector2 targetPos, ref List<Vector2> pathList)
        {
            byte[] bytes = GetMapInfo();
            AStar.GetPath(bytes, m_mapData.sizeW, m_mapData.sizeD, startPos, targetPos, ref pathList);
            return true;
        }

        // C#的A*不用了
        //public bool GetPath(ref Vector2 startPos, ref Vector2 targetPos, ref List<Vector2> path)
        //{
        //    if (m_aStar == null)
        //    {
        //        return false;
        //    }
        //    startPos = startPos / TerrainBlockData.nodesize;    // 真实坐标映射到寻路坐标
        //    targetPos = targetPos / TerrainBlockData.nodesize;
        //    bool result = m_aStar.FindPath(this, ref startPos, ref targetPos, ref path, 1024 * 1024);
        //    for (int i = 0; i < path.Count; i++)
        //    {
        //        path[i] = path[i] * TerrainBlockData.nodesize;
        //    }
        //    return result;
        //}

        private void CreateBgSound()
        {
            if(m_mapData.bgMusic == 0)
            {
                m_bBgSoundLoaded = true;
                CreateLightMap();
                return;
            }
            m_bgSoundHandle = SoundManager.Inst.PlaySound(m_mapData.bgMusic, OnBgSoundLoaded);
            m_loadProcess.strCurInfo = "加载背景音效";
        }

        private void OnBgSoundLoaded(Entity ent, object obj)
        {
            //Debug.LogError("声音加载完成、、、、、、、、、、、、、、、、、、、、、、、、");
            m_bBgSoundLoaded = true;
            CreateLightMap();
        }

        private void CreateLightMap()
        {
            if (m_mapData.terrainResID == 0)
            {
                m_bLightMapLoaded = true;
                CreateStaticEntity();
                return;
            }
            Resource res = ResourceFactory.Inst.LoadResource(m_mapData.terrainResID, OnLightMapLoaded);
            if (res.GetResInfo().iType != ResType.LightMapResource)
            {
                Debug.Log("该场景无光照图：" + m_mapData.id + "  lightMap:"+ m_mapData.terrainResID);
                m_bLightMapLoaded = true;
                CreateStaticEntity();
                return;
            }
            m_lightMapRes = (LightMapResource)res;
            m_loadProcess.strCurInfo = "加载光照贴图";
        }

        private void OnLightMapLoaded(Resource res)
        {
            m_lightMapRes = (LightMapResource)res;
            LightmapSettings.lightmapsMode = LightmapsMode.NonDirectional;

            Texture2D[] tex2D = m_lightMapRes.GetTex();
            if(LightmapSettings.lightmapsMode == LightmapsMode.CombinedDirectional)
            {
                LightmapData[] lightMap = new LightmapData[tex2D.Length >> 2];
                //Debug.Log("光照贴图个数：" + tex2D.Length);
                for (int i = 0; i < tex2D.Length; i += 2)
                {
                    int index = i >> 2;
                    if (index >= lightMap.Length)
                    {
                        break;
                    }
                    //Debug.Log("当前光照贴图个数：" + i);
                    lightMap[index] = new LightmapData();

                    lightMap[index].lightmapLight = tex2D[i + 1];
                    lightMap[index].lightmapDir = tex2D[i];
                }
                LightmapSettings.lightmaps = lightMap;
            }
            else if(LightmapSettings.lightmapsMode == LightmapsMode.NonDirectional)
            {
                // 5.x以下的写法
                LightmapData[] lightMap = new LightmapData[tex2D.Length];
                for (int i = 0; i < lightMap.Length; i++)
                {
                    //Debug.Log("光照贴图个数：" + i);
                    lightMap[i] = new LightmapData();
                    lightMap[i].lightmapLight = tex2D[i];
                }
                LightmapSettings.lightmaps = lightMap;
            }
            m_bLightMapLoaded = true;
            CreateStaticEntity();

        }

        private void CreateStaticEntity()
        {
            SceneCfg cfg = m_rescfg.GetCfg();

            List<EntityBaseInfo> list = null;
            // 只加载必须要加载的资源，其他的动态创建
            if (!string.IsNullOrEmpty(m_mapData.necessaryResIDs) && !m_mapData.necessaryResIDs.Equals("0"))
            {
                string[] needRes = m_mapData.necessaryResIDs.Split('|');
                List<EntityBaseInfo> notNeedList = null;
                cfg.GetEntityInfoListCfg().GetNeedEntityInfoList(needRes, ref list, ref notNeedList);
                for(int i = 0; i < notNeedList.Count; i ++)
                {
                    QuadTreeEntity item = new QuadTreeEntity();
                    item.m_baseInfo = notNeedList[i];
                    item.SwapOut();
                    if (m_tree != null)
                        m_tree.Receive(item);
                    m_listDynamicEntity.Add(item);
                }
            }
            else
            {
                list = cfg.GetEntityInfoListCfg().GetEntityInfoList();
            }
            m_maxStaticEntityNum = list.Count;
            for (int i = 0; i < m_maxStaticEntityNum; i++)
            {
                uint handle = EntityManager.Inst.CreateEntity(eEntityType.eStaticEntity, OnLoadedStaticEntity, list[i]);
                m_listStaticEnity.Add(EntityManager.Inst.GetEnity(handle, true));
            }
            m_loadProcess.strCurInfo = "加载场景资源";

        }

        private void OnLoadedStaticEntity(Entity entity, object userObj)
        {
            if (!m_listStaticEnity.Contains(entity))
            {
                m_listStaticEnity.Add(entity);
            }
            entity.SetOcc(true);
            m_curStaticEntityNum++;
            entity.GetObject().transform.SetParent(SceneManager.Inst.GetSceneRoot().transform);

            if (IsLoaded())
            {
                if (null != m_sceneLoaded)           // 对于底层来看，场景模型加载完成就算加载完成，而NPC初始化都是0
                {
                    m_sceneLoaded();
                    //SetSceneHeight(-0.2f);
                    m_sceneLoaded = null;

                    // 地图加载完成，设置场景属性
                    // 设置主灯光
                    //CCameraMgr.m_mainLight.transform.localEulerAngles = m_mapData.mainLightDir;
                    //CCameraMgr.m_mainLight.color = m_mapData.mainLightColor / 255.0f;
                    //CCameraMgr.m_mainLight.intensity = m_mapData.mainLightIntensity;
                    // 设置环境光
                    RenderSettings.ambientSkyColor = m_mapData.envLightColor / 255.0f;
                    RenderSettings.ambientIntensity = m_mapData.envLightIntensity;
                    // 雾
                    RenderSettings.fog = true;
                    RenderSettings.fogColor = m_mapData.fogColor / 255.0f;
                    RenderSettings.fogMode = (FogMode)m_mapData.fogType;
                    if (RenderSettings.fogMode == FogMode.Linear)
                    {
                        RenderSettings.fogStartDistance = m_mapData.fogVal1;
                        RenderSettings.fogEndDistance = m_mapData.fogVal2;
                    }
                    else
                    {
                        RenderSettings.fogDensity = m_mapData.fogVal1;
                    }
                }
                //m_sceneRoot.SetActive(false);
                //StaticBatchingUtility.Combine(SceneManager.Inst.GetSceneRoot());
            }
        }

        public void Update(float fTime, float fDTime)
        {
            if(m_mapData == null)
            {
                return;
            }
            if (!IsLoaded())
            {
                // 合计资源总量
                float cur = 0f;
                // 计算当前量
                if (null != m_rescfg)
                {
                    cur += m_bSceneCfgLoaded ? 0.1f : 0.1f * m_rescfg.GetDownLoadProcess();
                }
                if (null != m_lightMapRes)
                {
                    cur += m_bLightMapLoaded ? 0.3f : 0.3f * m_lightMapRes.GetDownLoadProcess();
                }
                if (m_maxStaticEntityNum != m_curStaticEntityNum)
                {
                    // 获取每个资源的下载进度累加值
                    for (int i = 0; i < m_maxStaticEntityNum; i++)
                    {
                        Entity ent = m_listStaticEnity[i];
                        cur += ent.GetLoadProcess() * 0.5f / m_maxStaticEntityNum;
                    }
                }
                else if (m_maxStaticEntityNum != 0 && m_curStaticEntityNum == m_maxStaticEntityNum)
                {
                    cur += 0.5f;
                }
                if (m_maxNpcCount != 0)
                {
                    m_loadProcess.strCurInfo = "加载场景角色";
                    cur += 0.1f * ((float)m_curNpcCount / (float)m_maxNpcCount);
                }
                m_loadProcess.fPercent = cur;
                return;
            }
            else
            {
                if(m_tree == null)
                    return;
                Vector3 pos = CPlayerMgr.GetMaster().GetPos();
                m_tree.Update(new Vector2(pos.x, pos.z));

                OnDrawGrid();
            }
        }

        public void SetBgSounStop(bool bMute)
        {
            SoundManager.Inst.SetStop(m_bgSoundHandle, bMute);
        }

        /// <summary>
        /// 根据地形获取高度,当配置下载完成时，就可以获取了
        /// </summary>
        public float GetTerrainHeight(float x, float z)
        {
            if (m_bSceneCfgLoaded)
            {
                return m_rescfg.GetTerrainHeight(x, z, true);
            }
            return 0.0f;
        }

        public void SetColorForever(Color color)
        {
            foreach(Entity item in m_listStaticEnity)
            {
                item.SetColorForever(color);
            }
        }

        public void RestoreColor()
        {
            foreach (Entity item in m_listStaticEnity)
            {
                item.RestoreColor();
            }
        }

        public void SetActive(bool bActive)
        {
            m_sceneRoot.gameObject.SetActive(bActive);
        }

        public uint GetMapID()
        {
            if (m_mapData == null)
            {
                return 0;
            }
            return m_mapData.id;
        }

        public void UnLoadMap()
        {
            if(null != m_rescfg)
            {
                ResourceFactory.Inst.UnLoadResource(m_rescfg, true);
                m_rescfg = null;
            }

            SoundManager.Inst.Remove(m_bgSoundHandle);

            if (null != m_lightMapRes)
            {
                ResourceFactory.Inst.UnLoadResource(m_lightMapRes, true);
                m_lightMapRes = null;
            }

            for (int i = 0; i < m_listStaticEnity.Count; i++)
            {
                EntityManager.Inst.RemoveEntity(m_listStaticEnity[i].m_handleID, true);
            }
            m_listStaticEnity.Clear();

            for(int i = 0; i < m_listDynamicEntity.Count; i ++)
            {
                m_listDynamicEntity[i].Destory();
            }
            m_listDynamicEntity.Clear();
            

            m_bSceneCfgLoaded = false;
            m_bBgSoundLoaded = false;
            m_bLightMapLoaded = false;

            m_curStaticEntityNum = 0;
            m_maxStaticEntityNum = 0;

            m_curNpcCount = 0;
            m_maxNpcCount = 0;

            m_mapData = null;
            m_loadProcess = null;
            m_sceneLoaded = null;
            //m_aStar = null;
            ResourceFactory.Inst.GC();  // 回收光照图的内存

            // 解决特殊情况下，其他地图没有清除的BUG
            if(m_sceneRoot != null && m_sceneRoot.transform != null)
            {
                for (int i = 0; i < m_sceneRoot.transform.childCount; i++)
                {
                    Transform item = m_sceneRoot.transform.GetChild(i);
                    if (item != null)
                        GameObject.Destroy(item.gameObject);
                }
            }
        }

        /// <summary>
        /// 能走
        /// 路障的原始接口
        /// 因为单位为0.5米每个单位所以传入的值必须是原始值的二倍
        /// </summary>
        public bool CanArriveDoubleInt(ref Vector2 vPos)
        {
            if (null == m_rescfg || null == m_rescfg.GetCfg())
            {
                return false;
            }
            eGridType type = m_rescfg.GetCfg().GetBlockData().GetTerrainBlock((int)vPos.x, (int)vPos.y);
            return type == eGridType.eGT_walkable;
        }

        /// <summary>
        /// 路障的原始接口
        /// 因为单位为0.5米每个单位所以传入的值必须是原始值的二倍
        /// </summary>
        public bool CanArriveDoubleInt(int x, int z)
        {
            if (null == m_rescfg || null == m_rescfg.GetCfg())
            {
                return false;
            }
            eGridType type = m_rescfg.GetCfg().GetBlockData().GetTerrainBlock(x, z);
            return type == eGridType.eGT_walkable;
        }

        /// <summary>
        /// 路障的原始接口
        /// 因为单位为0.5米每个单位所以传入的值必须是原始值的二倍
        /// </summary>
        public void SetTerrainBlock(int x, int z, bool bBlock)
        {
            if (null == m_rescfg)
            {
                return;
            }
            m_rescfg.GetCfg().GetBlockData().SetTerrainBlock(x, z, bBlock ? (byte)eGridType.eGT_block : (byte)eGridType.eGT_walkable);
        }

        public byte[] GetMapInfo()
        {
            return m_rescfg.GetCfg().GetBlockData().GetMapInfo();
        }

        /// <summary>
        /// 显示当前真实场景的路障信息
        /// </summary>
        private void OnDrawGrid()
        {
            return;
            TerrainBlockData blockData = m_rescfg.GetCfg().GetBlockData();

            int w = (int)blockData.m_nDW;
            int z = (int)blockData.m_nDH;
            for (int i = 0; i < w; i++)
            {
                for (int j = 0; j < z; j++)
                {
                    eGridType curState = (eGridType)blockData.m_bBlock[j * w + i];
                    // 矩形的起点在左上，就得向左和上移动一半的格子
                    float x = (float)i * TerrainBlockData.nodesize;
                    float y = (float)j * TerrainBlockData.nodesize;
                    Rect rect = new Rect(x, y, TerrainBlockData.nodesize, TerrainBlockData.nodesize);
                    if (curState == eGridType.eGT_walkable)
                    {
                        QtAlgo.DrawRect(rect, GetTerrainHeight(x, y) + 0.1f, Color.blue);
                    }
                    else if (curState == eGridType.eGT_block)
                    {
                        QtAlgo.DrawRect(rect, GetTerrainHeight(x, y) + 0.1f, Color.red);
                    }
                }
            }

            //int ww = (int)m_mapData.sizeW;
            //int zz = (int)m_mapData.sizeD;
            //for (int i = 0; i < ww; i++)
            //{
            //    for (int j = 0; j < zz; j++)
            //    {
            //        float h = m_rescfg.GetCfg().GetTerrainHeightData().SmoothInterpolTerrainHeight(i, j);
            //        if (h > 0)
            //        {
            //            Rect rect = new Rect(i, j, 1, 1);

            //            QtAlgo.DrawRect(rect, GetTerrainHeight(i, j) + 0.1f, Color.yellow);
            //        }
            //    }
            //}
        }
    }
}
