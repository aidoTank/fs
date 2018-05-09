using UnityEngine;

namespace Roma
{
    public partial class MainModule : Widget
    {
        public MainModule()
            : base(LogicModuleIndex.eLM_PanelMain)
        {
        }

        public static Widget CreateLogicModule()
        {
            return new MainModule();
        }

        public override void Init()
        {
            m_ui = GetUI<UIPanelMain>();
            UIEventListener.Get(m_ui.m_btnCreateRoom).onClick = OnClickBtn;
            UIEventListener.Get(m_ui.m_btnJoinRoom).onClick = OnClickBtn;
        }

        public void OnClickBtn(GameObject go)
        {
            if(go == m_ui.m_btnCreateRoom)
            {
                Debug.Log("创建房间");
            }
            else if(go == m_ui.m_btnJoinRoom)
            {
                MsgStartMatch msg = (MsgStartMatch)NetManager.Inst.GetMessage(eNetMessageID.MsgStartMatch);
                msg.m_sendMatchType = 1;
                NetRunTime.Inst.SendMessage(msg);
            }
        }

        public UIPanelMain m_ui;
    }
}

