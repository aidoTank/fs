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
    public class CBuffTrigger_Curve : CBuffTrigger
    {
        public CBuffTrigger_Curve(long id)
            : base(id)
        {

        }

        public override bool Create(int csvId, string name, Vector2 pos, Vector2 dir, float scale = 1)
        {
            base.Create(csvId, name, pos, dir, scale);

            if (m_vCreature != null)
            {
                sCurveParam param;
                param.endPos = m_skillPos.ToVector3();
                param.time = m_triggerData.ContinuanceTime;
                param.heigh = m_triggerData.iCurveHeight;
                m_vCreature.PlayCurve(param);
            }

            return true;
        }

        public override void InitPos(ref Vector2 startPos, ref Vector2 startDir)
        {
            // 修正起始位置startPos
            startPos = startPos + startDir.normalized * m_triggerData.vBulletDeltaPos.z;
            // 距离为0时，表示为发射方向上的偏移
            if(m_triggerData.disDelta == 0)
            {
                // 修正结束位置m_skillPos
                float dis = Vector2.Distance(startPos, m_skillPos);
                Vector2 dir = Collide.Rotate(startDir, m_triggerData.dirDelta);
                m_skillPos = startPos + dir.normalized * dis;
            }
            else
            {
                // 有距离时，表示为终点位置的偏移
                Vector2 dir = Collide.Rotate(startDir, m_triggerData.dirDelta);
                m_skillPos = m_skillPos + dir.normalized * m_triggerData.disDelta;
            }

            // 一定时间后设置逻辑位置
            //CFrameTimeMgr.Inst.RegisterEvent(m_triggerData.ContinuanceTime - 100, () =>
            //{
            //    SetPos(m_skillPos, true);
            //});
        }

        public override void ExecuteFrame(int frameId)
        {
            base.ExecuteFrame(frameId);
        }
    }
}

