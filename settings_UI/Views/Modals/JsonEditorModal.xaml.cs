using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using settings_UI.Models;
using System;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Threading.Tasks;

namespace settings_UI.Views.Modals
{
    public sealed partial class JsonEditorModal : Window
    {
        private readonly Window _parentWindow;
        private TaskCompletionSource<ProfileDto> _tcs;

        public JsonEditorModal(Window parentWindow, string initialJson)
        {
            this.InitializeComponent();
            _parentWindow = parentWindow;

            this.ExtendsContentIntoTitleBar = true;
            this.SetTitleBar(ModalTitleBar);

            IntPtr parentHwnd = WinRT.Interop.WindowNative.GetWindowHandle(parentWindow);
            IntPtr myHwnd = WinRT.Interop.WindowNative.GetWindowHandle(this);

            uint dpi = GetDpiForWindow(myHwnd);
            float scalingFactor = (float)dpi / 96;

            AppWindow.Resize(new Windows.Graphics.SizeInt32((int)(700.0f * scalingFactor), (int)(600.0f * scalingFactor)));

            if (IntPtr.Size == 8) SetWindowLongPtr(myHwnd, -8, parentHwnd);
            else SetWindowLong(myHwnd, -8, parentHwnd);

            OverlappedPresenter presenter = OverlappedPresenter.CreateForDialog();
            presenter.IsModal = true;
            AppWindow.SetPresenter(presenter);

            this.Closed += JsonEditorModal_Closed;

            JsonTextBox.Text = initialJson;
        }

        public Task<ProfileDto> ShowAsync()
        {
            _tcs = new TaskCompletionSource<ProfileDto>();
            AppWindow.Show();
            return _tcs.Task;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var parsedProfile = JsonSerializer.Deserialize<ProfileDto>(JsonTextBox.Text);

                if (parsedProfile == null)
                {
                    ShowError("Error: Deserialized object is null.");
                    return;
                }

                _tcs.TrySetResult(parsedProfile);
                this.Close();
            }
            catch (JsonException ex)
            {
                ShowError($"JSON Format Error: {ex.Message}");
            }
            catch (Exception ex)
            {
                ShowError($"Error: {ex.Message}");
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            _tcs.TrySetResult(null);
            this.Close();
        }

        private void JsonEditorModal_Closed(object sender, WindowEventArgs args)
        {
            _tcs.TrySetResult(null);
            _parentWindow.Activate();
        }

        private void ShowError(string message)
        {
            ErrorTextBlock.Text = message;
            ErrorTextBlock.Visibility = Visibility.Visible;
        }

        [DllImport("User32.dll", ExactSpelling = true)]
        public static extern uint GetDpiForWindow(IntPtr hwnd);

        [DllImport("User32.dll", CharSet = CharSet.Auto, EntryPoint = "SetWindowLongPtr")]
        public static extern IntPtr SetWindowLongPtr(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

        [DllImport("User32.dll", CharSet = CharSet.Auto, EntryPoint = "SetWindowLong")]
        public static extern IntPtr SetWindowLong(IntPtr hWnd, int nIndex, IntPtr dwNewLong);
    }
}