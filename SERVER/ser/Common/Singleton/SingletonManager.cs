using System.Collections;
using System.Collections.Generic;
using System;

public struct SingleName
{
    public static readonly string m_csvMgr = "csv";
    public static readonly string m_dbMgr = "db";
    public static readonly string m_netRun = "NetRun";
    public static readonly string m_netMgr = "NetMgr";

    public static readonly string m_mapMgr = "map";
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
		if(m_singletonMap.ContainsKey(name))
		{
			return;
		}
		m_singletonMap.Add(name, singleton);
	}
	
	public void RemoveSingleton(string name)
	{
		if(!m_singletonMap.ContainsKey(name))
		{
			return;
		}
		m_singletonMap.Remove(name);
	}
	
	public Singleton GetSingleton(string name)
	{
		if(!m_singletonMap.ContainsKey(name))
		{
            return null;
		}
		return m_singletonMap[name];
	}

    public void Init()
    {
        foreach (KeyValuePair<string, Singleton> item in m_singletonMap)
        {
            item.Value.Init();
        }
    }
	
	public void Update(long time)
	{
		foreach(KeyValuePair<string, Singleton> item in m_singletonMap)
		{
			if(item.Value.IsAutoUpdate())
			{
				item.Value.Update(time);
			}
		}
	}
	
	public void Destroy()
	{
		foreach(KeyValuePair<string, Singleton> item in m_singletonMap)
		{
			item.Value.UnInit();
		}
	}
}
