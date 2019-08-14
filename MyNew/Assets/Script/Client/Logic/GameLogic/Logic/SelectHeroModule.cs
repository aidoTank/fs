using UnityEngine;
using System.Collections.Generic;

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
            UIEventListener.Get(m_ui.m_btnReady).onClick = OnClickBtn;
        }

        public override void InitData()
        {
            base.InitData();
            UpdateHeroList();
            UpdateJoinInfo();
        }

        public void UpdateHeroList()
        {
            PlayerCsv playerCsv = CsvManager.Inst.GetCsv<PlayerCsv>((int)eAllCSV.eAC_Player);
            m_listHero = new List<PlayerCsvData>(playerCsv.m_dicData.Values);
            m_ui.m_list.Init(m_listHero.Count, (item, index) => 
            {
                UIItem.SetRawImage(item, "icon", m_listHero[index].Icon);

                UIEventListener lis = UIEventListener.Get(item.gameObject);
                lis.parameter = index;
                lis.onClick = (go) => {
                    m_heroIndex = m_listHero[index].Id;
                    m_ui.m_list.SetChoice(index);
                    m_ui.SetModel(m_listHero[index].ShowModelResId);
                };
            });
        }



        public void OnClickBtn(GameObject go)
        {
            if(go == m_ui.m_btnReady)
            {
                FspMsgReady msg = (FspMsgReady)NetManager.Inst.GetMessage(eNetMessageID.FspMsgReady);
                msg.m_heroIndex = m_heroIndex;
                FspNetRunTime.Inst.SendMessage(msg);
            }
        }


        private List<PlayerCsvData> m_listHero;

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
        private int m_heroIndex = 1;

        // 此处临时写的
        public int[] m_joinInfo;
        public void OnRecvJoinInfo(int[] joinInfo)
        {
            m_joinInfo = joinInfo;
        }
    }
}

