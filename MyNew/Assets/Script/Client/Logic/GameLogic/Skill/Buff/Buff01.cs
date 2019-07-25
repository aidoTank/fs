using UnityEngine;
using System.Collections.Generic;

namespace Roma
{
    /// <summary>
    /// 一次伤害的BUFF
    /// </summary>
    public class Buff01 : BuffBase
    {
        public Buff01(int uid, SkillBuffCsvData data)
            : base(uid, data)
        {

        }

        public override void Init()
        {
            m_lifeEventHid = CFrameTimeMgr.Inst.RegisterEvent(m_buffData.ContinuanceTime, () =>
            {
                Destroy();
            });


            int val = m_buffData.ParamValue1;

            if(OnHurt(val) == 1)
            {
                //SkillBase.AddBuff(m_caster, m_rec, 1000, Vector2.zero, Vector3.zero, Vector2.zero);
                // 一次伤害的BUFF，单独播放受击特效，而不是走表现层buff
                UpdateVO_ShowEffect_One(m_rec, m_buffData.effectId, m_buffData.effectPoint);

                // 受击者有霸体，不播放动作
                if (m_rec.CheckState(eBuffState.SuperArmor))
                    return;
                UpdateVO_HitAnima(m_rec, m_buffData.animaId);
            }

            // 如果子弹未检测到敌人，结束时播放特效
            if (m_rec == null && m_caster != null && m_caster.GetVObject() != null)
            {
                Vector3 casterH = m_caster.GetVObject().GetHitHeight();
                CEffectMgr.Create(m_buffData.effectId, 
                    m_skillPos.ToVector3() + casterH, 
                    m_skillDir.ToVector3());
            }
        }

        public override void ExecuteFrame()
        {

        }





    }
}