using CommunityToolkit.Mvvm.ComponentModel;
using settings_UI.Models;
using System.Collections.ObjectModel;
using System.Linq;

namespace settings_UI.ViewModels
{
    public partial class TargetWindowItem : ObservableObject
    {
        [ObservableProperty] private string _windowString = "";
    }

    public partial class WindowAwareRemapConfig : ObservableObject
    {
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(DisplayTriggerKey))]
        private string _triggerKey = "";

        public string DisplayTriggerKey => string.IsNullOrEmpty(TriggerKey) ? "Click to Set" : TriggerKey;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(DisplayEmitKey))]
        private string _shortcutToEmit = "";

        public string DisplayEmitKey => string.IsNullOrEmpty(ShortcutToEmit) ? "Click to Set" : ShortcutToEmit;

        public ObservableCollection<TargetWindowItem> TargetWindows { get; } = [];
    }

    public partial class WindowAwareShortcutViewModel : ObservableObject
    {
        [ObservableProperty] private bool _isEnabled;

        public ObservableCollection<WindowAwareRemapConfig> Remaps { get; } = [];

        public void LoadFromLoadedConfig(WindowAwareShortcutRemapDto config)
        {
            Remaps.Clear();

            if (config == null) return;

            IsEnabled = config.IsEnabled;

            if (config.Remaps != null)
            {
                foreach (var remapDto in config.Remaps)
                {
                    var configItem = new WindowAwareRemapConfig
                    {
                        TriggerKey = remapDto.TriggerKey ?? "",
                        ShortcutToEmit = remapDto.ShortcutToEmit ?? ""
                    };

                    if (remapDto.TargetWindows != null)
                    {
                        foreach (var window in remapDto.TargetWindows)
                        {
                            configItem.TargetWindows.Add(new TargetWindowItem { WindowString = window });
                        }
                    }

                    Remaps.Add(configItem);
                }
            }
        }

        public WindowAwareShortcutRemapDto GetPackedSettings()
        {
            var dto = new WindowAwareShortcutRemapDto
            {
                IsEnabled = IsEnabled,
                Remaps = []
            };

            foreach (var remap in Remaps)
            {
                var entryDto = new WindowAwareRemapEntryDto
                {
                    TriggerKey = remap.TriggerKey,
                    ShortcutToEmit = remap.ShortcutToEmit,
                    // convert the wrapper objects containing window name back to list of strings for the JSON
                    TargetWindows = remap.TargetWindows.Select(w => w.WindowString).ToList()
                };
                dto.Remaps.Add(entryDto);
            }

            return dto;
        }

        public void AddNewRemap()
        {
            var newRemap = new WindowAwareRemapConfig();
            newRemap.TargetWindows.Add(new TargetWindowItem { WindowString = "*" }); // Default to wildcard
            Remaps.Insert(0, newRemap);
        }

        public void DeleteRemap(WindowAwareRemapConfig config)
        {
            Remaps.Remove(config);
        }

        public void DeleteTargetWindow(TargetWindowItem windowItem)
        {
            foreach (var remap in Remaps)
            {
                if (remap.TargetWindows.Contains(windowItem))
                {
                    remap.TargetWindows.Remove(windowItem);
                    break;
                }
            }
        }
    }
}