using UnityEngine;
using UnityEditor;
using System.IO;
using Roma;
using System.Text;
using System.Collections.Generic;

/// <summary>
/// 自动打包一个或者多个U3D文件。
/// 根据在Resource下选择的prefab文件，自动打包到StreamingAssets/平台/Resource相同的文件夹下
/// 默认用于UI的资源打包，免去手选目录，提高开发速度
/// </summary>
public class ExportAssetBundlesAuto_Lua
{
    private static string m_luaScriptPath = Application.dataPath + "/Script/Client/Lua/LuaScript";
    private static string m_luaExportPath = Application.dataPath + "/StreamingAssets/" + ExportDefine.m_prefix + "/Config/alllua.unity3d";

    [MenuItem("配置/生成Lua脚本资源 &L")]    
	static void ExportResource ()
	{
        // 打包的
        List<Object> luaList = new List<Object>();
        // 要删除的
        List<string> copyList = new List<string>();
        // 获取文件
        List<FileInfo> fileList = new List<FileInfo>();
        DirectoryInfo folder = new DirectoryInfo(m_luaScriptPath);
        GetAllFile(folder, "*.txt", ref fileList);
        GetAllFile(folder, "*.bytes", ref fileList);
        //foreach (FileInfo file in fileList)
        //{
        //    // 通过资源获取路径
        //    string prefabPath = file.FullName;
        //    // 改名,复制
        //    string copyFile = Application.dataPath + "/Resource/config/lua/" + file.Name + ".txt";

        //    string strNewPath = Path.GetDirectoryName(copyFile);
        //    strNewPath = strNewPath.Replace("\\", "/");
        //    Directory.CreateDirectory(strNewPath);

        //    File.Copy(prefabPath, copyFile, true);
        //    copyList.Add(copyFile);
        //    //Debug.Log("增加lua文件：" + prefabPath);
        //}
        AssetDatabase.Refresh();

        // 打包
        foreach (FileInfo file in fileList)
        {
            string assetPath = file.FullName.Substring(file.FullName.IndexOf("Assets"));

            AssetImporter importer = AssetImporter.GetAtPath(assetPath);
            importer.assetBundleName = "config/alllua";
            //Debug.Log("成功设置lua脚本ab名：" + assetPath);
        }

        // 拿到该文件的文件夹路径, 创建文件夹路径
        //string strNewFile = m_luaExportPath;
        //string strNewPath = Path.GetDirectoryName(strNewFile);
        //strNewPath = strNewPath.Replace("\\", "/");
        //Directory.CreateDirectory(strNewPath);


        // 开始将选择的资源打包
        //BuildPipeline.BuildAssetBundle(null, luaList.ToArray(), strNewFile,
        //                                BuildAssetBundleOptions.CompleteAssets,
        //                                ExportDefine.m_buildTarget);
        Debug.Log("打包成功：config/alllua");

        //foreach (string file in copyList)
        //{
        //    File.Delete(file);
        //}
        copyList.Clear();
        AssetDatabase.Refresh();
        ExportAB.CreateAllAssetBundles();
    }

    private static void GetAllFile(DirectoryInfo folder, string searchPattern, ref List<FileInfo> list)
    {
        DirectoryInfo[] dirInfo = folder.GetDirectories();
        foreach (DirectoryInfo item in dirInfo)
        {
            GetAllFile(item, searchPattern, ref list);
        }

        foreach (FileInfo file in folder.GetFiles(searchPattern))
        {
            list.Add(file);
        }
    }

    static void ConvertFileEncoding(string sourceFile, string destFile, System.Text.Encoding targetEncoding)
    {
        destFile = string.IsNullOrEmpty(destFile) ? sourceFile : destFile;
        System.IO.File.WriteAllText(destFile,
        System.IO.File.ReadAllText(sourceFile, System.Text.Encoding.Default),
        targetEncoding);
    }
}