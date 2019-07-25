using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roma
{
    /// <summary>
    /// 追赶状态
    /// 
    /// 2.移动到目标位置
    /// </summary>

    public class Action_Chase : AIAction_
    {
        public override BtResult Execute()
        {
            ////// 休闲时，继续巡逻
            //if (m_creature.GetLogicState().GetCmdType() == CmdFspEnum.eFspStopMove || !m_creature.m_bMovePathing)
            //{
            //    int targetUid = m_dataBase.GetData<int>((int)eAIParam.INT_TARGET_UID);
            //    CCreature targetCC = CCreatureMgr.Get(targetUid);


            //    int skillIndex = m_dataBase.GetData<int>((int)eAIParam.INT_SELECT_SKILL_INDEX);
            //    CSkillInfo skillInfo = m_creature.GetSkillByIndex(skillIndex);
            //    float m_skillDis = skillInfo.GetRange();


            //    Vector2 dir = targetCC.GetPos() - m_creature.GetPos();
            //    Vector2 target = m_creature.GetPos() + dir.normalized * m_skillDis;
            //    Vector3 endPos = CMapMgr.m_map.GetRandomPos(target.x, target.y, 3);
            //    Vector2 pPos = targetCC.GetPos();
            //    if (CMapMgr.m_map.CanArrive(m_creature, (int)endPos.x, (int)endPos.y))
            //    {
            //        m_creature.GoTo(endPos);
            //    }
            //    else
            //    {
            //        m_creature.GoTo(pPos);
            //    }
            //}
            return BtResult.Running;
        }
    }
}
