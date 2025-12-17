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
        MSAA,
        MouseClick,
        GUIThreadInfo,
        Fallback,
        Error
    }
}
