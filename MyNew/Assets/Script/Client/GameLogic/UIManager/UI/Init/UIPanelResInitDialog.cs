using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

namespace Roma
{
    public delegate void DialogClickEvent(bool isOk, System.Object lstParam, bool isTips);
    public class UIPanelResInitDialog : UI
    {
        public Text m_txtInfo;
        public GameObject m_btnOk;
        public GameObject m_btnExit;
        public DialogClickEvent m_event;

        public override void Init()
        {
            base.Init();
            m_txtInfo = m_root.FindChild("panel/txt_info").GetComponent<Text>();
            m_btnOk = m_root.FindChild("panel/btn/btn_ok").gameObject;
            m_btnExit = m_root.FindChild("panel/btn/btn_cancel").gameObject;

            SetOrder(1001);
            UIEventListener.Get(m_btnOk).onClick = OnClickBtn;
            UIEventListener.Get(m_btnExit).onClick = OnClickBtn;
        }

        private void OnClickBtn(GameObject go)
        {
            if (m_event != null)
            {
                if (go == m_btnOk)
                {
                    m_event(true, null, false);
                }
                else
                {
                    m_event(false, null, false);
                }
            }
            else
            {
                if (go == m_btnOk)    // 没有回调，默认是重连和退出
                {
                    //Client.Inst().m_gameInit.ResetInit();
                }
                else
                {
                    Application.Quit();
                }
            }
            OpenPanel(false);
        }

        public void AddEvent(DialogClickEvent clickEvent)
        {
            m_event = clickEvent;
        }

        public void SetText(string okTxt, string cancelTxt, string text)
        {
            UIItem.SetText(m_btnOk, "txt", okTxt);
            if (!string.IsNullOrEmpty(cancelTxt))
            {
                m_btnExit.SetActive(true);
                UIItem.SetText(m_btnExit, "txt", cancelTxt);
            }
            else
            {
                m_btnExit.SetActive(false);
            }

            if (m_txtInfo != null)
            {
                m_txtInfo.text = text;
            }
        }
    }
}


