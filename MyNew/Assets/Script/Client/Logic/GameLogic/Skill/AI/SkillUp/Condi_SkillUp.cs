
using UnityEngine;

namespace Roma
{
    /// <summary>
    /// 检测伤害
    /// </summary>
    public class Condi_SkillUp : AICondi
    {
        public override bool Check()
        {
            for(int index = 3; index > 0; index--)
            {
                //int skillId = m_player.GetSkillIdByIndex(index);
                //CSkill pCurSkill = m_player.skillGetSkill(skillId);
                //int nCurSkillLv = 0;
                //if (pCurSkill != null)
                //{
                //    nCurSkillLv = pCurSkill.getSkillLevel();
                //}
                //bool bCanLevelUp = CSkillLevelMgr.CheckIsCanLevelUp(m_player, skillId, nCurSkillLv);
                //if (bCanLevelUp)
                //{
                //    m_dataBase.SetData<int>(AIParam.INT_SKILL_UP_INDEX, index);
                //    return true;
                //}
            }
            return false;
        }
    }
}


