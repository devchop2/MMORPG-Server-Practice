using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ServerCore
{
    //send, recv를 담당하는 세션
    abstract class Session
    {
        Socket _socket;

        SocketAsyncEventArgs recvArgs = new SocketAsyncEventArgs();

        SocketAsyncEventArgs sendArgs = new SocketAsyncEventArgs();
        Queue<byte[]> _sendQueue = new Queue<byte[]>();
        List<ArraySegment<byte>> pendingList = new List<ArraySegment<byte>>();

        object pendingLock = new object();
        
        int _disconnected = 0;

        #region Handlers

        public abstract void OnConnected(EndPoint endPoint);
        public abstract void OnDisconnected(EndPoint endPoint);
        public abstract void OnRecv(ArraySegment<byte> buffer);
        public abstract void OnSend(int numOfBytes);
        
        #endregion
        public void Start(Socket socket)
        {
            _socket = socket;
            
            recvArgs.Completed += new EventHandler<SocketAsyncEventArgs>(OnRecvCompleted);
            recvArgs.SetBuffer(new byte[1024], 0, 1024); //받은 정보를 여기에 넣어주세요.


            sendArgs.Completed += new EventHandler<SocketAsyncEventArgs>(OnSendCompleted);
            RegisterRecv();
        }

        public void Send(byte[] sendBuff)
        {
            //한번에 하나만 호출되도록 보장
            lock (pendingLock)
            {
                _sendQueue.Enqueue(sendBuff);
                if (pendingList.Count == 0) RegisterSend();  
                //아직 전송작업이 진행되고있지 않으면 바로 전송을 Regist > pendingList = 0 이라는것은 현재 작업중인 리스트가 없다는것.
                
            }
        }


        void RegisterSend()
        {
            //queue 안에 있는 모든 byte[] 를 넣음. 
            while(_sendQueue.Count > 0)
            {
                byte[] buff = _sendQueue.Dequeue();
                pendingList.Add(new ArraySegment<byte>(buff, 0, buff.Length));
            }

            sendArgs.BufferList = pendingList;
                
            bool pending = _socket.SendAsync(sendArgs);
            if (!pending) OnSendCompleted(null, sendArgs);
        }

       void OnSendCompleted(object sender, SocketAsyncEventArgs args)
        {

            lock (pendingLock)
            {
                
                int sendBytes = args.BytesTransferred;
                if (sendBytes > 0 && args.SocketError == SocketError.Success)
                {
                    sendArgs.BufferList = null;
                    pendingList.Clear();

                    
                    OnSend(sendBytes);
                    if (_sendQueue.Count > 0) RegisterSend();
                }
                else
                {
                    Console.WriteLine("OnRecvComplete Fail." + args.SocketError.ToString());
                    Disconnect();
                }
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
                    OnRecv(new ArraySegment<byte>(args.Buffer, args.Offset, recvBytes));
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

            OnDisconnected(_socket.RemoteEndPoint);
            _socket.Shutdown(SocketShutdown.Both);
            _socket.Close();
        }
        #endregion
    }
}
