using FlagTip.caret;
using FlagTip.models;
using FlagTip.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Automation;
using System.Windows.Forms; // Cursor.Position 사용
using UIAutomationClient;
using static FlagTip.Utils.NativeMethods;


namespace FlagTip.Helpers
{
    internal class UIAExplorerHelper
    {

        internal static bool TryGetCaretFromExplorerUIA(out RECT caretLocation)
        {
            caretLocation = new RECT();

            try
            {
                UIAHelper.TryGetCaretFromUIA(out caretLocation);



                if (caretLocation.left == 0 && caretLocation.top == 0)
                {


                    //Console.WriteLine("111. explorer mode UIA가 잡는데 실패하여 CUIA로 박스로 잡기");

                    CUIAutomation uia = new CUIAutomation();
                    IUIAutomationElement element = uia.GetFocusedElement();
                    if (element == null)
                    {
                        return false;
                    }

                    int controlType = element.CurrentControlType;
                    string className = ExplorerInfo.GetForegroundWindowClassName();
                    var parent = uia.ControlViewWalker.GetParentElement(element);
                    var boundingRect = element.CurrentBoundingRectangle;


                    // CABINET의 상단
                    if(parent.CurrentControlType == 50026)
                    {
                        // CABINET 상단의 수정상태
                        if (controlType == 50004)
                        {
                            caretLocation.left = boundingRect.left + 11;
                            caretLocation.top = boundingRect.top + 6;
                            caretLocation.right = boundingRect.right;
                            caretLocation.bottom = boundingRect.bottom;
                        }


                    // PROGRAM, CABINET의 파일
                    } else if (parent.CurrentControlType == 50008){

                        // PROGRAM, CABINET의 파일수정상태
                        if (controlType == 50004 || controlType == 50030)
                        {
                            caretLocation.left = boundingRect.left+1;
                            caretLocation.top = boundingRect.top;
                            caretLocation.right = boundingRect.right;
                            caretLocation.bottom = boundingRect.bottom;
                        }

                    }
                }
                else
                {

                    //Console.WriteLine("222. explorer mode UIA가 잡는데 성공");
                    //prevCaretLocation.left = caretLocation.left;
                    //prevCaretLocation.top = caretLocation.top;
                    //prevCaretLocation.right = caretLocation.right;
                    //prevCaretLocation.bottom = caretLocation.bottom;

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
