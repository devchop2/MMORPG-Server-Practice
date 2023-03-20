using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ServerCore
{
    public abstract class PacketSession : Session
    {
        public static readonly int HeaderSize = 2;
        public sealed override int OnRecv(ArraySegment<byte> buffer)
        {
            int processLen = 0;
            while (true)
            {
                if (buffer.Count < HeaderSize) break; //header를 파싱할 수 있는지를 확인.

                int dataSize = BitConverter.ToUInt16(buffer.Array, buffer.Offset);
                if (buffer.Count < dataSize) break; //아직 모든 데이터가 다 전송되지 않았음.

                var validData = new ArraySegment<byte>(buffer.Array, buffer.Offset, dataSize);
                OnRecvPacket(validData); //데이터 처리하기 
                processLen += dataSize;
                buffer = new ArraySegment<byte>(buffer.Array, buffer.Offset + dataSize, buffer.Count - dataSize); //처리되고 남은 데이터만 추출해서 다시 buffer에 넣어줌.

            }

            return processLen;
        }
        public abstract void OnRecvPacket(ArraySegment<byte> buffer);
    }
}
