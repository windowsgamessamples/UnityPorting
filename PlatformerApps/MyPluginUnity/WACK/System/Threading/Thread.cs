using System;
using SystemThreading = System.Threading;

namespace WACK.System.Threading
{
    public class Thread
    {
        public static void Sleep(int ms)
        {
            new SystemThreading.ManualResetEvent(false).WaitOne(ms);
        }
    }
}