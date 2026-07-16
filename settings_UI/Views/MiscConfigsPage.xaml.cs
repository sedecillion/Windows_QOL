using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using settings_UI.ViewModels;

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
    }
}