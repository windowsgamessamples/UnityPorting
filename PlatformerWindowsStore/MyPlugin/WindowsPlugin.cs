using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.UI.Core;

namespace MyPlugin
{

    public class WindowsPlugin
    {

        /// <summary>
        /// Show the Share UI
        /// </summary>
        public static async void ShowShareUI()
        {
            await CoreWindow.GetForCurrentThread().Dispatcher.RunAsync(CoreDispatcherPriority.High, delegate()
            {
                DataTransferManager.ShowShareUI();
            });
        }
    }
}
