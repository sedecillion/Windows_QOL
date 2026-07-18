using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using settings_UI.Helpers;
using settings_UI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Windows.System;

namespace settings_UI.Views.Modals;

public sealed partial class ShortcutCaptureModal : Window
{
    private readonly Window _parentWindow;
    private readonly CaptureConfiguration _config;
    private TaskCompletionSource<string> _tcs;
    private List<VirtualKey> _capturedKeys = new();
    private bool _isRecording = false;

    private string _keyboardInputHint = "";
    public ShortcutCaptureModal(Window parentWindow, CaptureConfiguration config)
    {
        this.InitializeComponent();
        _parentWindow = parentWindow;
        _config = config;

        if (config.SingleKeyOnly)
        {
            _keyboardInputHint = "Only single Key allowed";
        }
        else
        {
            _keyboardInputHint = "Press all at once, or one by one.";
        }

        this.ExtendsContentIntoTitleBar = true;
        this.SetTitleBar(ModalTitleBar);

        IntPtr parentHwnd = WinRT.Interop.WindowNative.GetWindowHandle(parentWindow);
        IntPtr myHwnd = WinRT.Interop.WindowNative.GetWindowHandle(this);

        uint dpi = GetDpiForWindow(myHwnd);
        float scalingFactor = (float)dpi / 96;

        AppWindow.Resize(new Windows.Graphics.SizeInt32((int)(600.0f * scalingFactor), (int)(420.0f * scalingFactor)));

        if (IntPtr.Size == 8) SetWindowLongPtr(myHwnd, -8, parentHwnd);
        else SetWindowLong(myHwnd, -8, parentHwnd);

        OverlappedPresenter presenter = OverlappedPresenter.CreateForDialog();
        presenter.IsModal = true;
        AppWindow.SetPresenter(presenter);

        this.Closed += ShortcutCaptureModal_Closed;

        if (!_config.AllowKeyboard) KeyboardTab.Visibility = Visibility.Collapsed;
        if (!_config.AllowMouse) MouseTab.Visibility = Visibility.Collapsed;
        InputSelector.SelectedItem = _config.AllowKeyboard ? KeyboardTab : MouseTab;

        if (this.Content is UIElement rootElement)
        {
            rootElement.PreviewKeyDown += Modal_PreviewKeyDown;
        }
    }


    public Task<string> ShowAsync()
    {
        _tcs = new TaskCompletionSource<string>();
        AppWindow.Show();
        return _tcs.Task;
    }

    private void InputSelector_SelectionChanged(SelectorBar sender, SelectorBarSelectionChangedEventArgs args)
    {
        StopRecording();
        KeyboardPanel.Visibility = sender.SelectedItem == KeyboardTab ? Visibility.Visible : Visibility.Collapsed;
        MousePanel.Visibility = sender.SelectedItem == MouseTab ? Visibility.Visible : Visibility.Collapsed;
    }

    private void Modal_PreviewKeyDown(object sender, KeyRoutedEventArgs e)
    {
        if (!_isRecording) return;

        if (!AhkKeyTranslator.IsValidKey(e.Key)) return;

        // mark handled so key dosen't stop recording or exit dialogue
        e.Handled = true;

        if (!_capturedKeys.Contains(e.Key))
        {
            _capturedKeys.Add(e.Key);
            UpdateDisplayText(KeyboardShortcutText);

            if (_config.SingleKeyOnly) StopRecording();
        }
    }

    private void RecordBtn_Checked(object sender, RoutedEventArgs e)
    {
        _isRecording = true;
        _capturedKeys.Clear();

        if (sender == KeyboardRecordBtn)
        {
            KeyboardShortcutText.Text = "Listening...";
            KeyboardCaptureBorder.Focus(FocusState.Programmatic);
        }
        else
        {
            MouseShortcutText.Text = "Click here to capture mouse button";
        }

        ((Microsoft.UI.Xaml.Controls.Primitives.ToggleButton)sender).Content = "Stop Recording";
    }

    private void RecordBtn_Unchecked(object sender, RoutedEventArgs e) => StopRecording(sender);

    private void StopRecording(object sender = null)
    {
        _isRecording = false;
        KeyboardRecordBtn.IsChecked = false;
        MouseRecordBtn.IsChecked = false;
        KeyboardRecordBtn.Content = "Start Recording";
        MouseRecordBtn.Content = "Start Recording";
    }

