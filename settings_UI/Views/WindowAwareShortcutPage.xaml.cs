using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using settings_UI.Models;
using settings_UI.ViewModels;
using settings_UI.Views.Modals;

namespace settings_UI.Views
{
    public sealed partial class WindowAwareShortcutPage : Page
    {
        public WindowAwareShortcutViewModel ViewModel { get; private set; }

        public WindowAwareShortcutPage()
        {
            this.InitializeComponent();
            this.NavigationCacheMode = NavigationCacheMode.Required;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (e.Parameter is WindowAwareShortcutViewModel passedVM)
            {
                ViewModel = passedVM;
                Bindings.Update();
            }
        }
        private void AddNewRemap_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.AddNewRemap();
        }

        private void DeleteRemap_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is WindowAwareRemapConfig config)
            {
                ViewModel.DeleteRemap(config);
            }
        }

        private async void ChangeTriggerKey_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is WindowAwareRemapConfig config)
            {
                var captureConfig = new CaptureConfiguration
                {
                    SingleKeyOnly = false,
                    AllowKeyboard = true,
                    AllowMouse = true,
                    IsEmitChord = false
                };

                ShortcutCaptureModal modal = new ShortcutCaptureModal(App.MainWindow, captureConfig);
                string capturedShortcut = await modal.ShowAsync();

                if (!string.IsNullOrEmpty(capturedShortcut))
                {
                    config.TriggerKey = capturedShortcut;
                }
            }
        }

        private async void ChangeEmitKey_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is WindowAwareRemapConfig config)
            {
                var captureConfig = new CaptureConfiguration
                {
                    SingleKeyOnly = false,
                    AllowKeyboard = true,
                    AllowMouse = true,
                    IsEmitChord = true 
                };

                ShortcutCaptureModal modal = new ShortcutCaptureModal(App.MainWindow, captureConfig);
                string capturedShortcut = await modal.ShowAsync();

                if (!string.IsNullOrEmpty(capturedShortcut))
                {
                    config.ShortcutToEmit = capturedShortcut;
                }
            }
        }
        private void AddTargetWindow_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is WindowAwareRemapConfig config)
            {
                config.TargetWindows.Add(new TargetWindowItem { WindowString = "*" });
            }
        }

        private void DeleteTargetWindow_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is TargetWindowItem windowItem)
            {
                ViewModel.DeleteTargetWindow(windowItem);
            }
        }

        private async void PickTargetWindow_Click(object sender, RoutedEventArgs e)
        {
            SelectWindowModal modal = new SelectWindowModal(App.MainWindow);

            string resultAhkString = await modal.ShowModalAsync();

            if (!string.IsNullOrEmpty(resultAhkString))
            {
                if (sender is Button button && button.DataContext is TargetWindowItem item)
                {
                    item.WindowString = resultAhkString;
                }
            }
        }
    }
}