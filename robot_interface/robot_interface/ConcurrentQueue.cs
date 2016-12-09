using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace robot_interface
{
    class ConcurrentQueue<T>
    {
        public Queue<T> _queue;
        private object m_SyncObject = new object();
        public ConcurrentQueue(int size)
        {
            _queue = new Queue<T>(size);
        }
        public void Add(T item)
        {
            lock (m_SyncObject)
            {
                _queue.Enqueue(item);
            }

        }
        public T Remove()
        {
            lock (m_SyncObject)
            {
                T item = _queue.Dequeue();
                return item;
            }
        }
    }
}
