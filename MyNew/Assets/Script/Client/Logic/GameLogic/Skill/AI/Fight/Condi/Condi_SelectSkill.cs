using System.Collections.Generic;
using UnityEngine;

namespace Roma
{
    /// <summary>
    /// 选择可用的技能
    /// 1.顺序选择技能
    /// </summary>
    public partial class Condi_SelectSkill : AICondi
    {

        public override bool Check()
        {
            return AIParam.RandomSelectSkill(m_creature, m_dataBase);
        }

    }
}


