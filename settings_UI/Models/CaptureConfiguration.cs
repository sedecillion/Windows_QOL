namespace settings_UI.Models
{
    public class CaptureConfiguration
    {
        public bool SingleKeyOnly { get; set; } = false;
        public bool AllowKeyboard { get; set; } = true;
        public bool AllowMouse { get; set; } = true;

        // If true, outputs in Send format (e.g. ^{Space} or {Enter})
        // If false, outputs in Hotkey format (e.g. ^Space or Enter)
        public bool IsEmitChord { get; set; } = false;
    }
}