using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Roma;

public class GameInfo : MonoBehaviour 
{
    /// <summary>
    /// 应用程序使用的堆大小
    /// </summary>
    public uint m_usedHeapSize = 0;
    /// <summary>
    /// mono堆栈大小
    /// </summary>
    public uint m_monoHeapSize = 0;
    /// <summary>
    /// 托管代码使用的大小
    /// </summary>
    public uint m_monoUsedSize = 0;

    /// <summary>
    /// 游戏总内存 = 已分配的内存 + 未分配的内存
    /// </summary>
    public uint m_totalReservedMemory = 0;
    /// <summary>
    /// 已分配的内存
    /// </summary>
    public uint m_TotalAllocatedMemory = 0;
    /// <summary>
    /// 未分配(临时)的内存，越小越好，频繁的字符串处理，new list等会增加未分配内存大小
    /// </summary>
    public uint m_totalUnusedReservedMenory = 0;


    private string m_curLoadRes;
    private string m_curUnLoadRes;
    private string m_dpRes;

    private string m_staticEntity;
    private string m_dynamicEntity;
    private string m_cacheEntity;


    private float fSample = 0.0f;

    private bool m_openWin = false;
    private Rect windowRect = new Rect(20, 20, 1000, 700);
    private Vector2 scrollPosition = Vector2.zero;

    private GUIStyle m_style = new GUIStyle();

    void Start()
    {
        if(Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
            m_style.fontSize = 30;
        else
            m_style.fontSize = 16;
        m_style.normal.textColor = Color.white;

        lastInterval = Time.realtimeSinceStartup;
    }

    void DrawWindow(int wId)
    {
        scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.Width(990), GUILayout.Height(700));
        GUILayout.Label("FPS: " + m_fpsText, m_style);

        GUILayout.Label("应用程序使用堆大小: " + (m_usedHeapSize >> 20) + "MB", m_style);
        GUILayout.Label("mono堆栈大小: " + (m_monoHeapSize >> 20) + "MB", m_style);
        GUILayout.Label("mono托管代码使用的大小(GC TotalMemory): " + (m_monoUsedSize >> 20) + "MB", m_style);
        GUILayout.Label("");

        GUILayout.Label("mono游戏总内存: " + MathEx.GetSize((int)m_totalReservedMemory), m_style);
        GUILayout.Label("mono已分配的内存: " + MathEx.GetSize((int)m_TotalAllocatedMemory), m_style);
        GUILayout.Label("mono未分配(临时)的内存: " + MathEx.GetSize((int)m_totalUnusedReservedMenory), m_style);
        GUILayout.Label("");


        GUILayout.Label("正在使用的资源：" + m_curLoadRes, m_style);
        GUILayout.Label("正在卸载的资源：" + m_curUnLoadRes, m_style);
        GUILayout.Label("依赖资源：" + m_dpRes, m_style);

        GUILayout.Label("静态实体：" + m_staticEntity, m_style);
        GUILayout.Label("动态实体：" + m_dynamicEntity, m_style);
        GUILayout.Label("缓存实体：" + m_cacheEntity, m_style);

        GUILayout.EndScrollView();

        GUI.DragWindow(new Rect(0, 0, 10000, 10000));
    }

	void OnGUI ()
    {
        m_openWin = GUI.Toggle(new Rect(10, 10, 400, 100), m_openWin, "打开调试界面", m_style);
        if (m_openWin)
        {
            GUI.backgroundColor = Color.black;
            windowRect = GUI.Window(0, windowRect, DrawWindow, "开发者面板");

            if (fSample > 4.0f)
            {
                m_usedHeapSize = Profiler.usedHeapSize;
                m_monoHeapSize = Profiler.GetMonoHeapSize();
#if UNITY_EDITOR
                m_monoUsedSize = Profiler.GetMonoUsedSize();
#else
                m_monoUsedSize = (uint)System.GC.GetTotalMemory(true);
#endif

                m_totalReservedMemory = Profiler.GetTotalReservedMemory();
                m_TotalAllocatedMemory = Profiler.GetTotalAllocatedMemory();
                m_totalUnusedReservedMenory = Profiler.GetTotalUnusedReservedMemory();

                //m_resNums = Resource.m_curTotalNums;
                //m_resSize = MathEx.GetSize(Resource.m_curTotalSize);

                m_curLoadRes = ResourceFactory.Inst.GetLoadResource();
                m_curUnLoadRes = ResourceFactory.Inst.GetUnLoadResource();
                m_dpRes = DPResourceManager.Inst.GetDPResource();

                m_staticEntity = EntityManager.Inst.GetStaticEntityInfo();
                m_dynamicEntity = EntityManager.Inst.GetDynamicEntityInfo();
                m_cacheEntity = EntityManager.Inst.GetCahceEntityInfo();

                Resources.UnloadUnusedAssets();

                fSample = 0.0f;
            }
            fSample += Time.deltaTime;
        }
    }


    private string m_fpsText;
    public float updateInterval = 0.5F;
    private double lastInterval;
    private int frames = 0;
    private float fps;

    //void Start()
    //{
    //    lastInterval = Time.realtimeSinceStartup;
    //    frames = 0;
    //}


    void Update()
    {
        ++frames;
        float timeNow = Time.realtimeSinceStartup;
        if (timeNow > lastInterval + updateInterval)
        {
            fps = (float)(frames / (timeNow - lastInterval));
            m_fpsText = fps.ToString("f2");
            frames = 0;
            lastInterval = timeNow;
        }
    } 
}
