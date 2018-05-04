using ProtoBuf;

[ProtoContract]
public struct CG_CreateRole
{
    [ProtoMember(1)]
    public string userName;
    [ProtoMember(2)]
    public string name;
    [ProtoMember(3)]
    public int occ;
}