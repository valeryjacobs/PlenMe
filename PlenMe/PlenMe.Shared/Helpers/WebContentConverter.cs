﻿using System;
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

            if (ControlLocator.ContentEditor == null) ControlLocator.ContentEditor = (Windows.UI.Xaml.Controls.WebView)FindDescendantByName((FrameworkElement)Window.Current.Content, parameter.ToString());

            if (ControlLocator.ContentEditorReady)
            {
                SetContentAsync(ControlLocator.ContentEditor, value.ToString());
            }
            else
            {
                Uri url = ControlLocator.ContentEditor.BuildLocalStreamUri("MyTag", "/ContentEditor/Editor.html");
                ControlLocator.ContentEditor.NavigateToLocalStreamUri(url, ControlLocator.StreamResolver);
                ControlLocator.ContentEditor.DOMContentLoaded += async (x, y) =>
                {
                    await SetContentAsync(ControlLocator.ContentEditor, value.ToString());
                    
                };
            }
            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return value;
        }

        private async Task SetContentAsync(WebView webView, string value)
        {
            await webView.InvokeScriptAsync("SetContent", new string[] { value });
          //  await webView.InvokeScriptAsync("SetViewSize", new string[] { "200", "200" });
            await webView.InvokeScriptAsync("SetZoom", new string[] { "1.8" });
            ControlLocator.ContentEditorReady = true;
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
