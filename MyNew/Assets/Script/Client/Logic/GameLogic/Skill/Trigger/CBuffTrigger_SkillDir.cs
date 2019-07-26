using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Roma
{
    /// <summary>
    /// 按照技能方向的为初始方向的飞行检测器
    /// 可以扩展为按照施法者面朝方向的飞行检测器
    /// </summary>
    public class CBuffTrigger_SkillDir :  CBuffTrigger
    {

        public CBuffTrigger_SkillDir(long id)
            : base(id)
        {

        }

        public override void InitPos(ref Vector2 startPos, ref Vector2 startDir)
        {
            // 修正起始位置XZ
            Vector3 deltaPos = m_triggerData.vBulletDeltaPos;
            float randomDir = m_triggerData.iCurveHeight;

            Vector2 vL = Collide.Rotate(startDir.normalized, 90) * deltaPos.x; // 左右偏移
            Vector2 vF = startDir.normalized * deltaPos.z;                     // 前后偏移
            startPos = m_caster.GetPos().ToVector2() + vL + vF;
            // 随机方向
            if (randomDir != 0)
            {
                int dirDelta = (int)randomDir;
                float random = GameManager.Inst.GetRand(-dirDelta, dirDelta + 1, 0);
                startDir = Collide.Rotate(startDir.normalized, (int)random);
            }

            // 修正起始方向
            if (m_triggerData.dirDelta != 0)
            {
                startDir = Collide.Rotate(startDir, m_triggerData.dirDelta);
            }
        }

        public override void _UpdatePos()
        {
            Vector2 moveDir = GetDir().ToVector2();
            float delta = FSPParam.clientFrameScTime * GetSpeed().value;
            Vector2 nextPos = m_curPos.ToVector2() + moveDir * delta;

            //Debug.Log("nextPos:" + nextPos);

            SetPos(nextPos.ToVector2d());
            SetDir(moveDir.ToVector2d());
            if (m_vCreature != null)
            {
                m_vCreature.SetMove(true);
                m_vCreature.SetPos(nextPos.ToVector3() + Vector3.up * m_triggerData.vBulletDeltaPos.y);  // 单独设置Y
            }
        }

    }
}

