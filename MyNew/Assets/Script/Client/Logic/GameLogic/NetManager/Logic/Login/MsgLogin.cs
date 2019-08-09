
using UnityEngine;
using  ProtoBuf;

namespace Roma
{
    public class MsgLogin : NetMessage
    {
        public MsgLogin()
            : base(eNetMessageID.MsgLogin)
        {
        
        }
        public static NetMessage CreateMessage()
        {
           return new MsgLogin();
        }

        public override void ToByte(ref LusuoStream ls)
        {
            SetByte<CS_Login>(login, ref ls);
        }

        public override void OnRecv()
        {
            if (eno == 0)
            {
                Debug.Log("无角色，跳转创建界面");
                LoginModule login = (LoginModule)LayoutMgr.Inst.GetLogicModule(LogicModuleIndex.eLM_PanelLogin);
                login.SetVisible(false);
                CreateRoleModule cRole = (CreateRoleModule)LayoutMgr.Inst.GetLogicModule(LogicModuleIndex.eLM_PanelCreate);
                cRole.SetVisible(true);
            }
            else if (eno == 1)
            {
                LoginModule login = (LoginModule)LayoutMgr.Inst.GetLogicModule(LogicModuleIndex.eLM_PanelLogin);
                login.SetVisible(false);
                Debug.Log("有角色，直接进入大厅");
                SC_PlayerPublicData data = GetData<SC_PlayerPublicData>(structBytes);
                Debug.Log(data.name);
                Debug.Log(data.occ);
                Debug.Log(data.x);
                Debug.Log(data.y);
                Debug.Log(data.dir);

                EnterMainUI(data);
            }
            else if(eno == -1)
            {
                Debug.Log("服务器数据异常");
            }
        }

        public static void EnterMainUI(SC_PlayerPublicData data)
        {
            Debug.Log("进入主界面，附带玩家信息 :"+ data.userName);
            MainModule main = (MainModule)LayoutMgr.Inst.GetLogicModule(LogicModuleIndex.eLM_PanelMain);
            main.SetVisible(true);
        }

        public CS_Login login = new CS_Login();
    }



}
