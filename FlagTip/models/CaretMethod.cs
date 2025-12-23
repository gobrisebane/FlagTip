using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlagTip.models
{
    internal enum CaretMethod
    {
        None,
        UIA,
        ExplorerUIA,
        MSAA,
        MouseClick,
        GUIThreadInfo,
        Cursor,
        Fallback,
        Error
    }
}
