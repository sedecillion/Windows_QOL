using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace settings_UI.Helpers;

public class WindowItemUI
{
    public string DisplayTitle { get; set; } = string.Empty;
    public string ExeName { get; set; } = string.Empty;
    public string AhkTargetString { get; set; } = string.Empty;

    public string FormattedAhkTarget => $"({AhkTargetString})";
}

public static class WindowEnumerationHelper
{
    private delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool EnumWindows(EnumWindowsProc lpEnumFunc, IntPtr lParam);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool IsWindowVisible(IntPtr hWnd);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

    [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
    private static extern int GetWindowTextLength(IntPtr hWnd);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern IntPtr GetWindow(IntPtr hWnd, uint uCmd);

    [DllImport("dwmapi.dll")]
    private static extern int DwmGetWindowAttribute(IntPtr hwnd, int dwAttribute, out int pvAttribute, int cbAttribute);

    private const int GWL_EXSTYLE = -20;
    private const int WS_EX_TOOLWINDOW = 0x00000080;
    private const uint GW_OWNER = 4;
    private const int DWMWA_CLOAKED = 14;

    public static List<WindowItemUI> GetActiveWindows()
    {
        List<WindowItemUI> windowList = new();

        EnumWindows((hWnd, lParam) =>
        {
            if (!IsWindowVisible(hWnd)) return true;

            DwmGetWindowAttribute(hWnd, DWMWA_CLOAKED, out int cloaked, sizeof(int));
            if (cloaked != 0) return true;

            int length = GetWindowTextLength(hWnd);
            if (length == 0) return true;


            int exStyle = GetWindowLong(hWnd, GWL_EXSTYLE);
            if ((exStyle & WS_EX_TOOLWINDOW) != 0) return true;

            if (GetWindow(hWnd, GW_OWNER) != IntPtr.Zero) return true;

            StringBuilder titleBuilder = new StringBuilder(length + 1);
            GetWindowText(hWnd, titleBuilder, titleBuilder.Capacity);
            string title = titleBuilder.ToString();

            if (title == "Program Manager") return true;

            GetWindowThreadProcessId(hWnd, out uint processId);
            string exeName = GetProcessExeName(processId);

            if (string.IsNullOrEmpty(exeName)) return true;

            string ahkTarget;
            if (string.Equals(exeName, "ApplicationFrameHost.exe", StringComparison.OrdinalIgnoreCase))
            {
                ahkTarget = $"{title} ahk_exe ApplicationFrameHost.exe";
            }
            else
            {
                ahkTarget = $"ahk_exe {exeName}";
            }

            windowList.Add(new WindowItemUI
            {
                DisplayTitle = title,
                ExeName = exeName,
                AhkTargetString = ahkTarget
            });

            return true;
        }, IntPtr.Zero);

        return windowList;
    }

    private static string GetProcessExeName(uint processId)
    {
        try
        {
            using var proc = System.Diagnostics.Process.GetProcessById((int)processId);
            return proc.MainModule?.ModuleName ?? string.Empty;
        }
        catch
        {
            return string.Empty;
        }
    }
}