#if NETFX_CORE
namespace System.ComponentModel
{
    public class RunWorkerCompletedEventArgs
    {
        public RunWorkerCompletedEventArgs(object result, Exception error, bool cancelled)
        {
            Result = result;
            Error = error;
            Cancelled = cancelled;
        }

        public object Result { get; private set; }
        public object UserState { get; private set; }
        public bool Cancelled { get; private set; }
        public Exception Error { get; private set; }
    }
}
#endif