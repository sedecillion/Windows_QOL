using CommunityToolkit.Mvvm.ComponentModel;
using settings_UI.Helpers;
using settings_UI.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace settings_UI.ViewModels
{
    // UI OBSERVABLE DATA MODELS
    public enum CapsActionType
    {
        WindowFocus,
        ShortcutRemap,
        ProfileSwitch,
        InsertText,
        OpenFileFolder,
        RunCommand
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
        [ObservableProperty][NotifyPropertyChangedFor(nameof(DisplayShortcutToEmit))] private string _shortcutToEmit = "";

        public string DisplayShortcutToEmit { get { return (ShortcutToEmit != "" ? AhkKeyTranslator.AhkKeysToString(ShortcutToEmit, true) : "No key assigned"); } }
    }

    public class ProfileDropdownItem
    {
        public int Index { get; set; }
        public string Name { get; set; }

        public ProfileDropdownItem(int index, string name)
        {
            Index = index;
            Name = name;
        }
    }
    public partial class ProfileSwitch : ObservableObject
    {
        [ObservableProperty] private int _profileIndex = 0;

        public ObservableCollection<ProfileDropdownItem> ProfileIndexNames { get; private set; } = [];

        [ObservableProperty]
        private ProfileDropdownItem _selectedProfileItem;

        // When the UI changes the SelectedItem, automatically update the raw integer index
        partial void OnSelectedProfileItemChanged(ProfileDropdownItem value)
        {
            if (value != null)
            {
                ProfileIndex = value.Index;
            }
        }

        public void LoadFromLoadedConfig(List<ProfileDto> profiles, int displayedProfileIndex)
        {
            ProfileIndexNames.Clear();
            if (profiles == null) return;

            for (int i = 0; i < profiles.Count; i++)
            {
                if (i == displayedProfileIndex)
                {
                    continue;
                }
                ProfileIndexNames.Add(new ProfileDropdownItem(i, profiles[i].ProfileProperties.Name));
            }

            // Sync the dropdown selection with the saved index when loading
            SelectedProfileItem = ProfileIndexNames.FirstOrDefault(p => p.Index == ProfileIndex);
        }
    }

    public partial class InsertText : ObservableObject
    {
        [ObservableProperty] private string _text;
    }

    public partial class OpenFileFolder : ObservableObject
    {
        [ObservableProperty] private string _path;
        [ObservableProperty] private string _targetApp;
    }

    public partial class RunCommand : ObservableObject
    {
        [ObservableProperty] private string _command;
        [ObservableProperty] private bool _runAsAdmin;
        [ObservableProperty] private bool _hidden;
    }


    // view model class for CapsModifiers
    public partial class CapsKeyConfig : ObservableObject
    {
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(DisplayTriggerKey))]
        private string _triggerKey = "";

        public string DisplayTriggerKey => $"CapsLock + {AhkKeyTranslator.AhkKeysToString(TriggerKey, false)}";

        [ObservableProperty]
        private bool _isCardExapanded = false;

        public WindowFocusData FocusPayload { get; } = new();
        public ObservableCollection<RemappedKeyPayload> RemappedKeys { get; } = [];
        public ProfileSwitch ProfileSwitchPayload { get; } = new();
        public InsertText InsertTextPayload { get; } = new();
        public OpenFileFolder OpenFileFolderPayload { get; } = new();
        public RunCommand RunCommandPayload { get; } = new();

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

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsFocusMode))]
        [NotifyPropertyChangedFor(nameof(IsRemapMode))]
        [NotifyPropertyChangedFor(nameof(IsProfileSwitchMode))]
        [NotifyPropertyChangedFor(nameof(IsInsertTextMode))]
        [NotifyPropertyChangedFor(nameof(IsOpenFileFolderMode))]
        [NotifyPropertyChangedFor(nameof(IsRunCommandMode))]
        [NotifyPropertyChangedFor(nameof(ActionIndex))]
        private CapsActionType _action;

        public bool IsFocusMode => Action == CapsActionType.WindowFocus;
        public bool IsRemapMode => Action == CapsActionType.ShortcutRemap;
        public bool IsProfileSwitchMode => Action == CapsActionType.ProfileSwitch;
        public bool IsInsertTextMode => Action == CapsActionType.InsertText;
        public bool IsOpenFileFolderMode => Action == CapsActionType.OpenFileFolder;
        public bool IsRunCommandMode => Action == CapsActionType.RunCommand;

        // the extra two params cause ProfileSwitch Action requires it
        public void LoadFromLoadedConfig(ModifierMappingDto mappingEntry, List<ProfileDto> allProfiles, int currentDisplayIndex)
        {
            TriggerKey = mappingEntry.TriggerKey ?? "";

            if (mappingEntry.Action == "WindowFocus")
            {
                Action = CapsActionType.WindowFocus;
                if (mappingEntry.WindowFocusPayload != null)
                {
                    FocusPayload.TargetExe = mappingEntry.WindowFocusPayload.TargetExe ?? "";
                    FocusPayload.Command = mappingEntry.WindowFocusPayload.Command ?? "";
                    FocusPayload.RequiredTitle = mappingEntry.WindowFocusPayload.RequiredTitle ?? "";
                    FocusPayload.ExcludeTitle = mappingEntry.WindowFocusPayload.ExcludeTitle ?? "";
                    FocusPayload.Fallback = mappingEntry.WindowFocusPayload.Fallback ?? "";
                }
            }
            else if (mappingEntry.Action == "ShortcutRemap")
            {
                Action = CapsActionType.ShortcutRemap;
                RemappedKeys.Clear();
                if (mappingEntry.RemappedKeys != null)
                {
                    foreach (var remapEntry in mappingEntry.RemappedKeys)
                    {
                        RemappedKeys.Add(new RemappedKeyPayload
                        {
                            TargetWindow = remapEntry.TargetWindow ?? "",
                            ShortcutToEmit = remapEntry.ShortcutToEmit ?? ""
                        });
                    }
                }
            }
            else if (mappingEntry.Action == "ProfileSwitch")
            {
                Action = CapsActionType.ProfileSwitch;
                ProfileSwitchPayload.ProfileIndex = mappingEntry.ProfileSwitchPayload.TargetIndex;
                ProfileSwitchPayload.LoadFromLoadedConfig(allProfiles, currentDisplayIndex);
            }
            else if (mappingEntry.Action == "InsertText")
            {
                Action = CapsActionType.InsertText;
                if (mappingEntry.InsertTextPayload != null)
                {
                    InsertTextPayload.Text = mappingEntry.InsertTextPayload.Text ?? "";
                }
            }
            else if (mappingEntry.Action == "OpenFileFolder")
            {
                Action = CapsActionType.OpenFileFolder;
                if (mappingEntry.OpenFileFolderPayload != null)
                {
                    OpenFileFolderPayload.Path = mappingEntry.OpenFileFolderPayload.Path ?? "";
                    OpenFileFolderPayload.TargetApp = mappingEntry.OpenFileFolderPayload.TargetApp ?? "";
                }
            }
            else if (mappingEntry.Action == "RunCommand")
            {
                Action = CapsActionType.RunCommand;
                if (mappingEntry.RunCommandPayload != null)
                {
                    RunCommandPayload.Command = mappingEntry.RunCommandPayload.Command ?? "";
                    RunCommandPayload.RunAsAdmin = mappingEntry.RunCommandPayload.RunAsAdmin;
                    RunCommandPayload.Hidden = mappingEntry.RunCommandPayload.Hidden;
                }
            }
        }

        public ModifierMappingDto GetPackedDto()
        {
            ModifierMappingDto entryDto = new()
            {
                TriggerKey = this.TriggerKey,
                Action = this.Action.ToString()
            };

            if (Action == CapsActionType.WindowFocus)
            {
                entryDto.WindowFocusPayload = new WindowFocusPayloadDto
                {
                    TargetExe = FocusPayload.TargetExe,
                    Command = FocusPayload.Command,
                    RequiredTitle = FocusPayload.RequiredTitle,
                    ExcludeTitle = FocusPayload.ExcludeTitle,
                    Fallback = FocusPayload.Fallback
                };
            }
            else if (Action == CapsActionType.ShortcutRemap)
            {
                entryDto.RemappedKeys = [];
                foreach (var windowRemap in RemappedKeys)
                {
                    entryDto.RemappedKeys.Add(new RemappedKeyPayloadDto
                    {
                        TargetWindow = windowRemap.TargetWindow,
                        ShortcutToEmit = windowRemap.ShortcutToEmit
                    });
                }
            }
            else if (Action == CapsActionType.ProfileSwitch)
            {
                entryDto.ProfileSwitchPayload = new()
                    {
                        TargetIndex = ProfileSwitchPayload.ProfileIndex
                    };
            }
            else if (Action == CapsActionType.InsertText)
            {
                entryDto.InsertTextPayload = new InsertTextPayloadDto
                {
                    Text = InsertTextPayload.Text
                };
            }
            else if (Action == CapsActionType.OpenFileFolder)
            {
                entryDto.OpenFileFolderPayload = new OpenFileFolderDto
                {
                    Path = OpenFileFolderPayload.Path,
                    TargetApp = OpenFileFolderPayload.TargetApp
                };
            }
            else if (Action == CapsActionType.RunCommand)
            {
                entryDto.RunCommandPayload = new RunCommandDto
                {
                    Command = RunCommandPayload.Command,
                    RunAsAdmin = RunCommandPayload.RunAsAdmin,
                    Hidden = RunCommandPayload.Hidden
                };
            }

            return entryDto;
        }
    }

    // VIEW MODEL
    public partial class CapsModifierViewModel : ObservableObject
    {
        public ObservableCollection<CapsKeyConfig> ModifierMappings { get; } = [];

        [NotifyPropertyChangedFor(nameof(CapslockOverriddenInfoBarVisibility))]
        [ObservableProperty] private bool _isEnabled;

        // caching so variables so AddNewCapsModiferKey will need it 
        private List<ProfileDto> _cachedProfiles = [];
        private int _cachedDisplayIndex = -1;

        [ObservableProperty] private bool _capslockOverriddenInfoBarVisibility;

        public static List<string> ActionDisplayNames { get; } =
        [
            "Window Focus",
            "Shortcut Remap",
            "Profile Switch",
            "Insert Text",
            "Open File/Folder",
            "Run Command"
        ];

        public CapsModifierViewModel()
        {
            CapslockOverriddenInfoBarVisibility = IsEnabled;
        }

        partial void OnIsEnabledChanged(bool value)
        {
            CapslockOverriddenInfoBarVisibility = value;
        }
        public void AddNewCapsModiferKey(string key)
        {
            CapsKeyConfig capsKeyConfig = new()
            {
                Action = CapsActionType.ShortcutRemap,
                TriggerKey = key,
                IsCardExapanded = true
            };
            capsKeyConfig.RemappedKeys.Add(new RemappedKeyPayload { ShortcutToEmit = "{Enter}", TargetWindow = "*" });

            capsKeyConfig.ProfileSwitchPayload.LoadFromLoadedConfig(_cachedProfiles, _cachedDisplayIndex);

            ModifierMappings.Insert(0, capsKeyConfig);
        }

        public void DeleteCapsModifierKey(CapsKeyConfig capsKeyConfig)
        {
            ModifierMappings.Remove(capsKeyConfig);
        }

        public void DeleteRemappedKeyPayload(RemappedKeyPayload payloadToRemove)
        {
            foreach (var config in ModifierMappings)
            {
                if (config.RemappedKeys.Contains(payloadToRemove))
                {
                    config.RemappedKeys.Remove(payloadToRemove);
                    break;
                }
            }
        }

        public void LoadFromLoadedConfig(CapsModifierDto capsModifier, List<ProfileDto> allProfiles, int currentDisplayIndex)
        {
            ModifierMappings.Clear();

            // store it since we need it when we add a new card and it needs its profile switch dropdown populated
            _cachedProfiles = allProfiles;
            _cachedDisplayIndex = currentDisplayIndex;

            if (capsModifier == null) return;

            IsEnabled = capsModifier.IsEnabled;

            if (capsModifier.ModifierMappings == null) return;

            foreach (var mappingEntry in capsModifier.ModifierMappings)
            {
                CapsKeyConfig keyConfig = new();
                keyConfig.LoadFromLoadedConfig(mappingEntry, allProfiles, currentDisplayIndex);
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
                capsModifierData.ModifierMappings.Add(keyConfig.GetPackedDto());
            }

            return capsModifierData;
        }
    }
}