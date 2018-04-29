using Roma;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System;


/**
正方贴图:
IOS下：
a.普通不透明： RGB PVRTC 4BITS
b.普通透明：   RGBA PVRTC 4BITS

Android下：
a.普通不透明： RGB ETC 4BITS
b.普通透明:    RGBA 16BIT
    因为没有通用最兼容的格式，所以一般情况是用RGBA 16BIT
    或有针对性的选择DXT5/ATC8 BITS/ETC2 8BITS。
    如果有技术支持，可以采用RGB ETC 4BITS加一张ALPHA 8的贴图来实现透明效果。

非正方贴图:
a.不透明贴图: RGB 16BITS
d.透明贴图：RGBA 16BITS

注：photo背景贴图直接压小
*/

/// <summary>
/// 导出所有数据配置 》二进制 》包文件
/// 
/// 1.填表
/// 2.制作资源
/// 3.设置所有主资源ab名(通过不同前缀表示不同类型资源)
///     1.场景制作-单独转为bytes-通过一键设置ab名
///     2.特效制作-通过一键设置ab名
///     3.模型制作-通过一键设置ab名
///     4.界面制作-通过一键设置ab名
///     5.配置填写-单独转为bytes-通过一键设置ab名
/// 4.打包
///     将设置ab名的资源进行打包
/// </summary>
public class ExportConfig
{

    private static string m_exportPath = Application.dataPath + "/StreamingAssets/" + ExportDefine.m_prefix + "/";

    private static string m_csvPath = Application.dataPath + "/Config/";
    private static uint m_csvNums = 0;
    private static string m_csvBytesPath = Application.dataPath + "/Resource/config/allcsvinfo.bytes";
    private static string m_csvAssetBundleName = "config/allcsvinfo";

    private static string m_resCsvPath = Application.dataPath + "/Config/resinfo.csv";
    private static string m_resPath = Application.dataPath + "/StreamingAssets/" + ExportDefine.m_prefix + "/";
    private static string m_resBytesPath = Application.dataPath + "/Resource/config/allresinfo.bytes";
    private static string m_resAssetBundleName = "config/allresinfo";

    private static LusuoStream m_csvStream;

    private static AllResInfoCsv m_resInfoCsv = null;
    protected static Dictionary<string, ResInfo> m_mapNameResInfo = null;
    protected static HashSet<int> m_setUseList = null;
    protected static List<ResInfo> m_lstResinfo = null;
    private static int m_curResID = 0;


    [MenuItem("配置/生成Csv和ResInfo配置(先打包资源，才能自动生成路径配置) &C")]
    public static void Export()
    {
        bool bSucc = ProcessCsvBytes();
        if (bSucc)
        {
            ProcessResBytes();
            BuildData();
        }
    }

    private static bool ProcessCsvBytes()
    {
        // 用于检测CSV是否写人LogicSystem
        CsvManager.Inst = new CsvManager();
        LogicSystem logicSystem = new LogicSystem();
        logicSystem.InitCsv(ref CsvManager.Inst);

        //LuaLogicSystem logicSystem = new LuaLogicSystem();
        //logicSystem.InitCsv(ref CsvManager.Inst);

        // 将所有的csv文件写入到m_csvStream
        m_csvNums = 0;
        m_csvStream = new LusuoStream(new FileStream(m_csvBytesPath, FileMode.Create));
        m_csvStream.WriteUInt(0); // 预留文件个数
        bool bSucc = ProcessCsvFile(new DirectoryInfo(m_csvPath));
        m_csvStream.Seek(0);
        m_csvStream.WriteUInt(m_csvNums);
        Debug.Log("成功写入csv个数：" + m_csvNums);
        m_csvStream.Close();

        return bSucc;
    }

