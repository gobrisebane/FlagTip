using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace FlagTip.Input.Tsf
{
    public static class JapaneseImeTsfHelper
    {
        // ===============================
        // GUID / CONST
        // ===============================

        private static readonly Guid CLSID_TF_ThreadMgr =
            new Guid("529A9E6B-6587-4F23-AB9E-9C7D683E3C50");

        private static readonly Guid GUID_COMPARTMENT_CONVERSIONMODE =
            new Guid("5562ebb0-2d09-11d3-9cdd-00805f0c1fcd");

        private const int TF_CONVERSIONMODE_NATIVE = 0x0001; // あ (Japanese Native)

        // ===============================
        // PUBLIC API
        // ===============================

        /// <summary>
        /// 일본어 IME에서 현재 상태가
        /// true  = 일본어 (あ, Native)
        /// false = 영어 (A, Alphanumeric)
        /// </summary>
        [STAThread]
        public static bool IsJapaneseNative()
        {



            ITfThreadMgr threadMgr = null;

            try
            {
                threadMgr = (ITfThreadMgr)Activator.CreateInstance(
                    Type.GetTypeFromCLSID(CLSID_TF_ThreadMgr));

                threadMgr.Activate(out _);

                threadMgr.GetFocus(out ITfContext context);
                if (context == null)
                    return false;

                var compMgr = (ITfCompartmentMgr)context;

                Guid guid = GUID_COMPARTMENT_CONVERSIONMODE;

                compMgr.GetCompartment(
                    ref guid,
                    out ITfCompartment compartment);

                compartment.GetValue(out object value);

                if (value is int mode)
                {
                    return (mode & TF_CONVERSIONMODE_NATIVE) != 0;
                }
            }
            catch
            {
                // TSF 실패 시 false 반환
            }
            finally
            {
                if (threadMgr != null)
                {
                    try { threadMgr.Deactivate(); } catch { }
                }
            }

            return false;
        }
    }

    // ===============================
    // COM INTERFACES
    // ===============================

    [ComImport]
    [Guid("AA80E801-2021-11D2-93E0-0060B067B86E")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    interface ITfThreadMgr
    {
        void Activate(out int clientId);
        void Deactivate();
        void GetFocus(out ITfContext context);
    }

    [ComImport]
    [Guid("AA80E7FF-2021-11D2-93E0-0060B067B86E")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    interface ITfContext
    {
    }

    [ComImport]
    [Guid("7DCF57AC-18AD-438B-824D-979BFFB74B7C")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    interface ITfCompartmentMgr
    {
        void GetCompartment(ref Guid rguid, out ITfCompartment compartment);
    }

    [ComImport]
    [Guid("BB08F7A9-607A-4384-8623-056892B64371")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    interface ITfCompartment
    {
        void GetValue(out object value);
    }
}