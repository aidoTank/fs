//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// Option: missing-value detection (*Specified/ShouldSerialize*/Reset*) enabled

// Generated from: Roma.proto
namespace Roma
{
    [global::System.Serializable, global::ProtoBuf.ProtoContract(Name = @"CG_Login")]
    public partial class CG_Login : global::ProtoBuf.IExtensible
    {
        public CG_Login() { }

        private string _userName;
        [global::ProtoBuf.ProtoMember(1, IsRequired = true, Name = @"userName", DataFormat = global::ProtoBuf.DataFormat.Default)]
        public string userName
        {
            get { return _userName; }
            set { _userName = value; }
        }
        private string _passWord;
        [global::ProtoBuf.ProtoMember(2, IsRequired = true, Name = @"passWord", DataFormat = global::ProtoBuf.DataFormat.Default)]
        public string passWord
        {
            get { return _passWord; }
            set { _passWord = value; }
        }
        private global::ProtoBuf.IExtension extensionObject;
        global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
        { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
    }

    [global::System.Serializable, global::ProtoBuf.ProtoContract(Name = @"CG_CreateRole")]
    public partial class CG_CreateRole : global::ProtoBuf.IExtensible
    {
        public CG_CreateRole() { }

        private string _userName;
        [global::ProtoBuf.ProtoMember(1, IsRequired = true, Name = @"userName", DataFormat = global::ProtoBuf.DataFormat.Default)]
        public string userName
        {
            get { return _userName; }
            set { _userName = value; }
        }
        private string _name;
        [global::ProtoBuf.ProtoMember(2, IsRequired = true, Name = @"name", DataFormat = global::ProtoBuf.DataFormat.Default)]
        public string name
        {
            get { return _name; }
            set { _name = value; }
        }
        private int _occ;
        [global::ProtoBuf.ProtoMember(3, IsRequired = true, Name = @"occ", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
        public int occ
        {
            get { return _occ; }
            set { _occ = value; }
        }
        private global::ProtoBuf.IExtension extensionObject;
        global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
        { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
    }

    [global::System.Serializable, global::ProtoBuf.ProtoContract(Name = @"GC_PlayerPublicData")]
    public partial class GC_PlayerPublicData : global::ProtoBuf.IExtensible
    {
        public GC_PlayerPublicData() { }

        private long _userName;
        [global::ProtoBuf.ProtoMember(1, IsRequired = true, Name = @"userName", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
        public long userName
        {
            get { return _userName; }
            set { _userName = value; }
        }
        private string _name;
        [global::ProtoBuf.ProtoMember(2, IsRequired = true, Name = @"name", DataFormat = global::ProtoBuf.DataFormat.Default)]
        public string name
        {
            get { return _name; }
            set { _name = value; }
        }
        private int _occ;
        [global::ProtoBuf.ProtoMember(3, IsRequired = true, Name = @"occ", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
        public int occ
        {
            get { return _occ; }
            set { _occ = value; }
        }
        private int _resId;
        [global::ProtoBuf.ProtoMember(4, IsRequired = true, Name = @"resId", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
        public int resId
        {
            get { return _resId; }
            set { _resId = value; }
        }
        private int _mapId;
        [global::ProtoBuf.ProtoMember(5, IsRequired = true, Name = @"mapId", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
        public int mapId
        {
            get { return _mapId; }
            set { _mapId = value; }
        }
        private int _x;
        [global::ProtoBuf.ProtoMember(6, IsRequired = true, Name = @"x", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
        public int x
        {
            get { return _x; }
            set { _x = value; }
        }
        private int _y;
        [global::ProtoBuf.ProtoMember(7, IsRequired = true, Name = @"y", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
        public int y
        {
            get { return _y; }
            set { _y = value; }
        }
        private int _dir;
        [global::ProtoBuf.ProtoMember(8, IsRequired = true, Name = @"dir", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
        public int dir
        {
            get { return _dir; }
            set { _dir = value; }
        }
        private global::ProtoBuf.IExtension extensionObject;
        global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
        { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
    }

    [global::System.Serializable, global::ProtoBuf.ProtoContract(Name = @"GC_MatchResult")]
    public partial class GC_MatchResult : global::ProtoBuf.IExtensible
    {
        public GC_MatchResult() { }

        private int __matchType;
        [global::ProtoBuf.ProtoMember(1, IsRequired = true, Name = @"_matchType", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
        public int _matchType
        {
            get { return __matchType; }
            set { __matchType = value; }
        }
        private string _serverIp;
        [global::ProtoBuf.ProtoMember(2, IsRequired = true, Name = @"serverIp", DataFormat = global::ProtoBuf.DataFormat.Default)]
        public string serverIp
        {
            get { return _serverIp; }
            set { _serverIp = value; }
        }
        private int _serverPort;
        [global::ProtoBuf.ProtoMember(3, IsRequired = true, Name = @"serverPort", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
        public int serverPort
        {
            get { return _serverPort; }
            set { _serverPort = value; }
        }
        private int _roomId;
        [global::ProtoBuf.ProtoMember(4, IsRequired = true, Name = @"roomId", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
        public int roomId
        {
            get { return _roomId; }
            set { _roomId = value; }
        }
        private global::ProtoBuf.IExtension extensionObject;
        global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
        { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
    }

    [global::System.Serializable, global::ProtoBuf.ProtoContract(Name = @"CG_CreateRoom")]
    public partial class CG_CreateRoom : global::ProtoBuf.IExtensible
    {
        public CG_CreateRoom() { }

        private int _userName;
        [global::ProtoBuf.ProtoMember(1, IsRequired = true, Name = @"userName", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
        public int userName
        {
            get { return _userName; }
            set { _userName = value; }
        }
        private int _roomId;
        [global::ProtoBuf.ProtoMember(2, IsRequired = true, Name = @"roomId", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
        public int roomId
        {
            get { return _roomId; }
            set { _roomId = value; }
        }
        private global::ProtoBuf.IExtension extensionObject;
        global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
        { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
    }

}