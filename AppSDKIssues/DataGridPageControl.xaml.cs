using AppSDKIssues.Model;
using CommunityToolkit.WinUI.UI;
using CommunityToolkit.WinUI.UI.Controls;
using Microsoft.Graphics.Canvas;
using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI.Xaml.Printing;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Streams;
using WinRT.Interop;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace AppSDKIssues
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class DataGridPageControl : Page
    {  
        private MenuFlyoutItem rankLowItem;
        private MenuFlyoutItem rankHighItem;
        private MenuFlyoutItem heightLowItem;
        private MenuFlyoutItem heightHighItem;
        private DataGridDataSource viewModel = new DataGridDataSource();

        private AppWindow GetAppWindowForCurrentWindow()
        {
            IntPtr hWnd = WindowNative.GetWindowHandle(this);
            WindowId wndId = Win32Interop.GetWindowIdFromWindow(hWnd);
            return AppWindow.GetFromWindowId(wndId);
        }
        public DataGridPageControl()
        {
            this.InitializeComponent();

            this.Loaded += DataGridPageControl_Loaded;
            /*
            printHelper = new PrintHelper(this);
            printHelper.RegisterForPrinting();

            // Initialize print content for this scenario
            printHelper.PreparePrintContent(new PageToPrint());
            */
        }

        private async void DataGridPageControl_Loaded(object sender, RoutedEventArgs e)
        {

            //dataGrid.ItemsSource = await viewModel.GetDataAsync();
        }

        private void DataGrid_PreparingCellForEdit(object sender, DataGridPreparingCellForEditEventArgs e)
        {
            if (e.Column is DataGridTemplateColumn column && (string)column?.Tag == "First_ascent" &&
                e.EditingElement is CalendarDatePicker calendar)
            {
                calendar.IsCalendarOpen = true;
            }
        }

        private void DataGrid_LoadingRowGroup(object sender, DataGridRowGroupHeaderEventArgs e)
        {
            ICollectionViewGroup group = e.RowGroupHeader.CollectionViewGroup;
            DataGridDataItem item = group.GroupItems[0] as DataGridDataItem;
            e.RowGroupHeader.PropertyValue = item.Range;
        }

        private void GroupButton_Click(object sender, RoutedEventArgs e)
        {
            if (dataGrid != null)
            {
                dataGrid.ItemsSource = viewModel.GroupData().View;
            }
        }

        private void DataGrid_Sorting(object sender, DataGridColumnEventArgs e)
        {
            // Clear previous sorted column if we start sorting a different column
            string previousSortedColumn = viewModel.CachedSortedColumn;
            if (previousSortedColumn != string.Empty)
            {
                foreach (DataGridColumn dataGridColumn in dataGrid.Columns)
                {
                    if (dataGridColumn.Tag != null && dataGridColumn.Tag.ToString() == previousSortedColumn &&
                        (e.Column.Tag == null || previousSortedColumn != e.Column.Tag.ToString()))
                    {
                        dataGridColumn.SortDirection = null;
                    }
                }
            }

            // Toggle clicked column's sorting method
            if (e.Column.Tag != null)
            {
                if (e.Column.SortDirection == null)
                {
                    dataGrid.ItemsSource = viewModel.SortData(e.Column.Tag.ToString(), true);
                    e.Column.SortDirection = DataGridSortDirection.Ascending;
                }
                else if (e.Column.SortDirection == DataGridSortDirection.Ascending)
                {
                    dataGrid.ItemsSource = viewModel.SortData(e.Column.Tag.ToString(), false);
                    e.Column.SortDirection = DataGridSortDirection.Descending;
                }
                else
                {
                    dataGrid.ItemsSource = viewModel.FilterData(DataGridDataSource.FilterOptions.All);
                    e.Column.SortDirection = null;
                }
            }
        }

        private void RankLowItem_Click(object sender, RoutedEventArgs e)
        {
            if (dataGrid != null)
            {
                dataGrid.ItemsSource = viewModel.FilterData(DataGridDataSource.FilterOptions.Rank_Low);
            }
        }

        private void RankHigh_Click(object sender, RoutedEventArgs e)
        {
            if (dataGrid != null)
            {
                dataGrid.ItemsSource = viewModel.FilterData(DataGridDataSource.FilterOptions.Rank_High);
            }
        }

        private void HeightLow_Click(object sender, RoutedEventArgs e)
        {
            if (dataGrid != null)
            {
                dataGrid.ItemsSource = viewModel.FilterData(DataGridDataSource.FilterOptions.Height_Low);
            }
        }

        private void HeightHigh_Click(object sender, RoutedEventArgs e)
        {
            if (dataGrid != null)
            {
                dataGrid.ItemsSource = viewModel.FilterData(DataGridDataSource.FilterOptions.Height_High);
            }
        }

        private void ClearFilter_Click(object sender, RoutedEventArgs e)
        {
            if (dataGrid != null)
            {
                dataGrid.ItemsSource = viewModel.FilterData(DataGridDataSource.FilterOptions.All);
            }
        }

        private void GroupByRange_Click(object sender, RoutedEventArgs e)
        {

        }

        private void GroupByParentMountain_Click(object sender, RoutedEventArgs e)
        {

        }
        private PrintHelper printHelper;

        private async Task SaveSoftwareBitmapToFile(SoftwareBitmap softwareBitmap, StorageFile outputFile)
        {
            using (IRandomAccessStream stream = await outputFile.OpenAsync(FileAccessMode.ReadWrite))
            {
                // Create an encoder with the desired format
                BitmapEncoder encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.JpegEncoderId, stream);

                // Set the software bitmap
                encoder.SetSoftwareBitmap(softwareBitmap);

                encoder.IsThumbnailGenerated = true;

                try
                {
                    await encoder.FlushAsync();
                }
                catch (Exception err)
                {
                    const int WINCODEC_ERR_UNSUPPORTEDOPERATION = unchecked((int)0x88982F81);
                    switch (err.HResult)
                    {
                        case WINCODEC_ERR_UNSUPPORTEDOPERATION:
                            // If the encoder does not support writing a thumbnail, then try again
                            // but disable thumbnail generation.
                            encoder.IsThumbnailGenerated = false;
                            break;
                        default:
                            throw;
                    }
                }

                if (encoder.IsThumbnailGenerated == false)
                {
                    await encoder.FlushAsync();
                }


            }
        }

        private async Task PrintHtmltoPDF(string htmlFile)
        {
            var url = htmlFile;
            var edgePath = @"C:\Program Files (x86)\Microsoft\Edge\Application\msedge.exe";
            var output = Path.Combine(@"c:\temp\printout.pdf");
            using (var p = new Process())
            {
                p.StartInfo.FileName = edgePath;
                p.StartInfo.Arguments = $"--headless --disable-gpu --print-to-pdf={output} {url}";
                p.Start();
                p.WaitForExit();
            }

            ContentDialog dialog = new ContentDialog
            {
                Title = "Print to PDF",
                Content = @"Print to c:\temp\printout.pdf",
                PrimaryButtonText = "OK"
            };
            dialog.XamlRoot = dataGrid.XamlRoot;
            await dialog.ShowAsync();
        }
        private async void printButton_Click(object sender, RoutedEventArgs e)
        {
          
            var renderBitmap = new RenderTargetBitmap();          
            await renderBitmap.RenderAsync(dataGrid);
            var buffer = await renderBitmap.GetPixelsAsync();
            using var originalSoftwareBitmap = SoftwareBitmap.CreateCopyFromBuffer(buffer, BitmapPixelFormat.Bgra8, renderBitmap.PixelWidth, renderBitmap.PixelHeight, BitmapAlphaMode.Premultiplied);
             
            StorageFolder sf = KnownFolders.PicturesLibrary;
            StorageFolder tn = await sf.CreateFolderAsync("DataGridPage", CreationCollisionOption.OpenIfExists);
            string outputFileName = $"{DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss_fff")}.jpg";

            StorageFile outputFile = await tn.CreateFileAsync(outputFileName);

            await SaveSoftwareBitmapToFile(originalSoftwareBitmap, outputFile);

            string HTMLString = $"<img src=\"{outputFileName}\" width=\"100%\" heigth=\"100%\"></img>";
            StorageFile printHtm = await tn.CreateFileAsync("print.htm", Windows.Storage.CreationCollisionOption.ReplaceExisting);
            await Windows.Storage.FileIO.WriteTextAsync(printHtm, HTMLString);

            await PrintHtmltoPDF(printHtm.Path);

        }


    }
}
