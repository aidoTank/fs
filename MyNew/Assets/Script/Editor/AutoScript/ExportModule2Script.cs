using Roma;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System;
public class Module2ScriptWindow : EditorWindow
{
    private string m_csvScriptPath = Application.dataPath + "/Script/Client/GameLogic/Logic/WidgetLogic/";
    private string m_moduleName;
    private string m_moduleScriptName;

    [MenuItem("ScriptTools/Module2Script")]
    static void OpenWindow()
    {
        //创建窗口
        Rect wr = new Rect(0, 0, 400, 150);
        Module2ScriptWindow window
            = (Module2ScriptWindow)EditorWindow.GetWindowWithRect(typeof(Module2ScriptWindow), wr, true, "一键生成逻辑脚本");
        window.Show();
    }

    void OnGUI()
    {
        GUILayout.Label("自动生成逻辑脚本：", EditorStyles.boldLabel);

        GUILayout.Label("模块名(如：模块文件夹，EMail)：", EditorStyles.boldLabel);
        m_moduleName = EditorGUILayout.TextField("", m_moduleName);

        GUILayout.Label("模块脚本名(如：EMailMainModule)：", EditorStyles.boldLabel);
        m_moduleScriptName = EditorGUILayout.TextField("", m_moduleScriptName);

        if(GUILayout.Button("开始生成"))
        {
            if (!m_moduleScriptName.Contains("Module"))
            {
                EditorUtility.DisplayDialog(
                    "模块脚本名不规范",
                    "脚本名必须以Module结尾！",
                    "确定");
                return;
            }
            WriteMain();
            AssetDatabase.Refresh();
        }
    }

    private void WriteMain()
    {
        string scriptFile = m_csvScriptPath + m_moduleName + "/" + m_moduleScriptName + ".cs";
        string fileDir = Path.GetDirectoryName(scriptFile);
        Directory.CreateDirectory(fileDir);
        FileStream file = new FileStream(scriptFile, FileMode.OpenOrCreate);
        StreamWriter sr = new StreamWriter(file, Encoding.Default);
 
        sr.WriteLine("using UnityEngine;");
        sr.WriteLine("using System.Collections.Generic;");
        sr.WriteLine("namespace Roma");
        sr.WriteLine("{");

        WriteClass(sr);

        sr.WriteLine("}");
        sr.Close();
        file.Close();
        Debug.Log("脚本生成成功：" + m_csvScriptPath + m_moduleName + "/" + m_moduleScriptName + ".cs");
    }

    private void WriteClass(StreamWriter sr)
    {
        sr.WriteLine("\t" + "public class " + m_moduleScriptName + " : Widget");
        sr.WriteLine("\t" + "{");

        sr.WriteLine("\t\t" + "/// <summary>");
        sr.WriteLine("\t\t" + "/// 需要在LogicModuleIndex中增加对应的模块枚举");
        sr.WriteLine("\t\t" + "/// </summary>");   
        sr.WriteLine("\t\t" + "public " + m_moduleScriptName + "():base(LogicModuleIndex.eLM_" + m_moduleScriptName + ")");
        sr.WriteLine("\t\t" + "{");
        sr.WriteLine("\t\t" + "}");
        sr.WriteLine();

        sr.WriteLine("\t\t" + "public static Widget CreateLogicModule()");
        sr.WriteLine("\t\t" + "{");
        sr.WriteLine("\t\t\t" + "return new " + m_moduleScriptName + "();");
        sr.WriteLine("\t\t" + "}");
        sr.WriteLine();

        Write_Init(sr);
        Write_OnLoaded(sr);
        Write_SetVisible(sr);
        Write_OnClickBtn(sr);
        Write_UpdateList(sr);
        Write_OnRecv(sr);
        Write_OnUIPanel(sr);

        sr.WriteLine("\t" + "}");
    }

    private void Write_Init(StreamWriter sr)
    {
        sr.WriteLine("\t\t" + "/// <summary>");
        sr.WriteLine("\t\t" + "/// 初始化，将GetUI<UI>中的UI改为当前逻辑对应的UIPanelXX");
        sr.WriteLine("\t\t" + "/// </summary>");  
        sr.WriteLine("\t\t" + "public override void Init(string mrootName)");
        sr.WriteLine("\t\t" + "{");
        sr.WriteLine("\t\t\t" + "base.Init(mrootName);");
        sr.WriteLine("\t\t\t" + "m_ui = this.GetUI<UIPanel" + m_moduleScriptName.Substring(0, m_moduleScriptName.IndexOf("Module")) + ">();");
        sr.WriteLine("\t\t\t" + "SetVisible(false);");
        sr.WriteLine("\t\t" + "}");
        sr.WriteLine();
    }

