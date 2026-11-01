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
        [NotifyPropertyChangedFor(nameof(IsFullScreenMode))]
        [NotifyPropertyChangedFor(nameof(IsRectangleMode))]
        [NotifyPropertyChangedFor(nameof(IsWindowMode))]
        [NotifyPropertyChangedFor(nameof(IsFreeformMode))]
        private int _screenshotMode = 0;

        public bool? IsFullScreenMode
        {
            get => ScreenshotMode == 0;
            set { if (value == true) ScreenshotMode = 0; }
        }

        public bool? IsRectangleMode
        {
            get => ScreenshotMode == 1;
            set { if (value == true) ScreenshotMode = 1; }
        }

        public bool? IsWindowMode
        {
            get => ScreenshotMode == 2;
            set { if (value == true) ScreenshotMode = 2; }
        }

        public bool? IsFreeformMode
        {
            get => ScreenshotMode == 3;
            set { if (value == true) ScreenshotMode = 3; }
        }

        public ScreenShotToolViewModel()
        {
        }

        public void LoadFromLoadedConfig(ScreenshotToolDto screenshotToolDto)
        {
            IsEnabled = screenshotToolDto.IsEnabled;
            Target_Dir = screenshotToolDto.TargetDir;
            TriggerKey = screenshotToolDto.TriggerKey;
            ScreenshotMode = screenshotToolDto.ScreenshotMode;
        }

        public ScreenshotToolDto GetPackedSettings()
        {
            ScreenshotToolDto screenshotToolDto = new()
            {
                IsEnabled = IsEnabled,
                TargetDir = Target_Dir,
                TriggerKey = TriggerKey,
                ScreenshotMode = ScreenshotMode
            };
            return screenshotToolDto;
        }
    }
}