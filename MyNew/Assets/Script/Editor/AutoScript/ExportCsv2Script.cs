using Roma;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Text;

public class Csv2ScriptWindow : EditorWindow
{
    private string m_csvFilePath = Application.dataPath + "/Config/";
    private string m_csvScriptPath = Application.dataPath + "/Script/Client/GameLogic/Csv/";
    private string m_csvFileName = "";
    private string m_csvClassName = "";

    private string[] m_listTitle;
    private string[] m_listParam;
    private string[] m_listType;

    [MenuItem("ScriptTools/Csv2Script")]
    static void OpenWindow()
    {
        //创建窗口
        Rect wr = new Rect(0, 0, 400, 150);
        Csv2ScriptWindow window 
            = (Csv2ScriptWindow)EditorWindow.GetWindowWithRect(typeof(Csv2ScriptWindow), wr, true, "CSV配置表一键生成CSV脚本");
        window.Show();
    }

    void OnGUI()
    {
        GUILayout.Label("配置表一键生成CSV脚本", EditorStyles.boldLabel);
        GUILayout.Label("csv文件名(如：游戏配置表，来自Config目录下的csv文件)：", EditorStyles.boldLabel);
        m_csvFileName = EditorGUILayout.TextField("", m_csvFileName);
        GUILayout.Label("csv类名(如：GameTextCsv)：", EditorStyles.boldLabel);
        m_csvClassName = EditorGUILayout.TextField("", m_csvClassName);
        if(GUILayout.Button("开始生成"))
        {
            string csvFile = m_csvFilePath + m_csvFileName + ".csv";
            if(!File.Exists(csvFile))
            {
                Debug.LogError("file is null:" + csvFile);
                return;
            }
            FileStream file = new FileStream(csvFile, FileMode.Open);
            StreamReader sr = new StreamReader(file, Encoding.Default);
            // 读取第一行中文名
            string strName = sr.ReadLine();
            // 读取第二行变量名
            string strParm = sr.ReadLine();
            // 类型第三行类型名称
            string strType = sr.ReadLine();
            m_listTitle = strName.Split(',');
            m_listParam = strParm.Split(',');
            m_listType = strType.Split(',');

            sr.Close();
            file.Close();
            WriteMain();
            AssetDatabase.Refresh();
        }
    }

    private void WriteMain()
    {
        string scriptFile = m_csvScriptPath + m_csvClassName + ".cs";
        FileStream file = new FileStream(scriptFile, FileMode.OpenOrCreate);
        StreamWriter sr = new StreamWriter(file, Encoding.Default);
 
        sr.WriteLine("using UnityEngine;");
        sr.WriteLine("using System.Collections.Generic;");
        sr.WriteLine("namespace Roma");
        sr.WriteLine("{");

        WriteEnum(sr);
        WriteParma(sr);
        WriteClass(sr);

        sr.WriteLine("}");
        sr.Close();
        file.Close();
        Debug.Log("脚本生成成功：" + m_csvScriptPath + m_csvClassName + ".cs");
    }

    private void WriteEnum(StreamWriter sr)
    {
        sr.WriteLine("\t" + "public enum e" + m_csvClassName);
        sr.WriteLine("\t" + "{");
        for (int i = 0; i < m_listParam.Length; i ++ )
        {
            sr.WriteLine("\t\t" + m_listParam[i] + ",");
        }
        sr.WriteLine("\t" +"}");
        sr.WriteLine("");
    }

    private void WriteParma(StreamWriter sr)
    {
        sr.WriteLine("\t" + "public class " + m_csvClassName + "Data");
        sr.WriteLine("\t" + "{");
        for (int i = 0; i < m_listParam.Length; i++)
        {
            sr.WriteLine("\t\t" + "/// <summary>");
            sr.WriteLine("\t\t" + "/// " +  m_listTitle[i]);
            sr.WriteLine("\t\t" + "/// </summary>");   
            sr.WriteLine("\t\t" + "public " + m_listType[i] + " " + m_listParam[i] + ";");
            sr.WriteLine("");
        }
        sr.WriteLine("\t" + "}");
        sr.WriteLine("");
    }

    private void WriteClass(StreamWriter sr)
    {
        sr.WriteLine("\t" + "public class " + m_csvClassName + " : CsvExWrapper");
        sr.WriteLine("\t" + "{");
        sr.WriteLine("\t\t" + "static public CsvExWrapper CreateCSV()");
        sr.WriteLine("\t\t" + "{");
        sr.WriteLine("\t\t\t" + "return new " + m_csvClassName + "();");
        sr.WriteLine("\t\t" + "}");
        sr.WriteLine("");

        Write_Load(sr);
        WriteGetData(sr);

        sr.WriteLine("\t" + "}");
    }

