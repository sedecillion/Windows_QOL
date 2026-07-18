using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml;
using Microsoft.Win32;
using settings_UI.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using WindowsInput;
using WindowsInput.Native;
using System.IO;
using System.Text.Json;

namespace settings_UI.ViewModels
{
    public record NavigationItem(string Title, string IconGlyph, Type PageType);

    
    public partial class MainWindowViewModel : ObservableObject
    {
        private ConfigModel _configModel = new();

        public CapsModifierViewModel CapsVM { get; } = new();
        public ScreenShotToolViewModel SCToolVM { get; } = new();
        public RedactedPasteViewModel RedactedPasteVM { get; } = new();
        public WindowAwareShortcutViewModel WAShortcutVM{ get; } = new();
        public MiscConfigsViewModel MiscVM { get; } = new();


        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(ServiceToggleText))]
        [NotifyPropertyChangedFor(nameof(ServiceToggleIcon))]
        [NotifyPropertyChangedFor(nameof(ActivateButtonVisibility))]
        private bool _isServiceRunning;

        [ObservableProperty]
        private bool _runOnStartup;

        public string ServiceToggleText => IsServiceRunning ? "Stop Service" : "Start Service";
        public string ServiceToggleIcon => IsServiceRunning ? "\uE71A" : "\uE768"; // Stop (Square) / Play (Triangle)

        public Visibility ActivateButtonVisibility => IsServiceRunning ? Visibility.Visible : Visibility.Collapsed;

        private const string ServiceExeName = "W_QOL";


        // get the absolute path of the directory where settings_UI.exe is currently running
        private static readonly string InstallDir = AppContext.BaseDirectory;

        private static readonly string ServiceExePath = Path.Combine(InstallDir, "W_QOL.exe");
        private const string RegistryAppKeyName = "W_QOL_Service";

        // what the system is currently running
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsDisplayedProfileNotActiveProfile))]
        [NotifyPropertyChangedFor(nameof(DisplayedProfileActivateOrActivated))]
        private int _activeProfileIndex;

        // what the UI is currently editing
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(DisplayedProfileName))]
        [NotifyPropertyChangedFor(nameof(IsDisplayedProfileNotActiveProfile))]
        [NotifyPropertyChangedFor(nameof(DisplayedProfileActivateOrActivated))]
        private int _displayedProfileIndex = -1;

        public string DisplayedProfileName => _configModel.CurrentConfig.Profiles[DisplayedProfileIndex].ProfileProperties.Name;

        public bool IsDisplayedProfileNotActiveProfile => (DisplayedProfileIndex != ActiveProfileIndex);

        public string DisplayedProfileActivateOrActivated => IsDisplayedProfileNotActiveProfile ? "Activate" : "Active";

        [ObservableProperty]
        private object _activeFeatureViewModel;

        [ObservableProperty]
        private NavigationItem _selectedSettingCategory;

        public ObservableCollection<string> ProfileNames { get; } = [];

        public ObservableCollection<NavigationItem> SidebarItems { get; } = [];

        private List<ProfileDto> _profiles;

        [ObservableProperty] private bool _isNotInstalledInTrustedLocation = false;

        public MainWindowViewModel()
        {
            IsInstalledInProgramFilesCheck();

            InitConfigLoad();

            InitSideBar();

            InitializeServiceState();
        }

        private void IsInstalledInProgramFilesCheck()
        {
            string currentPath = AppContext.BaseDirectory;

            string programFiles = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
            string programFilesX86 = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);

            bool inProgramFiles = currentPath.StartsWith(programFiles, StringComparison.OrdinalIgnoreCase);
            bool inProgramFilesX86 = currentPath.StartsWith(programFilesX86, StringComparison.OrdinalIgnoreCase);

            IsNotInstalledInTrustedLocation = !(inProgramFiles || inProgramFilesX86);
        }

        private void InitConfigLoad()
        {
            _configModel.LoadConfig();

            _profiles = _configModel.CurrentConfig?.Profiles;

            foreach (var profile in _configModel.CurrentConfig.Profiles)
            {
                ProfileNames.Add(profile.ProfileProperties.Name);
            }

            ActiveProfileIndex = _configModel.CurrentConfig.ActiveProfileIndex;

            DisplayedProfileIndex = ActiveProfileIndex;
        }

        private void InitSideBar()
        {
            SidebarItems.Add(new NavigationItem("Caps Modifiers", "\uE752", typeof(Views.CapsModifierPage)));
            SidebarItems.Add(new NavigationItem("Shortcut Remap", "\uEDA7", typeof(Views.WindowAwareShortcutPage)));
            SidebarItems.Add(new NavigationItem("ScreenShot Tool", "\uF406", typeof(Views.ScreenShotToolPage)));
            SidebarItems.Add(new NavigationItem("Redacted Paste", "\uF03E", typeof(Views.RedactedPastePage)));
            SidebarItems.Add(new NavigationItem("Miscellaneous", "\uE8BC", typeof(Views.MiscConfigsPage)));

            SelectedSettingCategory = SidebarItems[0];
        }


        public void InitializeServiceState()
        {
            // Check if process is already running
            IsServiceRunning = Process.GetProcessesByName(ServiceExeName).Any();

            // Check Registry for startup state
            using RegistryKey key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", false);
            RunOnStartup = key?.GetValue(RegistryAppKeyName) != null;
        }

        public void ToggleService()
        {
            if (IsServiceRunning)
            {
                // Stop: Emit Win + Esc
                EmitWinEscape();
                IsServiceRunning = false;
            }
            else
            {
                // Start: Launch Exe
                try
                {
                    Process.Start(new ProcessStartInfo { FileName = ServiceExePath, UseShellExecute = true });
                    IsServiceRunning = true;
                }
                catch {  }
            }

            OnPropertyChanged(nameof(DisplayedProfileActivateOrActivated));
        }

        public void ToggleStartup(bool isEnabled)
        {
            using RegistryKey key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);
            if (key != null)
            {
                if (isEnabled) key.SetValue(RegistryAppKeyName, $"\"{ServiceExePath}\"");
                else key.DeleteValue(RegistryAppKeyName, false);
            }
            RunOnStartup = isEnabled;
        }

        private void EmitWinEscape()
        {
            var sim = new InputSimulator();
            sim.Keyboard.ModifiedKeyStroke(VirtualKeyCode.LWIN, VirtualKeyCode.ESCAPE);
        }

        partial void OnDisplayedProfileIndexChanged(int value)
        {
            //LoadProfileDataIntoViewModels
            var selectedProfileData = _configModel.CurrentConfig.Profiles[value];

            // call child view models
            CapsVM.LoadFromLoadedConfig(selectedProfileData.CapsModifiers);
            SCToolVM.LoadFromLoadedConfig(selectedProfileData.ScreenshotTool);
            RedactedPasteVM.LoadFromLoadedConfig(selectedProfileData.RedactedPaste);
            WAShortcutVM.LoadFromLoadedConfig(selectedProfileData.WindowAwareShortcutRemap);
            MiscVM.LoadFromLoadedConfig(selectedProfileData.CalcSingleInstance, selectedProfileData.TerminalLaunch);

            OnPropertyChanged(nameof(DisplayedProfileName));
        }

        partial void OnSelectedSettingCategoryChanged(NavigationItem value)
        {
            if (SelectedSettingCategory?.Title == "Caps Modifiers")
            {
                ActiveFeatureViewModel = CapsVM;
            }
            else if (SelectedSettingCategory?.Title == "ScreenShot Tool")
            {
                 ActiveFeatureViewModel = SCToolVM;
            }
            else if(SelectedSettingCategory?.Title == "Redacted Paste")
            {
                ActiveFeatureViewModel = RedactedPasteVM;
            }
            else if(SelectedSettingCategory?.Title == "Shortcut Remap")
            {
                ActiveFeatureViewModel = WAShortcutVM;
            }
            else if(SelectedSettingCategory?.Title == "Miscellaneous")
            {
                ActiveFeatureViewModel = MiscVM;
            }
        }


        public ProfileDto GetDefaultProfileTemplate(string profileName)
        {
            return new ProfileDto
            {
                ProfileProperties = new ProfilePropertiesDto { Name = profileName, SilentMode = true },
                TerminalLaunch = new TerminalLaunchDto { IsEnabled = true, StartPath = @"C:\Windows\System32", TriggerKey = "^!t" },
                ScreenshotTool = new ScreenshotToolDto { IsEnabled = true, TargetDir = @"D:\Pictures", TriggerKey= "PrintScreen" },
                CalcSingleInstance = new CalcSingleInstanceDto { IsEnabled = true },
                WindowAwareShortcutRemap = new WindowAwareShortcutRemapDto
                {
                    IsEnabled = true,
                    Remaps = []
                },
                RedactedPaste = new RedactedPasteDto
                {
                    IsEnabled = true,
                    TriggerKey = "^!v",
                    Replacements = []
                },
                CapsModifiers = new CapsModifierDto
                {
                    IsEnabled = true,
                    ModifierMappings = []
                }
            };
        }

        public void AddNewProfile()
        {
            string newName = "New Profile";

            int counter = 1;
            while (ProfileNames.Contains(newName))
            {
                newName = $"New Profile ({counter})";
                counter++;
            }

            var newProfile = GetDefaultProfileTemplate(newName);

            _profiles.Add(newProfile);
            ProfileNames.Add(newName);
        }

        public void RenameProfile(int index, string newName)
        {
            if (index < 0 || index >= _profiles.Count) return;

            if (ProfileNames.Contains(newName) && ProfileNames.IndexOf(newName) != index)
                return;

            _profiles[index].ProfileProperties.Name = newName;
            ProfileNames[index] = newName;

            if (index == DisplayedProfileIndex)
            {
                OnPropertyChanged(nameof(DisplayedProfileName));
            }
        }

        public void DeleteProfile(int index)
        {
            if (index < 0 || index >= _profiles.Count || _profiles.Count <= 1) return;

            _profiles.RemoveAt(index);
            ProfileNames.RemoveAt(index);

            if (ActiveProfileIndex == index)
            {
                ActiveProfileIndex = 0;
            }
            else if (ActiveProfileIndex > index)
            {
                ActiveProfileIndex--;
            }

            if (DisplayedProfileIndex == index)
            {
                DisplayedProfileIndex = 0;
            }
            else if (DisplayedProfileIndex > index)
            {
                DisplayedProfileIndex--;
            }
        }

        public void SaveSettings()
        {
            _configModel.CurrentConfig.ActiveProfileIndex = ActiveProfileIndex;

            // ask all child ViewModels to pack their UI wrappers back into DTOs
             _configModel.CurrentConfig.Profiles[DisplayedProfileIndex].CapsModifiers = CapsVM.GetPackedSettings();
            _configModel.CurrentConfig.Profiles[DisplayedProfileIndex].ScreenshotTool = SCToolVM.GetPackedSettings();
            _configModel.CurrentConfig.Profiles[DisplayedProfileIndex].RedactedPaste = RedactedPasteVM.GetPackedSettings();
            _configModel.CurrentConfig.Profiles[DisplayedProfileIndex].WindowAwareShortcutRemap = WAShortcutVM.GetPackedSettings();
            _configModel.CurrentConfig.Profiles[DisplayedProfileIndex].CalcSingleInstance = MiscVM.GetPackedCalcSingleInstance();
            _configModel.CurrentConfig.Profiles[DisplayedProfileIndex].TerminalLaunch = MiscVM.GetPackedTerminalLaunch();

            _configModel.SaveConfig();
        }

        public string GetDisplayedProfileJson()
        {
            var profile = _configModel.CurrentConfig.Profiles[DisplayedProfileIndex];

            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                // prevent + <> from turning to unicodes \u things
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };

            return JsonSerializer.Serialize(profile, options);
        }

        public void ApplyImportedProfile(ProfileDto newProfile)
        {
            _configModel.CurrentConfig.Profiles[DisplayedProfileIndex] = newProfile;
            OnDisplayedProfileIndexChanged(DisplayedProfileIndex);

            SaveSettings();
        }

        public void OpenConfigFolder()
        {
            string folderPath = Path.GetDirectoryName(_configModel._configFilePath);

            if (Directory.Exists(folderPath))
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = folderPath,
                    UseShellExecute = true,
                    Verb = "open"
                });
            }
        }
    }
}