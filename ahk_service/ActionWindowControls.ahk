#Requires AutoHotkey v2.0

ExecuteActionWindowControls(payload) {
    global ScriptModeWindows
    
    if !payload.Has("ControlType") || payload["ControlType"] == ""
        return

    try {
        hwnd := WinGetID("A")
    } catch {
        return 
    }
    
    targetWin := "ahk_id " hwnd

    Switch payload["ControlType"] {
        
        Case "TransparencyPlus":
            currentAlpha := WinGetTransparent(targetWin)
            if (currentAlpha = "")
                currentAlpha := 255
            
            newAlpha := currentAlpha + 37
            
            try {
                WinSetExStyle("+0x80000", targetWin)
                if (newAlpha >= 255)
                    WinSetTransparent("Off", targetWin)
                else
                    WinSetTransparent(newAlpha, targetWin)
            } catch {
                ToolTip("This window dosen't support transparency")
                SetTimer(() => ToolTip(), -2000)
            }

        Case "TransparencyMinus":
            currentAlpha := WinGetTransparent(targetWin)
            if (currentAlpha = "")
                currentAlpha := 255
            
            newAlpha := currentAlpha - 37
            if (newAlpha < 0)
                newAlpha := 0
            
            try {
                WinSetExStyle("+0x80000", targetWin)
                WinSetTransparent(newAlpha, targetWin)
            } catch {
                ToolTip("This window dosen't support transparency")
                SetTimer(() => ToolTip(), -2000)
            }

        Case "TogglePinOnTop":
            WinSetAlwaysOnTop(-1, targetWin)
            
            if (WinGetExStyle(targetWin) & 0x8)
                ToolTip("Window Pinned")
            else
                ToolTip("Window Unpinned")
            
            SetTimer(() => ToolTip(), -1000)

        Case "ToggleClickThrough":
            WinSetExStyle("^0x20", targetWin)
            
            if (WinGetExStyle(targetWin) & 0x20)
                ToolTip("Click through enabled")
            else
                ToolTip("Click through disabled")
            
            SetTimer(() => ToolTip(), -1000)
            
        Case "ToggleScriptMode":
            if ScriptModeWindows.Has(hwnd) {
                try WinSetTransparent("Off", targetWin)
                WinSetAlwaysOnTop(0, targetWin)
                WinSetExStyle("-0x20", targetWin)
                
                ScriptModeWindows.Delete(hwnd)
                ToolTip("Script Mode Disabled")
                SetTimer(() => ToolTip(), -1500)
            } else {
                try {
                    WinSetExStyle("+0x80000", targetWin)
                    WinSetTransparent(180, targetWin)
                    
                    WinSetAlwaysOnTop(1, targetWin)
                    WinSetExStyle("+0x20", targetWin)
                    
                    ScriptModeWindows[hwnd] := true
                    ToolTip("Script Mode Enabled`nTo Disable: Bring window into focus from taskbar and press hotkey again.")
                    SetTimer(() => ToolTip(), -4000)
                } catch {
                    ToolTip("Script Mode: Can't set transparency on this window")
                    SetTimer(() => ToolTip(), -2000)
                }
            }
    }
}