using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;
using Roma;
public class ExportAB
{
    private static string fullPath = Application.dataPath + "/Resource/";    //将Assets/Prefab/文件夹下的所有预设进行打包

    [MenuItem("打包/清除无用资源---获取ab名为空的资源")]
    public static void GetNullAb()
    {
        DirectoryInfo dir = new DirectoryInfo(fullPath);    //获取目录信息
        FileInfo[] files = dir.GetFiles("*", SearchOption.AllDirectories);  //获取所有的文件信息
        for (var i = 0; i < files.Length; ++i)
        {
            FileInfo fileInfo = files[i];
            if (!fileInfo.Name.EndsWith(".meta"))   //判断去除掉扩展名为“.meta”的文件
            {
                string basePath = "Assets" + fileInfo.FullName.Substring(Application.dataPath.Length);  //编辑器下路径Assets/..
                AssetImporter importer = AssetImporter.GetAtPath(basePath);
                if (importer != null && importer.assetBundleName == "")
                {
                    Debug.Log("AssetImporter：" + basePath);
                }
            }
        }
    }


    [MenuItem("打包/清除自带Shader---获取所有Standard")]
    public static void GetAllStan()
    {
        DirectoryInfo dir = new DirectoryInfo(fullPath);    //获取目录信息
        FileInfo[] files = dir.GetFiles("*", SearchOption.AllDirectories);  //获取所有的文件信息
        for (var i = 0; i < files.Length; ++i)
        {
            FileInfo fileInfo = files[i];
            if (!fileInfo.Name.EndsWith(".meta"))   //判断去除掉扩展名为“.meta”的文件
            {
                string basePath = "Assets" + fileInfo.FullName.Substring(Application.dataPath.Length);  //编辑器下路径Assets/..
                Material mat = AssetDatabase.LoadAssetAtPath<Material>(basePath);
                if (mat != null)
                {
                    Shader shader = mat.shader;
                    if (shader.name.Equals("Standard"))
                    {
                        Debug.LogError("警告：项目包含Standard的Shader请更换Shader：" + basePath);
                    }
                }
            }
        }
    }

    [MenuItem("打包/清除Collider--获取所有Collider")]
    public static void GetAllCollider()
    {
        DirectoryInfo dir = new DirectoryInfo(fullPath);    //获取目录信息
        FileInfo[] files = dir.GetFiles("*", SearchOption.AllDirectories);  //获取所有的文件信息
        for (var i = 0; i < files.Length; ++i)
        {
            FileInfo fileInfo = files[i];
            if (!fileInfo.Name.EndsWith(".meta"))   //判断去除掉扩展名为“.meta”的文件
            {
                string basePath = "Assets" + fileInfo.FullName.Substring(Application.dataPath.Length);  //编辑器下路径Assets/..

                Transform mat = AssetDatabase.LoadAssetAtPath<Transform>(basePath);
                if (mat == null)
                    continue;

                Collider col = mat.GetComponent<Collider>();
                if (col != null)
                {
                    Debug.LogError("警告：项目包含碰撞体，请清除：" + basePath);
                }
                Collider[] list = mat.GetComponentsInChildren<Collider>();
                for (int j = 0; j < list.Length; j++)
                {
                    Debug.LogError("警告：项目包含碰撞体，请清除：" + basePath);
                }
            }
        }
    }

    [MenuItem("打包/清除编辑器本地信息")]
    public static void ClearVersion()
    {
        //PlayerPrefs.SetString("version", "-1");
        PlayerPrefs.DeleteAll();
    }

    /// <summary>
    /// 清除之前设置过的AssetBundleName，避免产生不必要的资源也打包 
    /// 因为只要设置了AssetBundleName的，都会进行打包，不论在什么目录下 
    /// </summary> 
    [MenuItem("打包/清除所有ab名")]
    public static void ClearAssetBundlesName()
    {
        string[] oldAssetBundleNames = AssetDatabase.GetAllAssetBundleNames();
        for (int j = 0; j < oldAssetBundleNames.Length; j++)
        {
            AssetDatabase.RemoveAssetBundleName(oldAssetBundleNames[j], true);
        }
        Debug.Log("ab名清除完成");
    }

    /// <summary>
    /// 只会设置我们自定义的主资源，然后通过主资源遍历设置依赖的资源
    /// </summary>
    [MenuItem("打包/设置主资源名")]
    public static void SetMainAssetBundleName()
    {
        SetABName(fullPath, true);
    }

