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

        bool _flush = false;

        public void Push(Action job)
        {
            bool flush = false;

            lock (lockObj)
            {
                _jobQueue.Enqueue(job);
                if (_flush == false)
                    flush = _flush = true;
            }

            if (flush)
                Flush();
        }

        public void Flush()
        {
            while (true)
            {
                Action action = Pop();
                if (action == null)
                    return;

                action.Invoke();
            }
        }

        public Action Pop()
        {
            lock (lockObj)
            {
                if (_jobQueue.Count == 0)
                {
                    _flush = false;
                    return null;
                }
                return _jobQueue.Dequeue();
            }
        }

    }
}

