using System.Collections.Generic;
using System;


public class CTimeEvent
{
    public float curTime;
    public float maxTime;
    public Action endEvent;
}

public class TimeMgr :  Singleton
{
    private Dictionary<int, CTimeEvent> m_timeEvent = new Dictionary<int, CTimeEvent>();
    private int m_startId;
    private List<int> m_delList = new List<int>();

    public static TimeMgr Inst;
    public TimeMgr() : base(true)
    {
    }

    /// <summary>
    /// 1秒为1000
    /// </summary>
    public int RegisterEvent(float maxTime, Action func)
    {
        m_startId++;
        CTimeEvent ev = new CTimeEvent();
        ev.curTime = 0;
        ev.maxTime = maxTime;
        ev.endEvent = func;
        m_timeEvent.Add(m_startId, ev);
        return m_startId;
    }

    public override void Update(float time, float fdTime)
    {
        Dictionary<int, CTimeEvent>.Enumerator map = m_timeEvent.GetEnumerator();
        while (map.MoveNext())
        {
            CTimeEvent ev = map.Current.Value;
            ev.curTime += fdTime;
            if(ev.curTime >= ev.maxTime)
            {
                ev.endEvent();
                ev.endEvent = null;
                m_delList.Add(map.Current.Key);
            }
        }

        for(int i = 0; i < m_delList.Count; i ++)
        {
            m_timeEvent.Remove(m_delList[i]);
        }
        m_delList.Clear();
    }

    public void RemoveEvent(int hid)
    {
        CTimeEvent fEvent;
        if (m_timeEvent.TryGetValue(hid, out fEvent))
        {
            fEvent.endEvent = null;
            fEvent = null;
            m_timeEvent.Remove(hid);
        }
    }

    /// <summary>
    /// 清除events
    /// </summary>
    public void ClearAllEvents()
    {
        m_timeEvent.Clear();
        m_delList.Clear();
        m_startId = 0;
    }


}
