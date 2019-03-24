using System.Collections.Generic;
using Roma;
using System;


public class CFrameEvent
{
    public int curTime;
    public int maxTime;
    public Action endEvent;
}

public class CFrameTimeMgr :  Singleton
{
    private Dictionary<int, CFrameEvent> m_timeEvent = new Dictionary<int, CFrameEvent>();
    private int m_startId;
    private List<int> m_delList = new List<int>();
    private List<int> m_syncKeyList = new List<int>();

    public static CFrameTimeMgr Inst;
    public CFrameTimeMgr() : base(true)
    {
    }

    /// <summary>
    /// 1秒为1000
    /// </summary>
    public int RegisterEvent(int maxTime, Action func)
    {
        m_startId++;
        CFrameEvent ev = new CFrameEvent();
        ev.curTime = 0;
        ev.maxTime = maxTime;
        ev.endEvent = func;
        m_timeEvent.Add(m_startId, ev);
        return m_startId;
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

    public void FixedUpdate()
    {
        m_syncKeyList.Clear();
        //ev.endEvent() -> May Cause InvalidOperationException: out of sync
        Dictionary<int, CFrameEvent>.Enumerator map = m_timeEvent.GetEnumerator();
        while (map.MoveNext())
            m_syncKeyList.Add(map.Current.Key);

        for(int i = 0; i < m_syncKeyList.Count; i++)
        {
            int nKey = m_syncKeyList[i];
            CFrameEvent ev;
            if (m_timeEvent.TryGetValue(nKey, out ev))
            {
                if (null == ev || null == ev.endEvent)
                {
                    m_delList.Add(nKey);
                }
                else
                {
                    ev.curTime += FSPParam.clientFrameMsTime;
                    if (ev.curTime >= ev.maxTime)
                    {
                        ev.endEvent();
                        ev.endEvent = null;
                        m_delList.Add(nKey);
                    }
                }
            }
        }

        for(int i = 0; i < m_delList.Count; i ++)
        {
            m_timeEvent.Remove(m_delList[i]);
        }
        m_delList.Clear();
    }


}
