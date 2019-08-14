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
            m_uiCreate = GetUI<UIPanelCreateRole>();
            UIEventListener.Get(m_uiCreate.m_btnOk).onClick = OnClickBtn;

            for(int i = 0; i < 6; i ++)
            {
                m_uiCreate.SetIocn(i, 2001 + i);
            }
            m_uiCreate.m_tabIcon.RegisterClickEvent(OnSelectIcon);
        }

        private void OnSelectIcon(int index)
        {
            m_iconIndex = index;
        }

        public void OnClickBtn(GameObject go)
        {
            if(go == m_uiCreate.m_btnOk)
            {
                MsgCreateRole msg = (MsgCreateRole)NetManager.Inst.GetMessage(eNetMessageID.MsgCreateRole);
                msg.createRole.userName = EGame.m_openid;
                msg.createRole.name = m_uiCreate.m_name.text;
                msg.createRole.gender = m_uiCreate.m_gender0.isOn ? 0 : 1;
                msg.createRole.icon = m_iconIndex;
                NetRunTime.Inst.SendMessage(msg);
            }
        }


        public UIPanelCreateRole m_uiCreate;
        private int m_iconIndex;
    }
}

