//using UnityEngine;
//using System.Collections;
//using System.Collections.Generic;

//namespace Roma
//{
//    /*
//    int s = 1;
//    UICom<UIComDialog>.Inst().Open(true, "1111", "22222", "33333333333", Click, s);

//    private static void Click(bool isTrue, System.Object obj, bool isShow)
//    {
//        Debug.Log("==="+isTrue + "   ="+obj + "   ="+isShow);
//    }
//     * */
//    /// <summary>
//    /// 对话框单击按钮回调事件
//    /// </summary>
//    /// <param name="isOk">是否是OK按钮</param>
//    /// <param name="lstParam">参数</param>
//    /// <param name="isTips">当前提示选择框是否勾选</param>
//    //public delegate void DialogClickEvent(bool isOk, System.Object lstParam, bool isTips);

//    public class UIComDialog : UICom<object>
//    {
//        /*public UIComDialog()
//        {
//            m_ui = GetCom<UIPanelDialog>(UIComName.s_dialog);
//            base.SetVisible(false);

//            UIEventListener.Get(m_ui.m_btnOk).onClick = OnClickBtn;
//            UIEventListener.Get(m_ui.m_btnCancel).onClick = OnClickBtn;
//            UIEventListener.Get(m_ui.m_btnClose).onClick = OnClickBtn;
//            SetVisible(false);
//        }

//        public override void SetVisible(bool bShow)
//        {
//            m_ui.OpenPanel(bShow);
//        }

//        private void OnClickBtn(GameObject go)
//        {
//            if (go == m_ui.m_btnOk)
//            {
//                if (null != m_clickEvent)
//                {
//                    m_clickEvent(true, m_param, m_ui.m_toggleTips.isOn);
//                }
//            }
//            else if(go == m_ui.m_btnCancel)
//            {
//                if (null != m_clickEvent)
//                {
//                    m_clickEvent(false, m_param, m_ui.m_toggleTips.isOn);
//                }
//            }
//            SetVisible(false);
//        }

//        public void SetTitle(string title)
//        {
//            m_ui.m_title.text = title;
//        }
//        public void Open(bool showTips, string okTxt, string cancelTxt, string info, DialogClickEvent clickEvent, System.Object param)
//        {
//            SetVisible(true);
//            m_ui.gameObject.SetActive(showTips);
//            m_clickEvent = clickEvent;
//            UIButton.Get(m_ui.m_btnOk).SetText(okTxt);
//            UIButton.Get(m_ui.m_btnCancel).SetText(cancelTxt);
//            m_ui.m_info.text = info;
//            m_param = param;
//        }

//        private System.Object m_param;
//        private DialogClickEvent m_clickEvent;
//        public UIPanelDialog m_ui;*/
//    }
//}
