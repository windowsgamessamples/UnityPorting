using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Core;

namespace MyPlugin
{
    /// <summary>
    /// Handles dispatching to the UI and App Threads
    /// </summary>
    public static class Dispatcher
    {

        // needs to be set via the app so we can invoke onto App Thread (see App.xaml.cs)
        public static Action<Action> AppDispatcher 
        { get; set; }

        // needs to be set via the app so we can invoke onto UI Thread (see App.xaml.cs)
        public static CoreDispatcher UIDispatcher
        { get; set; }
    }
}
