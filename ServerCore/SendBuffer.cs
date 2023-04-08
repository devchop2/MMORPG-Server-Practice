using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerCore
{

    public class SendBufferHelper
    {
        public static ThreadLocal<SendBuffer> currentBuffer = new ThreadLocal<SendBuffer>(() => { return null; });
        public static int ChunkSize { get; set; } = 65535 * 100;

        public static ArraySegment<byte> Open(int reserveSize)
        {
            if (currentBuffer.Value == null)
            {
                currentBuffer.Value = new SendBuffer(ChunkSize);
            }

            if (currentBuffer.Value.FreeSize < reserveSize)
            {
                currentBuffer.Value = new SendBuffer(ChunkSize);
            }
            return currentBuffer.Value.Open(reserveSize);
        }

        public static ArraySegment<byte> Close(int usedSize)
        {
            return currentBuffer.Value.Close(usedSize);
        }
    }

    public class SendBuffer
    {
        byte[] _buffers;
        int _usedSize = 0;  //이미 씌여진 버퍼사이즈

        public SendBuffer(int chunkSize)
        {
            _buffers = new byte[chunkSize];
        }
        public int FreeSize { get { return _buffers.Length - _usedSize; } }

        public ArraySegment<byte> Open(int reserveSize)
        {
            if (reserveSize > FreeSize) return null;
            return new ArraySegment<byte>(_buffers, _usedSize, reserveSize);
        }
        public ArraySegment<byte> Close(int usedSize)
        {
            ArraySegment<byte> segment = new ArraySegment<byte>(_buffers, _usedSize, usedSize);
            _usedSize += usedSize;

            return segment;

        }
    }
}
