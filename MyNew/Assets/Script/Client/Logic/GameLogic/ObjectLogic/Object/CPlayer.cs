using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Roma
{
    
    public partial class CPlayer : CCreature
    {

        public CPlayer(long id)
            : base(id)
        {
            m_type = EThingType.Player;

            // 可以根据类型来判断，是否new装备，技能等列表
            m_arrProp = new int[(int)eCreatureProp.Max];
            m_dicSkill = new Dictionary<int, CSkillInfo>();
            m_buffList = new List<BuffBase>();
            m_listTrigger = new List<int>();

            //m_dicEuqip = new Dictionary<eEquipType, Item>();
            //m_dicPartner = new Dictionary<int, int>();
        }

    }
}

