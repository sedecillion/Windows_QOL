global IsWaiting := false
global FullScreenHBM := 0

SetupScreenshotTool() {
    if EnableScreenshotTool && ScreenshotTriggerKey
        Hotkey(ScreenshotTriggerKey, CaptureScreenshot)
}


; main trigger
CaptureScreenshot(HotkeyName) {
    global IsWaiting, FullScreenHBM, ScreenshotFullScreenModeOnly
    if IsWaiting
        return
    
    ;ShowTip("Triggered: Capturing full screen to memory...", 1000)
    
    FullScreenHBM := CaptureScreenToHBITMAP()
    if !FullScreenHBM {
        ShowTip("Error: Failed to capture screen to memory.", 2000)
        return
    }

    if ScreenshotFullScreenModeOnly {
        ProcessAndSaveHBITMAP(FullScreenHBM)
        
        DllCall("DeleteObject", "ptr", FullScreenHBM)
        FullScreenHBM := 0
        return
    }

    IsWaiting := true
    A_Clipboard := ""
    
    ;ShowTip("Summoning Snipping Tool...", 2000)
    Send "#+s"
    
    if !WinWaitActive("ahk_class XamlWindow ahk_exe SnippingTool.exe",, 3) {
        ShowTip("Error: Couldn't properly hook to snipping tool. Try Again.", 2000)
        CleanupState()
        return
    }

    ShowTip("Press Enter to snip FullScreen. Escape to cancel", 2000)
    ; register clipboard changeed hook
    OnClipboardChange ClipChanged

    ; poll for active window 
    SetTimer MonitorWindow, 50
}


; resolved to enter - user wants a FullScreen snippet
#HotIf IsWaiting && WinActive("ahk_class XamlWindow ahk_exe SnippingTool.exe")
Enter:: {
    global IsWaiting, FullScreenHBM
    IsWaiting := false
    SetTimer MonitorWindow, 0
    OnClipboardChange ClipChanged, 0
    SetTimer DelayedCleanup, 0
    
    ShowTip("FullScreen Screenshot captured.", 1000)
    Send "{Esc}"

    ProcessAndSaveHBITMAP(FullScreenHBM)
    
    DllCall("DeleteObject", "ptr", FullScreenHBM)
    FullScreenHBM := 0
}
#HotIf


; resovled to: screnshot taken by snipping tool
ClipChanged(DataType) {
    global IsWaiting, FullScreenHBM
    if !IsWaiting || DataType != 2
        return

    IsWaiting := false
    SetTimer MonitorWindow, 0
    SetTimer DelayedCleanup, 0
    OnClipboardChange ClipChanged, 0
    
    ;ShowTip("Clipboard updated: Processing partial snip.", 2000)

    if FullScreenHBM {
        DllCall("DeleteObject", "ptr", FullScreenHBM)
        FullScreenHBM := 0
    }

    Sleep 250
    SaveClipboardImage()
}


; resolved to : cancelled
MonitorWindow() {
    global IsWaiting

    ; this should not happen usually, it this func ishalf way executing
    ; and clipboard changed was fired it can continue execution here.
    if !IsWaiting
        return

    if !WinActive("ahk_class XamlWindow ahk_exe SnippingTool.exe") {
        SetTimer MonitorWindow, 0
        ;ShowTip("Overlay closed. Waiting 1.5s for clipboard data...", 1500)
        SetTimer DelayedCleanup, -1500 
    }
}

DelayedCleanup() {
    global IsWaiting
    if !IsWaiting
        return
    ;ShowTip("Timeout/Cancel: Cleaning up state.", 2000)
    CleanupState()
}

CleanupState() {
    global IsWaiting, FullScreenHBM
    IsWaiting := false
    SetTimer MonitorWindow, 0
    SetTimer DelayedCleanup, 0
    OnClipboardChange ClipChanged, 0
    
    if FullScreenHBM {
        DllCall("DeleteObject", "ptr", FullScreenHBM)
        FullScreenHBM := 0
    }
}



