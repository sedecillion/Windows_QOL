using CommunityToolkit.Mvvm.ComponentModel;
using settings_UI.Helpers;
using settings_UI.Models;
using System;
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
        RunCommand,
        WindowControls,
        MediaControls
    }

    public enum WindowControlType
    {
        TransparencyPlus,
        TransparencyMinus,
        TogglePinOnTop,
        ToggleClickThrough,
        ToggleScriptMode
    }

    public enum MediaControlType
    {
        VolumePlus,
        VolumeMinus,
        MuteToggle,
        Prev,
        Next,
        PlayPauseToggle
    }

    public class EnumDropdownItem<TEnum>
    {
        public TEnum Value { get; set; }
        public string DisplayName { get; set; }

        public EnumDropdownItem(TEnum value, string displayName)
        {
            Value = value;
            DisplayName = displayName;
        }
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

    public partial class WindowControlsData : ObservableObject
    {
        public int ControlIndex
        {
            get => ControlTypeOptions.FindIndex(x => x.Value == ControlType);
            set
            {
                if (value >= 0 && value < ControlTypeOptions.Count)
                {
                    var newControl = ControlTypeOptions[value].Value;
                    if (ControlType != newControl) ControlType = newControl;
                }
            }
        }

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(ControlIndex))]
        private WindowControlType _controlType;

        public static List<EnumDropdownItem<WindowControlType>> ControlTypeOptions { get; } =
        [
            new(WindowControlType.TransparencyPlus, "Increase Transparency"),
            new(WindowControlType.TransparencyMinus, "Decrease Transparency"),
            new(WindowControlType.TogglePinOnTop, "Toggle Pin on Top"),
            new(WindowControlType.ToggleClickThrough, "Toggle Click Through"),
            new(WindowControlType.ToggleScriptMode, "Toggle Script Mode")
        ];
    }

    public partial class MediaControlsData : ObservableObject
    {
        public int ControlIndex
        {
            get => ControlTypeOptions.FindIndex(x => x.Value == ControlType);
            set
            {
                if (value >= 0 && value < ControlTypeOptions.Count)
                {
                    var newControl = ControlTypeOptions[value].Value;
                    if (ControlType != newControl) ControlType = newControl;
                }
            }
        }

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(ControlIndex))]
        private MediaControlType _controlType;

        public static List<EnumDropdownItem<MediaControlType>> ControlTypeOptions { get; } =
        [
            new(MediaControlType.VolumePlus, "Volume Up"),
            new(MediaControlType.VolumeMinus, "Volume Down"),
            new(MediaControlType.MuteToggle, "Toggle Mute"),
            new(MediaControlType.Prev, "Previous Track"),
            new(MediaControlType.Next, "Next Track"),
            new(MediaControlType.PlayPauseToggle, "Play/Pause")
        ];
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
        public WindowControlsData WindowControlsPayload { get; } = new();
        public MediaControlsData MediaControlsPayload { get; } = new();

        public int ActionIndex
        {
            get => CapsModifierViewModel.ActionOptions.FindIndex(x => x.Value == Action);
            set
            {
                if (value >= 0 && value < CapsModifierViewModel.ActionOptions.Count)
                {
                    var newAction = CapsModifierViewModel.ActionOptions[value].Value;
                    if (Action != newAction)
                    {
                        Action = newAction;
                    }
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
        [NotifyPropertyChangedFor(nameof(IsWindowControlsMode))]
        [NotifyPropertyChangedFor(nameof(IsMediaControlsMode))]
        [NotifyPropertyChangedFor(nameof(ActionIndex))]
        private CapsActionType _action;


        public bool IsFocusMode => Action == CapsActionType.WindowFocus;
        public bool IsRemapMode => Action == CapsActionType.ShortcutRemap;
        public bool IsProfileSwitchMode => Action == CapsActionType.ProfileSwitch;
        public bool IsInsertTextMode => Action == CapsActionType.InsertText;
        public bool IsOpenFileFolderMode => Action == CapsActionType.OpenFileFolder;
        public bool IsRunCommandMode => Action == CapsActionType.RunCommand;
        public bool IsWindowControlsMode => Action == CapsActionType.WindowControls;
        public bool IsMediaControlsMode => Action == CapsActionType.MediaControls;

        // the extra two params cause ProfileSwitch Action requires it
        public void LoadFromLoadedConfig(ModifierMappingDto mappingEntry, List<ProfileDto> allProfiles, int currentDisplayIndex)
        {
            TriggerKey = mappingEntry.TriggerKey ?? "";

            if (mappingEntry.Action == CapsActionType.WindowFocus.ToString())
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
            else if (mappingEntry.Action == CapsActionType.ShortcutRemap.ToString())
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
            else if (mappingEntry.Action == CapsActionType.ProfileSwitch.ToString())
            {
                Action = CapsActionType.ProfileSwitch;
                ProfileSwitchPayload.ProfileIndex = mappingEntry.ProfileSwitchPayload.TargetIndex;
                ProfileSwitchPayload.LoadFromLoadedConfig(allProfiles, currentDisplayIndex);
            }
            else if (mappingEntry.Action == CapsActionType.InsertText.ToString())
            {
                Action = CapsActionType.InsertText;
                if (mappingEntry.InsertTextPayload != null)
                {
                    InsertTextPayload.Text = mappingEntry.InsertTextPayload.Text ?? "";
                }
            }
            else if (mappingEntry.Action == CapsActionType.OpenFileFolder.ToString())
            {
                Action = CapsActionType.OpenFileFolder;
                if (mappingEntry.OpenFileFolderPayload != null)
                {
                    OpenFileFolderPayload.Path = mappingEntry.OpenFileFolderPayload.Path ?? "";
                    OpenFileFolderPayload.TargetApp = mappingEntry.OpenFileFolderPayload.TargetApp ?? "";
                }
            }
            else if (mappingEntry.Action == CapsActionType.RunCommand.ToString())
            {
                Action = CapsActionType.RunCommand;
                if (mappingEntry.RunCommandPayload != null)
                {
                    RunCommandPayload.Command = mappingEntry.RunCommandPayload.Command ?? "";
                    RunCommandPayload.RunAsAdmin = mappingEntry.RunCommandPayload.RunAsAdmin;
                    RunCommandPayload.Hidden = mappingEntry.RunCommandPayload.Hidden;
                }
            }
            else if (mappingEntry.Action == CapsActionType.WindowControls.ToString())
            {
                Action = CapsActionType.WindowControls;
                if (mappingEntry.WindowControlsPayload != null)
                {
                    if (Enum.TryParse(mappingEntry.WindowControlsPayload.ControlType, out WindowControlType parsedVal))
                        WindowControlsPayload.ControlType = parsedVal;
                }
            }
            else if (mappingEntry.Action == CapsActionType.MediaControls.ToString())
            {
                Action = CapsActionType.MediaControls;
                if (mappingEntry.MediaControlsPayload != null)
                {
                    if (Enum.TryParse(mappingEntry.MediaControlsPayload.ControlType, out MediaControlType parsedVal))
                        MediaControlsPayload.ControlType = parsedVal;
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
                entryDto.OpenFileFolderPayload = new OpenFileFolderPayloadDto
                {
                    Path = OpenFileFolderPayload.Path,
                    TargetApp = OpenFileFolderPayload.TargetApp
                };
            }
            else if (Action == CapsActionType.RunCommand)
            {
                entryDto.RunCommandPayload = new RunCommandPayloadDto
                {
                    Command = RunCommandPayload.Command,
                    RunAsAdmin = RunCommandPayload.RunAsAdmin,
                    Hidden = RunCommandPayload.Hidden
                };
            }
            else if (Action == CapsActionType.WindowControls)
            {
                entryDto.WindowControlsPayload = new WindowControlsPayloadDto
                {
                    ControlType = WindowControlsPayload.ControlType.ToString()
                };
            }
            else if (Action == CapsActionType.MediaControls)
            {
                entryDto.MediaControlsPayload = new MediaControlsPayloadDto
                {
                    ControlType = MediaControlsPayload.ControlType.ToString()
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

        public static List<EnumDropdownItem<CapsActionType>> ActionOptions { get; } =
        [
            new(CapsActionType.WindowFocus, "Window Focus"),
            new(CapsActionType.ShortcutRemap, "Shortcut Remap"),
            new(CapsActionType.ProfileSwitch, "Profile Switch"),
            new(CapsActionType.InsertText, "Insert Text"),
            new(CapsActionType.OpenFileFolder, "Open File/Folder"),
            new(CapsActionType.RunCommand, "Run Command"),
            new(CapsActionType.WindowControls, "Window Controls"),
            new(CapsActionType.MediaControls, "Media Controls")
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