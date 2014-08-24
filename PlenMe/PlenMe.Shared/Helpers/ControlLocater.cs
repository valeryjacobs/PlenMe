using System;
using System.Collections.Generic;
using System.Text;

namespace PlenMe.Helpers
{
    public static class ControlLocater
    {
        private static Windows.UI.Xaml.Controls.WebView _contentEditor;
       

        public static Windows.UI.Xaml.Controls.WebView ContentEditor
        {
            get { return _contentEditor; }
            set { _contentEditor = value; }
        }
        
    }
}
