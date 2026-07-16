using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using settings_UI.Helpers;
using System;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace settings_UI.Views.Modals;

public sealed partial class SelectWindowModal : Window
{
    private readonly Window _parentWindow;
    private readonly TaskCompletionSource<string> _completionSource = new();
    public ObservableCollection<WindowItemUI> ActiveWindows { get; } = new();

    public SelectWindowModal(Window parentWindow)
    {
        this.InitializeComponent();
        _parentWindow = parentWindow;

        this.ExtendsContentIntoTitleBar = true;
        this.SetTitleBar(ModalTitleBar);

        IntPtr parentHwnd = WinRT.Interop.WindowNative.GetWindowHandle(parentWindow);
        IntPtr myHwnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
        uint dpi = GetDpiForWindow(myHwnd);
        float scalingFactor = (float)dpi / 96;

        AppWindow.Resize(new Windows.Graphics.SizeInt32((int)(600.0f * scalingFactor), (int)(700.0f * scalingFactor)));

        if (IntPtr.Size == 8) SetWindowLongPtr(myHwnd, -8, parentHwnd);
        else SetWindowLong(myHwnd, -8, parentHwnd);

        OverlappedPresenter presenter = OverlappedPresenter.CreateForDialog();
        presenter.IsModal = true;
        AppWindow.SetPresenter(presenter);

        this.Closed += SelectWindowModal_Closed;

        WindowsListView.ItemsSource = ActiveWindows;
        LoadWindows();
    }

    private void LoadWindows()
    {
        ActiveWindows.Clear();
        var windows = WindowEnumerationHelper.GetActiveWindows();
        foreach (var window in windows)
        {
            ActiveWindows.Add(window);
        }
    }

    public Task<string> ShowModalAsync()
    {
        AppWindow.Show();
        return _completionSource.Task;
    }

    private void ConfirmSelection()
    {
        if (WindowsListView.SelectedItem is WindowItemUI selectedItem)
        {
            _completionSource.TrySetResult(selectedItem.AhkTargetString);
        }
        else
        {
            _completionSource.TrySetResult(string.Empty);
        }
        this.Close();
    }

    private void SelectButton_Click(object sender, RoutedEventArgs e) => ConfirmSelection();
    private void WindowsListView_DoubleTapped(object sender, Microsoft.UI.Xaml.Input.DoubleTappedRoutedEventArgs e)
    {
        if (WindowsListView.SelectedItem is WindowItemUI selectedItem)
        {
            ConfirmSelection();
        }
    }
    private void CancelButton_Click(object sender, RoutedEventArgs e) => this.Close();

    private void SelectWindowModal_Closed(object sender, WindowEventArgs args)
    {
        _completionSource.TrySetResult(string.Empty);
        _parentWindow.Activate();
    }

    [DllImport("User32.dll", ExactSpelling = true)]
    public static extern uint GetDpiForWindow(IntPtr hwnd);

    [DllImport("User32.dll", CharSet = CharSet.Auto, EntryPoint = "SetWindowLongPtr")]
    public static extern IntPtr SetWindowLongPtr(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

    [DllImport("User32.dll", CharSet = CharSet.Auto, EntryPoint = "SetWindowLong")]
    public static extern IntPtr SetWindowLong(IntPtr hWnd, int nIndex, IntPtr dwNewLong);
}