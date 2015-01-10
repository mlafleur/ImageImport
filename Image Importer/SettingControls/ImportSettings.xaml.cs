using System;
using Windows.Storage.AccessCache;
using Windows.Storage.Pickers;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace Image_Importer.SettingControls
{
    public sealed partial class ImportSettings : UserControl
    {
        public ImportSettings()
        {
            this.InitializeComponent();
        }

        private async void Button_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            var viewModel = this.DataContext as Common.Models.AppSettings;

            var picker = new FolderPicker();
            picker.FileTypeFilter.Add("*");
            picker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;

            var folder = await picker.PickSingleFolderAsync();
            StorageApplicationPermissions.FutureAccessList.AddOrReplace("DestinationFolder", folder);
            await viewModel.GetDestinationFolder();
        }
    }
}