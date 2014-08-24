using System;
using System.Collections.Generic;
using System.Text;

namespace PlenMe.DataModel
{
    public static class WebContentTemplate
    {
        private static string _HTML;

        public static string HTML
        {
            get { return _HTML; }
            set { _HTML = value; }
        }

    }
}
