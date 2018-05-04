using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ProtoBuf;

public class ProtobufHelper
{
    public static byte[] Serialize<T>(T t)
    {
        using (MemoryStream ms = new MemoryStream())
        {
            Serializer.Serialize<T>(ms, t);
            return ms.ToArray();
        }
    }

    public static T DeSerialize<T>(byte[] content)
    {
        using (MemoryStream ms = new MemoryStream(content))
        {
            T t = Serializer.Deserialize<T>(ms);
            return t;
        }
    }
}