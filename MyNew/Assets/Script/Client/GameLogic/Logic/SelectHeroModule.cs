using UnityEngine;

namespace Roma
{
    public partial class SelectHeroModule : Widget
    {
        public SelectHeroModule()
            : base(LogicModuleIndex.eLM_PanelSelectHero)
        {
        }

        public static Widget CreateLogicModule()
        {
            return new SelectHeroModule();
        }

        public override void Init()
        {
            m_ui = GetUI<UIPanelSelectHero>();
            UIEventListener.Get(m_ui.m_hero1).onClick = OnClickBtn;
            UIEventListener.Get(m_ui.m_hero2).onClick = OnClickBtn;
            UIEventListener.Get(m_ui.m_btnReady).onClick = OnClickBtn;
        }

        public void OnClickBtn(GameObject go)
        {
            if(go == m_ui.m_hero1)
            {

            }
            else if(go == m_ui.m_hero2)
            {

            }
            else if(go == m_ui.m_btnReady)
            {
                FspMsgFrame msg = (FspMsgFrame)NetManager.Inst.GetMessage(eNetMessageID.FspMsgFrame);
                FspVKey fsp = new FspVKey();
                fsp.vkey = FspVKeyType.READY;
                msg.m_frameData.vkeys.Add(fsp);
                FspNetRunTime.Inst.SendMessage(msg);
            }
        }

        public UIPanelSelectHero m_ui;
    }
}

