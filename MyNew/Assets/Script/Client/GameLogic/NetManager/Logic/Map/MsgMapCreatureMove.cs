using System;
using ProtoBuf;
using UnityEngine;
namespace Roma
{

    public class MsgMapCreatureMove : NetMessage
    {
        public MsgMapCreatureMove()
            : base(eNetMessageID.MsgMapCreatureMove)
        {

        }
        public static NetMessage CreateMessage()
        {
            return new MsgMapCreatureMove();
        }

        // 发送给其他人发送移动
        public override void ToByte(ref LusuoStream ls)
        {
            eno = 0;
            SetByte<GC_MapCreatureMove>(moveInfo, ref ls);
        }

        // 接受客户端移动消息
        public override void OnRecv()
        {
            if (eno == 0)
            {
                GC_MapCreatureMove moveInfo = GetData<GC_MapCreatureMove>(structBytes);
                //CPlayer player = CPlayerMgr.Get(moveInfo.uid);
                //if(player == null)
                //{
                //    return;
                //}
                //player.GoTo(moveInfo.x, moveInfo.y, eControlMode.eCM_auto, moveInfo.dir);
            }
        }

        public static void SendMoveInfo(Vector2 pos)
        {
            //Vector3 playerPos = CPlayerMgr.GetMaster().GetPos();
            //MsgMapCreatureMove move = (MsgMapCreatureMove)NetManager.Inst.GetMessage(eNetMessageID.MsgMapCreatureMove);
            //move.moveInfo.uid = CPlayerMgr.GetMaster().publicData.userName;
            //move.moveInfo.x = (int)playerPos.x;
            //move.moveInfo.y = (int)playerPos.z;
        
            //NetRunTime.Inst.SendMessge(move);
        }

        public GC_MapCreatureMove moveInfo;
    }
}
