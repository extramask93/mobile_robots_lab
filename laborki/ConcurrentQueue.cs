using System;
using System.Collections.Generic;
using System.Threading;
using System.Linq;
using System.Text;

namespace laborki
{
   public class ConcurrentQueue<T>
    {
        private  Queue<T> _queue;
        private object m_SyncObject = new object();
        public EventWaitHandle _wh;
        public ConcurrentQueue(int size, EventWaitHandle _wh)
        {
            _queue = new Queue<T>(size);
            this._wh = _wh;
        }
        public void Add(T item)
        {
            lock (m_SyncObject)
            {
                _queue.Enqueue(item);
            }
            _wh.Set();
        }
        public T Remove()
        {
            lock (m_SyncObject)
            {
                T item = _queue.Dequeue();
                return item;
            }
        }
        public int Count()
        {
            int a;
            lock (m_SyncObject)
            {
               a = _queue.Count;
            }
            return a;
        }
        public void Clear()
        {
            lock (m_SyncObject)
            {
                _queue.Clear();
            }
        }
    }
}
