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
                Debug.Log("加入房间");
            }
        }

        public UIPanelMain m_ui;
    }
}

