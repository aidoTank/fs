using UnityEngine;
using System.Collections.Generic;

namespace Roma
{
    /// <summary>
    /// 拉扯性BUFF
    /// </summary>
    public class Buff03 : BuffBase
    {
        public Buff03(int uid, SkillBuffCsvData data)
            : base(uid, data)
        {

        }

        public override void ExecuteFrame()
        {
            if (m_rec == null || m_rec.IsDie())
                return;

            //if (Collide.GetDis2(m_rec.GetPos(), m_skillPos) <= 0.5f * 0.5f)
            //{
            //    m_rec.m_vCreature.m_bMoveing = false;
            //    return;
            //}

            //Vector2 moveDir = m_skillPos - m_rec.GetPos();
            //moveDir.Normalize();
            //Vector2 nextPos = m_rec.GetPos() + moveDir * FSPParam.clientFrameScTime * 20f;
            //m_rec.SetPos(nextPos);
            //m_rec.m_vCreature.SetPos(nextPos.ToVector3());
            //m_rec.m_vCreature.SetDir(moveDir.ToVector3());
            //m_rec.m_vCreature.SetSpeed(20);
            //m_rec.m_vCreature.m_bMoveing = true;
        }

        public override void Destroy()
        {
            base.Destroy();
        }

    }
}