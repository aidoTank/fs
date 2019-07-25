using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Roma
{
    /// <summary>
    /// 可旋转发射变长矩形
    /// </summary>
    public class CBuffTrigger_SkillEndPos : CBuffTrigger
    {
        public CBuffTrigger_SkillEndPos(long id)
            : base(id)
        {

        }

        public override void InitPos(ref Vector2 startPos, ref Vector2 startDir)
        {
            float dis = Vector2.Distance(startPos, m_skillPos);
            Vector2 dir = Collide.Rotate(startDir, m_triggerData.dirDelta);
            m_skillPos = startPos + dis * dir.normalized;

            startPos = m_skillPos;
        }

    }
}

