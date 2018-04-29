using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Roma;

public struct SingleName
{
    public static string m_netRun = "NetRun";
    public static string m_netMgr = "NetMgr";
    public static string m_netBattle = "NetBattle";
    public static string m_downLoadHelper = "dowloadhelper";
    public static string m_ResFac = "resFac";
    public static string m_ResMgr = "resMgr";
    public static string m_ResDpMgr = "resDpMgr";

    public static string m_GUI = "gui";
	public static string m_CSV = "csv";

    public static string m_Scene = "scene";	
    public static string m_Entity = "entity";

    public static string m_shader = "shader";
    public static string m_sound = "sound";
    public static string m_sceneAnima = "sceneAnima";
}
public class SingletonManager
{
    public static SingletonManager Inst = new SingletonManager();

    private Dictionary<string, Singleton> m_singletonMap = new Dictionary<string, Singleton>();

    public SingletonManager()
    {
    }

    public void AddSingleton(string name, Singleton singleton)
    {
        if (m_singletonMap.ContainsKey(name))
        {
            //Debug.Log("重复添加单例类：" + name);
            return;
        }
        m_singletonMap.Add(name, singleton);
    }

    public void RemoveSingleton(string name)
    {
        if (!m_singletonMap.ContainsKey(name))
        {
            //Debug.Log("卸载失败，不包含单例类：" + name);
            return;
        }
        m_singletonMap.Remove(name);
    }

    public Singleton GetSingleton(string name)
    {
        if (!m_singletonMap.ContainsKey(name))
        {
            //Debug.Log("获取失败，不包含单例类：" + name);
        }
        return m_singletonMap[name];
    }

    public void Init()
    {
        foreach (KeyValuePair<string, Singleton> item in m_singletonMap)
        {
            if (item.Value.IsAutoUpdate())
            {
                item.Value.Init();
            }
        }
    }

    public void Update(float fTime, float fDTime)
    {
        Dictionary<string, Singleton>.Enumerator map = m_singletonMap.GetEnumerator();
        while (map.MoveNext())
        {
            if (map.Current.Value.IsAutoUpdate())
            {
                //Profiler.BeginSample(map.Current.Key);
                map.Current.Value.Update(fTime, fDTime);
                //Profiler.EndSample();
            }
        }
    }

    public void Destroy()
    {
        foreach (KeyValuePair<string, Singleton> item in m_singletonMap)
        {
            if (item.Value != null)
            {
                item.Value.Destroy();
            }
        }
    }
}
