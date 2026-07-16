using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using settings_UI.Models;
using settings_UI.ViewModels;
using settings_UI.Views.Modals;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace settings_UI.Views
{
    public sealed partial class CapsModifierPage : Page
    {
        public CapsModifierViewModel CapsViewModel { get; private set; }

        public CapsModifierPage()
        {
            this.InitializeComponent();
            this.NavigationCacheMode = NavigationCacheMode.Required;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (e.Parameter is CapsModifierViewModel passedVM)
            {
                CapsViewModel = passedVM;
                Bindings.Update();
            }
        }

        private async void NewCapsModiferItem_Click(object sender, RoutedEventArgs e)
        {
            var key = await CaptureKeyAsync();
            if (key == "") return;

            if (IsKeyAlreadyMapped(key))
            {
                await ShowDuplicateDialogAsync(key);
                return;
            }

            CapsViewModel.AddNewCapsModiferKey(key);
        }

        private async void ChangeTriggerKey_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is CapsKeyConfig config)
            {
                var newKey = await CaptureKeyAsync();
                if (newKey == "" || newKey == config.TriggerKey) return;

                if (IsKeyAlreadyMapped(newKey))
                {
                    await ShowDuplicateDialogAsync(newKey);
                    return;
                }

                config.TriggerKey = newKey;
            }
        }

        private void AddOverride_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is CapsKeyConfig config)
            {
                config.RemappedKeys.Add(new RemappedKeyPayload { TargetWindow = "*", ShortcutToEmit = "" });
            }
        }

        private void DeleteCapsModifierEntry_Click(object sender, RoutedEventArgs e)
        {
            var button = (FrameworkElement)sender;
            CapsViewModel.DeleteCapsModifierKey((CapsKeyConfig)button.DataContext);
        }

        private async Task<string> CaptureKeyAsync()
        {
            var captureConfig = new CaptureConfiguration
            {
                SingleKeyOnly = true,
                AllowKeyboard = true,
                AllowMouse = true,
                IsEmitChord = false,
            };

            ShortcutCaptureModal modal = new ShortcutCaptureModal(App.MainWindow, captureConfig);
            var resultKeys = await modal.ShowAsync();

            if (resultKeys != null)
            {
                return resultKeys; 
            }

            return "";
        }

        private bool IsKeyAlreadyMapped(string key)
        {
            return CapsViewModel.ModifierMappings.Any(m => m.TriggerKey == key);
        }

        private async Task ShowDuplicateDialogAsync(string key)
        {
            ContentDialog dialog = new ContentDialog
            {
                Title = "Key Already Mapped",
                Content = $"The combination CAPS + {key} is already mapped to an action. Please choose a different key or remove the existing mapping.",
                CloseButtonText = "OK",
                XamlRoot = this.XamlRoot,
                DefaultButton = ContentDialogButton.Close
            };

            await dialog.ShowAsync();
        }

        private void RemappedKeyDelete_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is RemappedKeyPayload payload)
            {
                CapsViewModel.DeleteRemappedKeyPayload(payload);
            }
        }

        private async void RemapEnterShorcut_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is RemappedKeyPayload payload)
            {
                var config = new CaptureConfiguration
                {
                    SingleKeyOnly = false,
                    AllowKeyboard = true,
                    AllowMouse = true,
                    IsEmitChord = true
                };

                ShortcutCaptureModal modal = new ShortcutCaptureModal(App.MainWindow, config);

                string capturedShortcut = await modal.ShowAsync();
                if (!string.IsNullOrEmpty(capturedShortcut))
                {
                    payload.ShortcutToEmit = capturedShortcut;
                }
            }
        }

        private async void OpenPickWindow_Click(object sender, RoutedEventArgs e)
        {
            SelectWindowModal modal = new SelectWindowModal(App.MainWindow);

            string resultAhkString = await modal.ShowModalAsync();

            if (!string.IsNullOrEmpty(resultAhkString))
            {
                if (sender is Button button && button.DataContext is CapsKeyConfig payload)
                {
                    if (button.Tag.ToString() == "TargetExe")
                    {
                        payload.FocusPayload.TargetExe = resultAhkString;
                    }
                    else if(button.Tag.ToString() == "Fallback")
                    {
                        payload.FocusPayload.Fallback = resultAhkString;
                    }
                }
                else if(sender is Button button1 && button1.DataContext is RemappedKeyPayload payload1)
                {
                    payload1.TargetWindow = resultAhkString;
                }
            }
        }
    }
}