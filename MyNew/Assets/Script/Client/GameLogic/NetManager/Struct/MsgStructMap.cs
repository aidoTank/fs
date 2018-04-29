using ProtoBuf;

[ProtoContract]
public struct GC_MapCreatureMove
{
    [ProtoMember(1)]
    public long uid;
    [ProtoMember(2)]
    public int x;
    [ProtoMember(3)]
    public int y;
    [ProtoMember(4)]
    public int dir;
}