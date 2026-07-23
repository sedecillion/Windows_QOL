;@Ahk2Exe-SetVersion 1.0.2.0
#Requires AutoHotkey v2.0
;@Ahk2Exe-SetMainIcon ..\settings_UI\Assets\app_icon.ico

ServiceExeName := "W_QOL.exe"
ServiceExePath := A_ScriptDir "\" ServiceExeName
RegKeyPath := "Software\Microsoft\Windows\CurrentVersion\Run"
RegValName := "W_QOL_Service"
ConfigPath := A_AppData "\Windows_QOL\config.json"


MainGui := Gui("-MaximizeBox -Resize +AlwaysOnTop", "Windows QOL")
MainGui.OnEvent("Close", (*) => ExitApp())
MainGui.SetFont("s10", "Segoe UI")

StatusText := MainGui.Add("Text", "w250 h20 Center", "Status: Checking...")
StatusText.SetFont("w700")

BtnToggle := MainGui.Add("Button", "w250 h40 y+10", "Toggle Service")
BtnToggle.OnEvent("Click", ToggleService)

ChkStartup := MainGui.Add("CheckBox", "w250 h20 y+10", "Run on startup")
ChkStartup.OnEvent("Click", ToggleStartup)

BtnConfig := MainGui.Add("Button", "w120 h30 y+15", "Open Config")
BtnConfig.OnEvent("Click", (*) => Run(ConfigPath))

BtnDocs := MainGui.Add("Button", "w120 h30 x+10 yp", "Documentation")
BtnDocs.OnEvent("Click", (*) => Run("https://github.com/sedecillion/Windows_QOL#readme"))

InitializeServiceState()
MainGui.Show("AutoSize Center")

InitializeServiceState() {
    IsRunning := ProcessExist(ServiceExeName)
    
    if (IsRunning) {
        StatusText.Value := "Status: Running"
        StatusText.SetFont("cGreen w700")
        BtnToggle.Text := "Stop Service"
    } else {
        StatusText.Value := "Status: Stopped"
        StatusText.SetFont("cRed w700")
        BtnToggle.Text := "Start Service"
    }

    try {
        RegValue := RegRead("HKCU\" RegKeyPath, RegValName)
        ChkStartup.Value := (RegValue != "")
    } catch {
        ChkStartup.Value := 0
    }
}

ToggleService(*) {
    if ProcessExist(ServiceExeName) {
        Send("{LWin down}{Esc}{LWin up}")
        Sleep(500)
    } else {
        try {
            Run('"' ServiceExePath '"', A_ScriptDir)
            Sleep(500)
        } catch {
            MsgBox("Could not start W_QOL.exe. Ensure it is in the same directory.", "Error", "IconX")
        }
    }
    InitializeServiceState()
}

ToggleStartup(Ctrl, *) {
    if (Ctrl.Value) {
        RegWrite('"' ServiceExePath '"', "REG_SZ", "HKCU\" RegKeyPath, RegValName)
    } else {
        try RegDelete("HKCU\" RegKeyPath, RegValName)
    }
}