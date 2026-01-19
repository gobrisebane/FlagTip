using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
/*
namespace FlagTip.Input.Tsf
{
    [ComImport]
    [Guid("AA80E801-2021-11D2-93E0-0060B067B86E")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface ITfThreadMgr
    {
        int Activate(out int clientId);
        int Deactivate();
        int GetFocus(out IntPtr context);
    }

    [ComImport]
    [Guid("7DCF57AC-18AD-438B-824D-979BFFB74B7C")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface ITfCompartmentMgr
    {
        int GetCompartment(ref Guid rguid, out ITfCompartment compartment);
    }

    [ComImport]
    [Guid("BB08F7A9-607A-4384-8623-056892B64371")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface ITfCompartment
    {
        int GetValue(out VARIANT value);
    }
}

[StructLayout(LayoutKind.Explicit)]
internal struct VARIANT
{
    [FieldOffset(0)]
    public ushort vt;

    [FieldOffset(2)]
    public ushort wReserved1;
    [FieldOffset(4)]
    public ushort wReserved2;
    [FieldOffset(6)]
    public ushort wReserved3;

    // union
    [FieldOffset(8)]
    public int lVal;

    [FieldOffset(8)]
    public IntPtr ptr;
}
*/