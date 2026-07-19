#Requires AutoHotkey v2.0

ExecuteActionOpenFileFolder(payload) {
    if !payload.Has("Path") || payload["Path"] == ""
        return
        
    path := StrReplace(payload["Path"], '"', "")
    
    path := Trim(path, "\")
    
    targetApp := payload.Has("TargetApp") ? payload["TargetApp"] : ""

    if (targetApp != "") {
        try {
            Run(targetApp . ' "' . path . '"')
        } catch {
            MsgBox("Failed to launch target app: " targetApp, "Execution Error", 16)
        }
        return
    }

    if DirExist(path) {
        SplitPath(path, &folderName)
        
        oldMatchMode := A_TitleMatchMode
        SetTitleMatchMode(2)
        
        hwndList := WinGetList("ahk_class CabinetWClass")
        for hwnd in hwndList {
            title := WinGetTitle("ahk_id " hwnd)
            if (folderName != "" && InStr(title, folderName)) {
                WinActivate("ahk_id " hwnd)
                SetTitleMatchMode(oldMatchMode)
                return
            }
        }
        SetTitleMatchMode(oldMatchMode)
    }
    
    try {
        Run('"' path '"')
    } catch {
        MsgBox("Failed to open path: " path, "Execution Error", 16)
    }
}