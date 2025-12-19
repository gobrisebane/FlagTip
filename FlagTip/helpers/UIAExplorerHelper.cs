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
                        Console.WriteLine("focused nothing");
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

                    prevCaretLocation.left = caretLocation.left;
                    prevCaretLocation.top = caretLocation.top;
                    prevCaretLocation.right = caretLocation.right;
                    prevCaretLocation.bottom = caretLocation.bottom;
                }







                /*
                                if (caretLocation.left == 0 && caretLocation.top == 0){
                                  Console.WriteLine("1.no caret detect");


                                    CUIAutomation uia = new CUIAutomation();
                                    IUIAutomationElement element = uia.GetFocusedElement();
                                    if (element == null)
                                    {
                                        Console.WriteLine("focused nothing");
                                        return false;
                                    }
                                    int controlType = element.CurrentControlType;
                                    //Console.WriteLine(">>> 222. controlType : " + controlType);

                                    string className = ExplorerInfo.GetForegroundWindowClassName();
                                    //Console.WriteLine(">>> 111. className : " + className);


                                  if (controlType == 50004 || controlType == 50030)
                                    {                        
                                        caretLocation.right = prevCaretLocation.right;
                                        caretLocation.bottom = prevCaretLocation.bottom;
                                        caretLocation.left = prevCaretLocation.left;
                                        caretLocation.top = prevCaretLocation.top;

                                    }

                                } else {
                                  prevCaretLocation.left = caretLocation.left;
                                  prevCaretLocation.top = caretLocation.top;
                                  prevCaretLocation.right = caretLocation.right;
                                  prevCaretLocation.bottom = caretLocation.bottom;
                                }

                */


























                /*if (caretLocation.left == 0 && caretLocation.top == 0)
                {
                    Console.WriteLine("1.no caret detect");


                    string className = ExplorerInfo.GetForegroundWindowClassName();
                    Console.WriteLine("className : " + className);

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
                        Console.WriteLine("controlType : " + controlType);




                    if (controlType == 50004){


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
                        caretLocation.left = boundingRect.left + 11;
                        caretLocation.top = boundingRect.top + 6;
                        caretLocation.right = boundingRect.right;
                        caretLocation.bottom = boundingRect.bottom;

                            }


                }

            }


                else if (className == "Program")
                {


                    Console.WriteLine("program works");
                }


                } else {

                    prevCaretLocation.left = caretLocation.left;
                    prevCaretLocation.top = caretLocation.top;
                    prevCaretLocation.right = caretLocation.right;
                    prevCaretLocation.bottom = caretLocation.bottom;

                }



*/



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
