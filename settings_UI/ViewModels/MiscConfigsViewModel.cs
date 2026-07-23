using CommunityToolkit.Mvvm.ComponentModel;
using settings_UI.Helpers;
using settings_UI.Models;

namespace settings_UI.ViewModels
{
    public partial class MiscConfigsViewModel : ObservableObject
    {
        [ObservableProperty] private bool _isCalcSingleInstanceEnabled;

        [ObservableProperty] private bool _isTerminalLaunchEnabled;
        [ObservableProperty] private string _terminalStartPath = "";
        [ObservableProperty] [NotifyPropertyChangedFor(nameof(DisplayTerminalLaunchTriggerKey))] private string _terminalLaunchTriggerKey = "";
        public string DisplayTerminalLaunchTriggerKey => string.IsNullOrEmpty(TerminalLaunchTriggerKey) ? "Click to Set" : AhkKeyTranslator.AhkKeysToString(TerminalLaunchTriggerKey, false);
        [ObservableProperty] private bool _openInExplorerTabPath = true;

        [ObservableProperty] private bool _isFileExplorerTweaksEnabled = true;
        [ObservableProperty] [NotifyPropertyChangedFor(nameof(DisplayFileExplorerTweaksTriggerKey))] private string _fileExplorerTweaksTriggerKey = "^n";
        public string DisplayFileExplorerTweaksTriggerKey => string.IsNullOrEmpty(FileExplorerTweaksTriggerKey) ? "Click to Set" : AhkKeyTranslator.AhkKeysToString(FileExplorerTweaksTriggerKey, false);


        public void LoadFromLoadedConfig(CalcSingleInstanceDto calcConfig, TerminalLaunchDto terminalConfig, FileExplorerTweaksDto fileExplorerTweaks)
        {
            if (calcConfig != null)
            {
                IsCalcSingleInstanceEnabled = calcConfig.IsEnabled;
            }

            if (terminalConfig != null)
            {
                IsTerminalLaunchEnabled = terminalConfig.IsEnabled;
                TerminalStartPath = terminalConfig.StartPath ?? "";
                TerminalLaunchTriggerKey = terminalConfig.TriggerKey;
                OpenInExplorerTabPath = terminalConfig.OpenInExplorerTabPath;
            }
            if(fileExplorerTweaks != null)
            {
                IsFileExplorerTweaksEnabled = fileExplorerTweaks.IsEnabled;
                FileExplorerTweaksTriggerKey = fileExplorerTweaks.TriggerKey;
            }
        }

        public CalcSingleInstanceDto GetPackedCalcSingleInstance()
        {
            return new CalcSingleInstanceDto
            {
                IsEnabled = IsCalcSingleInstanceEnabled
            };
        }

        public TerminalLaunchDto GetPackedTerminalLaunch()
        {
            return new TerminalLaunchDto
            {
                IsEnabled = IsTerminalLaunchEnabled,
                StartPath = TerminalStartPath,
                TriggerKey = TerminalLaunchTriggerKey,
                OpenInExplorerTabPath = OpenInExplorerTabPath
            };
        }

        public FileExplorerTweaksDto GetPackedFileExplorerTweaks()
        {
            return new FileExplorerTweaksDto
            {
                IsEnabled = IsFileExplorerTweaksEnabled,
                TriggerKey = FileExplorerTweaksTriggerKey
            };
        }
    }
}