    private void Write_OnLoaded(StreamWriter sr)
    {
        sr.WriteLine("\t\t" + "/// <summary>");
        sr.WriteLine("\t\t" + "/// 界面第一次加载完成时调用");
        sr.WriteLine("\t\t" + "/// </summary>");  
        sr.WriteLine("\t\t" + "public override void OnLoaded()");
        sr.WriteLine("\t\t" + "{");
        sr.WriteLine("\t\t\t" + "base.OnLoaded();");
        sr.WriteLine("\t\t" + "}");
        sr.WriteLine();
    }

    private void Write_SetVisible(StreamWriter sr)
    {
        sr.WriteLine("\t\t" + "public override void SetVisible(bool bShow)");
        sr.WriteLine("\t\t" + "{");
        sr.WriteLine("\t\t\t" + "base.SetVisible(bShow);");
        sr.WriteLine("\t\t\t" + "if (bShow)");
        sr.WriteLine("\t\t\t" + "{");
        sr.WriteLine("\t\t\t\t" + "InitData();");
        sr.WriteLine("\t\t\t" + "}");
        sr.WriteLine("\t\t\t" + "else");
        sr.WriteLine("\t\t\t" + "{");
        sr.WriteLine("\t\t\t\t" + "UnInitData();");
        sr.WriteLine("\t\t\t" + "}");
        sr.WriteLine("\t\t" + "}");
        sr.WriteLine();

        sr.WriteLine("\t\t" + "/// <summary>");
        sr.WriteLine("\t\t" + "/// 打开界面时，初始化数据");
        sr.WriteLine("\t\t" + "/// </summary>");  
        sr.WriteLine("\t\t" + "public void InitData()");
        sr.WriteLine("\t\t" + "{");
        sr.WriteLine("\t\t" + "}");
        sr.WriteLine();

        sr.WriteLine("\t\t" + "/// <summary>");
        sr.WriteLine("\t\t" + "/// 关闭界面时，清空数据");
        sr.WriteLine("\t\t" + "/// </summary>");  
        sr.WriteLine("\t\t" + "public void UnInitData()");
        sr.WriteLine("\t\t" + "{");
        sr.WriteLine("\t\t" + "}");
        sr.WriteLine();
    }

    private void Write_OnClickBtn(StreamWriter sr)
    {
        sr.WriteLine("\t\t" + "/// <summary>");
        sr.WriteLine("\t\t" + "/// 通用按钮事件，自行修改或者删除");
        sr.WriteLine("\t\t" + "/// </summary>");  
        sr.WriteLine("\t\t" + "private void OnClickBtn(GameObject go)");
        sr.WriteLine("\t\t" + "{");
        sr.WriteLine("\t\t\t" + "if (go == m_ui.m_btnClose)");
        sr.WriteLine("\t\t\t" + "{");
        sr.WriteLine("\t\t\t\t" + "SetVisible(false);");
        sr.WriteLine("\t\t\t" + "}");
        sr.WriteLine("\t\t" + "}");
        sr.WriteLine();
    }

    private void Write_UpdateList(StreamWriter sr)
    {
        sr.WriteLine("\t\t" + "/// <summary>");
        sr.WriteLine("\t\t" + "/// 更新界面列表方法，自行修改或者删除");
        sr.WriteLine("\t\t" + "/// </summary>");  
        sr.WriteLine("\t\t" + "public void UpdateXXList()");
        sr.WriteLine("\t\t" + "{");
        sr.WriteLine("\t\t" + "}");
        sr.WriteLine();
    }

    private void Write_OnRecv(StreamWriter sr)
    {
        sr.WriteLine("\t\t" + "/// <summary>");
        sr.WriteLine("\t\t" + "/// 用于给消息接受调用的界面，自行修改或者删除");
        sr.WriteLine("\t\t" + "/// </summary>");
        sr.WriteLine("\t\t" + "public void OnRecvXX()");
        sr.WriteLine("\t\t" + "{");
        sr.WriteLine("\t\t" + "}");
        sr.WriteLine();
    }

    private void Write_OnUIPanel(StreamWriter sr)
    {
        sr.WriteLine("\t\t" + "/// <summary>");
        sr.WriteLine("\t\t" + "/// 该逻辑所对应的UI类");
        sr.WriteLine("\t\t" + "/// </summary>");
        sr.WriteLine("\t\t" + "private UIPanel" + m_moduleScriptName.Substring(0, m_moduleScriptName.IndexOf("Module")) + " m_ui;");
        sr.WriteLine();
    }
}