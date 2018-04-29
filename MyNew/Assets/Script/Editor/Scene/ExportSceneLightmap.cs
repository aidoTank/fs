using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using UnityEditor.SceneManagement;

public class ExportSceneLightmap
{

    private static string m_scenePath = Application.dataPath + "/Resource/scene/";
    //private static string m_sceneExportPath = Application.dataPath + "/StreamingAssets/" + ExportDefine.m_prefix + "/Resource/scene/";

    //[MenuItem("场景/ExportSceneLightMap（打包光照贴图） &L")]
    static void Export()
    {
        ExportLM();
    }

    static private void ExportLM()
    {
        // 获取场景名
        string sceneAllName = EditorSceneManager.GetActiveScene().path;
        sceneAllName = sceneAllName.Replace("Assets", "");
        int startPos = sceneAllName.LastIndexOf("/") + 1;
        string sceneName = sceneAllName.Substring(startPos, sceneAllName.Length - startPos);
        string sceneNamePre = sceneName.Replace(".unity", "");

        BuildLightMap(sceneNamePre);
    }
    // 打包为U3D文件   
    public static void BuildLightMap(string sceneName)
    {
        // 通过场景名称获取光照贴图目录
        string lmDir = m_scenePath + sceneName + "/" + sceneName;
        //Debug.Log("LMDIR"+lmDir);
        List<Texture2D> assets = new List<Texture2D>();
        DirectoryInfo rootDirInfo = new DirectoryInfo(lmDir);
        if (rootDirInfo.GetFiles().Length == 0)
        {
            return;
        }
        foreach (FileInfo fileInfo in rootDirInfo.GetFiles())
        {
            if (fileInfo.Name.Contains(".exr") && !fileInfo.Name.Contains(".meta"))
            {
                string assetPath = fileInfo.FullName.Substring(fileInfo.FullName.IndexOf("Assets"));
                Debug.Log("光照原图："+assetPath);


                string adName = assetPath.Replace('\\', '/');   //注意此处的斜线一定要改成反斜线，否则不能设置名称
                adName = adName.Substring(0, adName.LastIndexOf('/')); //名称要去除扩展名
                adName = adName.Replace("Assets/Resource/", "");
                adName += "/lm_" + sceneName;

                AssetImporter importer = AssetImporter.GetAtPath(assetPath);
                importer.assetBundleName = adName;
                Debug.Log("成功设置光照图ab名：" + adName);
            }
        }
        if (assets.Count == 0)
        {
            Debug.Log("该场景的lm目录下无光照贴图资源。");
        }
        //string targetDir = m_sceneExportPath + sceneName + "/" + sceneName;
        //Directory.CreateDirectory(targetDir);
        //string targetPath = targetDir + "/lm_" + sceneName + ".unity3d";
        //BuildPipeline.BuildAssetBundle(null, assets.ToArray(), targetPath, BuildAssetBundleOptions.CollectDependencies | BuildAssetBundleOptions.CompleteAssets,
        //    ExportDefine.m_buildTarget);
        //AssetDatabase.Refresh();

        // 将多个光照图设置一样的ab包名进行打包
    }   
}