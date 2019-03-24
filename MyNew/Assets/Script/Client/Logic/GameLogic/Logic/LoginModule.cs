using UnityEngine;

namespace Roma
{
    public partial class LoginModule : Widget
    {
        public LoginModule()
            : base(LogicModuleIndex.eLM_PanelLogin)
        {
        }

        public static Widget CreateLogicModule()
        {
            return new LoginModule();
        }

        public override void Init()
        {
            m_uiLogin = GetUI<UIPanelLogin>();
            UIEventListener.Get(m_uiLogin.m_sureBtn.gameObject).onClick = OnClickBtn;
            ReadTempInfo();
        }

        public void OnClickBtn(GameObject go)
        {
            if (string.IsNullOrEmpty(GetUserName()) || string.IsNullOrEmpty(GetPassWord()))
            {
                Client.Inst().m_uiResInitDialog.OpenPanel(true);
                Client.Inst().m_uiResInitDialog.Open("我知道了", "", "用户名密码不能为空!", null);
                return;
            }
            SaveTempInfo();
            OnStartLogin();
            
        }

        public void OnStartLogin()
        {
            if (Client.Inst().isSingleTest)
            {
                int[] info = new int[1];
                info[0] = 0;
                GameManager.Inst.Start(info);
                GameManager.Inst.GetFspManager().m_bStartCtl = true;
                return;
            }
      
            NetRunTime.Inst.Conn(GlobleConfig.s_gameServerIP, GlobleConfig.s_gameServerPort, () =>
            {
                OnSendLoginMsg();
            });
            m_bStartConn = true;
            m_connCurTime = 0;

        }

        private void OnSendLoginMsg()
        {
            MsgLogin msgLogin = (MsgLogin)NetManager.Inst.GetMessage(eNetMessageID.MsgLogin);
            EGame.m_openid = GetUserName();
            msgLogin.login.userName = EGame.m_openid;
            msgLogin.login.passWord = GetPassWord();
            NetRunTime.Inst.SendMessage(msgLogin);
            Debug.Log("发送登陆");
        }

        public override void UpdateUI(float fTime, float fDTime)
        {
            if(m_bStartConn && NetRunTime.Inst.GetState() == NetRunTime.NetState.Connecting)
            {
                m_connCurTime += fDTime;
                if(m_connCurTime > m_connMaxTime)
                {
                    CloseConn();
                    OpenReConnDialog();
                }
            }
        }

        public void OpenReConnDialog()
        {
            // 先采用系统对话框
            Client.Inst().m_uiResInitDialog.OpenPanel(true);
            Client.Inst().m_uiResInitDialog.Open("重连", "退出", "连接登陆服务器失败!", (bOk, a) =>
            {
                if (bOk)
                {
                    Debug.Log("重连登陆服");
                    OnStartLogin();
                }
                else
                {
                    Application.Quit();
                }
            });
        }

        private void CloseConn()
        {
            NetRunTime.Inst.Stop();
            m_bStartConn = false;
        }
        
        /// <summary>
        /// 接受登陆成功
        /// </summary>
        public void OnRecvLoginSuccess()
        {
            //SelectModule moudle = (SelectModule)LayoutMgr.Inst.GetLogicModule(LogicModuleIndex.eLM_PanelCreate);
           
            //if (actor.lActorID > 0)
            //{
            //    Debug.Log("有角色的登陆1：OnRecvLoginSuccess ： 结束登陆成功的消息，有角色，开始下载主界面，开始进入游戏m_bEnterGameNoUI=true");
            //    SetVisible(false);
            //    moudle.SetVisible(false);
            //    moudle.SetActorInfo(ref actor, GetUserName(), GetPassWord());
            //}
            //else
            //{
            //    SetVisible(false);
            //    moudle.SetVisible(true);
            //}
        }

        private void ReadTempInfo()
        {
            m_uiLogin.m_id.text = PlayerPrefs.GetString("username", "");
            m_uiLogin.m_passwords.text = PlayerPrefs.GetString("password", "");
        }

        private void SaveTempInfo()
        {
            PlayerPrefs.SetString("username", m_uiLogin.m_id.text);
            PlayerPrefs.SetString("password", m_uiLogin.m_passwords.text);
        }

        private string GetUserName()
        {
            return m_uiLogin.m_id.text.Trim();
        }

        private string GetPassWord()
        {
            return m_uiLogin.m_passwords.text.Trim();
        }

        public UIPanelLogin m_uiLogin;
        private bool m_bStartConn;
        private float m_connCurTime;
        private const float m_connMaxTime = 3;
    }
}

