using ProtoBuf;

[ProtoContract]
public struct CG_UseSkill
{
    [ProtoMember(1)]
    public long attackUid;
    [ProtoMember(2)]
    public int skillId;
    [ProtoMember(3)]
    public long targetUid;
}

[ProtoContract]
public struct GC_UseSkill
{
    [ProtoMember(1)]
    public long attackUid;
    [ProtoMember(2)]
    public int skillId;
    [ProtoMember(3)]
    public long targetUid;
}

