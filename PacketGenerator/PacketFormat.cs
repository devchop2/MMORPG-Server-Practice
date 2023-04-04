using System;
namespace PacketGenerator
{
    public class PacketFormat
    {


        #region Manager
        //{0}packetName
        public static string managerRegisterFormat =
@"
        recvHandlers.Add((ushort)PacketID.{0}, MakePacket<{0}>);
        handler.Add((ushort)PacketID.{0}, PacketHandler.{0}Handler);
";
        //{0}register
        public static string managerFileFormat =
@"
using System;
using ServerCore;


public class PacketManager
{{

    Dictionary<ushort, Action<Session, ArraySegment<byte>>> recvHandlers = new Dictionary<ushort, Action<Session, ArraySegment<byte>>>();
    Dictionary<ushort, Action<Session, IPacket>> handler = new Dictionary<ushort, Action<Session, IPacket>>();

    public void Register()
    {{
{0}
    }}

    static PacketManager _instance = null;
    public static PacketManager Instance
    {{
        get
        {{
            if (_instance == null) _instance = new PacketManager();
            return _instance;
        }}
    }}

    public void OnRecvPacket(Session session, ArraySegment<byte> buffer)
    {{
        string recvData = BitConverter.ToString(buffer.Array, buffer.Offset, buffer.Count);

        
        ushort count = 0;
        ushort size = BitConverter.ToUInt16(buffer.Array, buffer.Offset);
        count += sizeof(ushort);
        ushort packetId = BitConverter.ToUInt16(buffer.Array, buffer.Offset + count);
        count += sizeof(ushort);

        if (recvHandlers.TryGetValue(packetId, out var act))
        {{
            act.Invoke(session, buffer);
        }}
    }}

    void MakePacket<T>(Session session, ArraySegment<byte> buffer) where T : IPacket, new()
    {{
        T p = new T();
        p.Deserialize(buffer);

        if (handler.TryGetValue(p.Protocol, out var act))
        {{
            act?.Invoke(session, p);
        }}

    }}
}}
";
        #endregion

        #region File Format
        //{0}PacketID format {1}packet Body
        public static string fileFormat =
@"
using ServerCore;
using System;
using System.Net;
using System.Text;

public enum PacketID
{{
    {0}
}}

public interface IPacket
{{
    ushort Protocol {{ get; }}
    void Deserialize(ArraySegment<byte> data);
    ArraySegment<byte> Serialize();
}}

{1}
";

        //{0} packetName {1}packet Number 
        public static string PacketIDForamt =
@"{0} = {1},";
        #endregion


        #region Packet Format
        //{0} : class Name {1}: members {2} Serialize {3}Deserialize
        public static string packetFormat =
@"

public class {0} : IPacket
{{
    {1}

    public ushort Protocol => (ushort)PacketID.{0};

    public ArraySegment<byte> Serialize()
    {{

        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);

        ushort count = 0;
        bool success = true;

        count += sizeof(ushort); //size

        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID. {0}); ;
        count += sizeof(ushort);
        {2}
        success &= BitConverter.TryWriteBytes(s, count);
 
        if (!success) return null;
        return SendBufferHelper.Close(count);

    }}

    public void Deserialize(ArraySegment<byte> data)
    {{

        ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(data.Array, data.Offset, data.Count);
        ushort count = 0;

        count += sizeof(ushort);
        count += sizeof(ushort);
        {3}
    }}
}}
";

        #endregion

        #region Member Format
        //{0} memberType {1}memberName 
        public static string memberFormat =
@"public {0} {1};";

        //{0}structName(Big) {1}variableName(Small)
        public static string listMemberFormat =
@"public List<{0}> {1}s = new List<{0}>();";

        //{0}structName(Big) {1}variableName(Small) {2}Variables {3}Seriaize {4}Deserialize
        public static string structFormat =
@"
    public struct {0}
    {{
        {2}
            
        public bool Serialize(Span<byte> s, ref ushort count)
        {{
            bool success = true;
            {3}
            return success;
        }}

        public void Deserialize(ReadOnlySpan<byte> s, ref ushort count)
        {{
            {4}
        }}
    }}
";
        #endregion

        #region Deserialize Format

        //{0} memberName {1} convertFormat(ToInt16..) {2} mebemrType
        public static string deserializeFormat =
@"
        this.{0} = BitConverter.{1}(s.Slice(count, s.Length - count));  
        count += sizeof({2});
";

        //{0} memberName  
        public static string deserializeByteFormat =
@"
        this.{0} = (byte)data.Array[data.Offset + count];
        count += sizeof(byte);
";

        //{0} memberName 
        public static string deserializeStringFormat =
@"
        ushort {0}Len = (ushort)BitConverter.ToInt16(s.Slice(count, s.Length - count));
        count += sizeof(ushort);
        {0} = Encoding.Unicode.GetString(s.Slice(count, nameLen));
        count += {0}Len;
";
        //{0}variableName, {1}structName
        public static string deserializelistFormat =
@"
        this.{0}s.Clear();
        ushort {0}Len = (ushort)BitConverter.ToInt16(s.Slice(count, s.Length - count));
        count += sizeof(ushort);

        for (int i = 0; i < {0}Len; i++)
        {{
            {1} {0}Info = new {1}();
            {0}Info.Deserialize(s, ref count);
            this.{0}s.Add({0}Info);
        }}
";
        #endregion

        #region Serialize Format

        //{0} memberName {1}memberType
        public static string serializeFormat =
@"
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), {0});
        count += sizeof({1});
";

        //{0}memberName
        public static string serializeByteFormat =
@"
        segment[segment.Offset + count] = (byte)this.{0};
        count += sizeof(byte);
";
        //{0}memberName 
        public static string serializeStringFormat =
@"
        ushort {0}Len = (ushort)Encoding.Unicode.GetBytes({0}, 0, {0}.Length, segment.Array, segment.Offset + count + sizeof(ushort)); 
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), {0}Len);  
        count += sizeof(ushort);
        count += {0}Len;
";

        //{0}variableName 
        public static string serializeListFormat =
@"
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)this.{0}s.Count);
        count += sizeof(ushort);
        foreach (var item in this.{0}s) success &= item.Serialize(s, ref count);
";
        #endregion
    }
}

