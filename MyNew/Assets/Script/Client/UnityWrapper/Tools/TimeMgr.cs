using System.Collections.Generic;
using System;


public class CFrameEvent
{
    public float curTime;
    public float maxTime;
    public Action endEvent;
}

public class TimeMgr :  Singleton
{
    private Dictionary<int, CFrameEvent> m_timeEvent = new Dictionary<int, CFrameEvent>();
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
        CFrameEvent ev = new CFrameEvent();
        ev.curTime = 0;
        ev.maxTime = maxTime;
        ev.endEvent = func;
        m_timeEvent.Add(m_startId, ev);
        return m_startId;
    }

    public override void Update(float time, float fdTime)
    {
        Dictionary<int, CFrameEvent>.Enumerator map = m_timeEvent.GetEnumerator();
        while (map.MoveNext())
        {
            CFrameEvent ev = map.Current.Value;
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


}
