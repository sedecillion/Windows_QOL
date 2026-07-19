#Requires AutoHotkey v2.0

global SwitcherGUI := ""
global SwitcherHwnds := []
global SwitcherCtrls := []
global CurrentIndex := 1
global SwitcherActive := false
global SwitcherCanceled := false
global SwitcherCurrentGroup := ""

ExecuteActionWindowFocus(payload) {
    success := SmartSwitch(payload["TargetExe"], payload["Command"], payload["RequiredTitle"], payload["ExcludeTitle"])
    
    if (!success && payload.Has("Fallback") && payload["Fallback"] != "") {
        if WinExist(payload["Fallback"])
            WinActivate(payload["Fallback"])
    }
}

; CapsDispatcher will call on caps lock release
ActionWindowFocus_Confirm() {
    global SwitcherActive, SwitcherCanceled, SwitcherGUI, SwitcherHwnds, CurrentIndex
    
    if (SwitcherActive) {
        SwitcherActive := false
        if (SwitcherGUI != "") {
            SwitcherGUI.Destroy()
            SwitcherGUI := ""
        }
        if (!SwitcherCanceled && SwitcherHwnds.Has(CurrentIndex)) {
            WinActivate("ahk_id " SwitcherHwnds[CurrentIndex])
        }
    }
}

ActionWindowFocus_IsActive() {
    global SwitcherActive
    return SwitcherActive
}

ActionWindowFocus_Cancel() {
    global SwitcherActive, SwitcherCanceled, SwitcherGUI
    
    if (SwitcherActive) {
        SwitcherCanceled := true
        SwitcherActive := false
        if (SwitcherGUI != "") {
            SwitcherGUI.Destroy()
            SwitcherGUI := ""
        }
    }
}

SmartSwitch(winCriteria, runCmd := "", requireTitleMatch := "", ExcludeTitleMatch := "") {
    global SwitcherActive, SwitcherCanceled, SwitcherGUI, SwitcherHwnds, SwitcherCtrls, CurrentIndex, SwitcherCurrentGroup

    groupId := winCriteria . "|" . requireTitleMatch . "|" . ExcludeTitleMatch

    if (SwitcherActive) {
        if (SwitcherCurrentGroup == groupId) {
            CycleGUI()
            return true
        } else {
            if (SwitcherGUI != "") {
                SwitcherGUI.Destroy()
                SwitcherGUI := ""
            }
            SwitcherActive := false
        }
    }

    oldMatchMode := A_TitleMatchMode
    SetTitleMatchMode(2) 

    SwitcherActive := true
    SwitcherCanceled := false
    SwitcherCurrentGroup := groupId
    SwitcherHwnds := []
    SwitcherCtrls := []
    titles := []
    validHwnds := []

    activeHwnd := WinExist("A")

    if (winCriteria == "*") {
        rawList := WinGetList()
        for hwnd in rawList {
            if !IsAltTabWindow(hwnd)
                continue
            if (hwnd == activeHwnd) {
                validHwnds.Push(hwnd)
                continue
            }
            if !IsRecognizedWindow(hwnd) {
                validHwnds.Push(hwnd)
            }
        }
    } else {
        rawList := WinGetList(winCriteria)
        for hwnd in rawList {
            if !IsAltTabWindow(hwnd)
                continue
            title := WinGetTitle("ahk_id " hwnd)
            if (title == "")
                continue
            if (requireTitleMatch != "" && !InStr(title, requireTitleMatch))
                continue
            if (ExcludeTitleMatch != "" && InStr(title, ExcludeTitleMatch))
                continue
            validHwnds.Push(hwnd)
        }
    }

    if (validHwnds.Length == 0) {
        SwitcherActive := false
        SetTitleMatchMode(oldMatchMode)
        if (runCmd != "") {
            Run(runCmd)
            return true
        }
        return false 
    }

    if (activeHwnd) {
        title := WinGetTitle("ahk_id " activeHwnd)
        if (title != "") {
            SwitcherHwnds.Push(activeHwnd)
            titles.Push(title)
        }
    }

    for hwnd in validHwnds {
        if (hwnd != activeHwnd) {
            SwitcherHwnds.Push(hwnd)
            titles.Push(WinGetTitle("ahk_id " hwnd))
        }
    }

    if (SwitcherHwnds.Length == 1) {
        SwitcherActive := false
        WinActivate("ahk_id " SwitcherHwnds[1])
        SetTitleMatchMode(oldMatchMode)
        return true
    }
    
    if (SwitcherHwnds.Length == 2 && SwitcherHwnds[1] == activeHwnd) {
        SwitcherActive := false
        WinActivate("ahk_id " SwitcherHwnds[2])
        SetTitleMatchMode(oldMatchMode)
        return true
    }

    CurrentIndex := 2 

    SwitcherGUI := Gui("+AlwaysOnTop -Caption +ToolWindow")
    SwitcherGUI.BackColor := "1E1E1E"
    SwitcherGUI.SetFont("s10 cWhite", "Segoe UI") 

    for index, title in titles {
        color := (index == CurrentIndex) ? "cFAF9F6" : "c999999"
        weight := (index == CurrentIndex) ? "w700" : "w400"
        
        pos := (index == 1) ? "x15 y15" : "x+15 y15"
        
        opt := pos " w220 r2 BackgroundTrans" 
        ctrl := SwitcherGUI.Add("Text", opt, title)
        ctrl.SetFont(color " " weight) 
        
        SwitcherCtrls.Push(ctrl)
    }

    SwitcherGUI.Show("Hide")
    SwitcherGUI.GetPos(,,, &h)
    SwitcherGUI.Show("h" (h + 20) " NoActivate Center")
    
    SetTitleMatchMode(oldMatchMode)
    return true
}

