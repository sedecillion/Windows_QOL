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
                var options = new JsonSerializerOptions { 
                    WriteIndented = true,
                    Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                };
                string jsonOutput = JsonSerializer.Serialize(CurrentConfig, options);

                string directory = Path.GetDirectoryName(_configFilePath);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                File.WriteAllText(_configFilePath, jsonOutput);
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
        ""StartPath"": ""C:\\Users\\<A_UserName>\\"",
        ""TriggerKey"": ""^!t""
      },
      ""ScreenshotTool"": {
        ""IsEnabled"": true,
        ""TargetDir"": ""C:\\Users\\<A_UserName>\\Pictures\\Screenshots"",
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
            ""TriggerKey"": ""e"",
            ""WindowFocusPayload"": {
              ""TargetExe"": ""ahk_exe explorer.exe"",
              ""Command"": ""explorer.exe"",
              ""RequiredTitle"": """",
              ""ExcludeTitle"": """",
              ""Fallback"": """"
            }
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
            }
          },
          {
            ""Action"": ""WindowFocus"",
            ""TriggerKey"": ""c"",
            ""WindowFocusPayload"": {
              ""TargetExe"": ""ahk_exe calc.exe"",
              ""Command"": ""calc.exe"",
              ""RequiredTitle"": """",
              ""ExcludeTitle"": """",
              ""Fallback"": """"
            }
          },
          {
            ""Action"": ""ShortcutRemap"",
            ""TriggerKey"": ""Space"",
            ""RemappedKeys"": [
              {
                ""TargetWindow"": ""ahk_exe explorer.exe"",
                ""ShortcutToEmit"": ""!{Enter}""
              },
              {
                ""TargetWindow"": ""ahk_exe notepad.exe"",
                ""ShortcutToEmit"": ""{F5}""
              },
              {
                ""TargetWindow"": ""ahk_exe chrome.exe"",
                ""ShortcutToEmit"": ""{F12}""
              },
              {
                ""TargetWindow"": ""ahk_exe msedge.exe"",
                ""ShortcutToEmit"": ""{F12}""
              }
            ]
          },
          {
            ""Action"": ""ShortcutRemap"",
            ""TriggerKey"": ""r"",
            ""RemappedKeys"": [
              {
                ""TargetWindow"": ""*"",
                ""ShortcutToEmit"": ""#r""
              }
            ]
          },
          {
            ""Action"": ""ShortcutRemap"",
            ""TriggerKey"": ""LButton"",
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
            ""RemappedKeys"": [
              {
                ""TargetWindow"": ""*"",
                ""ShortcutToEmit"": ""{Delete}""
              }
            ]
          },
          {
            ""Action"": ""ShortcutRemap"",
            ""TriggerKey"": ""MButton"",
            ""RemappedKeys"": [
              {
                ""TargetWindow"": ""*"",
                ""ShortcutToEmit"": ""^+{Esc}""
              }
            ]
          },
          {
            ""Action"": ""ShortcutRemap"",
            ""TriggerKey"": ""XButton1"",
            ""RemappedKeys"": [
              {
                ""TargetWindow"": ""*"",
                ""ShortcutToEmit"": ""^#{Left}""
              }
            ]
          },
          {
            ""Action"": ""ShortcutRemap"",
            ""TriggerKey"": ""XButton2"",
            ""RemappedKeys"": [
              {
                ""TargetWindow"": ""*"",
                ""ShortcutToEmit"": ""^#{Right}""
              }
            ]
          },
          {
            ""Action"": ""ShortcutRemap"",
            ""TriggerKey"": ""WheelUp"",
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
            ""Action"": ""InsertText"",
            ""TriggerKey"": ""1"",
            ""InsertTextPayload"": {
              ""Text"": ""demo_user@example.com""
            }
          },
          {
            ""Action"": ""RunCommand"",
            ""TriggerKey"": ""Numpad1"",
            ""RunCommandPayload"": {
              ""Command"": ""ms-availablenetworks:"",
              ""RunAsAdmin"": false,
              ""Hidden"": false
            }
          },
          {
            ""Action"": ""OpenFileFolder"",
            ""TriggerKey"": ""Numpad2"",
            ""OpenFileFolderPayload"": {
              ""Path"": ""C:\\Users"",
              ""TargetApp"": """"
            }
          },
          {
            ""Action"": ""RunCommand"",
            ""TriggerKey"": ""Numpad3"",
            ""RunCommandPayload"": {
              ""Command"": ""cmd.exe /k \""echo Step 1: Initializing... && timeout 1 >nul && echo Step 2: Dummy Executing... && timeout 2 >nul && echo Done. && exit\"""",
              ""RunAsAdmin"": false,
              ""Hidden"": false
            }
          }
        ]
      }
    }
  ]
}"; ; ;

            File.WriteAllText(_configFilePath, defaultJson);
        }
    }
}