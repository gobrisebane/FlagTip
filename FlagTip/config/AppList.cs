using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlagTip.Config
{
    internal class AppList
    {
        public static readonly HashSet<string> CursorAppList =
    new HashSet<string>(StringComparer.OrdinalIgnoreCase)
    {
        "whatsapp",
        "whatsapp.root",
        "photoshop",
        "illustrator",
        "indesign"
    };


        public static readonly HashSet<string> BrowserAppList =
   new HashSet<string>(StringComparer.OrdinalIgnoreCase)
   {
        "chrome",
        "msedge",
        "firefox",
        "opera",
        "brave",
        "vivaldi",
        "whale"
   };

    }
}
