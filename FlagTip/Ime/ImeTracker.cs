using FlagTip.models;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using System;
using System.ComponentModel; // 추가
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using static FlagTip.Utils.NativeMethods;
using System.Diagnostics;


namespace FlagTip.Ime
{
    public class ImeTracker
    {
        private Mat _korEdge;
        private Mat _engEdge;


        public ImeTracker()
        {
            string basePath = AppDomain.CurrentDomain.BaseDirectory;

            // 🔑 입력과 동일한 커널
            Mat kernel = Cv2.GetStructuringElement(
                MorphShapes.Rect,
                new OpenCvSharp.Size(2, 2));

            // --- KOR 템플릿 ---
            _korEdge = Cv2.ImRead(
                Path.Combine(basePath, "resources/ime/kor_dark.png"),
                ImreadModes.Grayscale);

            Cv2.Canny(_korEdge, _korEdge, 20, 80);
            Cv2.Dilate(_korEdge, _korEdge, kernel);   // ⭐ 핵심

            // --- ENG 템플릿 ---
            _engEdge = Cv2.ImRead(
                Path.Combine(basePath, "resources/ime/eng_dark.png"),
                ImreadModes.Grayscale);

            Cv2.Canny(_engEdge, _engEdge, 20, 80);
            Cv2.Dilate(_engEdge, _engEdge, kernel);   // ⭐ 핵심

        }

        public ImeState DetectIme()
        {

            SaveTemplateEdge(_korEdge, "kor_edge.png");
            SaveTemplateEdge(_engEdge, "eng_edge.png");


            Bitmap captured = CaptureImeIcon();
            if (captured == null)
                return ImeState.UNKNOWN;

            using (Mat src = BitmapConverter.ToMat(captured))
            using (Mat gray = new Mat())
            using (Mat edges = new Mat())
            {
                Cv2.CvtColor(src, gray, ColorConversionCodes.BGR2GRAY);
                //Cv2.Canny(gray, edges, 40, 120);
                Cv2.Canny(gray, edges, 20, 80);
                //Cv2.Canny(gray, edges, 10, 50);


                var kernel = Cv2.GetStructuringElement(
                MorphShapes.Rect,
                new OpenCvSharp.Size(2, 2));

                Cv2.Dilate(edges, edges, kernel);



                // 🔍 디버그용 저장 (여기!)
                SaveDebugCapture(src);
                SaveDebugEdge(edges);

                if (Match(edges, _korEdge,"kor"))
                {
                    return ImeState.KOR;
                }

                if (Match(edges, _engEdge,"eng"))
                {
                    return ImeState.ENG;
                }
            }








            return ImeState.UNKNOWN;
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










        private bool Match(Mat source, Mat template, String name)
        {


            double[] scales = { 1.0, 1.35};

            foreach (double scale in scales)
            {
                using (Mat resized = template.Resize(
                    new OpenCvSharp.Size(
                        (int)(template.Width * scale),
                        (int)(template.Height * scale))))
                {
                    if (resized.Width >= source.Width ||
                        resized.Height >= source.Height)
                        continue;

                    using (Mat result = new Mat())
                    {
                        Cv2.MatchTemplate(
                            source,
                            resized,
                            result,
                            TemplateMatchModes.CCoeffNormed);

                        Cv2.MinMaxLoc(
                            result,
                            out double minVal,
                            out double maxVal,
                            out _,
                            out _);

                        Console.WriteLine("source : " + name);
                        Console.WriteLine(
                            $"[IME-EDGE] scale={scale:F2}, score={maxVal:F3}");

                        // ⭐ Edge는 점수가 낮다
                        if (maxVal >= 0.45)

                        
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

            int width = 250;
            int height = Math.Max(1, r.height - 8);
            int x = r.right - 280;
            int y = r.top + 4;

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

                return bmp; // 🔴 저장/가공 ❌
            }
            catch (Win32Exception)
            {
                return null;
            }
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



    }
}
