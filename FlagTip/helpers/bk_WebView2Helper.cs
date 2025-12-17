using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Automation;
using System.Windows.Automation.Text; // TextPattern 사용을 위해 필요
using System.Management; // ManagementObjectSearcher 사용을 위해 필요
using System.Windows; // System.Windows.Rect 사용을 위해 필요 (WPF 참조 필요)


using FlagTip.Utils; // 외부 유틸리티 사용 가정
using static FlagTip.Utils.NativeMethods; // GetForegroundProcessId, RECT 정의 등이 있다고 가정

namespace FlagTip.Helpers
{
    internal class bk_WebView2Helper
    {
        // RECT 구조체 정의 (귀하의 NativeMethods.cs 등에 정의되어 있다고 가정)
        // public struct RECT { public int Left; public int Top; public int Right; public int Bottom; }


        internal static bool TryGetCaretFromWebView2(out RECT rect, out string method)
        {
            rect = new RECT();
            method = "None";

            try
            {
                // 1. 포그라운드 프로세스 ID 획득
                uint pid = GetForegroundProcessId();
                if (pid == 0)
                    return false;

                // 2. 포그라운드 프로세스의 자식 프로세스 중 msedgewebview2.exe 탐색
                var searcher = new ManagementObjectSearcher($"SELECT * FROM Win32_Process WHERE ParentProcessId={pid}");

                foreach (ManagementObject mo in searcher.Get())
                {
                    var name = mo["Name"]?.ToString();
                    if (name?.Equals("msedgewebview2.exe", StringComparison.OrdinalIgnoreCase) == true)
                    {
                        Console.WriteLine("WEBVIEW WORKS: msedgewebview2.exe 발견");
                        method = "UIA";

                        // --- [WEBVIEW WORKS] 부분에 UI 자동화 로직 삽입 시작 ---

                        // msedgewebview2.exe의 PID를 사용합니다.
                        int webView2Pid = (int)(uint)mo["ProcessId"];

                        // 3. AutomationElement.FocusedElement를 사용하여 활성화된 요소를 가져옵니다.
                        AutomationElement focusedElement = AutomationElement.FocusedElement;


                        Console.WriteLine("focusedElement : " + focusedElement);



                        // 4. 포커스된 요소가 WebView2 프로세스에 속하는지 확인
                        //if (focusedElement != null && focusedElement.GetCurrentPropertyValue(AutomationElement.ProcessIdProperty).Equals(webView2Pid))
                        if (focusedElement != null)

                            {
                                Console.WriteLine("11111111 ");
                            object pattern;
                            // 5. TextPattern 지원 여부 확인
                            if (focusedElement.TryGetCurrentPattern(TextPattern.Pattern, out pattern))
                            {
                                TextPattern textPattern = (TextPattern)pattern;

                                // 6. 캐럿 위치(degenerate range) 또는 선택 영역(selection) 가져오기
                                TextPatternRange caretRange = null;
                                TextPatternRange[] selection = textPattern.GetSelection();

                                Console.WriteLine("222222222");

                                if (selection.Length > 0 && selection[0].GetBoundingRectangles().Length > 0)
                                {
                                    Console.WriteLine("33333333333");
                                    // 텍스트 선택 영역이 있는 경우 첫 번째 영역을 사용 (일반적으로 캐럿 위치를 찾을 때는 선택 영역이 0입니다.)
                                    caretRange = selection[0];
                                }
                                else
                                {
                                    Console.WriteLine("44444444");
                                    // 선택 영역이 없는 경우 (캐럿이 깜박이는 위치), 캐럿 위치의 텍스트 범위 가져오기
                                    // 주의: GetCaretRange()는 .NET Framework 4.5 이상에서 사용 가능합니다.
                                    // 이전 버전이거나 작동하지 않는 경우, GetVisibleRanges()를 사용하여 수동으로 추론해야 합니다.
                                    try
                                    {
                                        Console.WriteLine("A1");
                                        //caretRange = textPattern.GetCaretRange();
                                    }
                                    catch (NotSupportedException)
                                    {
                                        // GetCaretRange가 지원되지 않으면, 이 방법을 포기할 수 있습니다.
                                        Console.WriteLine("TextPattern.GetCaretRange is not supported or failed.");
                                    }
                                }

                                if (caretRange != null)
                                {
                                    // 7. 텍스트 범위의 바운딩 사각형(화면 좌표) 추출
                                    System.Windows.Rect[] bounds = caretRange.GetBoundingRectangles();

                                    if (bounds.Length > 0 && bounds[0].Width > 0 && bounds[0].Height > 0)
                                    {
                                        System.Windows.Rect caretRect = bounds[0];

                                        Console.WriteLine("A2");
                                        // 8. RECT 구조체로 변환하고 반환
                                        //rect.Left = (int)caretRect.Left;
                                        //rect.Top = (int)caretRect.Top;
                                        //rect.Right = (int)caretRect.Right;
                                        //rect.Bottom = (int)caretRect.Bottom;

                                        return true; // 성공적으로 캐럿 좌표 획득
                                    }
                                }
                            }
                        }

                        // --- [WEBVIEW WORKS] 부분에 UI 자동화 로직 삽입 종료 ---

                        // msedgewebview2.exe 프로세스는 찾았으나, 캐럿 획득에 실패한 경우
                        return false;
                    }
                }

                // msedgewebview2.exe 자식 프로세스를 찾지 못한 경우
                return false;
            }
            catch (Exception ex)
            {
                // 로깅 등 필요 시 추가
                Console.WriteLine($"TryGetCaretFromWebView2 Error: {ex.Message}");
                method = $"Error ({method})";
            }

            return false;
        }
    }
}