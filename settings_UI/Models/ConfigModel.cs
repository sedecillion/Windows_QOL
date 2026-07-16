using System;
using System.IO;
using System.Text.Json;
using WindowsInput;
using WindowsInput.Native;
namespace settings_UI.Models
{
    public class ConfigModel
    {
        private const string FolderName = "Windows_QOL";
        private const string FileName = "config.json";

        private readonly string _configFilePath;

        public RootConfigDto CurrentConfig { get; private set; }

        public ConfigModel()
        {
            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            _configFilePath = Path.Combine(appDataPath, FolderName, FileName);
        }

        public void LoadConfig()
        {
            try
            {
                if (!File.Exists(_configFilePath))
                {
                    InitializeDefaultConfigFile();
                }

                // Read the actual data from the file
                string rawJson = File.ReadAllText(_configFilePath);
                CurrentConfig = JsonSerializer.Deserialize<RootConfigDto>(rawJson);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to process config file at: {_configFilePath}", ex);
            }
        }

        public void SaveConfig()
        {
            if (CurrentConfig == null) return;

            try
            {
                var options = new JsonSerializerOptions { WriteIndented = true };
                string jsonOutput = JsonSerializer.Serialize(CurrentConfig, options);

                string directory = Path.GetDirectoryName(_configFilePath);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                File.WriteAllText(_configFilePath, jsonOutput);
                InputSimulator saveInputTrigger = new();
                saveInputTrigger.Keyboard.ModifiedKeyStroke([VirtualKeyCode.CONTROL, VirtualKeyCode.MENU], VirtualKeyCode.VK_R);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to write config file to disk at: {_configFilePath}", ex);
            }
        }

        private void InitializeDefaultConfigFile()
        {
            string directory = Path.GetDirectoryName(_configFilePath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            string defaultJson = @"
{
    ""ActiveProfileIndex"": 0,
    ""Profiles"": [
    {
        ""ProfileProperties"": {
        ""Name"": ""Default Profile"",
        ""SilentMode"": true
        },
        ""TerminalLaunch"": {
        ""IsEnabled"": true,
        ""StartPath"": ""C:\\Windows\\System32""
        },
        ""ScreenshotTool"": {
        ""IsEnabled"": true,
        ""TargetDir"": ""D:\\Pictures""
        },
        ""CalcSingleInstance"": {
        ""IsEnabled"": true
        },
        ""WindowAwareShortcutRemap"": {
        ""IsEnabled"": true,
        ""Remaps"": [
            {
            ""TriggerKey"": ""^+w"",
            ""ShortcutToEmit"": ""!{F4}"",
            ""TargetWindows"": [
                ""taskmgr.exe""
            ]
            }
        ]
        },
        ""RedactedPaste"": {
        ""IsEnabled"": true,
        ""Replacements"": [
            {
            ""Dirty"": ""user1lname"",
            ""Clean"": ""fullname""
            },
            {
            ""Dirty"": ""username1"",
            ""Clean"": ""user1""
            }
        ]
        },
        ""CapsModifiers"": {
        ""IsEnabled"": true,
        ""ModifierMappings"": [
            {
            ""Action"": ""WindowFocus"",
            ""TriggerKey"": ""x"",
            ""WindowFocusPayload"": {
                ""TargetExe"": ""*"",
                ""Command"": """",
                ""RequiredTitle"": """",
                ""ExcludeTitle"": """",
                ""Fallback"": """"
            },
            ""RemappedKeys"": null
            },
            {
            ""Action"": ""WindowFocus"",
            ""TriggerKey"": ""e"",
            ""WindowFocusPayload"": {
                ""TargetExe"": ""ahk_class CabinetWClass"",
                ""Command"": ""explorer.exe"",
                ""RequiredTitle"": """",
                ""ExcludeTitle"": """",
                ""Fallback"": """"
            },
            ""RemappedKeys"": null
            },
            {
            ""Action"": ""ShortcutRemap"",
            ""TriggerKey"": ""LButton"",
            ""WindowFocusPayload"": null,
            ""RemappedKeys"": [
                {
                ""TargetWindow"": ""*"",
                ""ShortcutToEmit"": ""{Enter}""
                }
            ]
            },
            {
            ""Action"": ""ShortcutRemap"",
            ""TriggerKey"": ""Space"",
            ""WindowFocusPayload"": null,
            ""RemappedKeys"": [
                {
                ""TargetWindow"": ""ahk_class CabinetWClass"",
                ""ShortcutToEmit"": ""!{Up}""
                },
                {
                ""TargetWindow"": ""chrome.exe"",
                ""ShortcutToEmit"": ""{F12}""
                }
            ]
            }
        ]
        }
    }
    ]
}";

            File.WriteAllText(_configFilePath, defaultJson);
        }
    }
}