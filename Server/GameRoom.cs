using System;
using ServerCore;

namespace Server
{
    public class GameRoom : IJobQueue
    {
        public List<ClientSession> sessions = new List<ClientSession>();

        List<ArraySegment<byte>> _pendingList = new List<ArraySegment<byte>>();

        public void Enter(ClientSession session)
        {

            //add player
            sessions.Add(session);
            session.gameRoom = this;

            //send playerList to new player
            S_PlayerList players = new S_PlayerList();
            foreach (var item in sessions)
            {
                players.players.Add(new S_PlayerList.Player()
                {
                    isSelf = item == session,
                    playerId = item.sessionId,
                    posX = item.posX,
                    posY = item.posY,
                    posZ = item.posZ
                });

            }
            session.Send(players.Serialize());

            //send newPlayer to all player
            S_BroadcastEnterGame enter = new S_BroadcastEnterGame()
            {
                playerId = session.sessionId,
                posX = 0,
                posY = 0,
                posZ = 0,
            };

            BroadCast(enter.Serialize());

        }


        public void Leave(ClientSession session)
        {
            sessions.Remove(session);

            //broadcast player leave.
            S_BroadcastLeaveRoom leave = new S_BroadcastLeaveRoom() { playerId = session.sessionId, };
            BroadCast(leave.Serialize());
        }

        public void BroadCast(ArraySegment<byte> buffer)
        {
            _pendingList.Add(buffer);
        }


        public void Flush()
        {
            //Console.WriteLine("Flush called. cnt:" + _pendingList.Count);

            foreach (var item in sessions)
            {
                item.Send(_pendingList);
            }

            _pendingList.Clear();
        }
        #region Job Queue

        JobQueue jobQueue = new JobQueue();
        public void Push(Action action)
        {
            jobQueue.Push(action);
        }

        public Action Pop()
        {
            return jobQueue.Pop();
        }

        #endregion

        #region Move
        public void Move(ClientSession session, C_Move packet)
        {
            session.posX = packet.posX;
            session.posY = packet.posY;
            session.posZ = packet.posZ;

            S_BroadcastMove broadcast = new S_BroadcastMove()
            {
                playerId = session.sessionId,
                posX = session.posX,
                posY = session.posY,
                posZ = session.posZ,
            };
            BroadCast(broadcast.Serialize());
        }
        #endregion
    }
}

