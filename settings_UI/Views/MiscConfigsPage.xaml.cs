using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using settings_UI.Models;
using settings_UI.ViewModels;
using settings_UI.Views.Modals;

namespace settings_UI.Views
{
    public sealed partial class MiscConfigsPage : Page
    {
        public MiscConfigsViewModel ViewModel { get; private set; }

        public MiscConfigsPage()
        {
            this.InitializeComponent();
            this.NavigationCacheMode = NavigationCacheMode.Required;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (e.Parameter is MiscConfigsViewModel passedVM)
            {
                ViewModel = passedVM;
                Bindings.Update();
            }
        }

        private async void TerminalLaunchTriggerKeyCapture_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            var config = new CaptureConfiguration
            {
                SingleKeyOnly = false,
                AllowKeyboard = true,
                AllowMouse = true,
                IsEmitChord = false
            };
            ShortcutCaptureModal modal = new(App.MainWindow, config);

            string capturedShortcut = await modal.ShowAsync();

            if (!string.IsNullOrEmpty(capturedShortcut))
            {
                ViewModel.TerminalLaunchTriggerKey = capturedShortcut;
            }
        }
    }
}