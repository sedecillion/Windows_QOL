#Requires AutoHotkey v2.0

SetupCapsHotkeys() {
    HotIf (*) => GetKeyState("CapsLock", "P")
    
    if (EnableCapsModifiers) {
        for _, mapping in CapsModifierMappings {
            key := mapping["TriggerKey"]
            action := mapping["Action"]
            Hotkey("*" . key, DispatchCapsAction.Bind(action, mapping, key))
        }
    }
    
    HotIf
}

DispatchCapsAction(action, mapping, key, *) {
    if !EnableCapsModifiers
        return

    Switch action {
        Case "WindowFocus":
            if (mapping.Has("WindowFocusPayload") && mapping["WindowFocusPayload"] != "")
                ExecuteActionWindowFocus(mapping["WindowFocusPayload"])
                
        Case "ShortcutRemap":
            if (mapping.Has("RemappedKeys") && mapping["RemappedKeys"] != "")
                ExecuteActionShortcutRemap(key, mapping["RemappedKeys"])

        Case "ProfileSwitch":
            if (mapping.Has("ProfileSwitchPayload") && mapping["ProfileSwitchPayload"] != "")
                ExecuteActionProfileSwitch(mapping["ProfileSwitchPayload"])
                
        Case "InsertText":
            if (mapping.Has("InsertTextPayload") && mapping["InsertTextPayload"] != "")
                ExecuteActionInsertText(mapping["InsertTextPayload"])
                
        Case "OpenFileFolder":
            if (mapping.Has("OpenFileFolderPayload") && mapping["OpenFileFolderPayload"] != "")
                ExecuteActionOpenFileFolder(mapping["OpenFileFolderPayload"])
                
        Case "RunCommand":
            if (mapping.Has("RunCommandPayload") && mapping["RunCommandPayload"] != "")
                ExecuteActionRunCommand(mapping["RunCommandPayload"])

        Case "WindowControls":
            if (mapping.Has("WindowControlsPayload") && mapping["WindowControlsPayload"] != "")
                ExecuteActionWindowControls(mapping["WindowControlsPayload"])
        
        Case "MediaControls":
            if (mapping.Has("MediaControlsPayload") && mapping["MediaControlsPayload"] != "")
                ExecuteActionMediaControls(mapping["MediaControlsPayload"])
    }
}

#HotIf EnableCapsModifiers

#CapsLock:: {
    SetCapsLockState(!GetKeyState("CapsLock", "T"))
}


; action window focus specific handling since requires tracking the caps state after pressing for 
; alt tab styled window
*CapsLock:: {
    KeyWait("CapsLock") 
}

*CapsLock up:: {
    ActionWindowFocus_Confirm()
}

$Esc:: {
    if (ActionWindowFocus_IsActive()) {
        ActionWindowFocus_Cancel()
    } else {
        Send("{Esc}")
    }
}

#HotIf