CaptureScreenToHBITMAP() {
    DllCall("SetThreadDpiAwarenessContext", "ptr", -3)
    X := SysGet(76)
    Y := SysGet(77)
    W := SysGet(78)
    H := SysGet(79)

    HDC := DllCall("GetDC", "ptr", 0, "ptr")
    CDC := DllCall("CreateCompatibleDC", "ptr", HDC, "ptr")
    HBM := DllCall("CreateCompatibleBitmap", "ptr", HDC, "int", W, "int", H, "ptr")
    OBM := DllCall("SelectObject", "ptr", CDC, "ptr", HBM, "ptr")
    
    DllCall("BitBlt", "ptr", CDC, "int", 0, "int", 0, "int", W, "int", H, "ptr", HDC, "int", X, "int", Y, "uint", 0x00CC0020)
    
    DllCall("SelectObject", "ptr", CDC, "ptr", OBM)
    DllCall("DeleteDC", "ptr", CDC)
    DllCall("ReleaseDC", "ptr", 0, "ptr", HDC)
    
    return HBM
}

SaveClipboardImage() {
    if !DllCall("IsClipboardFormatAvailable", "uint", 2) {
        ShowTip("Error: No image format in clipboard.", 2000)
        return
    }
    if !DllCall("OpenClipboard", "ptr", 0) {
        ShowTip("Error: Could not open clipboard.", 2000)
        return
    }
    
    hBitmap := DllCall("GetClipboardData", "uint", 2, "ptr")
    if !hBitmap {
        DllCall("CloseClipboard")
        ShowTip("Error: Could not get clipboard data.", 2000)
        return
    }

    ProcessAndSaveHBITMAP(hBitmap)
    DllCall("CloseClipboard")
}

ProcessAndSaveHBITMAP(hBitmap) {
    global ScreenshotTargetDir
    
    if (!DirExist(ScreenshotTargetDir))
        DirCreate(ScreenshotTargetDir)

    timestamp := FormatTime(, "yyyy-MM-dd_HH_mm_ss")
    defaultName := "Screenshot_" timestamp
    
    InputBoxObj := InputBox("Enter file name or Close to discard", "WQOL - Save Screenshot", "w400 h130", defaultName)
    if (InputBoxObj.Result = "Cancel" || InputBoxObj.Value == "") {
        ShowTip("Screenshot Discarded.", 2000)
        return
    }
        
    sanitizedName := RegExReplace(InputBoxObj.Value, '[\\/:*?"<>|]', '_')
    targetPath := ScreenshotTargetDir "\" sanitizedName ".png"
    
    if FileExist(targetPath) {
        epoch := DateDiff(A_NowUTC, "19700101000000", "Seconds")
        targetPath := ScreenshotTargetDir "\" sanitizedName "_" epoch ".png"
    }

    hGdiPlus := DllCall("LoadLibrary", "str", "gdiplus", "ptr")
    SI := Buffer(24, 0)
    NumPut("uint", 1, SI)
    DllCall("gdiplus\GdiplusStartup", "ptr*", &pToken:=0, "ptr", SI, "ptr", 0)

    DllCall("gdiplus\GdipCreateBitmapFromHBITMAP", "ptr", hBitmap, "ptr", 0, "ptr*", &pBitmap:=0)

    CLSID := Buffer(16)
    DllCall("ole32\CLSIDFromString", "wstr", "{557CF406-1A04-11D3-9A73-0000F81EF32E}", "ptr", CLSID)
    
    status := DllCall("gdiplus\GdipSaveImageToFile", "ptr", pBitmap, "wstr", targetPath, "ptr", CLSID, "ptr", 0)

    DllCall("gdiplus\GdipDisposeImage", "ptr", pBitmap)
    DllCall("gdiplus\GdiplusShutdown", "ptr", pToken)
    DllCall("FreeLibrary", "ptr", hGdiPlus)

    if status == 0
        ShowTip("Screenshot saved to: " targetPath, 3000)
    else
        ShowTip("Save failed (GDI+ Error: " status ")", 3000)
}

ShowTip(msg, timeout) {
    ToolTip(msg)
    SetTimer(() => ToolTip(), -timeout)
}