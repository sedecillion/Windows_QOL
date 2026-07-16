using CommunityToolkit.Mvvm.ComponentModel;
using settings_UI.Models;
using System.Collections.ObjectModel;

namespace settings_UI.ViewModels
{
    // UI OBSERVABLE DATA MODELS
    public enum CapsActionType
    {
        WindowFocus,
        ShortcutRemap
    }

    public partial class WindowFocusData : ObservableObject
    {
        [ObservableProperty] private string _targetExe = "";
        [ObservableProperty] private string _command = "";
        [ObservableProperty] private string _requiredTitle = "";
        [ObservableProperty] private string _excludeTitle = "";
        [ObservableProperty] private string _fallback = "";
    }

    public partial class RemappedKeyPayload : ObservableObject
    {
        [ObservableProperty] private string _targetWindow = "";
        [ObservableProperty] private string _shortcutToEmit = "";
    }

    public partial class CapsKeyConfig : ObservableObject
    {
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(DisplayKey))]
        private string _triggerKey = "";

        public string DisplayKey => $"CAPS + {TriggerKey}";

        public WindowFocusData FocusPayload { get; } = new();
        public ObservableCollection<RemappedKeyPayload> RemappedKeys { get; } = [];

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsFocusMode))]
        [NotifyPropertyChangedFor(nameof(IsRemapMode))]
        [NotifyPropertyChangedFor(nameof(ActionIndex))]
        private CapsActionType _action;

        public int ActionIndex
        {
            get => (int)Action;
            set
            {
                var newAction = (CapsActionType)value;
                if (Action != newAction)
                {
                    Action = newAction;
                }
            }
        }

        public bool IsFocusMode => Action == CapsActionType.WindowFocus;
        public bool IsRemapMode => Action == CapsActionType.ShortcutRemap;
    }

    // VIEW MODEL
    public partial class CapsModifierViewModel : ObservableObject
    {
        public ObservableCollection<CapsKeyConfig> ModifierMappings { get; } = [];

        [ObservableProperty]
        private bool _isEnabled;

        public CapsModifierViewModel()
        {
        }

        public void AddNewCapsModiferKey(string key)
        {
            CapsKeyConfig capsKeyConfig = new()
            {
                Action = CapsActionType.ShortcutRemap,
                TriggerKey = key
            };
            capsKeyConfig.RemappedKeys.Add(new RemappedKeyPayload { ShortcutToEmit = "{Enter}", TargetWindow = "*" });
            ModifierMappings.Insert(0, capsKeyConfig);
        }

        public void DeleteCapsModifierKey(CapsKeyConfig capsKeyConfig)
        {
            ModifierMappings.Remove(capsKeyConfig);
        }

        public void DeleteRemappedKeyPayload(RemappedKeyPayload payloadToRemove)
        {
            // Search through all our master configs
            foreach (var config in ModifierMappings) { 
            
                if (config.RemappedKeys.Contains(payloadToRemove))
                {
                    config.RemappedKeys.Remove(payloadToRemove);
                    break;
                }
            }
        }

        public void LoadFromLoadedConfig(CapsModifierDto capsModifier)
        {
            ModifierMappings.Clear();
            if (capsModifier == null) return;

            IsEnabled = capsModifier.IsEnabled;

            if (capsModifier.ModifierMappings == null) return;

            foreach (var mappingEntry in capsModifier.ModifierMappings)
            {

                CapsKeyConfig keyConfig = new()
                {
                    TriggerKey = mappingEntry.TriggerKey
                };

                if (mappingEntry.Action == "WindowFocus")
                {
                    keyConfig.Action = CapsActionType.WindowFocus;
                    if (mappingEntry.WindowFocusPayload != null)
                    {
                        keyConfig.FocusPayload.TargetExe = mappingEntry.WindowFocusPayload.TargetExe ?? "";
                        keyConfig.FocusPayload.Command = mappingEntry.WindowFocusPayload.Command ?? "";
                        keyConfig.FocusPayload.RequiredTitle = mappingEntry.WindowFocusPayload.RequiredTitle ?? "";
                        keyConfig.FocusPayload.ExcludeTitle = mappingEntry.WindowFocusPayload.ExcludeTitle ?? "";
                        keyConfig.FocusPayload.Fallback = mappingEntry.WindowFocusPayload.Fallback ?? "";
                    }
                }
                else if (mappingEntry.Action == "ShortcutRemap")
                {
                    keyConfig.Action = CapsActionType.ShortcutRemap;
                    if (mappingEntry.RemappedKeys != null)
                    {
                        foreach (var remapEntry in mappingEntry.RemappedKeys)
                        {
                            keyConfig.RemappedKeys.Add(new RemappedKeyPayload
                            {
                                TargetWindow = remapEntry.TargetWindow ?? "",
                                ShortcutToEmit = remapEntry.ShortcutToEmit ?? ""
                            });
                        }
                    }
                }

                ModifierMappings.Add(keyConfig);
            }
        }

        public CapsModifierDto GetPackedSettings()
        {
            CapsModifierDto capsModifierData = new()
            {
                IsEnabled = IsEnabled,
                ModifierMappings = []
            };

            foreach (var keyConfig in ModifierMappings)
            {

                ModifierMappingDto entryDto = new()
                {
                    TriggerKey = keyConfig.TriggerKey,
                    Action = keyConfig.Action == CapsActionType.WindowFocus ? "WindowFocus" : "ShortcutRemap"
                };

                if (keyConfig.Action == CapsActionType.WindowFocus)
                {
                    entryDto.WindowFocusPayload = new WindowFocusPayloadDto
                    {
                        TargetExe = keyConfig.FocusPayload.TargetExe,
                        Command = keyConfig.FocusPayload.Command,
                        RequiredTitle = keyConfig.FocusPayload.RequiredTitle,
                        ExcludeTitle = keyConfig.FocusPayload.ExcludeTitle,
                        Fallback = keyConfig.FocusPayload.Fallback
                    };
                }
                else if (keyConfig.Action == CapsActionType.ShortcutRemap)
                {
                    entryDto.RemappedKeys = [];
                    foreach (var windowRemap in keyConfig.RemappedKeys)
                    {
                        entryDto.RemappedKeys.Add(new RemappedKeyPayloadDto
                        {
                            TargetWindow = windowRemap.TargetWindow,
                            ShortcutToEmit = windowRemap.ShortcutToEmit
                        });
                    }
                }

                capsModifierData.ModifierMappings.Add(entryDto);
            }

            return capsModifierData;
        }
    }
}