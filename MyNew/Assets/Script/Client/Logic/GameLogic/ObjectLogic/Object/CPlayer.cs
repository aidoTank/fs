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
        }

    

        // UI相关
        public void UpdateUI()
        {
            
            BattleModule bm = (BattleModule)LayoutMgr.Inst.GetLogicModule(LogicModuleIndex.eLM_PanelBattle);
            bm.m_openEnd = ()=>{
                bm.SetMasterIcon(m_csv.icon);
                bm.SetPorp(0, "力量：" + GetPropNum(eCreatureProp.Force) * 0.001f);
                bm.SetPorp(1, "敏捷：" + GetPropNum(eCreatureProp.Agility) * 0.001f);
                bm.SetPorp(2, "智力：" + GetPropNum(eCreatureProp.Brain) * 0.001f);
            };
            bm.SetVisible(true);
        }
    }
}

