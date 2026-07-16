using Microsoft.UI.Xaml.Controls;
using settings_UI.ViewModels;

namespace settings_UI.Views
{
    public sealed partial class SidebarControl : UserControl
    {
        public MainWindowViewModel MainViewModel { get; set; }

        public SidebarControl()
        {
            this.InitializeComponent();
        }
    }
}