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

        public readonly string _configFilePath;

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
                saveInputTrigger.Keyboard.ModifiedKeyStroke([VirtualKeyCode.CONTROL, VirtualKeyCode.MENU, VirtualKeyCode.SHIFT], VirtualKeyCode.ESCAPE);
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
        ""StartPath"": ""C:\\\\Users\\\\<A_UserName>\\\\"",
        ""TriggerKey"": ""^!t""
      },
      ""ScreenshotTool"": {
        ""IsEnabled"": true,
        ""TargetDir"": ""D:\\Pictures"",
        ""TriggerKey"": ""PrintScreen""
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
              ""*""
            ]
          }
        ]
      },
      ""RedactedPaste"": {
        ""IsEnabled"": true,
        ""Replacements"": [
          {
            ""Dirty"": ""fullname"",
            ""Clean"": """"
          },
          {
            ""Dirty"": ""<A_UserName>"",
            ""Clean"": ""user1""
          }
        ],
        ""TriggerKey"": ""^!v""
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
              ""TargetExe"": ""ahk_exe explorer.exe"",
              ""Command"": ""explorer.exe"",
              ""RequiredTitle"": """",
              ""ExcludeTitle"": """",
              ""Fallback"": """"
            },
            ""RemappedKeys"": null
          },
          {
            ""Action"": ""WindowFocus"",
            ""TriggerKey"": ""d"",
            ""WindowFocusPayload"": {
              ""TargetExe"": ""ahk_exe notepad.exe"",
              ""Command"": ""notepad.exe"",
              ""RequiredTitle"": """",
              ""ExcludeTitle"": """",
              ""Fallback"": """"
            },
            ""RemappedKeys"": null
          },
          {
            ""Action"": ""WindowFocus"",
            ""TriggerKey"": ""c"",
            ""WindowFocusPayload"": {
              ""TargetExe"": ""ahk_exe Code.exe"",
              ""Command"": """",
              ""RequiredTitle"": """",
              ""ExcludeTitle"": """",
              ""Fallback"": ""ahk_exe devenv.exe""
            },
            ""RemappedKeys"": null
          },
          {
            ""Action"": ""WindowFocus"",
            ""TriggerKey"": ""r"",
            ""WindowFocusPayload"": {
              ""TargetExe"": ""ahk_exe chrome.exe"",
              ""Command"": """",
              ""RequiredTitle"": """",
              ""ExcludeTitle"": """",
              ""Fallback"": ""ahk_exe chrome.exe""
            },
            ""RemappedKeys"": null
          },
          {
            ""Action"": ""WindowFocus"",
            ""TriggerKey"": ""t"",
            ""WindowFocusPayload"": {
              ""TargetExe"": ""ahk_exe chrome.exe"",
              ""Command"": ""\u0022C:\\Program Files\\Google\\Chrome\\Application\\chrome.exe\u0022 --profile-directory=\u0022Default\u0022 --incognito"",
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
            ""TriggerKey"": ""RButton"",
            ""WindowFocusPayload"": null,
            ""RemappedKeys"": [
              {
                ""TargetWindow"": ""*"",
                ""ShortcutToEmit"": ""{Delete}""
              }
            ]
          },
          {
            ""Action"": ""ShortcutRemap"",
            ""TriggerKey"": ""XButton1"",
            ""WindowFocusPayload"": null,
            ""RemappedKeys"": [
              {
                ""TargetWindow"": ""*"",
                ""ShortcutToEmit"": ""#d""
              }
            ]
          },
          {
            ""Action"": ""ShortcutRemap"",
            ""TriggerKey"": ""XButton2"",
            ""WindowFocusPayload"": null,
            ""RemappedKeys"": [
              {
                ""TargetWindow"": ""*"",
                ""ShortcutToEmit"": ""!{F4}""
              }
            ]
          },
          {
            ""Action"": ""ShortcutRemap"",
            ""TriggerKey"": ""MButton"",
            ""WindowFocusPayload"": null,
            ""RemappedKeys"": [
              {
                ""TargetWindow"": ""*"",
                ""ShortcutToEmit"": ""^+{Esc}""
              }
            ]
          },
          {
            ""Action"": ""ShortcutRemap"",
            ""TriggerKey"": ""WheelUp"",
            ""WindowFocusPayload"": null,
            ""RemappedKeys"": [
              {
                ""TargetWindow"": ""Code.exe"",
                ""ShortcutToEmit"": ""^{PgUp}""
              },
              {
                ""TargetWindow"": ""*"",
                ""ShortcutToEmit"": ""^+{Tab}""
              }
            ]
          },
          {
            ""Action"": ""ShortcutRemap"",
            ""TriggerKey"": ""WheelDown"",
            ""WindowFocusPayload"": null,
            ""RemappedKeys"": [
              {
                ""TargetWindow"": ""Code.exe"",
                ""ShortcutToEmit"": ""^{PgDn}""
              },
              {
                ""TargetWindow"": ""*"",
                ""ShortcutToEmit"": ""^{Tab}""
              }
            ]
          },
          {
            ""Action"": ""ShortcutRemap"",
            ""TriggerKey"": ""Space"",
            ""WindowFocusPayload"": null,
            ""RemappedKeys"": [
              {
                ""TargetWindow"": ""ahk_exe explorer.exe"",
                ""ShortcutToEmit"": ""!{Up}""
              },
              {
                ""TargetWindow"": ""Code.exe"",
                ""ShortcutToEmit"": ""^{vkC0}""
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