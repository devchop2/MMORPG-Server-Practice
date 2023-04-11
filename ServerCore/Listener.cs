using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ServerCore
{
    public class Listener
    {
        Socket _listenSocket;

        Func<Session> sessionFactory;
        public void Init(IPEndPoint endPoint, Func<Session> _handler, int register = 10, int backlog = 100)
        {
            this.sessionFactory = _handler;

            _listenSocket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            //소켓 초기화해줌.
            _listenSocket.Bind(endPoint);

            //영업시작. 클라에게 요청받을 준비가 완료됨.
            //최대 10명만 받음. 사람이 몰릴 경우 11번째 부터 fail를 리턴함.
            _listenSocket.Listen(backlog);

            for (int i = 0; i < register; i++)
            {
                SocketAsyncEventArgs socketArgs = new SocketAsyncEventArgs();
                socketArgs.Completed += new EventHandler<SocketAsyncEventArgs>(OnAcceptCompleted);
                RegisterAccept(socketArgs);

            }

        }

        public void RegisterAccept(SocketAsyncEventArgs socketArgs)
        {
            //socketArgs 를 초기화해줌. 재사용
            socketArgs.AcceptSocket = null;

            //만약 클라에서 요청이 들어올 경우 accept 
            //비동기방식으로사용해야햄.. 안그럼 대기하다 멈춘다규..
            //pending == false 라면 대기없이 바로 처리되었다는 의미
            bool pending = _listenSocket.AcceptAsync(socketArgs);
            if (!pending) OnAcceptCompleted(null, socketArgs);
        }

        void OnAcceptCompleted(object sender, SocketAsyncEventArgs args)
        {
            if (args.SocketError == SocketError.Success)
            {
                Console.WriteLine("Connect!");
                Session session = sessionFactory.Invoke();

                session.Start(args.AcceptSocket);
                session.OnConnected(args.AcceptSocket.RemoteEndPoint);
            }
            else
            {
                Console.WriteLine("Socket Listen fail. " + args.SocketError.ToString());
            }

            //다시 공간이 남았으니 다른 고객을  받을 준비를 함. 
            RegisterAccept(args);
        }
    }
}
