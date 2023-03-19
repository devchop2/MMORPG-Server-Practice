using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ServerCore
{
    public class RecvBuffer
    {
        ArraySegment<byte> _buffer;

        int _readIndex; //조립이 완료되어 read처리 할때마다 read ++
        int _writeIndex; //수신이 될때마다 wirteIndex ++

        public RecvBuffer(int bufferSize)
        {
            _buffer = new ArraySegment<byte>(new byte[bufferSize] , 0, bufferSize);
        }

        public int DataSize { get { return _writeIndex - _readIndex; } } //현재까지 write한 데이터 크기
        public int FreeSize { get { return _buffer.Count - _writeIndex; } } //write 가능한 남은 공간

        public ArraySegment<byte> ReadSegment
        {
            //읽기 가능한 데이터를 리턴
            get { return new ArraySegment<byte>(_buffer.Array, _buffer.Offset + _readIndex, DataSize); }
        }
        
        
        public ArraySegment<byte> WriteSegment
        {
            //다음에 recv할 때 쓰기 가능한 데이터 
            get { return new ArraySegment<byte>(_buffer.Array, _buffer.Offset + _writeIndex, FreeSize); }
        }


        //clean 을 하지 않으면 언젠가 readIndex, writeIndex가 배열의 범위를 넘어감 > 초기화해주는것.
        public void Clean()
        {
            int dataSize = DataSize;
            if(dataSize == 0)
            {
                _readIndex = 0;

            }
            else
            {
                //readIndex 앞부분은 불필요한 데이터. arr의 readindex부터 복사하는데 arr의 offindex부터 dataSize만큼 복사해줭
                Array.Copy(_buffer.Array, _buffer.Offset + _readIndex,  _buffer.Array, _buffer.Offset, dataSize);
                _readIndex = 0;
                _writeIndex = dataSize;
                     
            }
        }

        
        //데이터 조립이 완료되어서 읽기처리를 끝냈을때 호출
        public bool OnRead(int numOfBytes)
        {
            if (numOfBytes > DataSize) return false;

            _readIndex += numOfBytes;
            return true;
        }

        //통신이 와서 버퍼에 write처리를 완료하였을때 호출
        public bool OnWrite(int numOfBytes)
        {
            if (numOfBytes > FreeSize) return false;
            _writeIndex += numOfBytes;
            return true;
        }
    }
}
