using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Roma
{
    public class CMasterPlayer : CPlayer
    {
        public CMasterPlayer(long id)
            : base(id)
        {
            m_type = EThingType.Master;
        }


        /// <summary>
        /// 重置摇杆，在放完技能时
        /// </summary>
        public void ResetJoyStick(bool bTrue)
        {
            JoyStickModule js = (JoyStickModule)LayoutMgr.Inst.GetLogicModule(LogicModuleIndex.eLM_PanelJoyStick);
            js.m_isFirstJoyStick = bTrue;
        }

    }
}

