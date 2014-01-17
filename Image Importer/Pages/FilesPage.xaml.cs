using Image_Importer.Common;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
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
        private ObservableDictionary defaultViewModel = new ObservableDictionary();
        private NavigationHelper navigationHelper;
        private GridView photoGridInstance;

        private Model.DevicesViewModel.ImageDevice viewModel;

        public FilesPage()
        {
            this.InitializeComponent();
            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += navigationHelper_LoadState;
        }

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

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }

        private async void ExecuteImport()
        {
            ImportProgressPanel.Visibility = Windows.UI.Xaml.Visibility.Visible;

            ImportProgressBar.Maximum = photoGridInstance.SelectedItems.Count;
            ImportProgressBar.Value = 0;

            Windows.Storage.StorageFolder destinationFolder = null;

            Model.AppSettings appSettings = new Model.AppSettings();

            if (appSettings.OrgRule == "%DateImported%")
                destinationFolder =
                        await Windows.Storage.KnownFolders.PicturesLibrary.CreateFolderAsync(DateTime.Now.ToString("yyyy'-'MM'-'dd"),
                                                                                           Windows.Storage.CreationCollisionOption.GenerateUniqueName);

            foreach (var item in photoGridInstance.SelectedItems)
            {
                var fileInfo = item as Windows.Storage.BulkAccess.FileInformation;
                var file = Windows.Storage.StorageFile.GetFileFromPathAsync(fileInfo.Path);

                var props = await fileInfo.Properties.GetImagePropertiesAsync();
                if (appSettings.OrgRule == "%DateTaken%")
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

                try
                {
                    if (appSettings.MoveSourceFiles)
                        await fileInfo.MoveAsync(destinationFolder, fileInfo.Name, Windows.Storage.NameCollisionOption.GenerateUniqueName);
                    else
                        await fileInfo.CopyAsync(destinationFolder, fileInfo.Name, Windows.Storage.NameCollisionOption.GenerateUniqueName);

                    ImportProgressBar.Value = ImportProgressBar.Value + 1;
                }
                catch { continue; }
            }

            photoGridInstance.SelectedIndex = -1;
            await viewModel.Refresh();
            this.DefaultViewModel["Items"] = viewModel.Items;
            ImportProgressPanel.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
        }

        private void ImportAll_Click(object sender, RoutedEventArgs e)
        {
            if (photoGridInstance.Items.Count == 0) return;
            if (photoGridInstance.SelectedItems.Count == 0) photoGridInstance.SelectAll();
            ExecuteImport();
        }

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

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            navigationHelper.OnNavigatedFrom(e);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            navigationHelper.OnNavigatedTo(e);
        }

        #endregion NavigationHelper registration

        private void photoGrid_Loaded(object sender, RoutedEventArgs e)
        {
            this.photoGridInstance = (GridView)sender;
        }

        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            SettingsFlyout flyout = new SettingsFlyout();
            flyout.Content = new SettingsMain();
            flyout.Title = "Import Settings";
            flyout.Background = new SolidColorBrush(Windows.UI.Colors.DarkGray);
            flyout.ShowIndependent();
        }
    }
}