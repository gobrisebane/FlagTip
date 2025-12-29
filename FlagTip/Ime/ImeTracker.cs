using FlagTip.models;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using static FlagTip.Utils.NativeMethods;
using System.ComponentModel; // 추가


namespace FlagTip.Ime
{
    public class ImeTracker
    {
        private Mat _korDark;
        private Mat _engDark;


        public ImeTracker()
        {
            string basePath = AppDomain.CurrentDomain.BaseDirectory;

            _korDark = Cv2.ImRead(
                Path.Combine(basePath, "resources/ime/kor_dark.png"),
                ImreadModes.Grayscale);

            _engDark = Cv2.ImRead(
                Path.Combine(basePath, "resources/ime/eng_dark.png"),
                ImreadModes.Grayscale);
        }

        public ImeState DetectIme()
        {
            Bitmap captured = CaptureImeIcon();
            if (captured == null)
                return ImeState.UNKNOWN;

            using (Mat src = BitmapConverter.ToMat(captured))
            using (Mat gray = new Mat())
            {
                // 1️⃣ 그레이스케일
                Cv2.CvtColor(src, gray, ColorConversionCodes.BGR2GRAY);

                // 2️⃣ 살짝 블러
                Cv2.GaussianBlur(gray, gray, new OpenCvSharp.Size(3, 3), 0);

                if (Match(gray, _korDark))
                {
                    Console.WriteLine("KOR MATCH----");
                    return ImeState.KOR;
                }

                if (Match(gray, _engDark))
                {
                    Console.WriteLine("ENG MATCH----");
                    return ImeState.ENG;

                }
            }

            return ImeState.UNKNOWN;
        }

        private bool Match(Mat source, Mat template)
        {
            double[] scales = { 1.0, 1.25, 1.5 };

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

                        double minVal, maxVal;
                        OpenCvSharp.Point minLoc, maxLoc;

                        Cv2.MinMaxLoc(
                            result,
                            out minVal,
                            out maxVal,
                            out minLoc,
                            out maxLoc);

                        Console.WriteLine(
                            $"[IME] scale={scale:F2}, score={maxVal:F3}");

                        if (maxVal >= 0.7)
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
                Bitmap bmp = new Bitmap(clipped.Width, clipped.Height, PixelFormat.Format24bppRgb);
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

                // 🔽 🔽 🔽 캡처 이미지 저장 🔽 🔽 🔽
                SaveDebugCapture(bmp);

                return bmp;
            }
            catch (Win32Exception)
            {
                return null;
            }
        }

        private void SaveDebugCapture(Bitmap bmp)
        {
            try
            {
                string dir = Path.Combine(
                    AppDomain.CurrentDomain.BaseDirectory,
                    "debug_captures");

                Directory.CreateDirectory(dir);

                string file = Path.Combine(
                    dir,
                    $"ime_capture.png");

                bmp.Save(file, ImageFormat.Png);
                Console.WriteLine("[IME] capture saved: " + file);
            }
            catch (Exception ex)
            {
                Console.WriteLine("[IME] capture save failed: " + ex.Message);
            }
        }



    }
}
