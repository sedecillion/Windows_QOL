using System.Collections.Generic;
using System.Linq;
using Windows.System;

namespace settings_UI.Helpers
{
    public static class AhkKeyTranslator
    {
        public const VirtualKey CustomWheelUp = (VirtualKey)250;
        public const VirtualKey CustomWheelDown = (VirtualKey)251;
        private static readonly Dictionary<VirtualKey, string> NamedKeysMap = new()
        {
            // Mouse
            { VirtualKey.LeftButton, "LButton" },
            { VirtualKey.RightButton, "RButton" },
            { VirtualKey.MiddleButton, "MButton" },
            { VirtualKey.XButton1, "XButton1" },
            { VirtualKey.XButton2, "XButton2" },
            { CustomWheelUp, "WheelUp" },
            { CustomWheelDown, "WheelDown" },
            
            // Function Keys
            { VirtualKey.F1, "F1" }, { VirtualKey.F2, "F2" }, { VirtualKey.F3, "F3" }, { VirtualKey.F4, "F4" },
            { VirtualKey.F5, "F5" }, { VirtualKey.F6, "F6" }, { VirtualKey.F7, "F7" }, { VirtualKey.F8, "F8" },
            { VirtualKey.F9, "F9" }, { VirtualKey.F10, "F10" }, { VirtualKey.F11, "F11" }, { VirtualKey.F12, "F12" },
            
            // Navigation & Editing
            { VirtualKey.Insert, "Ins" }, { VirtualKey.Delete, "Del" },
            { VirtualKey.Home, "Home" }, { VirtualKey.End, "End" },
            { VirtualKey.PageUp, "PgUp" }, { VirtualKey.PageDown, "PgDn" },
            { VirtualKey.Up, "Up" }, { VirtualKey.Down, "Down" },
            { VirtualKey.Left, "Left" }, { VirtualKey.Right, "Right" },
            
            // Numpad
            { VirtualKey.NumberPad0, "Numpad0" }, { VirtualKey.NumberPad1, "Numpad1" },
            { VirtualKey.NumberPad2, "Numpad2" }, { VirtualKey.NumberPad3, "Numpad3" },
            { VirtualKey.NumberPad4, "Numpad4" }, { VirtualKey.NumberPad5, "Numpad5" },
            { VirtualKey.NumberPad6, "Numpad6" }, { VirtualKey.NumberPad7, "Numpad7" },
            { VirtualKey.NumberPad8, "Numpad8" }, { VirtualKey.NumberPad9, "Numpad9" },
            { VirtualKey.Divide, "NumpadDiv" }, { VirtualKey.Multiply, "NumpadMult" },
            { VirtualKey.Subtract, "NumpadSub" }, { VirtualKey.Add, "NumpadAdd" },
            { VirtualKey.Decimal, "NumpadDot" },
            
            // Misc Standard
            { VirtualKey.Escape, "Esc" },
            { VirtualKey.Tab, "Tab" },
            { VirtualKey.Space, "Space" },
            { VirtualKey.Enter, "Enter" },
            { VirtualKey.Back, "Backspace" }
        };

        // Modifiers mapped to their AHK prefix symbols
        private static readonly Dictionary<VirtualKey, string> ModifierSymbols = new()
        {
            { VirtualKey.Control, "^" }, { VirtualKey.LeftControl, "^" }, { VirtualKey.RightControl, "^" },
            { VirtualKey.Shift, "+" }, { VirtualKey.LeftShift, "+" }, { VirtualKey.RightShift, "+" },
            { VirtualKey.Menu, "!" }, { VirtualKey.LeftMenu, "!" }, { VirtualKey.RightMenu, "!" }, // Menu is Alt
            { VirtualKey.LeftWindows, "#" }, { VirtualKey.RightWindows, "#" }
        };

        public static bool IsValidKey(VirtualKey key)
        {
            return IsAlphaNumeric(key) || NamedKeysMap.ContainsKey(key) || ModifierSymbols.ContainsKey(key);
        }

        public static bool IsModifier(VirtualKey key)
        {
            return ModifierSymbols.ContainsKey(key);
        }

        private static bool IsAlphaNumeric(VirtualKey key)
        {
            int code = (int)key;
            // A-Z (65-90) or 0-9 (48-57)
            return (code >= 65 && code <= 90) || (code >= 48 && code <= 57);
        }

        public static string GetUserFriendlyName(VirtualKey key)
        {
            if (IsAlphaNumeric(key)) return key.ToString();
            if (NamedKeysMap.TryGetValue(key, out var name)) return name;

            if (key == VirtualKey.Control || key == VirtualKey.LeftControl || key == VirtualKey.RightControl) return "Ctrl";
            if (key == VirtualKey.Shift || key == VirtualKey.LeftShift || key == VirtualKey.RightShift) return "Shift";
            if (key == VirtualKey.Menu || key == VirtualKey.LeftMenu || key == VirtualKey.RightMenu) return "Alt";
            if (key == VirtualKey.LeftWindows || key == VirtualKey.RightWindows) return "Win";

            return key.ToString();
        }

        public static string GenerateAhkString(List<VirtualKey> keys, bool isEmitChord)
        {
            if (keys == null || keys.Count == 0) return "";

            string modifiers = "";
            VirtualKey mainKey = VirtualKey.None;

            // Separate modifiers from the main key
            foreach (var key in keys)
            {
                if (IsModifier(key))
                {
                    modifiers += ModifierSymbols[key];
                }
                else
                {
                    mainKey = key;
                }
            }

            modifiers = new string(modifiers.Distinct().ToArray());

            if (mainKey == VirtualKey.None) return modifiers;

            string ahkKeyName;
            bool needsBraces = false;

            if (IsAlphaNumeric(mainKey))
            {
                // ahk prefers lower case
                ahkKeyName = mainKey.ToString().ToLower(); 
                needsBraces = false;
            }
            else if (NamedKeysMap.TryGetValue(mainKey, out var name))
            {
                ahkKeyName = name;
                needsBraces = true; // Named keys like Space, Enter, LButton need braces IF emitting
            }
            else
            {
                return ""; 
            }

            if (isEmitChord && needsBraces)
            {
                return $"{modifiers}{{{ahkKeyName}}}"; //^{Space}
            }
            else
            {
                return $"{modifiers}{ahkKeyName}"; //^Space or ^a
            }
        }
    }
}