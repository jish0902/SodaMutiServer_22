using System;
using System.Diagnostics;
using System.Threading;

namespace ServerCore;

public static class TaskInterval
{
    /// <summary>
    /// 적절한 시간을 유지하는 코드
    /// </summary>
    /// <param name="interval">목표 시간</param>
    /// <param name="count">참조로 받을 현재 수</param>
    /// <param name="task">실행해야하는 함수</param>
    public static void Update(long interval, ref ushort count, Action task)
    {
        Stopwatch logicWatch = new Stopwatch();
        Stopwatch waitWatch = new Stopwatch();

        logicWatch.Start();
        waitWatch.Start();

        long timer = 0;
        int overtime = 0;

        while (true)
        {
            logicWatch.Restart();

            count++;
            task();

            // update 만큼 시간 지남
            logicWatch.Stop();
            timer = logicWatch.ElapsedMilliseconds + overtime;

            if (timer < interval) // 너무 빠르다면 빠른 시간 만큼 쉼
            {

                if ((float)timer / interval < 0.1)  // 지속적으로 감시
                {
                    waitWatch.Restart();
                    while (waitWatch.ElapsedMilliseconds < interval - timer)
                    {
                        // Spin-lock
                    }
                    Console.WriteLine("SpinLock : " + (int)(interval - timer));
                    waitWatch.Stop();
                }
                else //잠시 쉼
                { 
                    Thread.Sleep((int)(interval - timer));
                    Console.WriteLine("sleep" + (int)(interval - timer));
                }
                
                Console.Write("(" + count + ") ");
            }

            overtime = (int)(logicWatch.ElapsedMilliseconds - interval);
            overtime = overtime < 0 ? 0 : overtime;
        }
    }
    

}