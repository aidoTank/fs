using UnityEngine;

namespace Roma
{
    public partial class CreateRoleModule : Widget
    {
        public CreateRoleModule()
            : base(LogicModuleIndex.eLM_PanelCreate)
        {
        }

        public static Widget CreateLogicModule()
        {
            return new CreateRoleModule();
        }

        public override void Init()
        {
            m_uiLogin = GetUI<UIPanelCreateRole>();
            UIEventListener.Get(m_uiLogin.m_btnOcc1).onClick = OnClickBtn;
            UIEventListener.Get(m_uiLogin.m_btnOcc2).onClick = OnClickBtn;
            UIEventListener.Get(m_uiLogin.m_btnOk).onClick = OnClickBtn;
        }

        public void OnClickBtn(GameObject go)
        {
            if(go == m_uiLogin.m_btnOcc1)
            {
                m_occ = 1;
            }
            else if(go == m_uiLogin.m_btnOcc2)
            {
                m_occ = 2;
            }
            else if(go == m_uiLogin.m_btnOk)
            {
                MsgCreateRole msg = (MsgCreateRole)NetManager.Inst.GetMessage(eNetMessageID.MsgCreateRole);
                msg.createRole.userName = EGame.m_openid;
                msg.createRole.name = m_uiLogin.m_name.text;
                msg.createRole.occ = m_occ;
                NetRunTime.Inst.SendMessage(msg);
            }
        }



        private int m_occ = 1;
        public UIPanelCreateRole m_uiLogin;
    }
}

