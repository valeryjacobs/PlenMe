using System;
using System.Collections.Generic;
using System.Text;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using System.IO;
using PlenMe.DataModel;
using Windows.UI.Xaml.Controls;
using System.Threading.Tasks;
namespace PlenMe.Helpers
{
    public class WebContentConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null) return "";

                WebView webView = ControlLocater.ContentEditor;


                if (ControlLocater.ContentEditorReady)
                {
                   SetContentAsync(webView, value.ToString());
                }
                else
                {
                    Uri url = webView.BuildLocalStreamUri("MyTag", "/ContentEditor/index.html");
                    webView.NavigateToLocalStreamUri(url, ControlLocater.StreamResolver);
                    webView.DOMContentLoaded += async (x, y) => { await  SetContentAsync(webView, value.ToString()); };
                    
                }
                return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return value;
        }

        private async Task SetContentAsync(WebView webView, string value)
        {

           await webView.InvokeScriptAsync("SetContent", new string[] { value});
           ControlLocater.ContentEditorReady = true;
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
