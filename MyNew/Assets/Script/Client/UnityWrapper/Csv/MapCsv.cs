using UnityEngine;
using System.Collections.Generic;
namespace Roma
{
    enum eMapCsv_Enum
    {
        eID = 0,
        eName = 1,
        eSize = 2,
        eType,
        eSceneCfgResID,
        eTerrainResID,
        eMaxClimb,
        eMaxSlope,
        eSkyBoxID,
        eBgMusic,
        eBgFightMusic,
        eNecessaryResIDs,
        eIconName,
        eBirthPos,
        eBirthDir,
        eBirthCamDir,
        eFixedBirthDir,

        eCamInfo,

        eMainLight,
        eEnvLight,
        eFog,
        eBloom,
        eCCVintage,

        eCustomParam,
        eFightParam,
        eSceneAnimaId,
        eIsPK,
        eServerDataId,
    }

    public class MapCsv : CsvExWrapper
	{
        public override void Clear()
        {
            base.Clear();
            m_mapDataDic.Clear();
        }

        protected override void _Load()
        {
            for (int i = 0; i < m_csv.GetRows(); i++)
            {
                MapData ani = new MapData();
                ani.id = (uint)m_csv.GetIntData(i, (int)eMapCsv_Enum.eID);
                ani.name = m_csv.GetData(i, (int)eMapCsv_Enum.eName);
                ani.size = m_csv.GetData(i, (int)eMapCsv_Enum.eSize);
                ani.MakeSize();
                ani.type = m_csv.GetIntData(i, (int)eMapCsv_Enum.eType);
                ani.sceneCfgResID = m_csv.GetIntData(i, (int)eMapCsv_Enum.eSceneCfgResID);

                ani.terrainResID = m_csv.GetIntData(i, (int)eMapCsv_Enum.eTerrainResID);
                ani.maxClimb = m_csv.GetFloatData(i, (int)eMapCsv_Enum.eMaxClimb);
                ani.maxSlope = m_csv.GetFloatData(i, (int)eMapCsv_Enum.eMaxSlope);

                ani.skyBoxID = m_csv.GetIntData(i, (int)eMapCsv_Enum.eSkyBoxID);
                ani.bgMusic = m_csv.GetIntData(i, (int)eMapCsv_Enum.eBgMusic);
                ani.bgFightMusic = m_csv.GetIntData(i, (int)eMapCsv_Enum.eBgFightMusic);
                ani.necessaryResIDs = m_csv.GetData(i, (int)eMapCsv_Enum.eNecessaryResIDs);
                ani.iconName = m_csv.GetData(i, (int)eMapCsv_Enum.eIconName);
                string birthPos = m_csv.GetData(i, (int)eMapCsv_Enum.eBirthPos);
                string birthDir = m_csv.GetData(i, (int)eMapCsv_Enum.eBirthDir);
                string birthCamDir = m_csv.GetData(i, (int)eMapCsv_Enum.eBirthCamDir);
                string fixbirthDir = m_csv.GetData(i, (int)eMapCsv_Enum.eFixedBirthDir);
                
                if (!string.IsNullOrEmpty(birthPos))
                {
                    ani.vBirthPos = GetVector3(birthPos);
                }
                if (!string.IsNullOrEmpty(birthDir))
                {
                    ani.vBirthDir = GetVector3(birthDir);
                }
                if (!string.IsNullOrEmpty(birthCamDir))
                {
                    ani.vBirthCamDir = GetVector3(birthCamDir);
                }
                if (!string.IsNullOrEmpty(fixbirthDir))
                {
                    ani.vFixBirthDir = GetVector3(fixbirthDir);
                }
                

                ani.cameraInfo = m_csv.GetData(i, (int)eMapCsv_Enum.eCamInfo);
                if (!string.IsNullOrEmpty(ani.cameraInfo))
                {
                    string[] camInfoList = ani.cameraInfo.Split('#');   // 对应地图表的摄像机配置
                    for (int cam = 0; cam < camInfoList.Length; cam++)
                    {
                        string[] camItem = camInfoList[cam].Split('|');
                        Vector3 pos = GetVector3(camItem[0]);
                        Vector3 rota = GetVector3(camItem[1]);
                        float fov = float.Parse(camItem[2]);
                        CameraInfo cInfo = new CameraInfo();
                        cInfo.pos = pos;
                        cInfo.rota = rota;
                        cInfo.fov = fov;
                        ani.cameraInfoList.Add(cInfo);
                    }
                }
                // 主灯光
                string mainLight = m_csv.GetData(i, (int)eMapCsv_Enum.eMainLight);
                if(!string.IsNullOrEmpty(mainLight))
                {
                    string[] mainLightList = mainLight.Split('|');
                    ani.mainLightDir = GetVector3(mainLightList[0]);
                    ani.mainLightColor = GetColor(mainLightList[1]);
                    ani.mainLightIntensity = float.Parse(mainLightList[2]);
                }
                // 环境光
                string envLight = m_csv.GetData(i, (int)eMapCsv_Enum.eEnvLight);
                if (!string.IsNullOrEmpty(envLight))
                {
                    string[] envLightList = envLight.Split('|');
                    ani.envLightColor = GetColor(envLightList[0]);
                    ani.envLightIntensity = float.Parse(envLightList[1]);
                }
                // 雾
                string fog = m_csv.GetData(i, (int)eMapCsv_Enum.eFog);
                if (!string.IsNullOrEmpty(fog))
                {
                    string[] fogList = fog.Split('|');
                    ani.fogColor = GetColor(fogList[0]);
                    ani.fogType = int.Parse(fogList[1]);
                    ani.fogVal1 = float.Parse(fogList[2]);
                    ani.fogVal2 = float.Parse(fogList[3]);
                }
                // bloom
                string bloom = m_csv.GetData(i, (int)eMapCsv_Enum.eBloom);
                if (!string.IsNullOrEmpty(bloom))
                {
                    string[] bloomList = bloom.Split('|');
                    ani.m_bloomIntensity = float.Parse(bloomList[0]);
                    ani.m_bloomColorMix = GetColor(bloomList[1]) / 255.0f;
                    ani.m_bloomColorMixBlend = float.Parse(bloomList[2]);
                }

                // cc_vintage
                string ccVin = m_csv.GetData(i, (int)eMapCsv_Enum.eCCVintage);
                if (!string.IsNullOrEmpty(ccVin))
                {
                    string[] ccVinList = ccVin.Split('|');
                    ani.CCVintageFilter = int.Parse(ccVinList[0]);
                    ani.CCVintageAmount = float.Parse(ccVinList[1]);
                }

                ani.customParam = m_csv.GetData(i, (int)eMapCsv_Enum.eCustomParam);
                if (!string.IsNullOrEmpty(ani.customParam))
                {
                    ani.m_listCustomParam = ani.customParam.Split('|');
                }

                ani.fightParam = m_csv.GetData(i, (int)eMapCsv_Enum.eFightParam);
                if (!string.IsNullOrEmpty(ani.fightParam))
                {
                    string[] fParma = ani.fightParam.Split('|');
                    if(fParma.Length != 2)
                    {
                        Debug.LogError("地图表 fightParam错误："+ ani.fightParam);
                        continue;
                    }
                    int.TryParse(fParma[0], out ani.fightSceneId);
                    ani.fightPosOffset = GetVector3(fParma[1]);
                }
                ani.sceneAnimaId = m_csv.GetIntData(i, (int)eMapCsv_Enum.eSceneAnimaId);
                ani.isPK = m_csv.GetIntData(i, (int)eMapCsv_Enum.eIsPK) == 1;
                ani.serverDataId = m_csv.GetIntData(i, (int)eMapCsv_Enum.eServerDataId);

                m_mapDataDic.Add(ani.id, ani);
            }
        }

