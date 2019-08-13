using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

namespace Roma
{
    public class UIPanelResInitDialog : UIBase
    {
        public Text m_txtInfo;
        public static GameObject m_recheck;
        public GameObject m_btnOk;
        public GameObject m_btnExit;
        private System.Action<bool, object> m_event;

        public static Transform m_loading;

        private static bool m_run = false;

        private static float smothing = 180;

        public override void Init()
        {
            base.Init();
            m_recheck = m_root.FindChild("panel/dynamic/btn").gameObject;
            m_txtInfo = m_root.FindChild("panel/dynamic/btn/txt_info").GetComponent<Text>();
            m_btnOk = m_root.FindChild("panel/dynamic/btn/grid/0").gameObject;
            m_btnExit = m_root.FindChild("panel/dynamic/btn/grid/1").gameObject;
            m_loading = m_root.FindChild("panel/dynamic/loading");
            SetOrder(1002);
            UIEventListener.Get(m_btnOk).onClick = OnClickBtn;
            UIEventListener.Get(m_btnExit).onClick = OnClickBtn;
            SetConnetEnable(false);
            SetReloadEnable(false);
        }

        public override void Update()
        {
            base.Update();
            if (m_run)
                m_loading.Rotate(Vector3.back * smothing * Time.deltaTime);
        }

        private void OnClickBtn(GameObject go)
        {
            SetConnetEnable(false);

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
        }


        public void Open(string okTxt, string cancelTxt, string text, System.Action<bool, object> clickEvent)
        {
            Client.Inst().m_uiResInitDialog.OpenPanel(true);
            m_recheck.SetActiveNew(true);
            UIItem.SetText(m_btnOk, "txt", okTxt);
            if (!string.IsNullOrEmpty(cancelTxt))
            {
                m_btnExit.SetActiveNew(true);
                UIItem.SetText(m_btnExit, "txt", cancelTxt);
            }
            else
            {
                m_btnExit.SetActiveNew(false);
            }
            UIItem.SetItemAlign(UIItem.eItemAlignType.Center, m_btnOk.transform.parent);

            if (m_txtInfo != null)
            {
                m_txtInfo.text = text;
            }
            m_event = clickEvent;
        }

        public string GetTxtInfo()
        {
            if (m_txtInfo == null)
            {
                return "";
            }

            return m_txtInfo.text;
        }

        public bool IsSameCurTxt(string txt)
        {
            return GetTxtInfo() == txt;
        }

        /// <summary>
        /// 资源无网络连接
        /// </summary>
        public static void OpenNoConnNet()
        {
            Client.Inst().m_uiResInitDialog.Open("重连", "退出", "连接资源服务器失败，请联系客服。", (bOk, a) =>
            {
                if (bOk)
                {
                    Debug.Log("重连资源服");
                    //Client.Inst().m_gameinit.InitAndCheckUpdate();
                }
                else
                {
                    //Application.Quit();
                    OverrideFunction.AppQuit();
                }
            });
        }

        /// <summary>
        /// 断开提示框显示开关
        /// </summary>
        /// <param name="val"></param>
        public static void SetConnetEnable(bool val)
        {
            m_recheck.SetActiveNew(val);
        }

        /// <summary>
        /// loading转圈显示开关
        /// </summary>
        /// <param name="val"></param>
        public static void SetReloadEnable(bool val)
        {
            m_loading.SetActiveNew(val);
        }

        /// <summary>
        /// 重新加载调用
        /// </summary>
        //public static void SetReload(bool run)
        //{
        //    GameManager pManager = GameManager.Instance;
        //    if (pManager != null && pManager.m_isRunning)
        //    {
        //        if (pManager.Context.isBeignControl && pManager.m_bPve && run)
        //        {//正常PVE时,不显示LOADING
        //            return;
        //        }
        //    }

        //    Client.Inst().m_uiResInitDialog.OpenPanel(true);
        //    m_loading.SetActiveNew(run);
        //    m_run = run;
        //}
    }
}


