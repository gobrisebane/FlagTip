using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlagTip.Models
{
    internal enum CaretMethod
    {
        None,
        UIA,
        UIAExplorer,
        UIAorGUI,
        MSAA,
        MouseClick,
        GUIThreadInfo,
        Cursor,
        Selection,
        Fallback,
        Error
    }
}
