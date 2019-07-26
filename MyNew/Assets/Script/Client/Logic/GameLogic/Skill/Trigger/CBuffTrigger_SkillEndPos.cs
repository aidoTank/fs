using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Roma
{
    /// <summary>
    /// 按照技能的位置作为触发器位置
    /// 支持相对位置
    /// </summary>
    public class CBuffTrigger_SkillEndPos : CBuffTrigger
    {
        public CBuffTrigger_SkillEndPos(long id)
            : base(id)
        {

        }

        public override void InitPos(ref Vector2 startPos, ref Vector2 startDir)
        {
            // 如果技能位置是0，则起点为施法者位置
            if(m_skillPos == Vector2.zero)
            {
                m_skillPos = startPos;
            }

            // 有距离时，表示为终点位置的偏移
            Vector2 dir = Collide.Rotate(startDir, m_triggerData.dirDelta);
            startDir = dir;
            startPos = m_skillPos + dir.normalized * m_triggerData.disDelta;
        }

    }
}