CycleGUI() {
    global SwitcherCtrls, CurrentIndex, SwitcherHwnds
    
    SwitcherCtrls[CurrentIndex].SetFont("c999999 w400")
    CurrentIndex++
    if (CurrentIndex > SwitcherHwnds.Length) {
        CurrentIndex := 1
    }
    SwitcherCtrls[CurrentIndex].SetFont("cFAF9F6 w700")
}

IsRecognizedWindow(hwnd) {
    try {
        exe := WinGetProcessName("ahk_id " hwnd)
        cls := WinGetClass("ahk_id " hwnd)
        
        for _, mapping in CapsModifierMappings {
            if (mapping["Action"] != "WindowFocus" || !mapping.Has("WindowFocusPayload") || mapping["WindowFocusPayload"] == "")
                continue

            payload := mapping["WindowFocusPayload"]
            criteria := payload["TargetExe"]
            fallback := payload.Has("Fallback") ? payload["Fallback"] : ""
            
            if (criteria != "*") {
                if (exe != "" && InStr(criteria, "ahk_exe " . exe))
                    return true
                    
                if (cls != "" && InStr(criteria, "ahk_class " . cls))
                    return true
            }
            
            if (fallback != "") {
                if (exe != "" && InStr(fallback, "ahk_exe " . exe))
                    return true
                    
                if (cls != "" && InStr(fallback, "ahk_class " . cls))
                    return true
            }
        }
    }
    return false
}

IsAltTabWindow(hwnd) {
    title := WinGetTitle("ahk_id " hwnd)
    cls := WinGetClass("ahk_id " hwnd)
    
    if (title == "" || cls == "Progman" || cls == "WorkerW" || cls == "Shell_TrayWnd")
        return false
    if !DllCall("IsWindowVisible", "Ptr", hwnd)
        return false
    if (WinGetExStyle("ahk_id " hwnd) & 0x00000080)
        return false
        
    cloaked := 0
    if (DllCall("dwmapi\DwmGetWindowAttribute", "Ptr", hwnd, "UInt", 14, "Int*", &cloaked, "UInt", 4) == 0)
        if (cloaked != 0)
            return false
            
    return true
}