;@Ahk2Exe-UpdateManifest 0, , , 1
#Requires AutoHotkey v2.0
#SingleInstance Force

#Include _JXON.ahk

LoadConfig() {
    if !FileExist("config.json") {
        MsgBox("config.json not found!", "Error", 16)
        ExitApp
    }
    
    rawJson := FileRead("config.json")
    parsed := JXON_Load(&rawJson)
    
    activeIndex := parsed["active_profile_index"] + 1 
    if (activeIndex > parsed["profiles"].Length) {
        MsgBox("Active profile index out of bounds.", "Error", 16)
        ExitApp
    }
    
    profile := parsed["profiles"][activeIndex]
    
    global SilentMode := profile["profile_properties"]["SilentMode"]
    
    qol := profile["data"]["windows_qol"]
    global EnableQOL := qol[1] ; Master toggle for QOL block
    
    global EnableTerminalLaunch := EnableQOL ? qol[2]["EnableTerminalLaunch"] : false
    global TerminalStartPath := StrReplace(qol[2]["TerminalStartPath"], "<A_UserName>", A_UserName)
    
    global EnableScreenshotTool := EnableQOL ? qol[2]["EnableScreenshotTool"] : false
    global ScreenshotTargetDir := qol[2]["ScreenshotTargetDir"]
    
    global EnableCalcInstance := EnableQOL ? qol[2]["EnableCalcInstance"] : false
    global EnableCtrlShiftW := EnableQOL ? qol[2]["EnableCtrlShiftW"] : false
    global CloseWindowTargets := qol[2]["CloseWindowTargets"]
    
    global EnableRedactedPaste := EnableQOL ? qol[2]["EnableRedactedPaste"] : false
    global RedactedReplacements := qol[2]["RedactedReplacements"]
    for i, item in RedactedReplacements {
        item["dirty"] := StrReplace(item["dirty"], "<A_UserName>", A_UserName)
    }

    global EnableCapsModifiers := profile["data"]["caps_modifier_keys"][1]
    global CapsKeysMap := profile["data"]["caps_modifier_keys"][2]
}

LoadConfig()

#Include caps_modifier.ahk
#Include windows_qol.ahk

if (SilentMode) {
    A_IconHidden := true
} else {
    TraySetIcon("shell32.dll", 16) 
}

SetupCapsHotkeys()

#Esc::ExitApp

;@Ahk2Exe-IgnoreBegin
^!r::Reload

^+F12::
{
    KeyHistory
}
;@Ahk2Exe-IgnoreEnd