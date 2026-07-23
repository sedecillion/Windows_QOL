#Requires AutoHotkey v2.0

SetupMiscQOL() {
    if EnableTerminalLaunch && TerminalTriggerKey
        Hotkey(TerminalTriggerKey, LaunchTerminal)
    
    if EnableFileExplorerTweaks && FileExplorerTweaksTriggerKey != "" {
        HotIfWinActive("ahk_class CabinetWClass")
        Hotkey(FileExplorerTweaksTriggerKey, CreateFileInExplorer)
        HotIfWinActive()
    }
}

LaunchTerminal(HotkeyName) {
    targetPath := ""
    
    if OpenInExplorerTabPath && WinActive("ahk_class CabinetWClass") {
        targetPath := GetActiveExplorerPath()
    }
    
    if (targetPath == "" || !DirExist(targetPath)) {
        targetPath := TerminalStartPath
    }

    try {
        Run("cmd.exe", targetPath)
    }
    catch Error as err {
        ToolTip("Failed to launch Terminal: " . err.Message)
        SetTimer(() => ToolTip(), -2000)
    }
}

CreateFileInExplorer(HotkeyName) {
    targetPath := ""
    
    if WinActive("ahk_class CabinetWClass") {
        targetPath := GetActiveExplorerPath()
    }

    if (targetPath == "" || !DirExist(targetPath)) {
        ToolTip("Couldn't create file here")
        SetTimer(() => ToolTip(), -1500)
        return
    }

    timestamp := FormatTime(, "yyyy-MM-dd_HH_mm_ss")
    defaultName := "New Text Document (" timestamp ").txt"

    ib := InputBox("Enter full file name with extension:", "WQOL - Create File", "w400 h130", defaultName)
    if (ib.Result != "OK" || ib.Value == "")
        return

    sanitizedName := RegExReplace(ib.Value, '[\\/:*?"<>|]', '_')


    fullPath := targetPath "\" sanitizedName

    if FileExist(fullPath) {
        ToolTip("File already exists.")
        SetTimer(ToolTip,-1500)
        return
    }

    try {
        FileAppend("", fullPath)
        Send("{F5}")
        ToolTip("File created: " fullPath)
        SetTimer(() => ToolTip(), -1500)
    }
    catch {
        ToolTip("Couldn't create file.")
        SetTimer(() => ToolTip(), -1500)
    }
}

GetActiveExplorerPath() {
    hwnd := WinExist("A")
    if !hwnd
        return ""

    activeTab := 0
    try activeTab := ControlGetHwnd("ShellTabWindowClass1", "ahk_id " hwnd)

    try {
        shell := ComObject("Shell.Application")
        for window in shell.Windows {
            if window.Hwnd != hwnd
                continue
            
            if activeTab {
                static IID_IShellBrowser := "{000214E2-0000-0000-C000-000000000046}"
                try {
                    shellBrowser := ComObjQuery(window, IID_IShellBrowser, IID_IShellBrowser)
                    ComCall(3, shellBrowser, "uint*", &thisTab := 0)
                    if thisTab != activeTab
                        continue
                } catch {
                    continue
                }
            }

            try {
                path := window.Document.Folder.Self.Path
                if (SubStr(path, 1, 2) == "::")
                    return ""
                return path
            }
        }
    }
    return ""
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
