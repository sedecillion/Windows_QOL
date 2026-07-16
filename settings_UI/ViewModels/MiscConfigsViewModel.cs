using CommunityToolkit.Mvvm.ComponentModel;
using settings_UI.Models;

namespace settings_UI.ViewModels
{
    public partial class MiscConfigsViewModel : ObservableObject
    {
        [ObservableProperty] private bool _isCalcSingleInstanceEnabled;
        [ObservableProperty] private bool _isTerminalLaunchEnabled;
        [ObservableProperty] private string _terminalStartPath = "";

        public void LoadFromLoadedConfig(CalcSingleInstanceDto calcConfig, TerminalLaunchDto terminalConfig)
        {
            if (calcConfig != null)
            {
                IsCalcSingleInstanceEnabled = calcConfig.IsEnabled;
            }

            if (terminalConfig != null)
            {
                IsTerminalLaunchEnabled = terminalConfig.IsEnabled;
                TerminalStartPath = terminalConfig.StartPath ?? "";
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
                StartPath = TerminalStartPath
            };
        }
    }
}