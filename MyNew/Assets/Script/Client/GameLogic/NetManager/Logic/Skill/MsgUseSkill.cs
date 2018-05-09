using System;
using ProtoBuf;
using UnityEngine;
namespace Roma
{

    public class MsgUseSkill : NetMessage
    {
        public MsgUseSkill()
            : base(eNetMessageID.MsgUseSkill)
        {

        }
        public static NetMessage CreateMessage()
        {
            return new MsgUseSkill();
        }

        // 发送给其他人发送移动
        public override void ToByte(ref LusuoStream ls)
        {
            eno = 0;
            //SetByte<CG_UseSkill>(useSkill, ref ls);
        }

        // 接受客户端移动消息
        public override void OnRecv()
        {
            if (eno == 0)
            {
                //GC_UseSkill userSkill = GetData<GC_UseSkill>(structBytes);
                //CPlayer player = CPlayerMgr.Get(userSkill.attackUid);
                //if (player == null)
                //{
                //    return;
                //}
                //player.m_skillCastInfo.m_uSkillID = (uint)userSkill.skillId;
                //player.m_skillCastInfo.m_targetID = userSkill.targetUid;
                //player.PushCommand(StateID.AttackState);
            }
        }

        public static void SendUseSkill(long atkUid, int skillId, long targetUid)
        {
            //Vector3 playerPos = CPlayerMgr.GetMaster().GetPos();
            //MsgUseSkill msg = (MsgUseSkill)NetManager.Inst.GetMessage(eNetMessageID.MsgUseSkill);
            //msg.useSkill.attackUid = atkUid;
            //msg.useSkill.skillId = skillId;
            //msg.useSkill.targetUid = targetUid;
            //NetRunTime.Inst.SendMessge(msg);
        }

        //public CG_UseSkill useSkill;
    }
}
