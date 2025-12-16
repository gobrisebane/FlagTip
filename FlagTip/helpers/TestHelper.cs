using FlagTip.Models;
using FlagTip.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Automation;
using UIAutomationClient;
using static FlagTip.Utils.NativeMethods;

namespace FlagTip.Helpers
{
    internal class TestHelper
    {


        internal static bool TryGetCaretFromExplorerUIA(out RECT caretLocation, out string method)
        {
            caretLocation = new RECT();
            method = "None";


          

            try
            {
                UIAHelper.TryGetCaretFromUIA(out caretLocation, out method);

                

                if (caretLocation.left == 0 && caretLocation.top == 0)
                {
                    Console.WriteLine("1.no caret detect");


                    string className = ExplorerInfo.GetForegroundWindowClassName();

                    if (className == "CabinetWClass")
                    {
                        CUIAutomation uia = new CUIAutomation();
                        IUIAutomationElement element = uia.GetFocusedElement();
                        
                        if (element == null)
                        {
                            Console.WriteLine("focused nothing");
                            return false;
                        }

                        int controlType = element.CurrentControlType;

                        if(controlType == 50004)
                        {


                            var boundingRect = element.CurrentBoundingRectangle;


                            Console.WriteLine("5.1. boundingRect.left : " + boundingRect.left);
                            Console.WriteLine("5.2. boundingRect.right : " + boundingRect.right);

                            IntPtr hwndExplorer = GetForegroundWindow();

                            GetClientRect(hwndExplorer, out RECT clientRect);

                            int midX = (clientRect.right - clientRect.left) / 2;



                            if (boundingRect.left < midX)
                            {

                                Console.WriteLine("7.1 주소표시줄");
                                caretLocation.left = prevCaretLocation.left;
                                caretLocation.top = prevCaretLocation.top;
                                caretLocation.right = prevCaretLocation.right;
                                caretLocation.bottom = prevCaretLocation.bottom;

                            }
                            else
                            {
                                Console.WriteLine("7.2 검색창");
                                caretLocation.left = boundingRect.left+11;
                                caretLocation.top = boundingRect.top+6;
                                caretLocation.right = boundingRect.right;
                                caretLocation.bottom = boundingRect.bottom;

                            }


                        }
                    } 
                }
                else
                {

                    prevCaretLocation.left = caretLocation.left;
                    prevCaretLocation.top = caretLocation.top;
                    prevCaretLocation.right = caretLocation.right;
                    prevCaretLocation.bottom = caretLocation.bottom;

                }

                return true;

            }
            catch (Exception ex)
            {
                Console.WriteLine("오류: " + ex.Message);
                return false;
            }

        }


    }
}
