using Microsoft.UI.Xaml;
using settings_UI.ViewModels;
using System;

namespace settings_UI.Views
{
    public sealed partial class MainWindow : Window
    {
        public MainWindowViewModel ViewModel { get; }

        public MainWindow()
        {
            ViewModel = new MainWindowViewModel();
            this.InitializeComponent();
            this.Title = "Windows QOL";

            string iconPath = System.IO.Path.Combine(AppContext.BaseDirectory, "Assets", "app_icon.ico");
            this.AppWindow.SetIcon(iconPath);

            this.ExtendsContentIntoTitleBar = true;
            SetTitleBar(CustomAppTitleBar);
        }
    }
}