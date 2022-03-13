using ServerCore;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;


namespace Server.Game
{
    struct JobTimerElem : IComparable<JobTimerElem>
    {
        public int execTick; //실행시간
        public IJob job; //action

        public int CompareTo([AllowNull] JobTimerElem other)
        {
            return other.execTick - execTick;
        }
    }


    public class JobTimer
    {
        PriorityQueue<JobTimerElem> _pq = new PriorityQueue<JobTimerElem>();
        object _lock = new object();

        public void Push(IJob job, int tickAfter = 0)
        {
            JobTimerElem jobElement = new JobTimerElem();
            jobElement.execTick = System.Environment.TickCount + tickAfter;
            jobElement.job = job;

            lock (_lock)
            {
                _pq.Push(jobElement);
            }
        }





        public void Flush()
        {
            while (true)
            {
                int now = System.Environment.TickCount;
                JobTimerElem jobElement;

                lock (_lock)
                {
                    if (_pq.Count == 0)
                        return;

                    jobElement = _pq.Peek();
                    if (jobElement.execTick > now)
                        break;

                    _pq.Pop();
                    
                }

                jobElement.job.Execute();
            }




        }


    }





}

