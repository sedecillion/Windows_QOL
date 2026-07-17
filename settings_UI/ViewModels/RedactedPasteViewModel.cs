using CommunityToolkit.Mvvm.ComponentModel;
using settings_UI.Models;
using System.Collections.ObjectModel;
using System.Linq;

namespace settings_UI.ViewModels
{
    public partial class RedactedPasteViewModel : ObservableObject
    {
        [ObservableProperty] private bool _isEnabled;

        public ObservableCollection<ReplacementEntryDto> redactedPasteReplacements { get; } = [];
        [ObservableProperty] private string _triggerKey;

        public void LoadFromLoadedConfig(RedactedPasteDto redactedPaste)
        {
            redactedPasteReplacements.Clear();

            if (redactedPaste == null) return;

            IsEnabled = redactedPaste.IsEnabled;
            TriggerKey = redactedPaste.TriggerKey;

            if (redactedPaste.Replacements != null)
            {
                foreach (var replacement in redactedPaste.Replacements)
                {
                    redactedPasteReplacements.Add(replacement);
                }
            }
        }

        public RedactedPasteDto GetPackedSettings()
        {
            return new RedactedPasteDto
            {
                IsEnabled = IsEnabled,
                Replacements = redactedPasteReplacements.ToList(),
                TriggerKey = TriggerKey
            };
        }

        public void AddNewReplacement()
        {
            redactedPasteReplacements.Insert(0, new ReplacementEntryDto { Dirty = "", Clean = "" });
        }

        public void DeleteReplacement(ReplacementEntryDto entry)
        {
            redactedPasteReplacements.Remove(entry);
        }
    }
}