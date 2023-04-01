using System;
namespace PacketGenerator
{
    public class PacketFormat
    {

        #region Packet Format
        //{0} : class Name {1}: members {2} Serialize {3}Deserialize
        public static string packetFormat =
@" 
 class {0}  
{{
    {1}
    
    public override ArraySegment<byte> Serialize()
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

    public override void Deserialize(ArraySegment<byte> data)
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
@"
    public {0} {1};
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
        public static string deserializeStringFormat =
@"
        ushort {0}Len = (ushort)BitConverter.ToInt16(s.Slice(count, s.Length - count));
        count += sizeof(ushort);
        {0} = Encoding.Unicode.GetString(s.Slice(count, nameLen));
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
        public static string serializeStringFormat =
@"
        ushort {0}Len = (ushort)Encoding.Unicode.GetBytes({0}, 0, {0}.Length, segment.Array, segment.Offset + count + sizeof(ushort)); 
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), {0}Len);  
        count += sizeof(ushort);
        count += {0}Len;
";
        #endregion
    }
}

