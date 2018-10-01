using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Roma
{
    /// <summary>
    /// 用场景包含一切的方式
    /// </summary>
    public class CMap
    {
        public int m_mapId;

        public CMap(int mapId)
        {
            m_mapId = mapId;
        }

        public void Create()
        {
            // 创建NPC
            SceneManager.Inst.LoadScene(m_mapId, null);
        }

        public void ExecuteFrame()
        {
            //CPlayerMgr.ExecuteFrame();
        }

        public void Enter(CCreature obj)
        {
            if(obj is CPlayer)
            {
                CPlayer player = obj as CPlayer;
                CPlayerMgr.Add(obj.GetUid(), player);

                player.InitConfigure();
            }
        }

        public void Destroy()
        {

        }
    }
}
