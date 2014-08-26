using System;
using System.Collections.Generic;
using System.Text;
using Windows.UI.Xaml.Controls;

namespace PlenMe.Helpers
{
    public static class ControlLocater
    {
        static ControlLocater()
        {
            //ContentViewerReady = false;
            //ContentEditorReady = false;
        }
        public static bool ContentViewerReady { get; set; }
        public static bool ContentEditorReady { get; set; }
        private static WebView _contentEditor;
       

        public static WebView ContentEditor
        {
            get { return _contentEditor; }
            set { _contentEditor = value; }
        }

        private static WebView _contentViewer;

        public static WebView ContentViewer
        {
            get { return _contentViewer; }
            set { _contentViewer = value; }
        }
        

        private static StreamUriWinRTResolver _streamResolver;


        public static StreamUriWinRTResolver StreamResolver
        {
            get { return _streamResolver; }
            set { _streamResolver = value; }
        }

        
        
    }
}
