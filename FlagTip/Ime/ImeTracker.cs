using FlagTip.models;
using FlagTip.Utils;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using System;
using System.ComponentModel; // 추가
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using static FlagTip.Utils.CommonUtils;
using static FlagTip.Utils.NativeMethods;
using static FlagTip.Ime.ImeManager;

using static System.Windows.Forms.VisualStyles.VisualStyleElement;


namespace FlagTip.Ime
{

  





    public class ImeTracker
    {

        
        private Mat _korEdge;
        private Mat _engEdge;

        

        public ImeTracker()
        {
            string basePath = AppDomain.CurrentDomain.BaseDirectory;


            Mat kernel = Cv2.GetStructuringElement(
                MorphShapes.Rect,
                new OpenCvSharp.Size(2, 2));
            /*
                        _korEdge = Cv2.ImRead(
                            Path.Combine(basePath, "resources/ime/kor_dark.png"),
                            ImreadModes.Grayscale);
                        Cv2.Canny(_korEdge, _korEdge, 20, 80);
                        Cv2.Dilate(_korEdge, _korEdge, kernel);   // ⭐ 핵심

                        _engEdge = Cv2.ImRead(
                            Path.Combine(basePath, "resources/ime/eng_dark.png"),
                            ImreadModes.Grayscale);
                        Cv2.Canny(_engEdge, _engEdge, 20, 80);
                        Cv2.Dilate(_engEdge, _engEdge, kernel);   // ⭐ 핵심
            */


            _korEdge = Cv2.ImRead(
                Path.Combine(basePath, "resources/ime/kor_edge.png"),
                ImreadModes.Grayscale);

            if (_korEdge.Empty())
                throw new Exception("kor_edge.png 로드 실패");

            // --- ENG EDGE 템플릿 (미리 생성된 파일) ---
            _engEdge = Cv2.ImRead(
                Path.Combine(basePath, "resources/ime/eng_edge.png"),
                ImreadModes.Grayscale);






            if (_engEdge.Empty())
                throw new Exception("eng_edge.png 로드 실패");



        }


    

        public ImeState DetectIme()
        {

            
            ImeState imeResult = ImeState.UNKNOWN;

            if (IsProcessBrowserApp())
            {
                Console.WriteLine("A1. CHROME BROWSER");

                imeResult = WindowsImeDetector.GetWindowsImeState();

            
            } else
            {

                Console.WriteLine("A2. NOT BROWSER");

                //SaveTemplateEdge(_korEdge, "kor_edge.png");
                //SaveTemplateEdge(_engEdge, "eng_edge.png");

                Bitmap captured = CaptureImeIcon();
                if (captured == null)
                    return WindowsImeDetector.GetWindowsImeState();

                using (Mat src = BitmapConverter.ToMat(captured))
                using (Mat gray = new Mat())
                using (Mat edges = new Mat())
                {
                    Cv2.CvtColor(src, gray, ColorConversionCodes.BGR2GRAY);
                    Cv2.Canny(gray, edges, 20, 80);

                    var kernel = Cv2.GetStructuringElement(
                    MorphShapes.Rect,
                    new OpenCvSharp.Size(2, 2));
                    Cv2.Dilate(edges, edges, kernel);

                    // 🔍 디버그용 저장 
                    //SaveDebugCapture(src);

                    if (Match(edges, _engEdge, "eng"))
                    {
                        if (CommonUtils.IsCapsLockOn())
                            imeResult = ImeState.ENG_UP;
                        else
                            imeResult = ImeState.ENG_LO;
                    }
                    else if (Match(edges, _korEdge, "kor"))
                    {
                        imeResult = ImeState.KOR;
                    }
                    else
                    {
                        imeResult = WindowsImeDetector.GetWindowsImeState();
                    }
                }
            }



            //ImeState imeResult = ImeState.UNKNOWN;
            //imeResult = WindowsImeDetector.GetWindowsImeState();


            //ImeState imeResult = ImeState.UNKNOWN;
            //imeResult = WindowsImeDetector.GetImeMode();


            //ImeState imeResult = ImeState.UNKNOWN;
            //imeResult = ImeManager.GetChromeImeMode();



            return imeResult;
        }



        









