# Configuration File

Every shortcut uses the AutoHotkey v2 key format.

Note the below formats used in some of the payloads of features

Actual JSON structure format explanation [here](#root-structure)

# Input Formats

## Command Format

Command points to executable or is a URI

Examples

```
cmd.exe /c del path\\to\\folder
```
`/c` to close is after use

```
cmd.exe /k spin_up_dev_server
```
`/k' keeps it open

It can be powershell commands

```powershell
powershell.exe Get-Command
```

It can be a URI as well
```
ms-availablenetworks:
```
or
```
https://www.google.com
```


## Shortcut Format

[List of Keys (Keyboard, Mouse and Controller) | AutoHotkey v2](https://www.autohotkey.com/docs/v2/KeyList.htm)


### Single Key

Used by

- `CapsModifiers.TriggerKey`

Examples

```
a
1
F5
Space
LButton
WheelUp
XButton1
```

Not Supported

```
Ctrl + A
Ctrl + Shift + V
Ctrl + Alt + P
```

### Connected Shortcut

Used by

- `TerminalLaunch.TriggerKey`
- `ScreenshotTool.TriggerKey`
- `RedactedPaste.TriggerKey`
- `WindowAwareShortcutRemap.TriggerKey`
- `ShortcutRemap.ShortcutToEmit`

Examples

```
Ctrl + T
Ctrl + Shift + W
Alt + F4
Win + R
```

Not Supported

```
Ctrl + K then Ctrl + C
Ctrl + X then Ctrl + S
```

> [!note]
>
> Reserved Shortcuts
>
> - `Win + Esc`
>   - Terminates the background service.
>
> - `Ctrl + Alt + Shift + Esc`
>   - Reloads the background service.


## Window Identifier Format

The following AutoHotkey window identifiers are supported wherever window matching is used.

### `ahk_exe`

Matches windows belonging to an executable.

Example

```
ahk_exe explorer.exe

ahk_exe chrome.exe

ahk_exe Code.exe
```

### `ahk_class`

Matches windows by their window class.

Example

```
ahk_class CabinetWClass

ahk_class Notepad

ahk_class ApplicationFrameWindow
```

<br>
<br>

# Root Structure

```json
{
    "ActiveProfileIndex": 0,
    "Profiles":
    [
        {
            ...
        }
    ]
}
```

## ActiveProfileIndex

Type

```
Integer
```

Behavior

- Zero-based index into the `Profiles` array.
- Determines the profile loaded when the background service starts.
- Can be changed at runtime using the `ProfileSwitch` action.

Example

```json
"ActiveProfileIndex": 0
```

## Profiles

Type

```
Array
```

Element

```
Profile
```

Behavior

- Contains all available profiles.
- At least one profile is required.
- Every profile is independent.
- Switching profiles immediately replaces the configuration used by every feature.

Example

```json
"Profiles":
[
    {
        ...
    },
    {
        ...
    }
]
```

---

<br>

# Profile Structure

Each profile contains the following sections.

1. `ProfileProperties`
2. `CalcSingleInstance`
3. `CapsModifiers`
4. `RedactedPaste`
5. `ScreenshotTool`
6. `TerminalLaunch`
7. `WindowAwareShortcutRemap`

```json
{
    "ProfileProperties": { ... },
    "CalcSingleInstance": { ... },
    "CapsModifiers": { ... },
    "RedactedPaste": { ... },
    "ScreenshotTool": { ... },
    "TerminalLaunch": { ... },
    "WindowAwareShortcutRemap": { ... }
}
```
---
<br>

# ProfileProperties

General information about the profile.

```json
"ProfileProperties":
{
    "Name": "Default Profile",
    "SilentMode": true
}
```

## Name

Type

```
String
```

Behavior

- Friendly name displayed by the settings application.
- Does not affect the behavior of any feature.

Example

```json
"Name": "Default Profile"
```

## SilentMode

Type

```
Boolean
```

Values

```
true
false
```

Behavior

- `true` — Hides the notification area (system tray) icon.
- `false` — Shows the notification area (system tray) icon.
- Does not disable the background service or any configured features.

Example

```json
"SilentMode": true
```

<br>

> Apart from `ProfileProperties`, every top-level section represents an independent feature.

Each feature contains an `IsEnabled` property which enables or disables that feature.

| JSON Key | Description |
|----------|-------------|
| `CalcSingleInstance` | Ensures only a single Calculator instance exists when launched using the Calculator key. |
| `CapsModifiers` | Turns Caps Lock into a modifier key and executes configured actions. |
| `RedactedPaste` | Replaces configured text before pasting. |
| `ScreenshotTool` | Captures screenshots with save, rename and delete support. |
| `TerminalLaunch` | Launches a terminal in a configured working directory. |
| `WindowAwareShortcutRemap` | Remaps normal keyboard shortcuts based on the active window. |

Example

```json
{
    "CalcSingleInstance":
    {
        "IsEnabled": true,
        ...
    },

    "CapsModifiers":
    {
        "IsEnabled": true,
        ...
    },

    "RedactedPaste":
    {
        "IsEnabled": false,
        ...
    },

    "ScreenshotTool":
    {
        "IsEnabled": true,
        ...
    },

    "TerminalLaunch":
    {
        "IsEnabled": true,
        ...
    },

    "WindowAwareShortcutRemap":
    {
        "IsEnabled": true,
        ...
    }
}
```
---

<br>

# CapsModifiers

Turns **Caps Lock** into a modifier key similar to Ctrl or Alt.

```json
"CapsModifiers":
{
    "IsEnabled": true,
    "ModifierMappings":
    [
        ...
    ]
}
```

## ModifierMappings

Type

```
Array
```

Element

```json
{
    "Action": "...",
    "TriggerKey": "...",

    "WindowFocusPayload": { ... },
    "RemappedKeys": [ ... ],
    "ProfileSwitchPayload": { ... },
    "InsertTextPayload": { ... },
    "OpenFileFolderPayload": { ... },
    "RunCommandPayload": { ... },
    "WindowControlsPayload": { ... },
    "MediaControlsPayload": { ... }
}
```

Behavior

- Each element defines one `Caps + TriggerKey` mapping.
- Mappings are evaluated independently.

## TriggerKey

Type

```
String
```

Format

[Single Key](#single-key)

Behavior

- Defines the key used together with `Caps Lock`.
- Exactly one key must be specified.

Examples

```
a
1
F5
Space
LButton
WheelUp
XButton1
```

## Action

Type

```
String
```

Allowed Values

```
InsertText
OpenFileFolder
ProfileSwitch
RunCommand
ShortcutRemap
WindowFocus
WindowControls
MediaControls
```

Behavior

- Determines which payload is used.
- Only the payload corresponding to the selected action is read.
- All other payloads are ignored.
- Ignored payloads may:
  - contain valid data,
  - be `null`,
  - or be omitted entirely.

Example

```json
{
    "Action": "InsertText",
    "TriggerKey": "1",

    "InsertTextPayload":
    {
        "Text": "demo@example.com"
    },

    "RunCommandPayload": null,
    "WindowFocusPayload": null
}
```
<br>

# Caps Modifier Actions

## InsertText

Payload

```json
"InsertTextPayload":
{
    "Text": "..."
}
```

| Property | Type | Description |
|----------|------|-------------|
| `Text` | String | Text inserted into the active application. |

---

<br>

## OpenFileFolder

Payload

```json
"OpenFileFolderPayload":
{
    "Path": "...",
    "TargetApp": "..."
}
```

| Property | Type | Description |
|----------|------|-------------|
| `Path` | String | File, folder or URI to open |
| `TargetApp` | String | Optional application used to open `Path`. |

---

<br>

## ProfileSwitch

Payload

```json
"ProfileSwitchPayload":
{
    "TargetIndex": 0
}
```

| Property | Type | Description |
|----------|------|-------------|
| `TargetIndex` | Integer | Zero-based index inside the `Profiles` array. |

---

<br>

## RunCommand

Payload

```json
"RunCommandPayload":
{
    "Command": "...",
    "RunAsAdmin": false,
    "Hidden": false
}
```

| Property | Type | Description |
|----------|------|-------------|
| `Command` | String | Command or executable to run. Refer [Command Format](#command-format) |
| `RunAsAdmin` | Boolean | Requests administrator privileges. |
| `Hidden` | Boolean | Starts the process hidden when supported. |

---

<br>

## ShortcutRemap

Payload

```json
"RemappedKeys":
[
    ...
]
```

Type

```
Array
```

Element

```json
{
    "TargetWindow": "...",
    "ShortcutToEmit": "..."
}
```

| Property | Type | Description |
|----------|------|-------------|
| `TargetWindow` | String | Window used for matching. |
| `ShortcutToEmit` | String | Connected shortcut emitted when matched. |

---

<br>

## WindowFocus

Payload

```json
"WindowFocusPayload":
{
    "TargetExe": "...",
    "Command": "...",
    "RequiredTitle": "...",
    "ExcludeTitle": "...",
    "Fallback": "..."
}
```

| Property | Type | Description |
|----------|------|-------------|
| `TargetExe` | String | Primary window to search for. |
| `Command` | String | Executed if no suitable window is found. Refer [Command Format](#command-format) |
| `RequiredTitle` | String | Required window title, must be present as a substring. |
| `ExcludeTitle` | String | Window title exclusion, must not be present in window title. |
| `Fallback` | String | Secondary window match checked before launching `Command`. |

---

<br>

## WindowControls

Payload

```json
"WindowControlsPayload":
{
    "ControlType": "ToggleScriptMode"
}
```

| Property | Type | Description |
|----------|------|-------------|
| `ControlType` | String | Window control action. |

Allowed Values

```
TransparencyPlus
TransparencyMinus
TogglePinOnTop
ToggleClickThrough
ToggleScriptMode
```

---

<br>


## MediaControls

Payload

```json
"MediaControlsPayload":
{
    "ControlType": "MuteToggle"
}
```

| Property | Type | Description |
|----------|------|-------------|
| `ControlType` | String | Media control action. |

Allowed Values

```
VolumePlus
VolumeMinus
MuteToggle
Prev
Next
PlayPauseToggle
```

---

<br>

# WindowAwareShortcutRemap

Intercepts normal keyboard shortcuts and emits another shortcut depending on the active window.

```json
"WindowAwareShortcutRemap":
{
    "IsEnabled": true,
    "Remaps":
    [
        ...
    ]
}
```

## Remaps

Type

```
Array
```

Element

```json
{
    "TriggerKey": "...",
    "ShortcutToEmit": "...",
    "TargetWindows":
    [
        ...
    ]
}
```

Behavior

- Each element defines one shortcut remapping.
- Elements are evaluated from top to bottom.
- The first matching element is executed.
- Processing stops after the first match.

---

## TriggerKey

Type

```
String
```

Format: [Connected Shortcut](#connected-shortcut)

---

## ShortcutToEmit

Type

```
String
```

Format: [Connected Shortcut](#connected-shortcut)

---

## TargetWindows

Type

```
Array
```

Element

```
String
```

Format : [Window Identifier Format](#window-identifier-format) or use * to target all windows



Behavior

- Each entry is checked from top to bottom.
- The first matching window is used.
- `*` matches any window.
- `*` should be placed last when used as a fallback.

## Configuration Patterns

Because remaps are evaluated from top to bottom and each remap can target multiple windows, the same configuration can be written in two different ways.

### 1. Contextual Behavior

Use multiple remaps with the same `TriggerKey`.

The first matching remap is executed.

```json
[
    {
        "TriggerKey": "^+w",
        "ShortcutToEmit": "!{Tab}",
        "TargetWindows":
        [
            "ahk_exe code.exe"
        ]
    },
    {
        "TriggerKey": "^+w",
        "ShortcutToEmit": "!{F4}",
        "TargetWindows":
        [
            "*"
        ]
    }
]
```

Behavior

```
VS Code      → Alt + Tab
Everywhere   → Alt + F4
```

---

### 2. Grouped Behavior

Use one remap and group multiple windows inside `TargetWindows`.

```json
[
    {
        "TriggerKey": "^+w",
        "ShortcutToEmit": "!{F4}",
        "TargetWindows":
        [
            "ahk_exe code.exe",
            "ahk_exe chrome.exe",
            "ahk_class CabinetWClass"
        ]
    }
]
```

Behavior

```
VS Code
Chrome
Explorer

↓

Alt + F4
```


# RedactedPaste

Replaces configured text before pasting.

```json
"RedactedPaste":
{
    "IsEnabled": true,
    "TriggerKey": "^+v",
    "Replacements":
    [
        ...
    ]
}
```

## TriggerKey

Type

```
String
```

Format: [Connected Shortcut](#connected-shortcut)

---

## Replacements

Type

```
Array
```

Element

```json
{
    "DirtyText": "...",
    "ReplacementText": "..."
}
```

Behavior

- Replacements are evaluated from top to bottom.
- The first matching replacement is used.

### DirtyText

Type

```
String
```

Behavior

- Text to search for before pasting.
- Supports the `<A_UserName>` placeholder.

Example

```
C:\Users\<A_UserName>\Desktop
```

### ReplacementText

Type

```
String
```

Behavior

- Text that replaces `DirtyText`.

---

# ScreenshotTool

Captures screenshots to a configured directory.

```json
"ScreenshotTool":
{
    "IsEnabled": true,
    "TriggerKey": "^!s",
    "TargetDir": "..."
}
```

## TriggerKey

Type

```
String
```

Format: [Connected Shortcut](#connected-shortcut)

---

## TargetDir

Type

```
String
```

Behavior

- Directory where screenshots are saved.
- Supports the `<A_UserName>` placeholder.

Example

```
C:\Users\<A_UserName>\Pictures\Screenshots
```

---

<br>

# TerminalLaunch

Launches a terminal in a configured working directory.

```json
"TerminalLaunch":
{
    "IsEnabled": true,
    "TriggerKey": "^!t",
    "StartPath": "..."
}
```

## TriggerKey

Type

```
String
```

Format: [Connected Shortcut](#connected-shortcut)

---

## StartPath

Type

```
String
```

Behavior

- Working directory used when launching the terminal.
- Supports the `<A_UserName>` placeholder.

Example

```
C:\Users\<A_UserName>\
```

---

<br>

# CalcSingleInstance

Ensures only one Calculator window exists.

```json
"CalcSingleInstance":
{
    "IsEnabled": true
}
```

Behavior

- When enabled, launching Calculator using the Calculator key reuses an existing Calculator window instead of creating a new one.
- If no Calculator window exists, a new instance is launched.