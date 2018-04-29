//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using UnityEngine;

//namespace Roma
//{
//    public class UIComHUD : UICom<object>
//    {
//        public UIPanelHUD m_ui;

//        public UIComHUD()
//        {
//            m_ui = GetCom<UIPanelHUD>(UIComName.s_hud);
//            SetVisible(true);
//        }

//        public override void SetVisible(bool bShow)
//        {
//            m_ui.OpenPanel(bShow);
//        }

//        public void Add(eHUDType type, string info)
//        {
//            m_ui.Add(type, info, Vector2.zero);
//        }

//        public void Add(eHUDType type, string info, Vector2 screenPos)
//        {
//            m_ui.Add(type, info, screenPos);
//        }

//        public void Add(eHUDType type, string info, Vector3 worldPos)
//        {
//            m_ui.Add(type, info, worldPos);
//        }
//    }
//}
