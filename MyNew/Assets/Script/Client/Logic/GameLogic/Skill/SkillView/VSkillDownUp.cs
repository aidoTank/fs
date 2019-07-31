using UnityEngine;
using System.Collections.Generic;

namespace Roma
{
    public partial class VSkillDownUp : VSkillBase
    {


        public void OnDown(CmdFspSendSkill cmd)
        {
            if (m_casterObject == null || m_casterObject.GetEnt() == null)
                return;

            m_casterObject.StartRotate(cmd.m_dir.ToVector3(), 0.11f);

            BattleEntity ent = m_casterObject.GetEnt() as BattleEntity;
            int animaId = m_casterData.animaName;
            if (m_casterObject.m_state == CmdFspEnum.eFspMove || m_casterObject.m_state == CmdFspEnum.eFspAutoMove)
            {
                // 施法方向和移动方向一致时
                float dot = Vector2.Dot(cmd.m_dir.ToVector2(), m_casterObject.m_moveInfo.m_dir.ToVector2());
                if(dot > 0)
                {
                    animaId = m_casterData.forwardAnimaId;
                }
                else
                {
                    animaId = m_casterData.backAnimaId;
                }
            }
         
            ent.PlayAnima(animaId);

            // 表现层，玩家不发生移动转向，而是直接设置方向
            m_casterObject.m_bRotate = false;
        }

        public void OnUp(CmdFspSendSkill cmd)
        {
            if (m_casterObject == null || m_casterObject.GetEnt() == null)
                return;

            m_casterObject.StartRotate(cmd.m_dir.ToVector3(), 0.1f);
            m_casterObject.m_bRotate = true;
        }
    }
}