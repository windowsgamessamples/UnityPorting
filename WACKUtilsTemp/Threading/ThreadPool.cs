#if !UNITY_EDITOR && UNITY_METRO
namespace System.Threading
{
    public class ThreadPool
    {
        public static Func<WaitCallback, object, bool> DoQueueUserWorkItem;

        public static bool QueueUserWorkItem(WaitCallback callback)
        {
            return QueueUserWorkItem(callback, null);
        }

        public static bool QueueUserWorkItem(WaitCallback callback, object argument)
        {
            return DoQueueUserWorkItem(callback, argument);
        }
    }
}
#endif