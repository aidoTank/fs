using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Roma
{
    /// <summary>
    /// 移动行为
    /// 1.获取移动目标位置
    /// 2.移动到目标位置
    /// </summary>
    public class Action_MoveToTarget : AIAction_
    {
        private int m_skillDis;
        private int m_curTime;

        public void GetSkillDis()
        {
            int skillIndex = m_dataBase.GetData<int>((int)eAIParam.INT_SELECT_SKILL_INDEX);
            CSkillInfo skillInfo = m_creature.GetSkillByIndex(skillIndex);
            m_skillDis = skillInfo.GetRange();


            //Debug.Log(info.Name +" m_skillDis============================" + m_skillDis);
        }

        public override BtResult Execute()
        {
            GetSkillDis();
            if(m_skillDis == 0)
            {
                AIParam.SendSkill(m_creature, m_dataBase);
                return BtResult.Ended;
            }

            // 1.实时获取当前选择的技能和目标计算 当前的移动位置
            // 2.如果到达，即可结束
            int targetUid = m_dataBase.GetData<int>((int)eAIParam.INT_TARGET_UID);
            CCreature targetCC = CCreatureMgr.Get(targetUid);
            if (targetCC != null)
            {
                FixedPoint dis2 = FPCollide.GetDis2(targetCC.GetPos(), m_creature.GetPos());
                if (dis2 < new FixedPoint(m_skillDis * m_skillDis))
                {
                    return BtResult.Ended;
                }
                else
                {
                    // 一秒执行一次
                    m_curTime += FSPParam.clientFrameMsTime;
                    if (m_curTime >= 30 * FSPParam.clientFrameMsTime)
                    {
                        m_curTime = 0;

                        //Vector2 pPos = targetCC.GetPos();
                        //m_creature.GoTo(pPos);

                        Vector2d dir = targetCC.GetPos() - m_creature.GetPos();
                        Vector2d target = m_creature.GetPos() + dir.normalized * new FixedPoint(m_skillDis);
                        Vector2d endPos = CMapMgr.m_map.GetRandomPos(target.x.value, target.y.value, 3, m_creature.m_aiType).ToVector2d();
                        //Vector2d pPos = targetCC.GetPos();

                        CmdFspAutoMove cmd = new CmdFspAutoMove();
                        cmd.m_pos = endPos;

                        if (m_creature.m_aiType == eAIType.Player)
                        {
                            m_creature.SendFspCmd(cmd);
                        }
                        else
                        {
                            m_creature.PushCommand(cmd);
                        }
                    }
                    return BtResult.Running;
                }
            }

            return BtResult.Ended;
        }
    }
}
