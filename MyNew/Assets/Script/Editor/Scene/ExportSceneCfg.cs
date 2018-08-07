//using UnityEngine;
//using UnityEditor;
//using System.IO;
//using System.Collections.Generic;
//using System.Text;
//using Roma;
//using System;
//using UnityEditor.SceneManagement;

///// 场景打包：
///// 1.将模型作为prefab存入scenes/场景编号/prefab下
///// 2.使用Alt+a将prefab进行打包（自动生成包文件）
///// 3.使用Alt+c将打包的资源写入到配置表（自动生成包文件）
///// 4.制作场景，完成后使用Alt+c生成场景配置（自动生成包文件）
///// 5.再次使用Alt+c将打包的资源写入到配置表（自动生成包文件）
///// <summary>
///// 场景目录/Resource/scenes/场景编号/， 这个目录在美术制作时就会手动创建
///// 资源目录/Resource/scenes/场景编号/prefab， 这个目录在美术制作时就会手动创建
///// 配置地址/Resource/scenes/场景编号/cfg_场景编号.bytes，由打包工具生成
//public class ExportSceneAssetBundles
//{
//    private static string m_resCsvPath = Application.dataPath + "/Config/resinfo.csv";
//    private static string m_sceneCfgBundleName = "scene/";

//    protected static MapData m_mapData = null;

//    [MenuItem("场景/生成当前场景配置 &S")]
//    static void Export()
//    {
//        UnityEngine.Object[] objects = GameObject.FindObjectsOfType(typeof(GameObject));
//        string sceneAllName = EditorSceneManager.GetActiveScene().path;
//        sceneAllName = sceneAllName.Replace("Assets", "");
//        int startPos = sceneAllName.LastIndexOf("/") + 1;
//        string sceneName = sceneAllName.Substring(startPos, sceneAllName.Length - startPos);
//        string sceneNamePre = sceneName.Replace(".unity", "");
//        uint iSceneId = 0;
//        try
//        {
//            iSceneId = uint.Parse(sceneNamePre);
//        }
//        catch(Exception e)
//        {
//            EditorUtility.DisplayDialog("打包场景错误", "\n1.主场景main不能打包\n\n2.需要打包的场景名必须为数字ID!", "确定");
//            Debug.LogError("打包场景错误" + e);
//            return;
//        }
//        string cfgPath = Application.dataPath + sceneAllName.Replace(sceneName, "cfg_" + sceneNamePre + ".bytes");
//        string cfgDir = Path.GetDirectoryName(cfgPath);
//        Directory.CreateDirectory(cfgDir);
//        LusuoStream lf = new LusuoStream(new FileStream(cfgPath, FileMode.OpenOrCreate));
//        AllResInfoCsv allResCsv = new AllResInfoCsv();
//        if (!allResCsv.Load(m_resCsvPath, Encoding.Default))
//        {
//            Debug.LogError("Resinfo.csv 打开失败。");
//            return;
//        }
//        MapCsv mapCsv = new MapCsv();
//        if (!mapCsv.Load(Application.dataPath + "/Config" + "/地图表.csv", Encoding.Default))
//        {
//            Debug.LogError(Application.dataPath + "/Config" + "/地图表.csv" + " open fail");
//            return;
//        }
//        m_mapData = mapCsv.GetMapData(iSceneId);
//        SceneCfg sCfg = new SceneCfg(m_mapData.sizeW, m_mapData.sizeD);
//        SaveSceneCfg(lf, objects, sCfg, allResCsv);
//        lf.Close();

//        AssetDatabase.Refresh();

//        string assetPath = "Assets" + cfgPath.Replace(Application.dataPath, "");

//        string abName = m_sceneCfgBundleName + sceneNamePre + "/cfg_" + sceneNamePre;
//        AssetImporter importer = AssetImporter.GetAtPath(assetPath);
//        importer.assetBundleName = abName;
//        AssetDatabase.Refresh();
//        Debug.Log("成功设置地图配置ab名：" + abName);
//        ExportSceneLightmap.BuildLightMap(sceneNamePre);
//    }

//    private static void SaveSceneCfg(LusuoStream lf, UnityEngine.Object[] objs, SceneCfg cfg, AllResInfoCsv allResCsv)
//    {
//        if (null == lf)
//        {
//            return;
//        }
//        for (int i = 0; i < objs.Length; i++)
//        {
//            GameObject model = (GameObject) objs[i];
//            if (model && model.activeSelf)
//            {
//                // 根据对象的不同，做不一样的读写
//                if (ResInfo.PrefixString2Type(model.name) == ResType.ModelResource)
//                {
//                    Debug.Log("处理模型："+model.name);
//                    // 如果是模型，从配置总取出模型列表,后面就会将这个模型加到模型列表
//                    SaveSceneModel(lf, model, allResCsv, cfg.GetEntityInfoListCfg().GetEntityInfoList());
//                }
//            }
//        }
//        // 保存高度和障碍信息
//        SaveHeightAndBlocks(lf, cfg);
//        // 最后调用cfg类的写入到流即可。
//        cfg.Write(lf);
//    }

