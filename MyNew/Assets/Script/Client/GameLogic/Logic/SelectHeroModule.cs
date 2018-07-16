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

        public override void InitData()
        {
            base.InitData();
            UpdateJoinInfo();
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

        public void UpdateJoinInfo()
        {
            string info = "房间号：" + m_joinInfo[0] + "\n";
            for(int i = 1; i < m_joinInfo.Length; i ++)
            {
                info += m_joinInfo[i] + "|";
            }
            m_ui.m_textRoleJoin.text = info;
        }

        public UIPanelSelectHero m_ui;

        // 此处临时写的
        public int[] m_joinInfo;
        public void OnRecvJoinInfo(int[] joinInfo)
        {
            m_joinInfo = joinInfo;
        }
    }
}

