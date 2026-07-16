using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using settings_UI.ViewModels;
using settings_UI.Views.Modals;

namespace settings_UI.Views
{
    public sealed partial class TopBarControl : UserControl
    {
        public MainWindowViewModel MainViewModel
        {
            get => (MainWindowViewModel)GetValue(MainViewModelProperty);
            set => SetValue(MainViewModelProperty, value);
        }

        public static readonly DependencyProperty MainViewModelProperty =
            DependencyProperty.Register("MainViewModel", typeof(MainWindowViewModel), typeof(TopBarControl), new PropertyMetadata(null));

        public TopBarControl()
        {
            this.InitializeComponent();
        }

        private void ProfileSwitcherButton_Click(object sender, RoutedEventArgs e)
        {
            if (MainViewModel == null) return;
            ManageProfilesModal profileManager = new ManageProfilesModal(App.MainWindow, MainViewModel);
            profileManager.Closed += (s, args) => MainViewModel.SaveSettings();
            profileManager.Show();
        }

        private void ActivateProfileButton_Click(object sender, RoutedEventArgs e)
        {
            if (MainViewModel != null)
            {
                MainViewModel.ActiveProfileIndex = MainViewModel.DisplayedProfileIndex;
            }
            MainViewModel.SaveSettings();
        }

        private void MasterSaveButton_Click(object sender, RoutedEventArgs e)
        {
            MainViewModel?.SaveSettings();
        }


        private void ToggleServiceButton_Click(object sender, RoutedEventArgs e)
        {
            MainViewModel?.ToggleService();
        }

        private void StartupCheckBox_Changed(object sender, RoutedEventArgs e)
        {
            if (MainViewModel != null && sender is CheckBox cb && cb.IsChecked.HasValue)
            {
                MainViewModel.ToggleStartup(cb.IsChecked.Value);
            }
        }
    }
}