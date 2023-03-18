using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ServerCore
{
    //send, recv를 담당하는 세션
    internal class Session
    {
        Socket _socket;
        SocketAsyncEventArgs recvArgs;
        SocketAsyncEventArgs sendArgs;

        int _disconnected = 0;
        public void Start(Socket socket)
        {
            _socket = socket;
            recvArgs = new SocketAsyncEventArgs();
            sendArgs = new SocketAsyncEventArgs();

            recvArgs.Completed += new EventHandler<SocketAsyncEventArgs>(OnRecvCompleted);
            sendArgs.Completed += new EventHandler<SocketAsyncEventArgs>(OnSendCompleted);
             

            recvArgs.SetBuffer(new byte[1024], 0, 1024); //받은 정보를 여기에 넣어주세요.

            RegisterRecv();
        }

        public void Send(byte[] sendBuff)
        {
          
            sendArgs.SetBuffer(new byte[1024], 0, 1024); //받은 정보를 여기에 넣어주세요.
            RegisterSend();
        }

        void RegisterSend(SocketAsyncEventArgs sendArgs)
        {
            bool pending = _socket.SendAsync(sendArgs);
            if (!pending) OnSendCompleted(null, sendArgs);
        }

       void OnSendCompleted(object sender, SocketAsyncEventArgs args)
        {
            int sendBytes = args.BytesTransferred;
            if (sendBytes > 0 && args.SocketError == SocketError.Success)
            {
                 //해줄게딱히없음. 전송성공
            }
            else
            {
                Console.WriteLine("OnRecvComplete Fail." + args.SocketError.ToString());
                Disconnect();
            }
        }

        #region Network 
        void RegisterRecv()
        {

            bool pending = _socket.ReceiveAsync(recvArgs);
            if (!pending) OnRecvCompleted(null, recvArgs);
        }

        void OnRecvCompleted(object sender, SocketAsyncEventArgs args)
        {
            int recvBytes = args.BytesTransferred;
            if(recvBytes >0 && args.SocketError == SocketError.Success) 
            {
                try
                {
                    string recvData = Encoding.UTF8.GetString(args.Buffer, args.Offset, recvBytes);
                    Console.WriteLine("[From Client]" + recvData);
                    RegisterRecv();
                }
                catch (Exception e)
                {
                    Console.WriteLine("OnRecvComplete Fail." + e.Message);
                    Disconnect();
                }
                
            }
            else
            {
                //disconnect
            }
        }

        public void Disconnect()
        {
            //여러 군데에서 discconect를 연달아 호출하면 문제가 생김. disconnected 변수를 두어 한번만 호출될 수 있도록 함.
            if (Interlocked.Exchange(ref _disconnected, 1) == 1) return;

            _socket.Shutdown(SocketShutdown.Both);
            _socket.Close();
        }
        #endregion
    }
}
