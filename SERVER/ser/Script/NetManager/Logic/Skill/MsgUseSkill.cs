//using System;
//using ProtoBuf;
//using UnityEngine;

//    public class MsgUseSkill : NetMessage
//    {
//        public MsgUseSkill()
//            : base(eNetMessageID.MsgUseSkill)
//        {

//        }
//        public static NetMessage CreateMessage()
//        {
//            return new MsgUseSkill();
//        }

//        // 发送给其他人发送移动
//        public override void ToByte(ref LusuoStream ls)
//        {
//            eno = 0;
//            SetByte<GC_UseSkill>(useSkill, ref ls);
//        }

//        // 接受客户端移动消息
//        public override void OnRecv(ref Conn conn)
//        {
//            if (eno == 0)
//            {
//                CG_UseSkill userSkill = GetData<CG_UseSkill>(structBytes);
//                Lobby map = LobbyManager.Inst.GetMap(conn.player.publicData.mapId);

//                this.useSkill.attackUid = userSkill.attackUid;
//                this.useSkill.skillId = userSkill.skillId;
//                this.useSkill.targetUid = userSkill.targetUid;
//                map.BroadcastByPlayer(userSkill.attackUid, this);
         
//            }
//        }

//        public GC_UseSkill useSkill;
//    }

