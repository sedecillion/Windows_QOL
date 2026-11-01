using Microsoft.UI.Xaml.Controls;
using settings_UI.Models;

namespace settings_UI.Views.Modals
{
    public sealed partial class AboutDialogContent : UserControl
    {
        public string AppVersion => ConfigModel.AppVersion;

        public AboutDialogContent()
        {
            this.InitializeComponent();
        }
    }
}