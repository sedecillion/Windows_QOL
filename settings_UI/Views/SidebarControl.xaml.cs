using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using settings_UI.ViewModels;
using settings_UI.Views.Modals;
using System;

namespace settings_UI.Views
{
    public sealed partial class SidebarControl : UserControl
    {
        public MainWindowViewModel MainViewModel { get; set; }

        public SidebarControl()
        {
            this.InitializeComponent();
        }

        private async void ShowAboutDialog_Click(object sender, RoutedEventArgs e)
        {
            ContentDialog dialog = new ContentDialog
            {
                Title = "About Windows QOL",
                CloseButtonText = "OK",
                DefaultButton = ContentDialogButton.Close,
                XamlRoot = this.XamlRoot,
                Content = new AboutDialogContent()
            };

            await dialog.ShowAsync();
        }
    }
}