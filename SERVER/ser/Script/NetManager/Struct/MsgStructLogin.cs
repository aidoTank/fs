using ProtoBuf;

//[ProtoContract]
//public struct GC_PlayerPublicData
//{
//    [ProtoMember(1)]
//    public int userName;
//    [ProtoMember(2)]
//    public string name;
//    [ProtoMember(3)]
//    public int occ;
//    [ProtoMember(4)]
//    public int resId;
//    [ProtoMember(5)]
//    public int mapId;
//    [ProtoMember(6)]
//    public int x;
//    [ProtoMember(7)]
//    public int y;
//    [ProtoMember(8)]
//    public int dir;
//}

[ProtoContract]
public struct CG_Login
{
    [ProtoMember(1)]
    public string name;
    [ProtoMember(2)]
    public string passWord;
}