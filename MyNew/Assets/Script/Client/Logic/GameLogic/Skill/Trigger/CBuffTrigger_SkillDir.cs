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

        public override void InitPos(ref Vector2d startPos, ref Vector2d startDir)
        {
            // 修正起始位置XZ
            Vector3 deltaPos = m_triggerData.vBulletDeltaPos;
            float randomDir = m_triggerData.iCurveHeight;

            Vector2d vL = FPCollide.Rotate(startDir.normalized, 90) * new FixedPoint(deltaPos.x); // 左右偏移
            Vector2d vF = startDir.normalized * new FixedPoint(deltaPos.z);                     // 前后偏移
            startPos = m_caster.GetPos() + vL + vF;
            // 随机方向
            //if (randomDir != 0)
            //{
            //    int dirDelta = (int)randomDir;
            //    float random = GameManager.Inst.GetRand(-dirDelta, dirDelta + 1, 0);
            //    startDir = FPCollide.Rotate(startDir.normalized, (int)random);
            //}

            // 修正起始方向
            if (m_triggerData.dirDelta != 0)
            {
                startDir = FPCollide.Rotate(startDir, m_triggerData.dirDelta);
            }
        }

        public override void _UpdatePos()
        {
            Vector2d moveDir = GetDir();
            FixedPoint delta = new FixedPoint(FSPParam.clientFrameScTime) * GetSpeed();
            Vector2d nextPos = m_curPos + moveDir * delta;

            //Debug.Log("nextPos:" + nextPos);

            SetPos(nextPos);
            SetDir(moveDir);
            if (m_vCreature != null)
            {
                m_vCreature.SetMove(true);
                m_vCreature.SetPos(nextPos.ToVector3() + Vector3.up * m_triggerData.vBulletDeltaPos.y);  // 单独设置Y
            }
        }

    }
}

