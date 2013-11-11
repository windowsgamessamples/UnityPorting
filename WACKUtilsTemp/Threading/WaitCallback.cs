#if UNITY_METRO && !UNITY_EDITOR
using System;

namespace System.Threading
{
    public delegate void WaitCallback(object state);
}
#endif