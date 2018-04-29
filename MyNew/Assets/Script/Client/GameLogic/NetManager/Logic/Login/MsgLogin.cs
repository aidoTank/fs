
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


            //MsgLogin test = (MsgLogin)NetManager.Inst.GetMessage(eNetMessageID.MsgLogin);
            //test.login.name = "2";
            //test.login.passWord = "2";
            //NetRunTime.Inst.SendMessge(test);
        }

        public override void OnRecv()
        {
            if (eno == 0)
            {
                Debug.Log("无角色，跳转创建界面");
                //LoginModule login = LayoutMgr.Inst.GetLogicModule<LoginModule>(LayoutName.S_LoginUI);
                //login.SetVisible(false);
                //CreateRoleModule cRole = LayoutMgr.Inst.GetLogicModule<CreateRoleModule>(LayoutName.S_CreateRole);
                //cRole.SetVisible(true);
            }
            else if (eno == 1)
            {
                //LoginModule login = LayoutMgr.Inst.GetLogicModule<LoginModule>(LayoutName.S_LoginUI);
                //login.SetVisible(false);
                Debug.Log("有角色，直接进入游戏");
                GC_PlayerPublicData data = GetData<GC_PlayerPublicData>(structBytes);
                Debug.Log(data.name);
                Debug.Log(data.occ);
                Debug.Log(data.x);
                Debug.Log(data.y);
                Debug.Log(data.dir);

                InitMaster(data);
            }
        }

        public static void InitMaster(GC_PlayerPublicData data)
        {
            Debug.Log("创建角色id:"+ data.userName);
            //CPlayer player = CPlayerMgr.CreateMaster(data.userName);
            //player.publicData = data;
            //LogicSystem.Inst.LoadMap(data.mapId);
            //LogicSystem.Inst.Enter(player, data.x, data.y, data.dir);
        }

        public CG_Login login;
    }



}