    static void SetABName(string path, bool ContainDependences = false)
    {
        if (Directory.Exists(path))
        {
            EditorUtility.DisplayProgressBar("设置AssetName名称", "正在设置AssetName名称中...", 0f);   //显示进程加载条
            DirectoryInfo dir = new DirectoryInfo(path);    //获取目录信息
            FileInfo[] files = dir.GetFiles("*", SearchOption.AllDirectories);  //获取所有的文件信息
            for (var i = 0; i < files.Length; ++i)
            {
                FileInfo fileInfo = files[i];
                EditorUtility.DisplayProgressBar("设置AssetName名称", "正在设置AssetName名称中...", 1f * i / files.Length);
                if (!fileInfo.Name.EndsWith(".meta"))   //判断去除掉扩展名为“.meta”的文件
                {
                    if (!ResInfo.IsMainResFile(fileInfo.Name))
                    {
                        continue;
                    }

                    string basePath = "Assets" + fileInfo.FullName.Substring(Application.dataPath.Length);  //编辑器下路径Assets/..
                    string assetName = fileInfo.FullName.Substring(path.Length);  //预设的Assetbundle名字，带上一级目录名称
                    assetName = assetName.Substring(0, assetName.LastIndexOf('.')); //名称要去除扩展名
                    assetName = assetName.Replace('\\', '/');   //注意此处的斜线一定要改成反斜线，否则不能设置名称
                    AssetImporter importer = AssetImporter.GetAtPath(basePath);
                    if (importer && importer.assetBundleName != assetName && !basePath.Contains(".cs"))
                    {
                        importer.assetBundleName = assetName;  //设置预设的AssetBundleName名称
                        Debug.Log("新增资源 主资源名：" + assetName);
                    }
                    //Debug.Log("主资源的路径：" + basePath);
                    if (ContainDependences)    //把依赖资源分离打包
                    {
                        //获得他们的所有依赖，不过AssetDatabase.GetDependencies返回的依赖是包含对象自己本身的
                        string[] dps = AssetDatabase.GetDependencies(basePath); //获取依赖的相对路径Assets/...
                        //Debug.Log(string.Format("There are {0} dependencies!", dps.Length));
                        //遍历设置依赖资源的Assetbundle名称，用哈希Id作为依赖资源的名称
                        for (int j = 0; j < dps.Length; j++)
                        {
                            string allName = dps[j];
                            //要过滤掉依赖的自己本身和脚本文件，自己本身的名称已设置，而脚本不能打包
                            if (allName.Contains(assetName) || allName.Contains(".cs"))
                                continue;
                            else
                            {

                                AssetImporter importer2 = AssetImporter.GetAtPath(allName);
                                //string dpName = AssetDatabase.AssetPathToGUID(dps[j]);  //获取依赖资源的哈希ID
                                allName = allName.Substring(0, allName.LastIndexOf('.')); //名称要去除扩展名
                                string adName = allName.Replace("Assets/Resource/", "");
                                if (importer2 == null)
                                {
                                    Debug.Log("res is null：" + adName);
                                    continue;
                                }
                                //if(importer2.assetBundleName  != adName)
                                //{
                                importer2.assetBundleName = adName;
                                //}
                            }
                        }
                    }

                }
            }

            EditorUtility.ClearProgressBar();   //清除进度条
        }
    }

    /// <summary>
    /// 打包所有资源
    /// </summary>
    //[MenuItem("打包/打包  &n")]
    public static void CreateAllAssetBundles()
    {
        //打包资源的路径，打包在对应平台的文件夹下
        string targetPath = Application.dataPath + "/StreamingAssets/" + Client.m_prefix + "/";
        if (!Directory.Exists(targetPath))
        {
            Directory.CreateDirectory(targetPath);
        }

        //打包资源,
        BuildPipeline.BuildAssetBundles(targetPath, BuildAssetBundleOptions.ChunkBasedCompression, ExportTarget.m_buildTarget);

        //刷新编辑器
        AssetDatabase.Refresh();
    }


    /// <summary>
    /// 自动打包所有资源（设置了Assetbundle Name的资源）
    /// </summary>
    [MenuItem("打包/一键设置并打包  &n")] //设置编辑器菜单选项
    public static void CreateAllAssetBundles2()
    {
        //ClearAssetBundlesName();
        SetMainAssetBundleName();
        //打包资源的路径，打包在对应平台的文件夹下
        string targetPath = Application.dataPath + "/StreamingAssets/" + Client.m_prefix + "/";
        if (!Directory.Exists(targetPath))
        {
            Directory.CreateDirectory(targetPath);
        }

        //打包资源
        BuildPipeline.BuildAssetBundles(targetPath, BuildAssetBundleOptions.ChunkBasedCompression, ExportTarget.m_buildTarget);

        //刷新编辑器
        AssetDatabase.Refresh();
    }


}

