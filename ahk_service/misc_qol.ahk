#Requires AutoHotkey v2.0

^!t:: 
{
    if !EnableTerminalLaunch
        return
    try {
        Run("cmd.exe", TerminalStartPath)
    }
    catch Error as err {
        ToolTip("Failed to launch Terminal: " . err.Message)
        SetTimer(() => ToolTip(), -2000)
    }
}

$Launch_App2::
{
    if !EnableCalcInstance
        return

    if WinExist("ahk_class CalcFrame") {
        WinActivate("ahk_class CalcFrame")
    } 
    else if WinExist("ahk_exe CalculatorApp.exe") {
        WinActivate("ahk_exe CalculatorApp.exe")
    }
    else if WinExist("ahk_exe Calculator.exe") {
        WinActivate("ahk_exe Calculator.exe")
    }
    else if WinExist("Calculator ahk_class ApplicationFrameWindow") {
        WinActivate("Calculator ahk_class ApplicationFrameWindow")
    }
    else {
        Run("calc.exe")
    }
}


#HotIf

^!v:: 
{
    if !EnableRedactedPaste
        return

    rawText := A_Clipboard
    if (rawText = "")
        return

    cleanText := rawText
    
    for item in RedactedReplacements {
        cleanText := StrReplace(cleanText, item["dirty"], item["clean"])
    }

    A_Clipboard := "" 
    DllCall("OpenClipboard", "ptr", A_ScriptHwnd)
    DllCall("EmptyClipboard")
    
    hMem := DllCall("GlobalAlloc", "uint", 0x42, "ptr", (StrLen(cleanText) + 1) * 2, "ptr")
    pMem := DllCall("GlobalLock", "ptr", hMem, "ptr")
    StrPut(cleanText, pMem, "UTF-16")
    DllCall("GlobalUnlock", "ptr", hMem)
    
    DllCall("SetClipboardData", "uint", 13, "ptr", hMem)
    DllCall("CloseClipboard")

    Send("^v")
}

$PrintScreen::
{
    if !EnableScreenshotTool
        return

    static isCapturing := false
    if (isCapturing)
        return
    isCapturing := true

    DllCall("SetThreadDpiAwarenessContext", "ptr", -3)

    X := SysGet(76) ; SM_XVIRTUALSCREEN
    Y := SysGet(77) ; SM_YVIRTUALSCREEN
    W := SysGet(78) ; SM_CXVIRTUALSCREEN
    H := SysGet(79) ; SM_CYVIRTUALSCREEN

    if (!DirExist(ScreenshotTargetDir))
        DirCreate(ScreenshotTargetDir)

    tempFile := ScreenshotTargetDir . "\~temp_capture_" . A_TickCount . ".png"
    SaveScreenToDisk(X, Y, W, H, tempFile)

    InputBoxObj := InputBox("Enter Screenshot file name", "Save Screenshot", "w400 h130")
    
    if (InputBoxObj.Result = "Cancel" || InputBoxObj.Value == "") {
        if FileExist(tempFile)
            FileDelete(tempFile)
        ToolTip("Screenshot Discarded.")
        SetTimer(() => ToolTip(), -1500)
        isCapturing := false
        return
    }
    
    SanitizedName := RegExReplace(InputBoxObj.Value, '[\\/:*?"<>|]', '_')
    TargetFile := ScreenshotTargetDir . "\" . SanitizedName . ".png"
    
    if FileExist(TargetFile) {
        epochTimestamp := DateDiff(A_NowUTC, "19700101000000", "Seconds")
        TargetFile := ScreenshotTargetDir . "\" . SanitizedName . "_" . epochTimestamp . ".png"
    }

    try {
        FileMove(tempFile, TargetFile, 1)
        ToolTip("Saved: " . TargetFile)
    } catch {
        ToolTip("Error: Could not rename file.")
    }
    
    SetTimer(() => ToolTip(), -2000)
    isCapturing := false
}

SaveScreenToDisk(X, Y, W, H, TargetPath) {
    hGdiPlus := DllCall("LoadLibrary", "Str", "gdiplus.dll", "Ptr")
    if !hGdiPlus
        return false

    GDIPToken := 0
    SI := Buffer(24, 0)
    NumPut("UInt", 1, SI)
    
    status := DllCall("gdiplus\GdiplusStartup", "Ptr*", &GDIPToken, "Ptr", SI, "Ptr", 0)
    if (status != 0) {
        DllCall("FreeLibrary", "Ptr", hGdiPlus)
        return false
    }
    
    HDC := DllCall("GetDC", "Ptr", 0, "Ptr")
    CDC := DllCall("CreateCompatibleDC", "Ptr", HDC, "Ptr")
    HBM := DllCall("CreateCompatibleBitmap", "Ptr", HDC, "Int", W, "Int", H, "Ptr")
    OBM := DllCall("SelectObject", "Ptr", CDC, "Ptr", HBM, "Ptr")
    DllCall("BitBlt", "Ptr", CDC, "Int", 0, "Int", 0, "Int", W, "Int", H, "Ptr", HDC, "Int", X, "Int", Y, "UInt", 0x00CC0020)
    
    pBitmap := 0
    DllCall("gdiplus\GdipCreateBitmapFromHBITMAP", "Ptr", HBM, "Ptr", 0, "Ptr*", &pBitmap)
    
    CLSID := Buffer(16)
    DllCall("ole32\CLSIDFromString", "WStr", "{557CF406-1A04-11D3-9A73-0000F81EF32E}", "Ptr", CLSID)
    
    DllCall("gdiplus\GdipSaveImageToFile", "Ptr", pBitmap, "WStr", TargetPath, "Ptr", CLSID, "Ptr", 0)
    
    DllCall("gdiplus\GdipDisposeImage", "Ptr", pBitmap)
    DllCall("SelectObject", "Ptr", CDC, "Ptr", OBM)
    DllCall("DeleteObject", "Ptr", HBM)
    DllCall("DeleteDC", "Ptr", CDC)
    DllCall("ReleaseDC", "Ptr", 0, "Ptr", HDC)
    
    DllCall("gdiplus\GdiplusShutdown", "Ptr", GDIPToken)
    DllCall("FreeLibrary", "Ptr", hGdiPlus)
    
    return true
}