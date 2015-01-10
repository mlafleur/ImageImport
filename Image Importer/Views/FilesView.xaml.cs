using Image_Importer.Common;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Items Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234233

namespace Image_Importer.Views
{
    /// <summary>
    /// A page that displays a collection of item previews.  In the Split Application this page
    /// is used to display and select one of the available groups.
    /// </summary>
    public sealed partial class FilesView : Page
    {
        private ObservableDictionary defaultViewModel = new ObservableDictionary();
        private NavigationHelper navigationHelper;
        private GridView photoGridInstance;

        private ViewModels.FilesPageViewModel viewModel;

        //private ViewModels.DevicesViewModel.ImageDevice viewModel;

        public FilesView()
        {
            this.InitializeComponent();
            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += navigationHelper_LoadState;
            this.navigationHelper.SaveState += navigationHelper_SaveState;
        }

        private void navigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
            viewModel = this.DataContext as ViewModels.FilesPageViewModel;
            viewModel.SaveState();
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

        private async void ImportAll_Click(object sender, RoutedEventArgs e)
        {
            if (photoGridInstance.Items.Count == 0) return;
            if (photoGridInstance.SelectedItems.Count == 0) photoGridInstance.SelectAll();
            await viewModel.Importer.Execute(photoGridInstance.SelectedItems);
            viewModel.Device.Refresh();
            //await viewModel.ExecuteImport(photoGridInstance.SelectedItems);
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
            viewModel = this.DataContext as ViewModels.FilesPageViewModel;
            viewModel.LoadState((Common.Models.ImageDevice)e.NavigationParameter);
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
            flyout.Content = new SettingControls.ImportSettings();
            flyout.Title = "Import Settings";
            flyout.Background = new SolidColorBrush(Windows.UI.Colors.DarkGray);
            flyout.ShowIndependent();
        }
    }
}