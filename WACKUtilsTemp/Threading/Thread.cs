#if !UNITY_EDITOR && UNITY_METRO

namespace System.Threading
{
    public class Thread
    {
        public static void Sleep(int ms)
        {
            new System.Threading.ManualResetEvent(false).WaitOne(ms);
        }
    }
}

#endif