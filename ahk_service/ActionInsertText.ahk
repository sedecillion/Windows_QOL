#Requires AutoHotkey v2.0

ExecuteActionInsertText(payload) {
    if !payload.Has("Text") || payload["Text"] == ""
        return

    savedClip := ClipboardAll()
    
    A_Clipboard := ""
    A_Clipboard := payload["Text"]
    
    if !ClipWait(1)
        return

    Send("^v")
    
    Sleep(100) 
    
    A_Clipboard := savedClip
}