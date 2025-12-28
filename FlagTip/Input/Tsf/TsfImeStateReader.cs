using FlagTip.models;
using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

namespace FlagTip.Input.Tsf
{
    internal static class TsfImeStateReader
    {
        internal static ImeState GetTsfImeState()
        {
            var threadMgr = (ITfThreadMgr)Activator.CreateInstance(
                Type.GetTypeFromCLSID(TsfGuids.CLSID_TF_ThreadMgr));

            threadMgr.Activate(out _);

            var compartmentMgr = (ITfCompartmentMgr)threadMgr;

            Guid guid = TsfGuids.KEYBOARD_OPENCLOSE;

            compartmentMgr.GetCompartment(ref guid, out ITfCompartment compartment);

            Console.WriteLine("test1");

            VARIANT v;
            Console.WriteLine("test2");

            int hr = compartment.GetValue(out v);
            Console.WriteLine("test3");   // ✅ 이제 반드시 찍힘

            threadMgr.Deactivate();
            Console.WriteLine("test4");

            if (hr != 0)
                return ImeState.ENG;

            bool imeOn =
                v.vt == (short)VarEnum.VT_I4 &&
                v.lVal != 0;

            return imeOn ? ImeState.KOR : ImeState.ENG;
        }
    }
}