        private bool Match(Mat source, Mat template, string name)
        {
            if (source is null || source.Empty()) return false;
            if (template is null || template.Empty()) return false;



            //double[] scales = { 1.0, 1.35};
            double[] scales = { 1.0 };

            foreach (double scale in scales)
            {
                var w = (int)(template.Width * scale);
                var h = (int)(template.Height * scale);
                if (w <= 0 || h <= 0) continue;

                using (Mat resized = template.Resize(new OpenCvSharp.Size(w, h)))
                {
                    if (resized.Width >= source.Width || resized.Height >= source.Height)
                        continue;

                    using (Mat result = new Mat())
                    {
                        Cv2.MatchTemplate(source, resized, result, TemplateMatchModes.CCoeffNormed);
                        Cv2.MinMaxLoc(result, out _, out double maxVal, out _, out _);

                        
/*                        Console.WriteLine(
                         $"[IME] method={name} scale={scale:F2}, score={maxVal:F3}");*/
                        

                          if (maxVal >= 0.65)
                            return true;
                    }
                }
            }
            return false;
        }

        private Bitmap CaptureImeIcon()
        {
            IntPtr hTaskbar = FindWindow("Shell_TrayWnd", null);
            if (hTaskbar == IntPtr.Zero) return null;

            IntPtr hTray = FindWindowEx(hTaskbar, IntPtr.Zero, "TrayNotifyWnd", null);

            RECT r;
            if (hTray != IntPtr.Zero)
            {
                if (!GetWindowRect(hTray, out r)) return null;
            }
            else
            {
                if (!GetWindowRect(hTaskbar, out r)) return null;
            }

            // ⭐ DPI 스케일 (Taskbar 기준)
            float dpiScale = GetDpiForWindow(hTaskbar) / 96f;

            // 100% 기준 값들
            int logicalWidth = 250;
            int logicalOffset = 280;
            int logicalTopPadding = 4;
            int logicalHeightPadding = 8;

            int width = (int)(logicalWidth * dpiScale);
            int height = Math.Max(1, r.height - (int)(logicalHeightPadding * dpiScale));

            int x = r.right - (int)(logicalOffset * dpiScale);
            int y = r.top + (int)(logicalTopPadding * dpiScale);

            Rectangle virtualScreen = SystemInformation.VirtualScreen;
            Rectangle wanted = new Rectangle(x, y, width, height);
            Rectangle clipped = Rectangle.Intersect(wanted, virtualScreen);

            if (clipped.Width <= 0 || clipped.Height <= 0)
                return null;

            try
            {
                Bitmap bmp = new Bitmap(
                    clipped.Width,
                    clipped.Height,
                    PixelFormat.Format24bppRgb);

                using (Graphics g = Graphics.FromImage(bmp))
                {
                    g.CopyFromScreen(
                        clipped.Left,
                        clipped.Top,
                        0,
                        0,
                        bmp.Size,
                        CopyPixelOperation.SourceCopy);
                }

                bmp = NormalizeToDpi1(bmp, dpiScale);

                return bmp;
            }
            catch (Win32Exception)
            {
                return null;
            }
        }

        private Bitmap NormalizeToDpi1(Bitmap src, float dpiScale)
        {
            if (Math.Abs(dpiScale - 1.0f) < 0.01f)
                return src;

            int w = (int)(src.Width / dpiScale);
            int h = (int)(src.Height / dpiScale);

            Bitmap dst = new Bitmap(w, h, PixelFormat.Format24bppRgb);
            dst.SetResolution(96, 96);   // ⭐ 핵심

            using (Graphics g = Graphics.FromImage(dst))
            {
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                g.DrawImage(src, 0, 0, w, h);
            }

            src.Dispose();
            return dst;
        }

        private void SaveDebugCapture(Mat src)
        {
            try
            {
                string dir = Path.Combine(
                    AppDomain.CurrentDomain.BaseDirectory,
                    "debug_captures");

                Directory.CreateDirectory(dir);
                Cv2.ImWrite(Path.Combine(dir, "ime_capture.png"), src);
            }
            catch { }
        }

        private void SaveDebugEdge(Mat edge)
        {
            try
            {
                string dir = Path.Combine(
                    AppDomain.CurrentDomain.BaseDirectory,
                    "debug_captures");

                Directory.CreateDirectory(dir);
                Cv2.ImWrite(Path.Combine(dir, "ime_edge.png"), edge);
            }
            catch { }
        }
        private void SaveTemplateEdge(Mat edge, string fileName)
        {
            try
            {
                string dir = Path.Combine(
                    AppDomain.CurrentDomain.BaseDirectory,
                    "debug_captures");

                Directory.CreateDirectory(dir);

                Cv2.ImWrite(Path.Combine(dir, fileName), edge);
            }
            catch { }
        }




      



    }
}


