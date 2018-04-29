using System.Collections.Generic;
using UnityEngine;
namespace Roma
{
	public class SceneManager : Singleton
	{
        public new static SceneManager Inst;

        public Dictionary<uint, MapData> m_mapDataDic = new Dictionary<uint, MapData>();

        public Map m_curMap;

        public SceneManager()
            : base(true)
        {
            m_curMap = new Map();
        }

        public bool LoadMap(uint mapId, ref LoadProcess process, SceneLoaded load)
        {
            MapData mapData;
            if (m_mapDataDic.TryGetValue(mapId, out mapData))
            {
                return m_curMap.LoadMap(mapData, ref process, load);
            }
            else
            {
                Debug.LogError("无法获取map数据,ID：" + mapId);
                return false;
            }
        }

        // 将地图信息给底层
        public void SetSceneData(MapData data)
        {
            m_mapDataDic.Add(data.id, data);
        }

        public void SetActive(bool bActive)
        {
            m_curMap.SetActive(bActive);
        }

        public void SetBgStop(bool bMute)
        {
            m_curMap.SetBgSounStop(bMute);
        }

        public MapData GetMapData()
        {
            if (m_mapDataDic.ContainsKey(m_curMap.GetMapID()))
            {
                return m_mapDataDic[m_curMap.GetMapID()];
            }
            else
            {
                Debug.Log("找不到地图数据：" + m_curMap.GetMapID());
                return null;
            }
        }

        public MapData GetMapDataByMapID(int mapID)
        {
            if (m_mapDataDic.ContainsKey((uint)mapID))
            {
                return m_mapDataDic[(uint)mapID];
            }
            else
            {
                Debug.Log("找不到地图数据：" + m_curMap.GetMapID());
                return null;
            }
        }

        public Map GetMap()
        {
            return m_curMap;
        }

        public override void Update(float fTime, float fDTime)
        {
            m_curMap.Update(fTime, fDTime);
        }

        public float GetTerrainHeight(float x, float z)
        {
            return m_curMap.GetTerrainHeight(x, z);
        }

        public bool CanArriveDoubleInt(int x, int z)
        {
            return m_curMap.CanArriveDoubleInt(x, z);
        }

	    public bool IsLoaded()
	    {
	        return m_curMap.IsLoaded();
	    }
        //当前场景是否可PK
        public bool IsCanPK()
        {
            if (GetMapData() != null)
                return GetMapData().isPK;
            else return false;
        }

        public GameObject GetSceneRoot()
        {
            return m_curMap.m_sceneRoot;
        }

        public override void Destroy()
        {
            m_curMap.UnLoadMap();
        }

        internal static Vector2 BlockToWorldSpace(int x, int y)
        {
            return new Vector2(x * TerrainBlockData.nodesize + TerrainBlockData.halfnodesize,
                y * TerrainBlockData.nodesize + TerrainBlockData.halfnodesize);
        }

        internal static void WorldSpaceToBlock(float fx, float fy, out int x, out int y)
        {
            x = (int)(fx / TerrainBlockData.nodesize);
            y = (int)(fy / TerrainBlockData.nodesize);
        }

        public Vector2 GetRandomPos(float fx, float fz, float range)
        {
            if (range < TerrainBlockData.nodesize)
                return Vector3.zero;

            int resultNum = 0;
            int x, y;
            WorldSpaceToBlock(fx, fz, out x, out y);
            int irange = (int)(range);
            int srange = irange * irange;
            int sign = 1;
            int filter = 0;

            while (resultNum != 1)
            {
                int idivx = resultNum + UnityEngine.Random.Range(-irange, irange) + sign * filter;
                int idivy = resultNum + UnityEngine.Random.Range(-irange, irange) - sign * filter;
                if (idivx == 0 || idivy == 0)
                {
                    filter++;
                    continue;
                }

                int ix = idivx % irange;
                int iy = idivy % irange;

                Vector2 bv = new Vector2(x + ix, y + iy);
                if (m_curMap.CanArriveDoubleInt(ref bv))
                {
                    filter = 0;
                    resultNum = 1;
                    return BlockToWorldSpace((int)bv.x, (int)bv.y);
                }
                else if (filter > srange)
                {
                    resultNum = 1;
                    return new Vector2(fx, fz);
                }
                else
                {
                    filter++;
                    sign = -sign;
                }
            }
            return Vector3.zero;
        }

        public List<Vector2> RandomPos(float fx, float fz, float range, int count = 1)
        {
            if (count <= 0)
                return null;

            if (range < TerrainBlockData.nodesize)
                return null;

            List<Vector2> result = new List<Vector2>();
            //if (IsBlockInWorldSpace(fx, fz))
            //{
            //    Debug.LogWarning("RandomPos 位置无效或场景未加载好");
            //    return null;
            //}
            result.Clear();
            int x, y;
            WorldSpaceToBlock(fx, fz, out x, out y);
            int irange = (int)(range);
            int srange = irange * irange;
            int sign = 1;
            int filter = 0;

            while (result.Count != count)
            {
                int idivx = result.Count + UnityEngine.Random.Range(-irange, irange) + sign * filter;
                int idivy = result.Count + UnityEngine.Random.Range(-irange, irange) - sign * filter;
                if (idivx == 0 || idivy == 0)
                {
                    filter++;
                    continue;
                }

                int ix = idivx % irange;
                int iy = idivy % irange;

                Vector2 bv = new Vector2(x + ix, y + iy);
                //Vector2 pos = BlockToWorldSpace(ix, iy) + new Vector2(fx, fz);
                if (m_curMap.CanArriveDoubleInt(ref bv))
                {
                    filter = 0;
                    result.Add(BlockToWorldSpace((int)bv.x, (int)bv.y));
                }
                else if (filter > srange)
                {
                    result.Add(new Vector2(fx, fz));
                }
                else
                {
                    filter++;
                    sign = -sign;
                }
            }
            return result;
        }
	} 
}
