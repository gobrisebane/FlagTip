using System;
using System.Runtime.InteropServices;

namespace FlagTip.Ime
{


    [ComImport]
    [Guid("AA80E803-2021-11D2-93E0-0060B067B86E")] // ⭐ 중요
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface ITfContext
    {
        void RequestEditSession();
        void InWriteSession();
        void GetSelection();
        void GetStatus();
        void GetEnd();
        void GetActiveView();
        void EnumViews();
        void GetDocumentMgr();
        void CreateRangeBackup();
        void GetText();
        void SetText();
        void GetFormattedText();
        void GetEmbedded();
        void InsertEmbedded();
        void InsertTextAtSelection();
        void InsertEmbeddedAtSelection();
        void GetSelectionStatus();
        void GetProperty();
        void GetCompartmentMgr(out ITfCompartmentMgr compMgr); // ⭐ 이게 핵심
    }

    [ComImport]
    [Guid("AA80E801-2021-11D2-93E0-0060B067B86E")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface ITfThreadMgr
    {
        void Activate(out int clientId);
        void Deactivate();
        void GetFocus(out ITfDocumentMgr docMgr);
    }

    [ComImport]
    [Guid("1F02B6C5-7842-4EE6-8A0B-9A24183A95CA")] // ⭐ 중요
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface ITfInputProcessorProfiles
    {
        void Register(ref Guid rclsid);
        void Unregister(ref Guid rclsid);

        void AddLanguageProfile(
            ref Guid rclsid,
            short langid,
            ref Guid guidProfile,
            [MarshalAs(UnmanagedType.LPWStr)] string desc,
            int cchDesc,
            [MarshalAs(UnmanagedType.LPWStr)] string iconFile,
            int cchFile,
            int iconIndex);

        void RemoveLanguageProfile(
            ref Guid rclsid,
            short langid,
            ref Guid guidProfile);

        void EnumInputProcessorInfo(out IntPtr enumIPP);

        void GetDefaultLanguageProfile(
            short langid,
            ref Guid catid,
            out Guid clsid,
            out Guid profileGuid);
    }

    [ComImport]
    [Guid("AA80E7F4-2021-11D2-93E0-0060B067B86E")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface ITfDocumentMgr
    {
        void CreateContext(
            int clientId,
            uint flags,
            IntPtr punk,
            out IntPtr context,
            out int editCookie);

        void Push(IntPtr context);
        void Pop(uint flags);
        void GetTop(out IntPtr context);
        void GetBase(out IntPtr context);
    }


    [ComImport]
    [Guid("7DCF57AC-18AD-438B-824D-979BFFB74B7C")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface ITfCompartmentMgr
    {
        void GetCompartment(
            ref Guid rguid,
            out ITfCompartment compartment);
    }
    [ComImport]
    [Guid("BB08F7A9-607A-4384-8623-056892B64371")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface ITfCompartment
    {
        void SetValue(int tid, ref object value);
        void GetValue(out object value);
    }

}