using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Navigation;
using settings_UI.Models;
using settings_UI.ViewModels;

namespace settings_UI.Views
{
    public sealed partial class RedactedPastePage : Page
    {
        public RedactedPasteViewModel ViewModel { get; private set; }

        public RedactedPastePage()
        {
            this.InitializeComponent();
            this.NavigationCacheMode = NavigationCacheMode.Required;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (e.Parameter is RedactedPasteViewModel passedVM)
            {
                ViewModel = passedVM;
                Bindings.Update();
            }
        }

        private void AddReplacement_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.AddNewReplacement();
        }

        private void DeleteReplacement_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is ReplacementEntryDto entry)
            {
                ViewModel.DeleteReplacement(entry);
            }
        }
    }
}