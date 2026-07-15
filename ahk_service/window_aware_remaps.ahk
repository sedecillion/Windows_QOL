#Requires AutoHotkey v2.0

SetupWindowAwareShortcuts() {
    if (!EnableWindowAwareRemaps)
        return

    for _, remap in WindowAwareList {
        trigger := remap["TriggerKey"]
        emit := remap["ShortcutToEmit"]
        targets := remap["TargetWindows"]

        if (trigger == "")
            continue

        conditionFn := CheckTargetWindows.Bind(targets)
        
        HotIf(conditionFn)
        
        Hotkey(trigger, ExecuteWindowAwareRemap.Bind(emit))
    }
    
    HotIf()
}

CheckTargetWindows(targets, hotkeyName) {
    if (!EnableWindowAwareRemaps)
        return false

    for _, criteria in targets {
        ; Wildcard match - always intercept
        if (criteria == "*")
            return true
            
        ; Auto-prefix ahk_exe if not fully specified, matching your Caps logic
        checkStr := (SubStr(criteria, 1, 4) == "ahk_") ? criteria : "ahk_exe " . criteria
        
        if WinActive(checkStr)
            return true
    }
    
    return false
}

ExecuteWindowAwareRemap(shortcutToEmit, hotkeyName) {
    if (shortcutToEmit != "")
        Send(shortcutToEmit)
}