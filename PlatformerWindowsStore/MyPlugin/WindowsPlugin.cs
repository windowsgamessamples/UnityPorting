using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#if !UNITY_EDITOR
using Windows.ApplicationModel.DataTransfer;
using Windows.UI.Core;
#endif

namespace MyPlugin
{

    public class WindowsPlugin
    {

        /// <summary>
        /// Show the Share UI
        /// </summary>
        
#if UNITY_EDITOR

        public static void ShowShareUI()
        {

        // do nothing

#else

        public static async void ShowShareUI()
        {

            await CoreWindow.GetForCurrentThread().Dispatcher.RunAsync(CoreDispatcherPriority.High, delegate()
            {
                DataTransferManager.ShowShareUI();
            });
#endif
        }
    }
}
