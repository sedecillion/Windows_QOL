;@Ahk2Exe-UpdateManifest 0, , , 1
;@Ahk2Exe-SetMainIcon ..\settings_UI\Assets\app_icon.ico

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

    profileName := parsed["Profiles"][activeIndex]["ProfileProperties"]["Name"]
    
    ToolTip("Running Profile: " profileName)
    SetTimer(() => ToolTip(), -1500)
    
    profile := parsed["Profiles"][activeIndex]
    
    global SilentMode := profile["ProfileProperties"]["SilentMode"]
    
    global EnableTerminalLaunch := profile["TerminalLaunch"]["IsEnabled"]
    global TerminalStartPath := StrReplace(profile["TerminalLaunch"]["StartPath"], "<A_UserName>", A_UserName)
    global TerminalTriggerKey := profile["TerminalLaunch"]["TriggerKey"]
    global OpenInExplorerTabPath := profile["TerminalLaunch"].Has("OpenInExplorerTabPath") ? profile["TerminalLaunch"]["OpenInExplorerTabPath"] : true

    global EnableFileExplorerTweaks := profile.Has("FileExplorerTweaks") && profile["FileExplorerTweaks"].Has("IsEnabled") ? profile["FileExplorerTweaks"]["IsEnabled"] : true
    global FileExplorerTweaksTriggerKey := profile.Has("FileExplorerTweaks") && profile["FileExplorerTweaks"].Has("TriggerKey") ? profile["FileExplorerTweaks"]["TriggerKey"] : "^n"
    
    global EnableScreenshotTool := profile["ScreenshotTool"]["IsEnabled"]
    global ScreenshotTargetDir := StrReplace(profile["ScreenshotTool"]["TargetDir"], "<A_UserName>", A_UserName)
    global ScreenshotTriggerKey := profile["ScreenshotTool"]["TriggerKey"]
    global ScreenshotFullScreenModeOnly := profile["ScreenshotTool"].Has("FullScreenModeOnly") ? profile["ScreenshotTool"]["FullScreenModeOnly"] : true
    
    global EnableCalcInstance := profile["CalcSingleInstance"]["IsEnabled"]
    
    waremap := profile["WindowAwareShortcutRemap"]
    global EnableWindowAwareRemaps := waremap["IsEnabled"]
    global WindowAwareList := waremap.Has("Remaps") && waremap["Remaps"] != "" ? waremap["Remaps"] : []
    
    global EnableRedactedPaste := profile["RedactedPaste"]["IsEnabled"]
    global RedactedReplacements := profile["RedactedPaste"]["Replacements"]
    global RedactedPasteTriggerKey := profile["RedactedPaste"]["TriggerKey"]
    
    for i, item in RedactedReplacements {
        item["Dirty"] := StrReplace(item["Dirty"], "<A_UserName>", A_UserName)
    }

    caps := profile["CapsModifiers"]
    global EnableCapsModifiers := caps["IsEnabled"]
    
    global CapsModifierMappings := caps.Has("ModifierMappings") && caps["ModifierMappings"] != "" ? caps["ModifierMappings"] : []
}

LoadConfig()


#Include FeatureCapsModifier.ahk
#Include ActionWindowFocus.ahk
#Include ActionShortcutRemap.ahk
#Include ActionProfileSwitch.ahk
#Include ActionInsertText.ahk
#Include ActionOpenFileFolder.ahk
#Include ActionRunCommand.ahk
#Include ActionWindowControls.ahk
#Include ActionMediaControls.ahk
#Include FeatureWindowAwareShortcutRemaps.ahk
#Include FeatureScreenShotTool.ahk
#Include FeatureRedactedPaste.ahk
#Include FeatureMisc.ahk


if (SilentMode) {
    A_IconHidden := true
} else {
    TraySetIcon("shell32.dll", 16) 
}

SetupCapsHotkeys()
SetupWindowAwareShortcuts()
SetupScreenshotTool()
SetupRedactedPaste()
SetupMiscQOL()


; Global things for sub FeatureScreenShotTool

; so ctrl + backspace in MsgBox dosen't insert that delete character
#HotIf WinActive("WQOL ahk_class #32770")
^Backspace::Send("^+{Left}{Backspace}")
#HotIf

; exiting apps should toggle all script mode apps to normal
Global ScriptModeWindows := Map()

CleanupScriptModeWindows() {
    for hwnd, _ in ScriptModeWindows {
        targetWin := "ahk_id " hwnd
        if WinExist(targetWin) {
            try WinSetTransparent("Off", targetWin)
            try WinSetAlwaysOnTop(0, targetWin)
            try WinSetExStyle("-0x20", targetWin)
        }
    }
    ScriptModeWindows.Clear()
}

^!+Esc:: {
    CleanupScriptModeWindows()
    if (A_IsCompiled) {
        Run('"' A_ScriptFullPath '" /restart')
        ExitApp()
    } else {
        Reload()
    }
}

#Esc:: {
    CleanupScriptModeWindows()
    ExitApp()
}