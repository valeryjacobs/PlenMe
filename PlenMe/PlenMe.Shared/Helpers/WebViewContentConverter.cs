using System;
using System.Collections.Generic;
using System.Text;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using System.IO;
using PlenMe.DataModel;

namespace PlenMe.Helpers
{
    public class WebViewContentConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            Windows.UI.Xaml.Controls.WebView webView = (Windows.UI.Xaml.Controls.WebView)FindDescendantByName((FrameworkElement)Window.Current.Content, parameter.ToString());

            if (value == null) value = "";
            if (WebContentTemplate.HTML != null && webView != null)
            {
                if (ControlLocater.ContentViewerReady)
                {
                    webView.InvokeScriptAsync("SetContent", new string[] { value.ToString() });

                }
                else
                {
                    Uri url = webView.BuildLocalStreamUri("MyTag", "/ContentEditor/ContentTemplate.html");
                    webView.NavigateToLocalStreamUri(url, ControlLocater.StreamResolver);

                    webView.DOMContentLoaded += (x, y) => { webView.InvokeScriptAsync("SetContent", new string[] { value.ToString() }); };

                    ControlLocater.ContentViewerReady = true;
                }
            }

            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return value;
        }


        public static FrameworkElement FindDescendantByName(FrameworkElement element, string name)
        {
            if (element == null || string.IsNullOrWhiteSpace(name)) { return null; }

            if (name.Equals(element.Name, StringComparison.OrdinalIgnoreCase))
            {
                return element;
            }
            var childCount = VisualTreeHelper.GetChildrenCount(element);
            for (int i = 0; i < childCount; i++)
            {
                var result = (VisualTreeHelper.GetChild(element, i) as FrameworkElement).FindDescendantByName(name);
                if (result != null) { return result; }
            }
            return null;
        }

    }

}
