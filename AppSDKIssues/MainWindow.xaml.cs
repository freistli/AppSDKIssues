using Microsoft.UI;
using Microsoft.UI.Input;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Devices.Input;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using WinRT.Interop;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace AppSDKIssues
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        private AppWindow m_AppWindow;
        public MainWindow()
        {
            this.InitializeComponent();

            ExtendsContentIntoTitleBar = true;
            SetTitleBar(AppTitleBar);
        }

        private AppWindow GetAppWindowForCurrentWindow()
        {
            IntPtr hWnd = WindowNative.GetWindowHandle(this);
            WindowId wndId = Win32Interop.GetWindowIdFromWindow(hWnd);
            return AppWindow.GetFromWindowId(wndId);
        }
        private void detailsGridSplitter_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            var cursor = detailsGridSplitter.GetType().GetProperty("ProtectedCursor", BindingFlags.NonPublic | BindingFlags.Instance);
            cursor.SetValue(detailsGridSplitter,InputSystemCursor.Create(Microsoft.UI.Input.InputSystemCursorShape.SizeNorthSouth));
        }

        private void slideOutGridSplitter_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            var cursor = slideOutGridSplitter.GetType().GetProperty("ProtectedCursor", BindingFlags.NonPublic | BindingFlags.Instance);
            cursor.SetValue(slideOutGridSplitter, InputSystemCursor.Create(Microsoft.UI.Input.InputSystemCursorShape.SizeWestEast));
        }
    }
}
