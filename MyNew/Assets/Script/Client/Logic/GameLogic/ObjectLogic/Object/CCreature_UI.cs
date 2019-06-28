/**************************************
** company: 辰亚科技
** auth： 木木
** date： 2019/6/28 16:07:30
** desc： 尚未编写描述
** Ver.:  V1.0.0
***************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Roma
{
    public partial class CCreature : CObject
    {
        public void UpdateUI()
        {

            BattleModule bm = (BattleModule)LayoutMgr.Inst.GetLogicModule(LogicModuleIndex.eLM_PanelBattle);
            bm.m_openEnd = () => {
                bm.SetMasterIcon(m_csv.icon);
                bm.SetPorp(0, "力量：" + GetPropNum(eCreatureProp.Force) * 0.001f);
                bm.SetPorp(1, "敏捷：" + GetPropNum(eCreatureProp.Agility) * 0.001f);
                bm.SetPorp(2, "智力：" + GetPropNum(eCreatureProp.Brain) * 0.001f);
            };
            bm.SetVisible(true);
        }
    }
}
