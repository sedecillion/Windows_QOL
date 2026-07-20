using System.Collections.Generic;

namespace settings_UI.Models
{
    public class RootConfigDto
    {
        public int ActiveProfileIndex { get; set; }
        public List<ProfileDto> Profiles { get; set; } = [];
    }

    public class ProfileDto
    {
        public ProfilePropertiesDto ProfileProperties { get; set; }
        public TerminalLaunchDto TerminalLaunch { get; set; }
        public ScreenshotToolDto ScreenshotTool { get; set; }
        public CalcSingleInstanceDto CalcSingleInstance { get; set; }
        public WindowAwareShortcutRemapDto WindowAwareShortcutRemap { get; set; }
        public RedactedPasteDto RedactedPaste { get; set; }
        public CapsModifierDto CapsModifiers { get; set; }
    }

    public class ProfilePropertiesDto
    {
        public string Name { get; set; }
        public bool SilentMode { get; set; }
    }

    public class TerminalLaunchDto
    {
        public bool IsEnabled { get; set; }
        public string StartPath { get; set; }
        public string TriggerKey { get; set; }
    }

    public class ScreenshotToolDto
    {
        public bool IsEnabled { get; set; }
        public string TargetDir { get; set; }
        public string TriggerKey { get; set; }
    }

    public class CalcSingleInstanceDto
    {
        public bool IsEnabled { get; set; }
    }

    public class WindowAwareShortcutRemapDto
    {
        public bool IsEnabled { get; set; }
        public List<WindowAwareRemapEntryDto> Remaps { get; set; } = [];
    }

    public class WindowAwareRemapEntryDto
    {
        public string TriggerKey { get; set; }
        public string ShortcutToEmit { get; set; }
        public List<string> TargetWindows { get; set; } = [];
    }

    public class RedactedPasteDto
    {
        public bool IsEnabled { get; set; }
        public List<ReplacementEntryDto> Replacements { get; set; } = [];
        public string TriggerKey { get; set; }
    }

    public class ReplacementEntryDto
    {
        public string Dirty { get; set; }
        public string Clean { get; set; }
    }

    public class CapsModifierDto
    {
        public bool IsEnabled { get; set; }
        public List<ModifierMappingDto> ModifierMappings { get; set; } = [];
    }

    public class ModifierMappingDto
    {
        public string Action { get; set; } 
        public string TriggerKey { get; set; }
        public WindowFocusPayloadDto WindowFocusPayload { get; set; }
        public List<RemappedKeyPayloadDto> RemappedKeys { get; set; }
        public ProfileSwitchPayloadDto ProfileSwitchPayload {  get; set; }
        public InsertTextPayloadDto InsertTextPayload { get; set; }
        public OpenFileFolderPayloadDto OpenFileFolderPayload { get; set; }
        public RunCommandPayloadDto RunCommandPayload { get; set; }
        public WindowControlsPayloadDto WindowControlsPayload { get; set; }
        public MediaControlsPayloadDto MediaControlsPayload{ get; set; }
    }

    public class ProfileSwitchPayloadDto
    {
        public int TargetIndex { get; set; }
    }

    public class InsertTextPayloadDto
    {
        public string Text { get; set; }
    }
    
    public class OpenFileFolderPayloadDto
    {
        public string Path { get; set; }
        public string TargetApp { get; set; }
    }
    
    public class RunCommandPayloadDto
    {
        public string Command { get; set; }
        public bool RunAsAdmin { get; set; }
        public bool Hidden{ get; set; }

    }

    public class WindowControlsPayloadDto
    {
        public string ControlType { get; set; }
    }
    public class MediaControlsPayloadDto
    {
        public string ControlType { get; set; }
    }
    public class WindowFocusPayloadDto
    {
        public string TargetExe { get; set; }
        public string Command { get; set; }
        public string RequiredTitle { get; set; }
        public string ExcludeTitle { get; set; }
        public string Fallback { get; set; }
    }

    public class RemappedKeyPayloadDto
    {
        public string TargetWindow { get; set; }
        public string ShortcutToEmit { get; set; }
    }
}