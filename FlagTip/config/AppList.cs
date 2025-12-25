using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlagTip.config
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

    }
}
