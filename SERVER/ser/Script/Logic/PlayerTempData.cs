using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class PlayerTempData
{
    public int m_roomId;
    // 当前队伍信息
    public int m_team;
    // 当前匹配类型
    public int m_matchType;

    /// <summary>
    /// 是否点击准备
    /// </summary>
    public bool bReady;
    /// <summary>
    /// 加载进度
    /// </summary>
    public int m_loadPct;
    /// <summary>
    /// 是否加载完成
    /// </summary>
    public bool bLoaded;

    public PlayerTempData()
    {

    }
}

