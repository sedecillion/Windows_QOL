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

        private async void PickPathFile_Click(object sender, RoutedEventArgs e)
        {
            if (sender is MenuFlyoutItem menuItem && menuItem.DataContext is CapsKeyConfig config)
            {
                menuItem.IsEnabled = false;

                var picker = new Windows.Storage.Pickers.FileOpenPicker();
                IntPtr hwnd = WinRT.Interop.WindowNative.GetWindowHandle(App.MainWindow);
                WinRT.Interop.InitializeWithWindow.Initialize(picker, hwnd);

                picker.CommitButtonText = "Pick File";
                picker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary;
                picker.ViewMode = Windows.Storage.Pickers.PickerViewMode.List;
                picker.FileTypeFilter.Add("*");

                var file = await picker.PickSingleFileAsync();
                if (file != null)
                {
                    config.OpenFileFolderPayload.Path = file.Path;
                }

                menuItem.IsEnabled = true;
            }
        }

        private async void PickPathFolder_Click(object sender, RoutedEventArgs e)
        {
            if (sender is MenuFlyoutItem menuItem && menuItem.DataContext is CapsKeyConfig config)
            {
                menuItem.IsEnabled = false;

                var picker = new Windows.Storage.Pickers.FolderPicker();
                IntPtr hwnd = WinRT.Interop.WindowNative.GetWindowHandle(App.MainWindow);
                WinRT.Interop.InitializeWithWindow.Initialize(picker, hwnd);

                picker.CommitButtonText = "Pick Folder";
                picker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.ComputerFolder;
                picker.ViewMode = Windows.Storage.Pickers.PickerViewMode.List;
                picker.FileTypeFilter.Add("*");

                var folder = await picker.PickSingleFolderAsync();
                if (folder != null)
                {
                    config.OpenFileFolderPayload.Path = folder.Path;
                }

                menuItem.IsEnabled = true;
            }
        }

        private async void PickTargetApp_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is CapsKeyConfig config)
            {
                button.IsEnabled = false;

                var picker = new Windows.Storage.Pickers.FileOpenPicker();
                IntPtr hwnd = WinRT.Interop.WindowNative.GetWindowHandle(App.MainWindow);
                WinRT.Interop.InitializeWithWindow.Initialize(picker, hwnd);

                picker.CommitButtonText = "Pick App";
                picker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.ComputerFolder;
                picker.ViewMode = Windows.Storage.Pickers.PickerViewMode.List;

                picker.FileTypeFilter.Add(".exe");
                picker.FileTypeFilter.Add(".bat");
                picker.FileTypeFilter.Add(".lnk");

                var file = await picker.PickSingleFileAsync();
                if (file != null)
                {
                    config.OpenFileFolderPayload.TargetApp = file.Path;
                }

                button.IsEnabled = true;
            }
        }
    }
}