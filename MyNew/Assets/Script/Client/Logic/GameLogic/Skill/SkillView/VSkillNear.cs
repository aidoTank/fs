using UnityEngine;
using System.Collections.Generic;

namespace Roma
{
    /// <summary>
    /// 技能的逻辑层
    /// </summary>
    public partial class VSkillNear : VSkillBase
    {

        public override void Start()
        {
            BattleEntity ent = m_casterObject.GetEnt() as BattleEntity;

            int animaId = m_casterData.animaName;

            if(m_casterObject.m_bMaster)
                animaId += ent.m_commonAtkIndex;

            ent.PlayAnima(animaId, () =>
            {
                m_casterObject.ResetState();
            });

            TimeMgr.Inst.RegisterEvent(m_casterData.startTime * 0.001f, () =>
            {
                int effectId = m_casterData.effectId;
                effectId += ent.m_commonAtkIndex;
                //Debug.Log("effectId:" + effectId);
                m_curCasterEffectHid = CEffectMgr.Create(effectId, ent, m_casterData.bindPoint);

                if (m_casterObject.m_bMaster)
                {
                    ent.m_commonAtkIndex++;
                    if (ent.m_commonAtkIndex >= 3)
                    {
                        ent.m_commonAtkIndex = 0;
                    }
                }
            });
        }
    }
}