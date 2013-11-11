#if NETFX_CORE
using System.Threading;
namespace System.ComponentModel
{
    public delegate void DoWorkEventHandler(object sender, DoWorkEventArgs args);
    public delegate void ProgressChangedEventHandler(object sender, ProgressChangedEventArgs args);
    public delegate void RunWorkerCompletedEventHandler(object sender, RunWorkerCompletedEventArgs args);

    public class BackgroundWorker
    {
        public bool WorkerReportsCancellation { get; set; }
        public bool WorkerReportsProgress { get; set; }
        public bool WorkerSupportsCancellation { get; set; }
        public bool CancellationPending { get; private set; }

        public event DoWorkEventHandler DoWork;
        public event ProgressChangedEventHandler ProgressChanged; // Not required
        public event RunWorkerCompletedEventHandler RunWorkerCompleted;

        public BackgroundWorker()
        {
            CancellationPending = false;
            WorkerReportsProgress = false;
            WorkerReportsCancellation = false;
            WorkerSupportsCancellation = false;
        }

        public void RunWorkerAsync()
        {
            RunWorkerAsync(null);
        }

        public void RunWorkerAsync(object argument)
        {
            if (DoWork == null) return;

            ThreadPool.QueueUserWorkItem((obj) =>
                {
                    var args = new DoWorkEventArgs(obj);
                    Exception storedException = null;
                    try
                    {
                        DoWork(this, args);
                    }
                    catch (Exception e)
                    {
                        storedException = e;
                    }
                    if (RunWorkerCompleted != null)
                        RunWorkerCompleted(this, new RunWorkerCompletedEventArgs(args.Result, storedException, args.Cancel));
                }, argument);
        }

        public void CancelAsync()
        {
            if (!WorkerSupportsCancellation) return; // Can't cancel if it isn't supported
            CancellationPending = true;
        }
    }
}
#endif