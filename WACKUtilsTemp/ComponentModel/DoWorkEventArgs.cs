#if NETFX_CORE
namespace System.ComponentModel
{
    public class DoWorkEventArgs : CancelEventArgs
    {
        public Object Argument { get; private set; }
        public Object Result { get; set; }

        public DoWorkEventArgs(object argument)
        {
            Argument = argument;
        }
    }
}
#endif