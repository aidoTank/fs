using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Roma
{

    /// <summary>
    /// 远离伤害区域
    /// </summary>
    public class Action_SkillUp : AIAction_
    {
        public override BtResult Execute()
        {
            //int index = m_dataBase.GetData<int>(AIParam.INT_SKILL_UP_INDEX);
            //m_player.SendSkillUp(index);
            return BtResult.Ended;
        }
    }
}
