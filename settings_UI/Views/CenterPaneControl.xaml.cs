using Microsoft.UI.Xaml.Controls;
using settings_UI.ViewModels;
using System.ComponentModel;

namespace settings_UI.Views
{
    public sealed partial class CenterPaneControl : UserControl
    {
        private MainWindowViewModel _mainViewModel;
        public MainWindowViewModel MainViewModel
        {
            get => _mainViewModel;
            set
            {
                if (_mainViewModel != null)
                {
                    _mainViewModel.PropertyChanged -= MainViewModel_PropertyChanged;
                }
                _mainViewModel = value;
                if (_mainViewModel != null)
                {
                    _mainViewModel.PropertyChanged += MainViewModel_PropertyChanged;
                    NavigateToCategory();
                }
            }
        }

        public CenterPaneControl()
        {
            this.InitializeComponent();
        }

        private void MainViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(MainWindowViewModel.SelectedSettingCategory))
            {
                NavigateToCategory();
            }
        }

        private void NavigateToCategory()
        {
            var category = MainViewModel?.SelectedSettingCategory;
            if (category?.PageType != null)
            {
                // Blind Handoff: Pass whatever the Master set as the active data model
                ContentFrame.Navigate(category.PageType, MainViewModel.ActiveFeatureViewModel);
            }
        }
    }
}