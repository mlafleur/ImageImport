using Image_Importer.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Items Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234233

namespace Image_Importer.Pages
{
    /// <summary>
    /// A page that displays a collection of item previews.  In the Split Application this page
    /// is used to display and select one of the available groups.
    /// </summary>
    public sealed partial class FilesPage : Page
    {
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();
        private GridView photoGridInstance; 

        /// <summary>
        /// This can be changed to a strongly typed view model.
        /// </summary>
        public ObservableDictionary DefaultViewModel
        {
            get { return this.defaultViewModel; }
        }

        /// <summary>
        /// NavigationHelper is used on each page to aid in navigation and 
        /// process lifetime management
        /// </summary>
        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }

        public FilesPage()
        {
            this.InitializeComponent();
            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += navigationHelper_LoadState;
        }

        private Model.DevicesViewModel.ImageDevice viewModel;
        private Model.ImportOptions importOptions = new Model.ImportOptions();

        /// <summary>
        /// Populates the page with content passed during navigation.  Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="sender">
        /// The source of the event; typically <see cref="NavigationHelper"/>
        /// </param>
        /// <param name="e">Event data that provides both the navigation parameter passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested and
        /// a dictionary of state preserved by this page during an earlier
        /// session.  The state will be null the first time a page is visited.</param>
        private void navigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            viewModel = (Model.DevicesViewModel.ImageDevice)e.NavigationParameter;
            importOptions.RemoveSourceFile = false;
            HubSection1.DataContext = importOptions;
            this.DefaultViewModel["Items"] = viewModel.Items;
            this.pageTitle.Text = viewModel.Title;
        }

        #region NavigationHelper registration

        /// The methods provided in this section are simply used to allow
        /// NavigationHelper to respond to the page's navigation methods.
        /// 
        /// Page specific logic should be placed in event handlers for the  
        /// <see cref="GridCS.Common.NavigationHelper.LoadState"/>
        /// and <see cref="GridCS.Common.NavigationHelper.SaveState"/>.
        /// The navigation parameter is available in the LoadState method 
        /// in addition to page state preserved during an earlier session.

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            navigationHelper.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            navigationHelper.OnNavigatedFrom(e);
        }

        #endregion

        private void ImportAll_Click(object sender, RoutedEventArgs e)
        {
            photoGridInstance.SelectAll();
            ExecuteImport();
        }

        private void ImportSelected_Click(object sender, RoutedEventArgs e)
        {
            ExecuteImport();
        }

        private async void ExecuteImport()
        {
            ImportProgressPanel.Visibility = Windows.UI.Xaml.Visibility.Visible;

            ImportProgressBar.Maximum = photoGridInstance.SelectedItems.Count;
            ImportProgressBar.Value = 0;

            Windows.Storage.StorageFolder destinationFolder = null;


            if (importOptions.OrganizationRule == "%DateImported%")
                destinationFolder =
                        await Windows.Storage.KnownFolders.PicturesLibrary.CreateFolderAsync(DateTime.Now.ToString("yyyy'-'MM'-'dd"),
                                                                                           Windows.Storage.CreationCollisionOption.GenerateUniqueName);

            foreach (var item in photoGridInstance.SelectedItems)
            {
                var fileInfo = item as Windows.Storage.BulkAccess.FileInformation;

                if (importOptions.OrganizationRule == "%DateTaken%")
                {
                    /* In some case I've found images that don't have complete DateTake data. When this happens we go with date created */
                    string folderName = string.Empty;

                    try
                    {
                        folderName = fileInfo.ImageProperties.DateTaken.ToString("yyyy'-'MM'-'dd");
                    }
                    catch
                    {
                        folderName = fileInfo.DateCreated.ToString("yyyy'-'MM'-'dd");
                    }

                    destinationFolder =
                                await Windows.Storage.KnownFolders.PicturesLibrary.CreateFolderAsync(folderName, Windows.Storage.CreationCollisionOption.OpenIfExists);
                }

                if (importOptions.RemoveSourceFile)
                    await fileInfo.MoveAsync(destinationFolder, fileInfo.Name, Windows.Storage.NameCollisionOption.GenerateUniqueName);
                else
                    await fileInfo.CopyAsync(destinationFolder, fileInfo.Name, Windows.Storage.NameCollisionOption.GenerateUniqueName);

                ImportProgressBar.Value = ImportProgressBar.Value + 1;
            }

            photoGridInstance.SelectedIndex = -1;
            await viewModel.Refresh();
            ImportProgressPanel.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void photoGrid_Loaded(object sender, RoutedEventArgs e)
        {
            this.photoGridInstance = (GridView)sender;
        }
    }
}
