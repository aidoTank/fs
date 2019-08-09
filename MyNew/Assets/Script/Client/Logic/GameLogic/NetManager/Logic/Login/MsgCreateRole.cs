
using UnityEngine;
using  ProtoBuf;

namespace Roma
{
    public class MsgCreateRole : NetMessage
    {
        public MsgCreateRole()
            : base(eNetMessageID.MsgCreateRole)
        {

        }

        public static NetMessage CreateMessage()
        {
            return new MsgCreateRole();
        }

        public override void ToByte(ref LusuoStream ls)
        {
            SetByte<CS_CreateRole>(createRole, ref ls);
        }

        public override void OnRecv()
        {
            if (eno == 0)
            {
                CreateRoleModule cRole = (CreateRoleModule)LayoutMgr.Inst.GetLogicModule(LogicModuleIndex.eLM_PanelCreate);
                cRole.SetVisible(false);

                Debug.Log("创建角色成功，直接进入大厅");
                SC_PlayerPublicData data = GetData<SC_PlayerPublicData>(structBytes);
                Debug.Log(data.name);
                Debug.Log(data.occ);
                Debug.Log(data.x);
                Debug.Log(data.y);
                Debug.Log(data.dir);

       

                MsgLogin.EnterMainUI(data);
            }
            else if (eno == -1)
            {
                Debug.Log("创建角色失败");

            }
        }

        public CS_CreateRole createRole = new CS_CreateRole();
    }

    /// <summary>
    /// 客户端的生物属性用Dictionary<CreatureProp, int>存储的好处
    /// 1.支持服务器通过键值对的方式，单独更新单个属性
    /// 2.支持客户端单独更新属性值时，单独调用显示前端显示
    /// 3.支持其他模块通过key的方法获取属性值
    /// </summary>
   
}
