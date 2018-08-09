using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;

namespace Roma
{
    public class FspVKeyType
    {
        public const int Move = 1;
        public const int Stop = 2;
    }


    /// <summary>
    /// 为了兼容键盘和轮盘操作，将玩家的操作抽象为【虚拟按键+参数】的【命令】形式：VKey+Arg
    /// </summary>
    [ProtoContract]
    public class FspVKey
    {
        /// <summary>
        /// 键值
        /// </summary>
        [ProtoMember(1)] public int vkey;

        /// <summary>
        /// 参数列表
        /// </summary>
        [ProtoMember(2)] public int[] args;

        /// <summary>
        /// S2C  服务器下发PlayerId
        /// C2S  客户端上报ClientFrameId
        /// </summary>
        [ProtoMember(3)] public uint playerIdOrClientFrameId;

        public uint playerId
        {
            get { return playerIdOrClientFrameId; }
            set { playerIdOrClientFrameId = value; }
        }

        public uint clientFrameId
        {
            get { return playerIdOrClientFrameId; }
            set { playerIdOrClientFrameId = value; }
        }

        public override string ToString()
        {
            return "{vkey:" + vkey + ",arg:" + args[0] + ",playerIdOrClientFrameId:" + playerIdOrClientFrameId + "}";
        }
    }

    [ProtoContract]
    public class FspFrame
    {
        [ProtoMember(1)]
        public int frameId;
        [ProtoMember(2)]
        public List<FspVKey> vkeys = new List<FspVKey>();

        public bool IsEmpty()
        {
            if (vkeys == null || vkeys.Count == 0)
            {
                return true;
            }
            return false;
        }

        public override string ToString()
        {
            string tmp = "";
            if (vkeys != null && vkeys.Count > 0)
            {
                for (int i = 0; i < vkeys.Count - 1; i++)
                {
                    tmp += vkeys[i].ToString() + ",";
                }
                tmp += vkeys[vkeys.Count - 1].ToString();
            }
            return "{frameId:" + frameId + ", vkeys:[" + tmp + "]}";
        }
    }

}
