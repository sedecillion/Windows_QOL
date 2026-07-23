using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using settings_UI.Models;
using settings_UI.ViewModels;
using settings_UI.Views.Modals;
using System;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace settings_UI.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ScreenShotToolPage : Page
    {
        public ScreenShotToolViewModel SC_ToolViewModel { get; set; }
        public ScreenShotToolPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (e.Parameter is ScreenShotToolViewModel passedVM)
            {
                SC_ToolViewModel = passedVM;
                Bindings.Update();
            }
        }

        private async void SCToolPickFolder(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                button.IsEnabled = false;

                var picker = new Windows.Storage.Pickers.FolderPicker();

                // 1. Get the window handle (HWND) of your main window
                IntPtr hwnd = WinRT.Interop.WindowNative.GetWindowHandle(App.MainWindow);

                // 2. Bind the picker to the window handle
                WinRT.Interop.InitializeWithWindow.Initialize(picker, hwnd);

                picker.CommitButtonText = "Pick Folder";
                picker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.PicturesLibrary;
                picker.ViewMode = Windows.Storage.Pickers.PickerViewMode.List;

                var folder = await picker.PickSingleFolderAsync();
                if (folder != null)
                {
                    SC_ToolViewModel.Target_Dir = folder.Path;
                };

                button.IsEnabled = true;
            }
        }

        private async void SC_ToolTriggerKeyPicker_Click(object sender, RoutedEventArgs e)
        {
            var config = new CaptureConfiguration
            {
                SingleKeyOnly = false,
                AllowKeyboard = true,
                AllowMouse = true,
                IsEmitChord = false
            };
            ShortcutCaptureModal modal = new(App.MainWindow,config);

            string capturedShortcut = await modal.ShowAsync();

            if (!string.IsNullOrEmpty(capturedShortcut))
            {
                SC_ToolViewModel.TriggerKey = capturedShortcut;
            }

        }

        private void TriggerKeyChangePrintScreen_Click(object sender, RoutedEventArgs e)
        {
            SC_ToolViewModel.TriggerKey = "PrintScreen";
        }

        
    }
}
