
using UnityEngine;

namespace Roma
{
    /// <summary>
    /// 消息基类 |消息长度(不包含自己)|消息编号|ENO|消息具体内容|
    /// </summary>
    public class NetMessage
    {
        // 发送所需参数
        public ushort msgID;
        public int msgMaxLen;    // 当前消息总长度

        // 接受所需参数
        public int eno;
        public byte[] structBytes;

        //public bool bFspMsg;
        //public bool GetFspMsg()
        //{
        //    return bFspMsg;
        //}

        public NetMessage(ushort uID)
        {
            msgID = uID;
        }

        public NetMessage(eNetMessageID uID)
        {
            msgID = (ushort)uID;
        }

        #region 发送
        /// <summary>
        /// 消息发送时的内部方法，将逻辑结构体数据存放到ls中
        /// 子类无需重载
        /// </summary>
        public void SetByte<T>(T t, ref LusuoStream ls)
        {
            ls.WriteInt(0);             // 预留总字节数
            ls.WriteUShort(msgID);    // 写消息编号
            ls.WriteInt(eno);           // 写eno
            byte[] bytes = ProtobufHelper.Serialize<T>(t);
            ls.Write(ref bytes);        // 写具体结构体
            ls.Seek(0);
            // 内容字节数
            int contentLen = StringHelper.s_ShortSize + StringHelper.s_IntSize + bytes.Length;
            ls.WriteInt(contentLen);   // 再次写内容长度
            msgMaxLen = StringHelper.s_IntSize + contentLen; // 长度字节数 + 内容字节数
        }

        /// <summary>
        /// 消息发送时的外部方法，将内部结构体数据存放到ls中
        /// 子类需重写无需重载，并调用 SetByte<T> 
        /// </summary>
        public virtual void ToByte(ref LusuoStream ls)
        {
            Debug.Log("发送基类，把数据写入到Stream");
        }

        #endregion

        #region 接受
        /// <summary>
        /// 接受消息时的外部方法,读取流对象取数据
        /// 子类无需重写
        /// </summary>
        public void OnRecv(int contentLen, ref LusuoStream ls)
        {
            eno = ls.ReadInt();
            structBytes = new byte[contentLen - StringHelper.s_ShortSize - StringHelper.s_IntSize];
            ls.Read(ref structBytes);
            Debug.Log("消息:" + msgID + " eno:" + eno);
        }

        public virtual T GetData<T>(byte[] bytes)
        {
            return ProtobufHelper.DeSerialize<T>(bytes);
        }

        /// <summary>
        /// 接受消息时的内部方法，通过字节流获取结构体
        /// 子类需重载并调用GetData<T>
        /// </summary>
        public virtual void OnRecv()
        {

        }
        #endregion
    }
}
