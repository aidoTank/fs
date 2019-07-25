using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Roma
{
    /// <summary>
    /// 释放技能
    /// 1.通过当前目标位置获取方向
    /// 2.释放技能
    /// </summary>
    public class Action_SendSkill_ : AIAction_
    {
        public override BtResult Execute()
        {
            // 发送常用技能
            AIParam.SendSkill(m_creature, m_dataBase, false);
            return BtResult.Ended;
        }
    }
}
