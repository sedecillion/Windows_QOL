#Requires AutoHotkey v2.0

ExecuteActionRunCommand(payload) {
    if !payload.Has("Command") || payload["Command"] == ""
        return

    cmd := payload["Command"]
    runAsAdmin := (payload.Has("RunAsAdmin") && payload["RunAsAdmin"])
    hidden := (payload.Has("Hidden") && payload["Hidden"])

    if (runAsAdmin) {
        cmd := "*RunAs " cmd
    }

    options := hidden ? "Hide" : ""

    try {
        Run(cmd, , options)
    } catch as err {
        MsgBox("Command execution failed:`n" cmd "`n`nReason: " err.Message, "Execution Error", 16)
    }
}