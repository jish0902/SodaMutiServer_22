using System;
using System.Collections.Generic;
using System.Text;

namespace ServerCore
{
    class PriorityQueue<T> where T : IComparable<T>
    {

        List<T> _heap = new List<T>();
        public int Count { get { return _heap.Count; } }


        public void Push(T data)
        {
            _heap.Add(data);

            int now = _heap.Count - 1;
            while (now > 0)
            {
                int next = (now - 1) / 2;
                if (_heap[now].CompareTo(_heap[next]) < 0)
                    break;
                
                T t = _heap[next];
                _heap[next] = _heap[now];
                _heap[now] = t;

                now = next;
            }
        }


        public T Pop()
        {
            //반환할 데이터 저장
            T temp = _heap[0];

            int lastIndex = Count - 1;
            _heap[0] = _heap[lastIndex];
            _heap.RemoveAt(lastIndex);
            lastIndex--;

            int now = 0;
            while (now < Count -1)
            {
                int left = now * 2 + 1;
                int right = now * 2 + 2;

                int next = now;
                if (left <= lastIndex && _heap[next].CompareTo(_heap[left]) < 0)
                    next = left;
                if (right <= lastIndex && _heap[next].CompareTo(_heap[right]) < 0)
                    next = right;

                if (next == now)
                    break;

                
                T t = _heap[next];
                _heap[next] = _heap[now];
                _heap[now] = t;

                now = next;
                
            }

            return temp;
        }



        public T Peek()
        {
            if (Count == 0)
                return default(T);
            
            return _heap[0];
        }


    }
}