        private Vector3 GetVector3(string pos)
        {
            string[] posList = pos.Split('_');
            if (posList.Length != 3)
            {
                //Debug.LogError("位置解析错误。");
                return Vector3.zero;
            }
            return new Vector3(float.Parse(posList[0]), float.Parse(posList[1]), float.Parse(posList[2]));
        }

        private Color GetColor(string pos)
        {
            string[] colorList = pos.Split('_');
            if (colorList.Length == 3)
            {
                return new Color(float.Parse(colorList[0]), float.Parse(colorList[1]), float.Parse(colorList[2]));
            }
            else if (colorList.Length == 4)
            {
                return new Color(float.Parse(colorList[0]), float.Parse(colorList[1]), float.Parse(colorList[2]), float.Parse(colorList[3]));
            }
            return Color.white;
        }

        static public CsvExWrapper CreateCSV()
        {
            return new MapCsv();
        }

        public MapData GetMapData(uint id)
		{
            MapData ani;
            if (m_mapDataDic.TryGetValue(id, out ani))
			{
                return ani;
			}
			return null;
		}

        public string[] GetMapName()
        {
            string[] nameList = new string[m_mapDataDic.Count];
            int i = 0;
            foreach(KeyValuePair<uint, MapData> item in m_mapDataDic)
            {
                nameList[i] = item.Value.name;
                i++;
            }
            return nameList;
        }

        public int GetId(string mapName)
        {
            foreach (KeyValuePair<uint, MapData> item in m_mapDataDic)
            {
                if(item.Value.name == mapName)
                {
                    return (int)item.Value.id;
                }
            }
            return 0;
        }

        public int GetIndex(int mapId)
        {
            int index = 0;
            foreach (KeyValuePair<uint, MapData> item in m_mapDataDic)
            {
                if (item.Value.id == mapId)
                    break;
                index++;
            }
            return index;
        }

        public int GetId(int index)
        {
            int curIndex = 0;
            foreach (KeyValuePair<uint, MapData> item in m_mapDataDic)
            {
                if (curIndex == index)
                    return (int)item.Key;
                curIndex++;
            }
            return curIndex;
        }

        public Dictionary<uint, MapData> m_mapDataDic = new Dictionary<uint, MapData>();
	}
}