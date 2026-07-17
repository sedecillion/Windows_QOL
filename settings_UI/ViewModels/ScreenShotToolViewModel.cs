using CommunityToolkit.Mvvm.ComponentModel;
using settings_UI.Models;

namespace settings_UI.ViewModels
{
    public partial class ScreenShotToolViewModel : ObservableObject
    {
        [ObservableProperty]
        private bool _isEnabled;

        [ObservableProperty]
        private string _target_Dir;

        [ObservableProperty]
        private string _triggerKey;
        public ScreenShotToolViewModel()
        {

        }

        public void LoadFromLoadedConfig(ScreenshotToolDto screenshotToolDto)
        {
            IsEnabled = screenshotToolDto.IsEnabled;
            Target_Dir = screenshotToolDto.TargetDir;
            TriggerKey = screenshotToolDto.TriggerKey;
        }
        
        public ScreenshotToolDto GetPackedSettings()
        {
            ScreenshotToolDto screenshotToolDto = new()
            {
                IsEnabled = IsEnabled,
                TargetDir = Target_Dir,
                TriggerKey = TriggerKey
            };
            return screenshotToolDto;
        }
    }
}
