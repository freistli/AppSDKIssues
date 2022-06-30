using CommunityToolkit.WinUI.Helpers;
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
using System.Globalization;
using System.IO;
using System.Linq;
using System.Management;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Devices.Input;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Security.ExchangeActiveSyncProvisioning;
using Windows.System;
using Windows.System.Profile;
using Windows.UI.Core;
using WinRT.Interop;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace AppSDKIssues
{
    
    public struct OSVersion
    {
        /// <summary>
        /// Value describing major version
        /// </summary>
        public ushort Major;

        /// <summary>
        /// Value describing minor version
        /// </summary>
        public ushort Minor;

        /// <summary>
        /// Value describing build
        /// </summary>
        public ushort Build;

        /// <summary>
        /// Value describing revision
        /// </summary>
        public ushort Revision;

        /// <summary>
        /// Converts OSVersion to string
        /// </summary>
        /// <returns>Major.Minor.Build.Revision as a string</returns>
        public override string ToString()
        {
            return $"{Major}.{Minor}.{Build}.{Revision}";
        }
    }
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {

        [DllImport("kernel32")]
        private static extern void GetNativeSystemInfo(ref SYSTEM_INFO lpSystemInfo);

        public enum Architecture
        {
            Unknown,
            X86,
            X64,
            ARM64,
        }


        [StructLayout(LayoutKind.Sequential)]
        private struct SYSTEM_INFO
        {
            public short wProcessorArchitecture;
            public short wReserved;
            public int dwPageSize;
            public IntPtr lpMinimumApplicationAddress;
            public IntPtr lpMaximumApplicationAddress;
            public IntPtr dwActiveProcessorMask;
            public int dwNumberOfProcessors;
            public int dwProcessorType;
            public int dwAllocationGranularity;
            public short wProcessorLevel;
            public short wProcessorRevision;
        }
        private static Architecture GetArchitecture(ref SYSTEM_INFO si)
        {
            switch (si.wProcessorArchitecture)
            {
                case PROCESSOR_ARCHITECTURE_AMD64:
                    return Architecture.X64;

                case PROCESSOR_ARCHITECTURE_ARM64:
                    return Architecture.ARM64;

                case PROCESSOR_ARCHITECTURE_INTEL:
                    return Architecture.X86;

                default:
                    throw new PlatformNotSupportedException();
            }
        }

        private const int PROCESSOR_ARCHITECTURE_AMD64 = 9;
        private const int PROCESSOR_ARCHITECTURE_INTEL = 0;
        private const int PROCESSOR_ARCHITECTURE_ARM64 = 12;
        private const int IMAGE_FILE_MACHINE_ARM64 = 0xAA64;
        private const int IMAGE_FILE_MACHINE_I386 = 0x14C;
        private const int IMAGE_FILE_MACHINE_AMD64 = 0x8664;
         
        

        private AppWindow m_AppWindow;
        public MainWindow()
        {
            this.InitializeComponent();

            ExtendsContentIntoTitleBar = true;

            SetTitleBar(AppTitleBar);

            //IntPtr windowHandle = WinRT.Interop.WindowNative.GetWindowHandle(this);
            //WindowId windowId = Win32Interop.GetWindowIdFromWindow(windowHandle);
            //var appWindow = AppWindow.GetFromWindowId(windowId);
            //appWindow.SetIcon("image/Icon1.ico");
        }

        private AppWindow GetAppWindowForCurrentWindow()
        {
            IntPtr hWnd = WindowNative.GetWindowHandle(this);
            WindowId wndId = Win32Interop.GetWindowIdFromWindow(hWnd);
            return AppWindow.GetFromWindowId(wndId);
             
        }

        private async void SimpleMathButton_Click(object sender, RoutedEventArgs e)
        {
            WinUI3RCCPP.SimpleMath math = new WinUI3RCCPP.SimpleMath();
            ContentDialog LoadDialog = new ContentDialog()
            {
                Title = "Simple Math",
                Content = $"5.5 + 5.6 = {math.add(5.5,5.6)}",
                CloseButtonText = "Ok"
            };
            LoadDialog.XamlRoot = mathPage.XamlRoot;
            await LoadDialog.ShowAsync();
        }

        private async void systemInfo_Click(object sender, RoutedEventArgs e)
        {
            var DeviceFamily = AnalyticsInfo.VersionInfo.DeviceFamily;

            ulong version = ulong.Parse(AnalyticsInfo.VersionInfo.DeviceFamilyVersion);

            var OperatingSystemVersion = new OSVersion
            {
                Major = (ushort)((version & 0xFFFF000000000000L) >> 48),
                Minor = (ushort)((version & 0x0000FFFF00000000L) >> 32),
                Build = (ushort)((version & 0x00000000FFFF0000L) >> 16),
                Revision = (ushort)(version & 0x000000000000FFFFL)
            };

            EasClientDeviceInformation deviceInfo = new();

            var OperatingSystem = deviceInfo.OperatingSystem;
            var DeviceManufacturer = deviceInfo.SystemManufacturer;
            var DeviceModel = deviceInfo.SystemProductName;

            
            //Cannot use it in unpackaged app.
            //var AvailableMemory = (float)MemoryManager.AppMemoryUsageLimit / 1024 / 1024;

            ObjectQuery objectQuery = new ObjectQuery("SELECT * FROM Win32_OperatingSystem");
            ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher(objectQuery);
            ManagementObjectCollection managementObjectCollection = managementObjectSearcher.Get();
            string availableMemory = "";
            foreach (ManagementObject managementObject in managementObjectCollection)
            {
                availableMemory = $"{String.Format("{0:0.##}", Convert.ToDouble(managementObject["FreePhysicalMemory"]) /1024)} MB";
                managementObject.Dispose();
            }
            
            managementObjectSearcher.Dispose();
            managementObjectCollection.Dispose();

            //Cannot use it in unpackaged app.
            //var OperatingSystemArchitecture = Package.Current.Id.Architecture;

            var si = new SYSTEM_INFO();
            GetNativeSystemInfo(ref si);
            var ProcessorArch = GetArchitecture(ref si);
           
            ContentDialog LoadDialog = new ContentDialog()
            {
                Title = "Info",
                Content = $"System Version: {OperatingSystemVersion}\r\n" +
                $"Device Family: {DeviceFamily}\r\n" +
                $"Operation System: {OperatingSystem}\r\n" +
                $"Devie Manufacture: {DeviceManufacturer}\r\n" +
                $"Device Mode: {DeviceModel}\r\n" +
                $"Processor Arch: {Enum.GetName(ProcessorArch.GetType(), ProcessorArch)}\r\n" +
                $"Available Memory: {availableMemory}",
                CloseButtonText = "Ok"
            };


            LoadDialog.XamlRoot = mathPage.XamlRoot;
            await LoadDialog.ShowAsync();
        }
    }
}
