;@Ahk2Exe-UpdateManifest 0, , , 1
#Requires AutoHotkey v2.0
#SingleInstance Force

#Include _JXON.ahk
LoadConfig() {
    configDir := A_AppData "\Windows_QOL"
    configPath := configDir "\config.json"

    if !DirExist(configDir) {
        DirCreate(configDir)
    }

    if !FileExist(configPath) {
        MsgBox("config.json not found at: " configPath, "Error", 16)
        ExitApp
    }
    
    rawJson := FileRead(configPath)
    parsed := JXON_Load(&rawJson)
    
    activeIndex := parsed["ActiveProfileIndex"] + 1 
    if (activeIndex > parsed["Profiles"].Length) {
        MsgBox("Active profile index out of bounds.", "Error", 16)
        ExitApp
    }
    
    profile := parsed["Profiles"][activeIndex]
    
    global SilentMode := profile["ProfileProperties"]["SilentMode"]
    
    global EnableTerminalLaunch := profile["TerminalLaunch"]["IsEnabled"]
    global TerminalStartPath := StrReplace(profile["TerminalLaunch"]["StartPath"], "<A_UserName>", A_UserName)
    
    global EnableScreenshotTool := profile["ScreenshotTool"]["IsEnabled"]
    global ScreenshotTargetDir := profile["ScreenshotTool"]["TargetDir"]
    
    global EnableCalcInstance := profile["CalcSingleInstance"]["IsEnabled"]
    
    waremap := profile["WindowAwareShortcutRemap"]
    global EnableWindowAwareRemaps := waremap["IsEnabled"]
    global WindowAwareList := waremap.Has("Remaps") && waremap["Remaps"] != "" ? waremap["Remaps"] : []
    
    global EnableRedactedPaste := profile["RedactedPaste"]["IsEnabled"]
    global RedactedReplacements := profile["RedactedPaste"]["Replacements"]
    for i, item in RedactedReplacements {
        item["Dirty"] := StrReplace(item["Dirty"], "<A_UserName>", A_UserName)
    }

    caps := profile["CapsModifiers"]
    global EnableCapsModifiers := caps["IsEnabled"]
    
    global CapsModifierMappings := caps.Has("ModifierMappings") && caps["ModifierMappings"] != "" ? caps["ModifierMappings"] : []
}

LoadConfig()

#Include caps_modifier.ahk
#Include window_aware_remaps.ahk
#Include misc_qol.ahk

if (SilentMode) {
    A_IconHidden := true
} else {
    TraySetIcon("shell32.dll", 16) 
}

SetupCapsHotkeys()
SetupWindowAwareShortcuts()

^!r:: {
    if (A_IsCompiled) {
        Run('"' A_ScriptFullPath '" /restart')
        ExitApp()
    } else {
        Reload()
    }
}