
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
            SetByte<CG_Login>(login, ref ls);
        }

        public override void OnRecv()
        {
            if (eno == 0)
            {
                Debug.Log("无角色，跳转创建界面");
                LoginModule login = (LoginModule)LayoutMgr.Inst.GetLogicModule(LogicModuleIndex.eLM_PanelLogin);
                login.SetVisible(false);
                //CreateRoleModule cRole = LayoutMgr.Inst.GetLogicModule<CreateRoleModule>(LayoutName.S_CreateRole);
                //cRole.SetVisible(true);
            }
            else if (eno == 1)
            {
                //LoginModule login = LayoutMgr.Inst.GetLogicModule<LoginModule>(LayoutName.S_LoginUI);
                //login.SetVisible(false);
                Debug.Log("有角色，直接进入大厅");
                GC_PlayerPublicData data = GetData<GC_PlayerPublicData>(structBytes);
                Debug.Log(data.name);
                Debug.Log(data.occ);
                Debug.Log(data.x);
                Debug.Log(data.y);
                Debug.Log(data.dir);

                InitMaster(data);
            }
            else if(eno == -1)
            {
                Debug.Log("服务器数据异常");
            }
        }

        public static void InitMaster(GC_PlayerPublicData data)
        {
            Debug.Log("创建角色id:"+ data.userName);
        }

        public CG_Login login;
    }



}