    private void KeyboardCaptureBorder_KeyDown(object sender, KeyRoutedEventArgs e)
    {
        if (!_isRecording) return;
        e.Handled = true;

        // Ignore unsupported keys or random symbols
        if (!AhkKeyTranslator.IsValidKey(e.Key)) return;

        if (!_capturedKeys.Contains(e.Key))
        {
            _capturedKeys.Add(e.Key);
            UpdateDisplayText(KeyboardShortcutText);

            if (_config.SingleKeyOnly) StopRecording();
        }
    }

    private void MouseCaptureBorder_PointerPressed(object sender, PointerRoutedEventArgs e)
    {
        if (!_isRecording) return;
        e.Handled = true;

        var properties = e.GetCurrentPoint(this.Content).Properties;
        VirtualKey capturedButton = VirtualKey.None;

        if (properties.IsLeftButtonPressed) capturedButton = VirtualKey.LeftButton;
        else if (properties.IsRightButtonPressed) capturedButton = VirtualKey.RightButton;
        else if (properties.IsMiddleButtonPressed) capturedButton = VirtualKey.MiddleButton;
        else if (properties.IsXButton1Pressed) capturedButton = VirtualKey.XButton1;
        else if (properties.IsXButton2Pressed) capturedButton = VirtualKey.XButton2;

        if (capturedButton != VirtualKey.None && !_capturedKeys.Contains(capturedButton))
        {
            _capturedKeys.Add(capturedButton);
            UpdateDisplayText(MouseShortcutText);

            if (_config.SingleKeyOnly) StopRecording();
        }
    }

    private void UpdateDisplayText(TextBlock targetText)
    {
        var friendlyNames = _capturedKeys.Select(AhkKeyTranslator.GetUserFriendlyName);
        targetText.Text = string.Join(" + ", friendlyNames);
    }

    private async void OKButton_Click(object sender, RoutedEventArgs e)
    {
        // Generate final AHK string and return it
        string finalAhkString = AhkKeyTranslator.GenerateAhkString(_capturedKeys, _config.IsEmitChord);
        if(finalAhkString == "#Esc" || finalAhkString == "^!+Esc")
        {
            ContentDialog errorDialog = new ContentDialog
            {
                Title = "Disallowed key chord",
                Content = $"{string.Join(" + ",_capturedKeys.Select(AhkKeyTranslator.GetUserFriendlyName))} is resereved key. Choose a different key",
                CloseButtonText = "OK",
                XamlRoot = this.Content.XamlRoot
            };
            await errorDialog.ShowAsync();
            _capturedKeys.Clear();
            UpdateDisplayText(KeyboardShortcutText);
            KeyboardShortcutText.Text = "No keys Captured...";
            return;
        }
        _tcs.TrySetResult(string.IsNullOrEmpty(finalAhkString) ? null : finalAhkString);
        this.Close();
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
        _tcs.TrySetResult(null);
        this.Close();
    }

    private void ShortcutCaptureModal_Closed(object sender, WindowEventArgs args)
    {
        _tcs.TrySetResult(null);
        _parentWindow.Activate();
    }

    [DllImport("User32.dll", ExactSpelling = true)]
    public static extern uint GetDpiForWindow(IntPtr hwnd);

    [DllImport("User32.dll", CharSet = CharSet.Auto, EntryPoint = "SetWindowLongPtr")]
    public static extern IntPtr SetWindowLongPtr(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

    [DllImport("User32.dll", CharSet = CharSet.Auto, EntryPoint = "SetWindowLong")]
    public static extern IntPtr SetWindowLong(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

    private void MouseCaptureBorder_PointerWheelChanged(object sender, PointerRoutedEventArgs e)
    {
        if (!_isRecording) return;
        e.Handled = true;

        var properties = e.GetCurrentPoint(this.Content).Properties;
        VirtualKey capturedButton = VirtualKey.None;

        if (properties.MouseWheelDelta > 0)
        {
            capturedButton = AhkKeyTranslator.CustomWheelUp;
        }
        else if (properties.MouseWheelDelta < 0)
        {
            capturedButton = AhkKeyTranslator.CustomWheelDown;
        }

        if (capturedButton != VirtualKey.None && !_capturedKeys.Contains(capturedButton))
        {
            _capturedKeys.Add(capturedButton);
            UpdateDisplayText(MouseShortcutText);

            if (_config.SingleKeyOnly) StopRecording();
        }
    }
}