using System;
namespace ServerCore
{
    public interface IJobQueue
    {
        public void Push(Action action);
        public Action Pop();
    }

    public class JobQueue : IJobQueue
    {
        Queue<Action> _jobQueue = new Queue<Action>();
        object lockObj = new object();

        public void Push(Action action)
        {
            lock (lockObj)
            {
                _jobQueue.Enqueue(action);
            }
        }

        public Action Pop()
        {
            lock (lockObj)
            {
                if (_jobQueue.Count == 0) return null;
                return _jobQueue.Dequeue();
            }
        }

    }
}

