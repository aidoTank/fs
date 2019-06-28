using UnityEngine;
using System.Collections.Generic;

namespace Roma
{
   
    public partial class VObject 
    {

        private int m_masterHaloHid;

        public void ShowFootHalo_Master()
        {
            if (!m_bMaster)
                return;
            if (m_ent == null || !m_ent.IsInited())
                return;
            DestoryFootHalo_Master();
            m_masterHaloHid = CEffectMgr.Create(6, GetEnt(), "origin");
        }

        public void DestoryFootHalo_Master()
        {
            if (!m_bMaster)
                return;

            if (m_masterHaloHid != 0)
            {
                CEffectMgr.Destroy(m_masterHaloHid);
                m_masterHaloHid = 0;
            }
        }


        // 玩家特效
        public int m_vipEffect;
        public const int m_vip1 = 21082;

        public void ShowVipEffect(int vipLv)
        {
            if(vipLv != 0)
            {
                m_vipEffect = CEffectMgr.CreateByCreaturePos(m_vip1 + vipLv, GetEnt(), 1);
            }
            else
            {
                CEffectMgr.Destroy(m_vipEffect);
                m_vipEffect = 0;
            }
        }

        // 怪物脚底
        private int m_footHaloEffect;
        private int m_footHaloHid;

        public void ShowFootHalo(int footHaloEffect)
        {
            m_footHaloEffect = footHaloEffect;
            if (m_ent == null || !m_ent.IsInited())
                return;
            DestoryFootHalo();
            m_footHaloHid = CEffectMgr.CreateByCreaturePos(footHaloEffect, GetEnt(), 1);
        }

        public void DestoryFootHalo()
        {
            if(m_footHaloHid != 0)
            {
                CEffectMgr.Destroy(m_footHaloHid);
                m_footHaloHid = 0;
            }
        }


    }
}
