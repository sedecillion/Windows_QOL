using CommunityToolkit.Mvvm.ComponentModel;
using settings_UI.Helpers;
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
        [NotifyPropertyChangedFor(nameof(DisplayTriggerKey))]
        private string _triggerKey;

        public string DisplayTriggerKey => string.IsNullOrEmpty(TriggerKey) ? "Click to Set" : AhkKeyTranslator.AhkKeysToString(TriggerKey, false);

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsInteractiveMode))]
        [NotifyPropertyChangedFor(nameof(IsBasicMode))]
        private bool _fullScreenModeOnly = true;

        public bool IsInteractiveMode
        {
            get => !FullScreenModeOnly;
            set
            {
                if (value) FullScreenModeOnly = false;
            }
        }

        public bool IsBasicMode
        {
            get => FullScreenModeOnly;
            set
            {
                if (value) FullScreenModeOnly = true;
            }
        }

        public ScreenShotToolViewModel()
        {

        }

        public void LoadFromLoadedConfig(ScreenshotToolDto screenshotToolDto)
        {
            IsEnabled = screenshotToolDto.IsEnabled;
            Target_Dir = screenshotToolDto.TargetDir;
            TriggerKey = screenshotToolDto.TriggerKey;
            FullScreenModeOnly = screenshotToolDto.FullScreenModeOnly;
        }
        
        public ScreenshotToolDto GetPackedSettings()
        {
            ScreenshotToolDto screenshotToolDto = new()
            {
                IsEnabled = IsEnabled,
                TargetDir = Target_Dir,
                TriggerKey = TriggerKey,
                FullScreenModeOnly = FullScreenModeOnly
            };
            return screenshotToolDto;
        }
    }
}
