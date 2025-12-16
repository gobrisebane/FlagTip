using FlagTip.UI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static FlagTip.Utils.NativeMethods;


/*
namespace FlagTip.Hooking
{
    internal static class MouseHook
    {
        //private IndicatorForm _indicatorForm;

        internal static IntPtr MouseHookCallback(int nCode, IntPtr wParam, IntPtr lParam, IntPtr hookID, IndicatorForm indicatorForm)
        {

            //_indicatorForm = indicatorForm;
            if (nCode >= 0 && (MouseMessages)wParam == MouseMessages.WM_LBUTTONDOWN)
            {

                Console.WriteLine("Left mouse button clicked!");

                // 1. 클릭 좌표 가져오기
                MSLLHOOKSTRUCT hookStruct = (MSLLHOOKSTRUCT)Marshal.PtrToStructure(
                    lParam, typeof(MSLLHOOKSTRUCT));

                // 2. 해당 좌표의 HWND 얻기
                IntPtr hWnd = WindowFromPoint(hookStruct.pt);

                // 3. HWND에서 PID 얻기
                GetWindowThreadProcessId(hWnd, out uint processId);

                // 4. PID로 프로세스 이름 얻기
                try
                {
                    string procName = Process.GetProcessById((int)processId).ProcessName;

                    if (procName.Equals("explorer", StringComparison.OrdinalIgnoreCase))
                    {
                        Console.WriteLine("Explorer에서 클릭되었습니다!");



                        indicatorForm?.BeginInvoke(new Action(() =>
                            indicatorForm.SetPosition(50,50,50,50)
                        ));


                    }
                }
                catch {  }
            }

            return CallNextHookEx(hookID, nCode, wParam, lParam);
        }


        internal static IntPtr SetMouseHook(LowLevelMouseProc proc)
        {
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule)
            {
                return SetWindowsHookEx(WH_MOUSE_LL, proc,
                    GetModuleHandle(curModule.ModuleName), 0);
            }
        }

    }
}
*/