//    private static void SaveSceneModel(LusuoStream lf, GameObject model, AllResInfoCsv resCsv, List<EntityBaseInfo> entityList)
//    {
//        UnityEngine.Object prefabObj = PrefabUtility.GetPrefabParent(model);
//        string prefabName = prefabObj.name.Replace(".prefab", "");
//        int strindex = prefabName.LastIndexOf("/") + 1;
//        prefabName = prefabName.Substring(strindex, prefabName.Length - strindex);
//        prefabName = prefabName.ToLower();
//        ResInfo resInfo = null;
//        if (resCsv.m_mapNameResInfo.TryGetValue(prefabName, out resInfo))
//        {
//            EntityBaseInfo entity = new EntityBaseInfo();
//            entity.m_resID = resInfo.nResID;
//            entity.m_strName = resInfo.strName;
//            entity.m_vPos = model.transform.localPosition;
//            entity.m_vRotate = model.transform.localEulerAngles;
//            entity.m_vScale = model.transform.localScale;
//            entity.m_bStatic = model.isStatic;
//            // 写入光照图信息
//            List<Renderer> list = new List<Renderer>();
//            GetChildRander(ref list, model.transform);
//            //Debug.Log("个数："+list.Count);
//            entity.m_lightMapIndexNum = list.Count;
//            entity.m_lightMapIndex = new int[list.Count];
//            entity.m_lightMapScaleOffset = new Vector4[list.Count];
//            for (int i = 0; i < list.Count; i++ )
//            {
//                entity.m_lightMapIndex[i] = list[i].lightmapIndex;
//                entity.m_lightMapScaleOffset[i] = list[i].lightmapScaleOffset;
//            }
//            // 写入3D环境音效信息
//            List<AudioSource> listAudio = new List<AudioSource>();
//            GetChildAudio(ref listAudio, model.transform);
//            entity.m_envSoundNum = listAudio.Count;
//            entity.m_envSoundResId = new int[listAudio.Count];
//            for (int i = 0; i < listAudio.Count; i ++ )
//            {
//                ResInfo audioResInfo = null;
//                if(resCsv.m_mapNameResInfo.TryGetValue(listAudio[i].clip.name, out audioResInfo))
//                {
//                    entity.m_envSoundResId[i] = audioResInfo.nResID;
//                }
//            }
//            entityList.Add(entity);
//            //Debug.Log("添加模型到实体列表：" + resInfo.nResID + "  name;" + resInfo.strName);
//        }
//    }

//    private static void SaveHeightAndBlocks(LusuoStream lf, SceneCfg cfg)
//    {
//        TerrainHeightData height = cfg.GetTerrainHeightData();
//        TerrainBlockData block = cfg.GetBlockData();

//        GraphCollision.collisionMode = eCollisionMode.eCM_DownUp;

//        //Debug.Log("==" + m_mapData.maxSlope);
//        //Debug.Log("==" + m_mapData.maxClimb);

//        float maxSlope = m_mapData.maxSlope;   // 最大坡度
//        GridGenerate.maxClimb = m_mapData.maxClimb;   // 最大爬坡高度
//        if (maxSlope != 0.0f)
//        {
//            GridGenerate.maxSlope = maxSlope;
//            GridGenerate.bCheckSlope = true;
//        }
//        else
//        {
//            GridGenerate.bCheckSlope = false;
//        }

//        GridGenerate.Scan(block.m_nW, block.m_nH, out block.m_bBlock, out height.m_fHeight);
//        GridGenerate.GenGizmos(Color.blue, Color.red, Color.black);
//    }

//    private static void GetChildRander(ref List<Renderer> list, Transform parent)
//    {
//        Terrain ter = parent.GetComponent<Terrain>();
//        if (ter != null)   // 如果是地形
//        {
//            Renderer tRen = parent.GetComponent<Renderer>();
//            tRen.lightmapIndex = ter.lightmapIndex;
//            tRen.lightmapScaleOffset = ter.lightmapScaleOffset;
//            list.Add(tRen);
//        }
//        else
//        {
//            Renderer ren = parent.GetComponent<Renderer>();
//            if (ren != null)
//            {
//                list.Add(ren);
//            }
//        }
//        for (int i = 0; i < parent.childCount; i++)
//        {
//            Transform item = parent.GetChild(i);
//            GetChildRander(ref list, item);
//        }
//    }

//    private static void GetChildAudio(ref List<AudioSource> list, Transform parent)
//    {
//        AudioSource ren = parent.GetComponent<AudioSource>();
//        if (ren != null)
//        {
//            list.Add(ren);
//        }
//        for (int i = 0; i < parent.childCount; i++)
//        {
//            Transform item = parent.GetChild(i);
//            GetChildAudio(ref list, item);
//        }
//    }
//}