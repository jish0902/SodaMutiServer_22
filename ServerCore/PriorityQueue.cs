using System;
using System.Collections.Generic;

namespace ServerCore;

public class PriorityQueue<T> where T : IComparable<T>
{
    private readonly List<T> _heap = new();
    public int Count => _heap.Count;


    public void Push(T data)
    {
        _heap.Add(data);

        var now = _heap.Count - 1;
        while (now > 0)
        {
            var next = (now - 1) / 2;
            if (_heap[now].CompareTo(_heap[next]) < 0)
                break;

            (_heap[next], _heap[now]) = (_heap[now], _heap[next]);

            now = next;
        }
    }


    public T Pop()
    {
        //반환할 데이터 저장
        var temp = _heap[0];

        var lastIndex = Count - 1;
        _heap[0] = _heap[lastIndex];
        _heap.RemoveAt(lastIndex);
        lastIndex--;

        var now = 0;
        while (now < Count - 1)
        {
            var left = now * 2 + 1;
            var right = now * 2 + 2;

            var next = now;
            if (left <= lastIndex && _heap[next].CompareTo(_heap[left]) < 0)
                next = left;
            if (right <= lastIndex && _heap[next].CompareTo(_heap[right]) < 0)
                next = right;

            if (next == now)
                break;


            var t = _heap[next];
            _heap[next] = _heap[now];
            _heap[now] = t;

            now = next;
        }

        return temp;
    }


    public T Peek()
    {
        if (Count == 0)
            return default;

        return _heap[0];
    }
}