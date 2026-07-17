using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using settings_UI.ViewModels;
using System;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace settings_UI.Views.Modals;

public partial class ProfileRowUI : ObservableObject
{
    [ObservableProperty] private string _profileName;
    [ObservableProperty] private bool _isActive;
    [ObservableProperty] private bool _isDisplayed;

    public Visibility ActiveBadgeVisibility => IsActive ? Visibility.Visible : Visibility.Collapsed;
    public Visibility DisplayedBadgeVisibility => (IsDisplayed && !IsActive) ? Visibility.Visible : Visibility.Collapsed;
}

public sealed partial class ManageProfilesModal : Window
{
    private readonly Window _parentWindow;
    public MainWindowViewModel ViewModel { get; private set; }

    public ObservableCollection<ProfileRowUI> ModalProfiles { get; } = new();

    public ManageProfilesModal(Window parentWindow, MainWindowViewModel viewModel)
    {
        this.InitializeComponent();
        _parentWindow = parentWindow;
        ViewModel = viewModel;

        this.ExtendsContentIntoTitleBar = true;
        this.SetTitleBar(ModalTitleBar);

        IntPtr parentHwnd = WinRT.Interop.WindowNative.GetWindowHandle(parentWindow);
        IntPtr myHwnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
        uint dpi = GetDpiForWindow(myHwnd);
        float scalingFactor = (float)dpi / 96;

        AppWindow.Resize(new Windows.Graphics.SizeInt32((int)(600.0f * scalingFactor), (int)(600.0f * scalingFactor)));

        if (IntPtr.Size == 8) SetWindowLongPtr(myHwnd, -8, parentHwnd);
        else SetWindowLong(myHwnd, -8, parentHwnd);

        OverlappedPresenter presenter = OverlappedPresenter.CreateForDialog();
        presenter.IsModal = true;
        AppWindow.SetPresenter(presenter);

        this.Closed += ManageProfilesModal_Closed;

        RefreshList();
    }

    private void RefreshList()
    {
        ModalProfiles.Clear();
        for (int i = 0; i < ViewModel.ProfileNames.Count; i++)
        {
            ModalProfiles.Add(new ProfileRowUI
            {
                ProfileName = ViewModel.ProfileNames[i],
                IsActive = (i == ViewModel.ActiveProfileIndex),
                IsDisplayed = (i == ViewModel.DisplayedProfileIndex)
            });
        }
    }

    public void Show() => AppWindow.Show();
    private void CloseButton_Click(object sender, RoutedEventArgs e) => this.Close();
    private void ManageProfilesModal_Closed(object sender, WindowEventArgs args) => _parentWindow.Activate();

    private void AddNewProfile_Click(object sender, RoutedEventArgs e)
    {
        ViewModel.AddNewProfile();
        RefreshList();
    }

    private void ViewProfile_Click(object sender, RoutedEventArgs e)
    {
        ViewModel.SaveSettings();
        if (sender is Button btn && btn.DataContext is ProfileRowUI row)
        {
            int index = ViewModel.ProfileNames.IndexOf(row.ProfileName);
            if (index >= 0)
            {
                ViewModel.DisplayedProfileIndex = index;
                ViewModel.SaveSettings();
                RefreshList();
            }
        }
    }

    private void ActivateProfile_Click(object sender, RoutedEventArgs e)
    {
        ViewModel.SaveSettings();
        if (sender is Button btn && btn.DataContext is ProfileRowUI row)
        {
            int index = ViewModel.ProfileNames.IndexOf(row.ProfileName);
            if (index >= 0)
            {
                ViewModel.ActiveProfileIndex = index;
                ViewModel.DisplayedProfileIndex = index;
                ViewModel.SaveSettings();
                RefreshList();
            }
        }
    }

    private async void RenameProfile_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.DataContext is ProfileRowUI row)
        {
            int index = ViewModel.ProfileNames.IndexOf(row.ProfileName);
            if (index < 0) return;

            TextBox inputTextBox = new TextBox { Text = row.ProfileName, Width = 300 };

            ContentDialog renameDialog = new ContentDialog
            {
                Title = "Rename Profile",
                Content = inputTextBox,
                PrimaryButtonText = "Save",
                CloseButtonText = "Cancel",
                DefaultButton = ContentDialogButton.Primary,
                XamlRoot = this.Content.XamlRoot
            };

            if (await renameDialog.ShowAsync() == ContentDialogResult.Primary && !string.IsNullOrWhiteSpace(inputTextBox.Text))
            {
                ViewModel.RenameProfile(index, inputTextBox.Text.Trim());
                RefreshList();
                ViewModel.SaveSettings();
            }
        }
    }

    private async void DeleteProfile_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.DataContext is ProfileRowUI row)
        {
            int index = ViewModel.ProfileNames.IndexOf(row.ProfileName);
            if (index < 0) return;

            if (ViewModel.ProfileNames.Count <= 1)
            {
                await ShowErrorDialog("You cannot delete the last remaining profile.");
                return;
            }

            ContentDialog deleteDialog = new ContentDialog
            {
                Title = "Delete Profile?",
                Content = $"Are you sure you want to delete '{row.ProfileName}'? This action cannot be undone.",
                PrimaryButtonText = "Delete",
                CloseButtonText = "Cancel",
                DefaultButton = ContentDialogButton.Close,
                XamlRoot = this.Content.XamlRoot
            };

            if (await deleteDialog.ShowAsync() == ContentDialogResult.Primary)
            {
                ViewModel.DeleteProfile(index);
                RefreshList();
                ViewModel.SaveSettings();
            }
        }
    }

    private async Task ShowErrorDialog(string message)
    {
        ContentDialog errorDialog = new ContentDialog
        {
            Title = "Action Denied",
            Content = message,
            CloseButtonText = "OK",
            XamlRoot = this.Content.XamlRoot
        };
        await errorDialog.ShowAsync();
    }

    [DllImport("User32.dll", ExactSpelling = true)]
    public static extern uint GetDpiForWindow(IntPtr hwnd);

    [DllImport("User32.dll", CharSet = CharSet.Auto, EntryPoint = "SetWindowLongPtr")]
    public static extern IntPtr SetWindowLongPtr(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

    [DllImport("User32.dll", CharSet = CharSet.Auto, EntryPoint = "SetWindowLong")]
    public static extern IntPtr SetWindowLong(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

    private void OpenConfigFolder_Click(object sender, RoutedEventArgs e)
    {
        ViewModel.OpenConfigFolder();
    }
}