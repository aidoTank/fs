
using UnityEngine;
using System.Collections.Generic;
namespace Roma
{
    /// <summary>
    /// 1.获取目标位置
    /// </summary>
    public class Condi_CheckChase : AICondi
    {

        public override void Activate(BtDatabase database)
        {
            base.Activate(database);
        }

        public override bool Check()
        {
            // 没有目标不处理
            //int targetUid = m_dataBase.GetData<int>((int)eAIParam.INT_TARGET_UID);
            //CCreature targetCC = CCreatureMgr.Get(targetUid);
            //if (targetUid == 0 || targetCC == null || targetCC.IsDie())
            //{
            //    return false;
            //}

            //int skillIndex = m_dataBase.GetData<int>((int)eAIParam.INT_SELECT_SKILL_INDEX);
            //CSkillInfo skillInfo = m_creature.GetSkillByIndex(skillIndex);
            //if(skillInfo == null)
            //{
            //    Debug.LogError("无法获取技能信息：" + skillIndex);
            //    return false;
            //}
            //float m_skillDis = skillInfo.GetRange();

            //// 大于技能的攻击才追逐
            //float dis = Vector2.Distance(targetCC.GetPos(), m_creature.GetPos());
            //if (dis <= m_skillDis)
            //{
            //    return false;
            //}

            return true;
        }



    }
}
