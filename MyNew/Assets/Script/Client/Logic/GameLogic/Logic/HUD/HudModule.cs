using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roma
{
    public class HudModule : Widget
    {
        public UIPanelHUD m_ui;     
        public HudModule()
            : base(LogicModuleIndex.eLM_PanelHud)
        {

        }
        public static Widget CreateLogicModule()
        {
            return new HudModule();
        }
        public override void Init()
        {
            m_ui = GetUI<UIPanelHUD>();

        }
       
        public void SetHUD(eHUDType type, string text, CCreature cc)
        {
            if (cc == null || cc.m_vCreature == null)
                return;
            VBase vObj = cc.m_vCreature;
            Vector3 pos = vObj.GetEnt().GetPos() + Vector3.up * vObj.m_baseInfo.m_headHeight;
            m_ui.SetHUD(type, text, GUIManager.GetUIPos(pos));
        }
    }
}