    private static bool ProcessCsvFile(DirectoryInfo folder)
    {
        DirectoryInfo[] dirInfo = folder.GetDirectories();
        foreach (DirectoryInfo item in dirInfo)
        {
            if (item.Name.Contains(".svn"))
            {
                continue;
            }
            ProcessCsvFile(item);
        }
        FileInfo[] fileInfo = folder.GetFiles();
        foreach (FileInfo item in fileInfo)
        {
            string postfix = ".csv";
            int iPos = item.Name.IndexOf(postfix, 0);
            if (iPos != -1 && iPos + postfix.Length == item.Name.Length)
            {
                string csvName = item.Name.ToLower().Replace(".csv", "");
                // 如果这个表没有写入到LogicSystem中，就不需要打包
                if (CsvManager.Inst.GetType(csvName) == (int)eAllCSV.eAC_None)
                {
                    Debug.LogWarning(item.Name + " 没有写入到LogicSystem中，是否为客户端不需要的表格？");
                    continue;
                }
                FileStream file;
                try
                {
                    file = new FileStream(item.FullName, FileMode.Open);
                    if (file == null)
                    {
                        Debug.LogError(item.Name + "处理失败。");
                    }
                }
                catch (IOException ioe)
                {
                    Debug.LogError("处理csv失败，检查是否已经打开：" + item.FullName + " "+ ioe);
                    EditorUtility.DisplayDialog(
                    "打包配置错误",
                    "请关闭配置表，重新打包： " + item.FullName,
                    "确定");
                    file = null;
                    return false;
                }

                m_csvNums++;
                // 这里存csv的id效率更好
                //m_csvStream.WriteString(ref csvName);
                int type = CsvManager.Inst.GetType(csvName);
                m_csvStream.WriteInt(type);

                byte[] readData = new byte[file.Length];
                file.Read(readData, 0, (int)file.Length);
                // 转码并写入到流
                readData = Encoding.Convert(Encoding.Default, Encoding.UTF8, readData);
                m_csvStream.WriteInt((int)readData.Length);
                m_csvStream.Write(ref readData);
                Debug.Log("处理配置表：" + item.Name);
            }
        }
        return true;
    }

    private static void ProcessResBytes()
    {
        m_resInfoCsv = new AllResInfoCsv();
        m_mapNameResInfo = new Dictionary<string, ResInfo>(); // 下面批处理文件时，处理一个加一个，会审核是否有重复资源
        m_lstResinfo = new List<ResInfo>();                   // 保持时使用的数据
        m_setUseList = new HashSet<int>();                   // 存资源id
        m_curResID = 0;

        bool b = m_resInfoCsv.Load(m_resCsvPath, Encoding.Default);
        if (!b)
        {
            Debug.LogError("打开csv失败。");
            return;
        }

        foreach (ResInfo key in m_resInfoCsv.m_lstResinfo)
        {
            m_setUseList.Add(key.nResID);
        }

        string strFull = Path.GetDirectoryName(m_resPath);
        Directory.CreateDirectory(strFull);

        // 然后处理该文件夹中所有的文件
        ProcessResFile(new DirectoryInfo(m_resPath));

        // 然后遍历原始资源表列表，将无资源的预留id也写入到资源列表
        foreach (ResInfo oldItem in m_resInfoCsv.m_lstResinfo)
        {
            bool bHave = false;
            foreach (ResInfo newItem in m_lstResinfo)
            {
                if (oldItem.nResID == newItem.nResID)
                    bHave = true;
            }
            // 如果新的不包含，就加到新的
            if (!bHave)
            {
                oldItem.iType = ResType.None;
                oldItem.strUrl = "";
                if (m_mapNameResInfo.ContainsKey(oldItem.strName))
                {
                    Debug.LogError("总资源表含有重复的资源:" + oldItem.strName);
                    continue;
                }
                m_mapNameResInfo.Add(oldItem.strName, oldItem);
                m_lstResinfo.Add(oldItem);
            }
        }

        m_resInfoCsv.Clear();
        m_resInfoCsv.m_mapNameResInfo = m_mapNameResInfo;
        m_resInfoCsv.m_lstResinfo = m_lstResinfo;

        // 保存资源总配置文件
        m_resInfoCsv.Save("", Encoding.Default, true);

        // 生成二进制文件
        FileStream stream =
            new FileStream(m_resBytesPath, FileMode.Create);
        LusuoStream ls = new LusuoStream(stream);
        int iNums = m_lstResinfo.Count;
        ls.WriteInt(iNums);                     // 将真的资源个数写入到流的开始位置
        Debug.Log("成功写入资源个数：" + iNums);
        foreach (KeyValuePair<string, ResInfo> item in m_mapNameResInfo)
        {
            item.Value.Save(ref ls);
        }
        ls.Close();
    }

