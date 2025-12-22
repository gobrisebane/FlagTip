using System;
using System.Runtime.InteropServices;

using static FlagTip.Utils.NativeMethods;

namespace FlagTip.Ime
{
    internal static class TsfImeState
    {
        static readonly Guid CLSID_TF_InputProcessorProfiles =
            new Guid("33C53A50-F456-4884-B049-85FD643ECFED");

        static readonly Guid GUID_TFCAT_TIP_KEYBOARD =
            new Guid("34745C63-B2F0-4784-8B67-5E12C8701A31");


        static readonly Guid CLSID_TF_ThreadMgr =
            new Guid("529A9E6B-6587-4F23-AB9E-9C7D683E3C50");

        static readonly Guid GUID_COMPARTMENT_KEYBOARD_OPENCLOSE =
            new Guid("58273AAD-01BB-4164-95C6-755BAA0B516D");

        public static bool IsKoreanOpen_Foreground()
        {
            IntPtr hwnd = GetForegroundWindow();
            if (hwnd == IntPtr.Zero)
                return false;

            var type = Type.GetTypeFromCLSID(CLSID_TF_ThreadMgr);
            var threadMgr = (ITfThreadMgr)Activator.CreateInstance(type);

            threadMgr.Activate(out _);

            try
            {
                // 1️⃣ 포커스된 Document
                threadMgr.GetFocus(out ITfDocumentMgr docMgr);
                if (docMgr == null)
                    return false;

                // 2️⃣ Context 가져오기
                docMgr.GetTop(out IntPtr contextPtr);
                if (contextPtr == IntPtr.Zero)
                    return false;

                // ⭐ 3️⃣ ITfContext로 변환
                var context = (ITfContext)
                    Marshal.GetObjectForIUnknown(contextPtr);

                // ⭐ 4️⃣ Context → CompartmentMgr
                context.GetCompartmentMgr(out ITfCompartmentMgr compMgr);

                // ⭐ 5️⃣ KEYBOARD_OPENCLOSE 읽기
                Guid guid = GUID_COMPARTMENT_KEYBOARD_OPENCLOSE;
                compMgr.GetCompartment(ref guid, out ITfCompartment comp);

                comp.GetValue(out object value);

                Console.WriteLine("IME open value = " + value);

                return value != null && Convert.ToInt32(value) != 0;
            }
            finally
            {
                threadMgr.Deactivate();
                Marshal.ReleaseComObject(threadMgr);
            }
        }

        public static bool IsKoreanOpen()
        {
            // ThreadMgr 생성
            var type = Type.GetTypeFromCLSID(CLSID_TF_ThreadMgr);
            var threadMgr = (ITfThreadMgr)Activator.CreateInstance(type);

            // TSF 활성화
            threadMgr.Activate(out int clientId);

            try
            {
                // ThreadMgr → CompartmentMgr
                var compMgr = (ITfCompartmentMgr)threadMgr;

                Guid guid = GUID_COMPARTMENT_KEYBOARD_OPENCLOSE;
                compMgr.GetCompartment(ref guid, out ITfCompartment compartment);

                compartment.GetValue(out object value);

                if (value == null)
                    return false;

                // VARIANT → int
                int open = Convert.ToInt32(value);

                return open != 0; // 1 = 한글, 0 = 영문
            }
            finally
            {
                threadMgr.Deactivate();
                Marshal.ReleaseComObject(threadMgr);
            }
        }

        public static bool IsKorean()
        {
            var type = Type.GetTypeFromCLSID(CLSID_TF_InputProcessorProfiles);
            var profiles =
                (ITfInputProcessorProfiles)Activator.CreateInstance(type);

            // ⭐ ref 전달용 로컬 변수
            Guid catid = GUID_TFCAT_TIP_KEYBOARD;

            profiles.GetDefaultLanguageProfile(
                0x0412,   // Korean LANGID
                ref catid,
                out Guid clsid,
                out _);

            return clsid != Guid.Empty;
        }
    }
}