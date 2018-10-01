using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

namespace Roma
{
    public class UIPanelResInitDialog : UIBase
    {
        public Text m_txtInfo;
        public GameObject m_btnOk;
        public GameObject m_btnExit;
        private System.Action<bool, object> m_event;

        public override void Init()
        {
            base.Init();
            m_txtInfo = m_root.FindChild("panel/dynamic/txt_info").GetComponent<Text>();
            m_btnOk = m_root.FindChild("panel/dynamic/btn/btn_ok").gameObject;
            m_btnExit = m_root.FindChild("panel/dynamic/btn/btn_cancel").gameObject;

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
                    m_event(true, null);
                }
                else
                {
                    m_event(false, null);
                }
            }
            OpenPanel(false);
        }


        public void Open(string okTxt, string cancelTxt, string text, System.Action<bool, object> clickEvent)
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
            m_event = clickEvent;
        }


        /// <summary>
        /// 资源无网络连接
        /// </summary>
        public static void OpenNoConnNet()
        {
            Client.Inst().m_uiResInitDialog.OpenPanel(true);
            Client.Inst().m_uiResInitDialog.Open("重连", "退出", "连接资源服务器失败，请联系客服。", (bOk, a) =>
            {
                if (bOk)
                {
                    Debug.Log("重连资源服");
                    //Client.Inst().m_gameinit.InitAndCheckUpdate();
                }
                else
                {
                    Application.Quit();
                }
            });
        }
    }
}


