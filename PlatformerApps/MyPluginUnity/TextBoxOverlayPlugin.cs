using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#if NETFX_CORE
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI;
using Windows.UI.Xaml;
#endif

namespace MyPlugin
{

    public class TextBoxOverlayPlugin
    {

        private static TextBoxOverlayPlugin _instance;
        private static readonly object _sync = new object();

#if NETFX_CORE

        private SwapChainBackgroundPanel panel;
        private TextBox textBox;

#endif

        public static TextBoxOverlayPlugin Instance
        {
            get
            {
                lock (_sync)
                {
                    if (_instance == null)
                        _instance = new TextBoxOverlayPlugin();
                }
                return _instance;
            }
        }

        /// <summary>
        /// Show a text box
        /// </summary>
        public void ShowTextBox(float x, float y, float width, float height,
            string backgroundColor, string borderColor, float fontSize, string fontFamily, string fontColor)
        {

#if NETFX_CORE
            Dispatcher.InvokeOnUIThread(() =>            
            {
                textBox = new TextBox();
                textBox.Background = new SolidColorBrush(ColorFromARGBString(backgroundColor));
                textBox.BorderBrush = new SolidColorBrush(ColorFromARGBString(borderColor));
                float scaleFactor = GetScaleFactor();
                float originalTextSize = 72.0f;

                if (scaleFactor != 1.0)
                {
                    //these fudge is because TextBox template has padding of 4 pixels.. 
                    // as you scale the factor, it might be noticeable in small screens.. 
                    // it is best to avoid very small text :) 
                    originalTextSize *= ((originalTextSize - (4 * scaleFactor)) / originalTextSize);
                }

                textBox.FontSize = originalTextSize / (scaleFactor);
                textBox.Foreground = new SolidColorBrush(ColorFromARGBString(fontColor));
                textBox.Margin = new Thickness(x / scaleFactor, y / scaleFactor, 0, 0);
                textBox.Width = width / scaleFactor;
                textBox.Height = height / scaleFactor;
                textBox.FontFamily = new FontFamily(fontFamily);
                textBox.Text = Window.Current.Bounds.Width.ToString("F2") + "," + Window.Current.Bounds.Height.ToString("F2");
                textBox.VerticalAlignment = Windows.UI.Xaml.VerticalAlignment.Top;
                textBox.HorizontalAlignment = Windows.UI.Xaml.HorizontalAlignment.Left;
                this.DXSwapChainPanel.Children.Add(textBox);
            });
#endif
        }

        /// <summary>
        /// Ensure the text box is hidden
        /// </summary>
        public void HideTextBox()
        {
#if NETFX_CORE
            Dispatcher.InvokeOnUIThread(() =>       
            {
                foreach (UIElement e in DXSwapChainPanel.Children)
                {
                    if (e is TextBox)
                        DXSwapChainPanel.Children.Remove(e);
                }
            }); 
#endif
        }

        /// <summary>
        /// retrieve the text from the text box
        /// </summary>
        public void GetText(Action<string> callback)
        {
#if NETFX_CORE
            Dispatcher.InvokeOnUIThread(() =>
            {
                if (textBox != null && callback != null)
                {
                    Dispatcher.InvokeOnAppThread(() =>
                    {
                        callback(textBox.Text);
                    });
                }
            });
#endif
        }

#if NETFX_CORE
        private SwapChainBackgroundPanel DXSwapChainPanel
        {
            get
            {
                if (panel == null)
                {
                    panel = GetFirstChildOfType(Window.Current.Content, typeof(SwapChainBackgroundPanel)) as SwapChainBackgroundPanel;
                }
                System.Diagnostics.Debug.Assert(panel != null);
                return panel;
            }
        }

        private DependencyObject GetFirstChildOfType(DependencyObject current, Type targetType)
        {
            int childrenCount = VisualTreeHelper.GetChildrenCount(current);
            for (int index = 0; index < childrenCount; index++)
            {
                var child = Windows.UI.Xaml.Media.VisualTreeHelper.GetChild(current, index);

                if (child != null)
                {
                    if (child.GetType() == targetType)
                        return child;
                    else
                    {
                        var recurseResult = GetFirstChildOfType(child, targetType);
                        if (recurseResult != null)
                            return recurseResult;
                    }
                }
            }
            return null;
        }

        private Color ColorFromARGBString(string hex)
        {
            //remove the # at the front
            hex = hex.Replace("#", "");

            byte a = 255;
            byte r = 255;
            byte g = 255;
            byte b = 255;

            int start = 0;

            //handle ARGB strings (8 characters long)
            if (hex.Length == 8)
            {
                a = byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
                start = 2;
            }

            //convert RGB characters to bytes
            r = byte.Parse(hex.Substring(start, 2), System.Globalization.NumberStyles.HexNumber);
            g = byte.Parse(hex.Substring(start + 2, 2), System.Globalization.NumberStyles.HexNumber);
            b = byte.Parse(hex.Substring(start + 4, 2), System.Globalization.NumberStyles.HexNumber);

            return Color.FromArgb(a, r, g, b);
        }

        private float GetScaleFactor()
        {
            switch (Windows.Graphics.Display.DisplayProperties.ResolutionScale)
            {
                case Windows.Graphics.Display.ResolutionScale.Scale100Percent:
                    return 1.0f;
                case Windows.Graphics.Display.ResolutionScale.Scale140Percent:
                    return 1.4f;
                case Windows.Graphics.Display.ResolutionScale.Scale180Percent:
                    return 1.8f;

            }
            return 1.0f;
        }

#endif

    }
}
