#Requires AutoHotkey v2.0

ExecuteActionProfileSwitch(payload) {
    if !payload.Has("TargetIndex")
        return
        
    targetIndex := payload["TargetIndex"]
    configPath := A_AppData "\Windows_QOL\config.json"
    
    if !FileExist(configPath) {
        MsgBox("config.json not found at: " configPath, "Error", 16)
        return
    }
    
    rawJson := FileRead(configPath)
    
    ; using regex since AHK dosen't have null or bool, writing with JXON will convert everything to 0/1 and ""
    newJson := RegExReplace(rawJson, '("ActiveProfileIndex"\s*:\s*)\d+', "${1}" . targetIndex)
    
    fileObj := FileOpen(configPath, "w", "UTF-8")
    fileObj.Write(newJson)
    fileObj.Close()

    if (A_IsCompiled) {
        Run('"' A_ScriptFullPath '" /restart')
        ExitApp()
    } else {
        Reload()
    }
}