    private static void ProcessResFile(DirectoryInfo folder)
    {
        DirectoryInfo[] dirInfo = folder.GetDirectories();
        foreach (DirectoryInfo item in dirInfo)
        {
            if (item.Name.Contains(".svn"))
            {
                continue;
            }
            ProcessResFile(item);
        }
        FileInfo[] fileInfo = folder.GetFiles();
        foreach (FileInfo item in fileInfo)
        {
            // 排除.mate
            if(ResInfo.IsMainResFile(item.Name))
            { 
                ResInfo res = new ResInfo();
                string resName = item.Name.ToLower();
                string resPath = item.FullName;
                int pathStartPos = resPath.IndexOf(ExportDefine.m_prefix);     // 从android开始截取
                resPath = item.FullName.Substring(pathStartPos, item.FullName.Length - pathStartPos);

                res.strUrl = resPath.Replace("\\", "/").Replace(ExportDefine.m_prefix + "/", "");
                res.strName = resName;
                //res.iSize = (int)item.Length;
                //byte[] bytes = File.ReadAllBytes(item.FullName);
                //res.md5 = Md5.GetMd5Hash(bytes);

                res.iType = ResInfo.PrefixString2Type(res.strName);
                // 从表中取出这个名词以前的资源
                ResInfo oldRes = null;
                if (m_resInfoCsv.m_mapNameResInfo.TryGetValue(resName, out oldRes))
                {
                    res.nResID = oldRes.nResID;

                    if (!m_mapNameResInfo.ContainsKey(resName))
                    {
                        m_mapNameResInfo.Add(resName, res);
                        m_lstResinfo.Add(res);
                    }
                    else
                    {
                        Debug.LogError("有重复的资源：" + item.Name);
                    }
                }
                else // 如果当前的资源表中，无这个资源
                {
                    while (true)
                    {
                        if (m_setUseList.Contains(m_curResID))// 不停的从列表中找id,如果有就找下一个，直到没有才停止
                        {
                            m_curResID++;
                        }
                        else
                        {
                            break;
                        }
                    }
                    // m_setUseList不包含的这个id就作为这个新资源的id，然后加到列表
                    res.nResID = m_curResID;
                    m_setUseList.Add(m_curResID);

                    if (!m_mapNameResInfo.ContainsKey(resName))
                    {
                        m_mapNameResInfo.Add(resName, res);
                        m_lstResinfo.Add(res);
                        Debug.LogWarning(item.Name + "处理成功,新增加的资源，写入到资源总表。资源ID:" + res.nResID + "  【注：资源为手动添加到资源表，此资源是否为无用资源？】");
                    }
                    else
                    {
                        Debug.LogError("有重复的资源：" + item.Name);
                    }
                }
            }
        }
    }

    private static void BuildData()
    {
        AssetDatabase.Refresh();
        // 创建打包地址的文件夹
        string folder = Path.GetDirectoryName(m_exportPath);
        Directory.CreateDirectory(folder);
        // loadCSV二进制文件
        string assetPath = "Assets" + m_csvBytesPath.Replace(Application.dataPath, "");
        UnityEngine.Object obj = AssetDatabase.LoadMainAssetAtPath(assetPath);
        if (obj == null)
        {
            Debug.LogError("无法找到：" + m_csvBytesPath);
            return;
        }
        else
        {
            AssetImporter importer2 = AssetImporter.GetAtPath(assetPath);
            importer2.assetBundleName = m_csvAssetBundleName;
            Debug.Log("成功设置allcsvinfo.bytes的ab名：" + m_csvAssetBundleName);
        }

        assetPath = "Assets" + m_resBytesPath.Replace(Application.dataPath, "");
        obj = AssetDatabase.LoadMainAssetAtPath(assetPath);
        if (obj == null)
        {
            Debug.LogError("无法找到：" + m_resBytesPath);
            return;
        }
        else
        {
            AssetImporter importer2 = AssetImporter.GetAtPath(assetPath);
            importer2.assetBundleName = m_resAssetBundleName;
            Debug.Log("成功设置allresinfo.bytes的ab名：" + m_resAssetBundleName);
        }

        //打包资源
        //BuildPipeline.BuildAssetBundles(m_exportPath, BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows);

        //File.Delete(m_resBytesPath);
        //File.Delete(m_csvBytesPath);
        AssetDatabase.Refresh();
        ExportAB.CreateAllAssetBundles();
    }
}