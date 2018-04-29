//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using UnityEngine;

//namespace Roma
//{
//    public class UIComMouse : UICom<object>
//    {
//        public UIComMouse()
//        {
//            m_ui = GetCom<UIMainMouse>(UIComName.s_mianMouse);
//            SetVisible(true);
//            SetMouse(MouseImage.eCommon);
//        }

//        public override void SetVisible(bool bShow)
//        {
//            m_ui.OpenPanel(bShow);
//        }

//        public void SetMouse(MouseImage type)
//        {
//            if (m_curType == type)
//            {
//                return;
//            }
//            m_curType = type;
//            int[] info;
//            if (m_mouseAnimaDic.TryGetValue(type, out info))
//            {
//                m_ui.m_image.GetComponent<UIImageAnima>().Play((uint)UIIconID.icon_mouse, info[0], info[1]);
//            }
//        }

//        public override void Update()
//        {
//            Cursor.visible = false;
//            m_ui.m_image.transform.localPosition = Input.mousePosition - 0.5f * new Vector3(Screen.width, Screen.height, 0f);
//        }

//        public UIMainMouse m_ui;
//        private MouseImage m_curType = MouseImage.eNone;
//        private Dictionary<MouseImage, int[]> m_mouseAnimaDic = new Dictionary<MouseImage, int[]>()
//        {
//            {MouseImage.eCommon, new int[2]{0, 11}},
//            {MouseImage.eAttack, new int[2]{12, 17}},
//            {MouseImage.ePlayer, new int[2]{18, 19}},
//            {MouseImage.eTransmit, new int[2]{20, 21}},
//            {MouseImage.eDialog, new int[2]{22, 25}},
//            {MouseImage.eCannotUse, new int[2]{26, 45}},
//        };
//    }
//}
