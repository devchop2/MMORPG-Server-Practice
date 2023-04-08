using System;
using ServerCore;

namespace Server
{
    public struct JobTimerElem : IComparable<JobTimerElem>
    {
        public int execTick;
        public Action action;

        public int CompareTo(JobTimerElem other)
        {
            return other.execTick - execTick;
        }
    }
    public class JobTimer
    {
        PriorityQueue<JobTimerElem> _queue = new PriorityQueue<JobTimerElem>();
        object lockObj = new object();

        public static JobTimer Instance = new JobTimer();

        public void Push(Action action, int tickAfter = 0)
        {
            JobTimerElem job;
            job.execTick = System.Environment.TickCount + tickAfter;
            job.action = action;

            lock (lockObj)
            {
                _queue.Push(job);
            }
        }

        public void Flush()
        {
            while (true)
            {
                int now = System.Environment.TickCount;

                JobTimerElem job;

                lock (lockObj)
                {
                    if (_queue.Count == 0)
                        break;

                    job = _queue.Peek();
                    if (job.execTick > now)
                        break;

                    _queue.Pop();
                }

                job.action.Invoke();
            }
        }
    }
}

