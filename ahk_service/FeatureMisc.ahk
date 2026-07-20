#Requires AutoHotkey v2.0

SetupMiscQOL() {
    if EnableTerminalLaunch && TerminalTriggerKey
        Hotkey(TerminalTriggerKey, LaunchTerminal)
}

LaunchTerminal(HotkeyName) {
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