    private void Write_Load(StreamWriter sr)
    {
        sr.WriteLine("\t\t" + "protected override void _Load()");
        sr.WriteLine("\t\t" + "{");
        sr.WriteLine("\t\t\t" + "for (int i = 0; i < m_csv.GetRows(); i++)");
        sr.WriteLine("\t\t\t" + "{");
        sr.WriteLine("\t\t\t\t" + m_csvClassName + "Data" + " data = new " + m_csvClassName + "Data();");
        for (int i = 0; i < m_listParam.Length; i ++ )
        {
            if (m_listType[i].Equals("int"))
            {
                sr.WriteLine("\t\t\t\t" + "data." + m_listParam[i] + 
                    " = m_csv.GetIntData(i, (int)" + "e" + m_csvClassName + "."+m_listParam[i] +");");
            }
            else if (m_listType[i].Equals("uint"))
            {
                sr.WriteLine("\t\t\t\t" + "data." + m_listParam[i] +
                    " = (uint)m_csv.GetIntData(i, (int)" + "e" + m_csvClassName + "." + m_listParam[i] + ");");
            }
            else if (m_listType[i].Equals("string"))
            {
                sr.WriteLine("\t\t\t\t" + "data." + m_listParam[i] +
                    " = m_csv.GetData(i, (int)" + "e" + m_csvClassName + "." + m_listParam[i] + ");");
            }
            else if (m_listType[i].Equals("float"))
            {
                sr.WriteLine("\t\t\t\t" + "data." + m_listParam[i] +
                    " = m_csv.GetFloatData(i, (int)" + "e" + m_csvClassName + "." + m_listParam[i] + ");");
            }
        }
        sr.WriteLine("\t\t\t\t" + "m_dicData.Add(data." + m_listParam[0] + ", data);");
        sr.WriteLine("\t\t\t" + "}");
        sr.WriteLine("\t\t" + "}");
        sr.WriteLine("");
    }

    private void WriteGetData(StreamWriter sr)
    {
        sr.WriteLine("\t\t" + "public " + m_csvClassName + "Data GetData(int csvId)");
        sr.WriteLine("\t\t" +"{");
        sr.WriteLine("\t\t\t" + m_csvClassName + "Data" + " data;");
        sr.WriteLine("\t\t\t" + "if (m_dicData.TryGetValue(csvId, out data))");
        sr.WriteLine("\t\t\t" + "{");
        sr.WriteLine("\t\t\t\t" + "return data;");
        sr.WriteLine("\t\t\t" + "}");
        sr.WriteLine("\t\t\t" + "return null;");
        sr.WriteLine("\t\t" + "}");
        sr.WriteLine("");

        sr.WriteLine("\t\t" + "public Dictionary<int, " + m_csvClassName + "Data" + "> m_dicData = new Dictionary<int, " + m_csvClassName + "Data>();");
    }
    private void WriteGetListData(StreamWriter sr)
    {
        sr.WriteLine("\t\t" + "public List<" + m_csvClassName + "Data> GetListData(int csvId)");
        sr.WriteLine("\t\t" + "{");
        sr.WriteLine("\t\t" + "List<"+m_csvClassName+"Data> m_list = new List<"+m_csvClassName+"Data>();");
        sr.WriteLine("\t\t\t" + "foreach(KeyValuePair<int,"+m_csvClassName+"Data> item in m_dicData)");
        sr.WriteLine("\t\t\t" + "{");
        sr.WriteLine("\t\t\t\t\t" + "if(item.Value."+m_listParam[0]+"Equals(csvId))");
        sr.WriteLine("\t\t\t\t\t" + "{");
        sr.WriteLine("\t\t\t\t\t\t\t" + "m_list.Add(item.Value);");
        sr.WriteLine("\t\t\t\t\t" + "}");
        sr.WriteLine("\t\t\t" + "}");
        sr.WriteLine("\t\t\t" + "return m_list;");
        sr.WriteLine("\t\t" + "}");
        sr.WriteLine("");

        sr.WriteLine("\t\t" + "public Dictionary<int, " + m_csvClassName + "Data" + "> m_dicData = new Dictionary<int, " + m_csvClassName + "Data>();");
    }
}