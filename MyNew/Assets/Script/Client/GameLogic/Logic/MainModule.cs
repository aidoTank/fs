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

        public override void InitData()
        {
            base.InitData();

            FspNetRunTime.Inst = new FspNetRunTime();
            FspNetRunTime.Inst.Init();
            FspNetRunTime.Inst.ConServer(() =>
            {
                Debug.Log("连接帧服务器成功，发送创建房间");
            });
        }

        public void OnClickBtn(GameObject go)
        {
            if(go == m_ui.m_btnCreateRoom)
            {
                int roomId = 0;
                int.TryParse(m_ui.m_roomId.text, out roomId);


                FspMsgCreateRoom msg = (FspMsgCreateRoom)NetManager.Inst.GetMessage(eNetMessageID.FspMsgCreateRoom);
                msg.m_createRoom.roomId = roomId;
                msg.m_createRoom.userName = int.Parse(EGame.m_openid);
                FspNetRunTime.Inst.SendMessage(msg);

            }
            else if(go == m_ui.m_btnJoinRoom)
            {
                int roomId = 0;
                int.TryParse(m_ui.m_roomId.text, out roomId);

                FspMsgJoinRoom msg = (FspMsgJoinRoom)NetManager.Inst.GetMessage(eNetMessageID.FspMsgJoinRoom);
                msg.m_joinRoom.roomId = roomId;
                msg.m_joinRoom.userName = int.Parse(EGame.m_openid);
                FspNetRunTime.Inst.SendMessage(msg);
            }
        }

        public UIPanelMain m_ui;
    }
}

