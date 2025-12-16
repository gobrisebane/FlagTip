using FlagTip.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlagTip.Utils
{
    internal static class CommonUtils
    {
        internal static bool IsRectValid(RECT r) => r.left != 0 || r.top != 0 || r.right != 0 || r.bottom != 0;



    }
}
