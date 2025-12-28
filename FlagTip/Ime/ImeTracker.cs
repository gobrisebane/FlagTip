using FlagTip.models;
using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using static FlagTip.Utils.NativeMethods;

namespace FlagTip.Ime
{
    public class ImeTracker
    {
        private Bitmap _bmpKor;
        private Bitmap _bmpEng;
        private Bitmap _bmpKorDark;
        private Bitmap _bmpEngDark;
        private Bitmap _bmpKorLight;
        private Bitmap _bmpEngLight;

        [DllImport("user32.dll")]
        static extern uint GetDpiForWindow(IntPtr hwnd);

        public ImeTracker()
        {
            // 절대 경로 기준으로 이미지 로드
            string basePath = AppDomain.CurrentDomain.BaseDirectory;

            string korDarkPath = Path.Combine(basePath, "resources/ime/kor_dark.png");
            string engDarkPath = Path.Combine(basePath, "resources/ime/eng_dark.png");
            string korLightPath = Path.Combine(basePath, "resources/ime/kor_dark.png");
            string engLightPath = Path.Combine(basePath, "resources/ime/eng_dark.png");

            _bmpKorDark = new Bitmap(korDarkPath);
            _bmpEngDark = new Bitmap(engDarkPath);
            _bmpKorLight = new Bitmap(korLightPath);
            _bmpEngLight = new Bitmap(engLightPath);
        }

        // 1️⃣ 캡쳐 + 2️⃣ 슬라이딩 비교 → 3️⃣ IME 상태 반환
        public ImeState DetectIme()
        {
            Console.WriteLine("------------ DETECT IME CALL");

            // 작업표시줄 핸들을 DPI 기준으로 사용 (캡쳐 대상과 동일)
            IntPtr hTaskbar = FindWindow("Shell_TrayWnd", null);
            if (hTaskbar == IntPtr.Zero)
            {
                Console.WriteLine("Taskbar not found");
                return ImeState.UNKNOWN;
            }

            Bitmap captured = CaptureImeIcon();
            if (captured == null)
                return ImeState.UNKNOWN;

            Point? korPos = FindTemplate(captured, _bmpKorDark, hTaskbar);
            Point? engPos = FindTemplate(captured, _bmpEngDark, hTaskbar);

            if (korPos.HasValue)
            {
                Console.WriteLine("--- KOR");
                return ImeState.KOR;
            }

            if (engPos.HasValue)
            {
                Console.WriteLine("--- ENG");
                return ImeState.ENG;
            }

            Console.WriteLine("--- NONE");
            return ImeState.UNKNOWN;
        }

        private bool IsTaskbarLightTheme()
        {
            const string keyPath =
        @"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize";

            using (RegistryKey key = Registry.CurrentUser.OpenSubKey(keyPath))
            {
                if (key == null)
                    return true; // 기본값은 Light

                object value = key.GetValue("SystemUsesLightTheme");
                if (value == null)
                    return true;

                return (int)value == 1;
            }
        }

        // 캡쳐만 수행
        private Bitmap CaptureImeIcon()
        {
            // 작업표시줄 찾기
            IntPtr hTaskbar = FindWindow("Shell_TrayWnd", null);
            if (hTaskbar == IntPtr.Zero)
            {
                Console.WriteLine("Taskbar not found");
                return null;
            }

            if (!GetWindowRect(hTaskbar, out RECT taskbar))
            {
                Console.WriteLine("Failed to get taskbar rect");
                return null;
            }

            int width = 250;
            int height = taskbar.height - 8;
            int x = taskbar.right - 280;
            int y = taskbar.top + 4;

            Bitmap bmp = new Bitmap(width, height);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.CopyFromScreen(x, y, 0, 0, bmp.Size);
            }

            // 선택 사항: 캡쳐 파일 저장
            bmp.Save("ime_capture.png", ImageFormat.Png);
            Console.WriteLine("IME icon captured: ime_capture.png");

            return bmp;
        }

        private Point? FindTemplate(Bitmap bigBmp, Bitmap smallBmp, IntPtr hwndForDpi)
        {
            // 1️⃣ DPI scale 계산 (딱 한 번)
            float scale = GetDpiForWindow(hwndForDpi) / 96f;
            Console.WriteLine("CURRENT scale : " + scale);

            Bitmap template = smallBmp;
            bool shouldDisposeTemplate = false;

            try
            {
                // 2️⃣ template DPI 리사이즈 (딱 한 번)
                if (Math.Abs(scale - 1.0f) > 0.01f)
                {
                    template = new Bitmap(
                        (int)(smallBmp.Width * scale),
                        (int)(smallBmp.Height * scale));

                    shouldDisposeTemplate = true;

                    using (Graphics g = Graphics.FromImage(template))
                    {
                        g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                        g.DrawImage(
                            smallBmp,
                            0, 0,
                            template.Width,
                            template.Height);
                    }
                }

                // 3️⃣ 기존 로직 그대로 (template만 교체)
                int w = bigBmp.Width - template.Width;
                int h = bigBmp.Height - template.Height;

                for (int y = 0; y <= h; y++)
                {
                    for (int x = 0; x <= w; x++)
                    {
                        if (IsMatch(bigBmp, template, x, y))
                            return new Point(x, y);
                    }
                }

                return null;
            }
            finally
            {
                if (shouldDisposeTemplate && template != null)
                    template.Dispose();
            }
        }

        private unsafe bool IsMatch(
    Bitmap big,
    Bitmap small,
    int offsetX,
    int offsetY,
    int tolerance = 130)
        {
            Rectangle rectBig = new Rectangle(0, 0, big.Width, big.Height);
            Rectangle rectSmall = new Rectangle(0, 0, small.Width, small.Height);

            BitmapData bdBig = big.LockBits(rectBig, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            BitmapData bdSmall = small.LockBits(rectSmall, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

            try
            {
                byte* ptrBig = (byte*)bdBig.Scan0;
                byte* ptrSmall = (byte*)bdSmall.Scan0;

                int strideBig = bdBig.Stride;
                int strideSmall = bdSmall.Stride;

                for (int y = 0; y < small.Height; y++)
                {
                    byte* rowBig = ptrBig + (y + offsetY) * strideBig + offsetX * 4;
                    byte* rowSmall = ptrSmall + y * strideSmall;

                    for (int x = 0; x < small.Width; x++)
                    {
                        // small (template)
                        byte b2 = rowSmall[0];
                        byte g2 = rowSmall[1];
                        byte r2 = rowSmall[2];

                        // ✅ 핵심 추가: 템플릿의 흰 배경은 무시
                        if (r2 > 240 && g2 > 240 && b2 > 240)
                        {
                            rowBig += 4;
                            rowSmall += 4;
                            continue;
                        }

                        // big (captured)
                        byte b1 = rowBig[0];
                        byte g1 = rowBig[1];
                        byte r1 = rowBig[2];

                        if (Math.Abs(r1 - r2) > tolerance ||
                            Math.Abs(g1 - g2) > tolerance ||
                            Math.Abs(b1 - b2) > tolerance)
                            return false;

                        rowBig += 4;
                        rowSmall += 4;
                    }
                }

                return true;
            }
            finally
            {
                big.UnlockBits(bdBig);
                small.UnlockBits(bdSmall);
            }
        }
    }
}