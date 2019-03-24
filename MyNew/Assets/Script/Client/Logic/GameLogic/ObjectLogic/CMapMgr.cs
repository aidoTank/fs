using System;
using System.Collections.Generic;
using UnityEngine;
namespace Roma
{

    public class CMapMgr
    {
        public static CMap m_map;


        public static CMap Create(int mapId)
        {
            // 可根据不同地图类型创建不同类
            m_map = new CMap(mapId);
            return m_map;
        }

        public static void ExecuteFrame()
        {
            if (m_map != null)
            {
                m_map.ExecuteFrame();
            }
        }

        public static void Destroy()
        {
            if(m_map != null)
            {
                m_map.Destroy();
            }
        }
    }
}
