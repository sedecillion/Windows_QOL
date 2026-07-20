#Requires AutoHotkey v2.0

ExecuteActionMediaControls(payload) {
    if !payload.Has("ControlType") || payload["ControlType"] == ""
        return

    Switch payload["ControlType"] {
        Case "VolumePlus":
            Send("{Volume_Up}")
        Case "VolumeMinus":
            Send("{Volume_Down}")
        Case "MuteToggle":
            Send("{Volume_Mute}")
        Case "Prev":
            Send("{Media_Prev}")
        Case "Next":
            Send("{Media_Next}")
        Case "PlayPauseToggle":
            Send("{Media_Play_Pause}")
    }
}