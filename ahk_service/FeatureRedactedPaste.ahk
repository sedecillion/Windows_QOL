#Requires AutoHotkey v2.0

SetupRedactedPaste() {
    if EnableRedactedPaste && RedactedPasteTriggerKey
        Hotkey(RedactedPasteTriggerKey, PasteRedacted)
}

PasteRedacted(HotkeyName) {
    rawText := A_Clipboard
    if (rawText = "")
        return

    cleanText := rawText
    
    for item in RedactedReplacements {
        cleanText := StrReplace(cleanText, item["Dirty"], item["Clean"])
    }

    A_Clipboard := "" 
    DllCall("OpenClipboard", "ptr", A_ScriptHwnd)
    DllCall("EmptyClipboard")
    
    hMem := DllCall("GlobalAlloc", "uint", 0x42, "ptr", (StrLen(cleanText) + 1) * 2, "ptr")
    pMem := DllCall("GlobalLock", "ptr", hMem, "ptr")
    StrPut(cleanText, pMem, "UTF-16")
    DllCall("GlobalUnlock", "ptr", hMem)
    
    DllCall("SetClipboardData", "uint", 13, "ptr", hMem)
    DllCall("CloseClipboard")

    Send("^v")
}