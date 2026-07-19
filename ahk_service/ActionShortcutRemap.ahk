#Requires AutoHotkey v2.0

global TS_LastTabSwitch := 0
global TS_TabSwitchCooldown := 30

ExecuteActionShortcutRemap(key, payloadArray) {
    if (InStr(key, "Wheel") && !TS_CanSwitchTabs())
        return
        
    for _, item in payloadArray {
        criteria := item["TargetWindow"]
        keysToSend := item["ShortcutToEmit"]
        
        if (criteria == "*")
            continue
        
        checkStr := (SubStr(criteria, 1, 4) = "ahk_") ? criteria : "ahk_exe " . criteria
        
        if WinActive(checkStr) {
            if (keysToSend != "")
                Send(keysToSend)
            return
        }
    }
    
    for _, item in payloadArray {
        if (item["TargetWindow"] == "*") {
            if (item["ShortcutToEmit"] != "")
                Send(item["ShortcutToEmit"])
            return
        }
    }
}

TS_CanSwitchTabs() {
    global TS_LastTabSwitch, TS_TabSwitchCooldown
    now := A_TickCount
    if (now - TS_LastTabSwitch < TS_TabSwitchCooldown)
        return false
    TS_LastTabSwitch := now
    return true
}