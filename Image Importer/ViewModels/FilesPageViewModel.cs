using Image_Importer.Common.Models;
using System;
using Windows.UI.ApplicationSettings;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

namespace Image_Importer.ViewModels
{
    internal class FilesPageViewModel : System.ComponentModel.INotifyPropertyChanged
    {
        private Common.Models.ImageDevice _device;

        public FilesPageViewModel()
        {
            if (Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
                Device = ImageDevice.GetDesignTime("Device 1", "1", "12 Files", new BitmapImage(new Uri("ms-appx://Assets/SplashScreen.scale-100.png")));
            }
        }

        private Common.ImageHandlers.Importer _importer = new Common.ImageHandlers.Importer();

        public Common.ImageHandlers.Importer Importer
        {
            get { return _importer; }
            private set { _importer = value; }
        }

        public Common.Models.ImageDevice Device
        {
            get { return _device; }
            set { _device = value; NotifyPropertyChanged(); }
        }

        #region Settings Page

        //public void OnCommandsRequested(SettingsPane settingsPane, SettingsPaneCommandsRequestedEventArgs eventArgs)
        //{
        //    SettingsCommand aboutCommand = new SettingsCommand("mainSettings", "Import Settings", (x) =>
        //    {
        //        SettingsFlyout flyout = new SettingsFlyout();
        //        flyout.Content = new SettingControls.ImportSettings();
        //        flyout.Title = "Import Settings";
        //        flyout.Background = new Windows.UI.Xaml.Media.SolidColorBrush(Windows.UI.Colors.DarkGray);
        //        flyout.Show();
        //    });
        //    eventArgs.Request.ApplicationCommands.Add(aboutCommand);
        //}

        #endregion Settings Page

        public void LoadState(Common.Models.ImageDevice currentDevice)
        {
            Device = currentDevice;
            Device.Refresh();
            //SettingsPane.GetForCurrentView().CommandsRequested += OnCommandsRequested;
        }

        public void SaveState()
        {
           // SettingsPane.GetForCurrentView().CommandsRequested -= OnCommandsRequested;
        }

        #region INotifyPropertyChanged

        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        // This method is called by the Set accessor of each property.
        // The CallerMemberName attribute that is applied to the optional propertyName
        // parameter causes the property name of the caller to be substituted as an argument.
        private void NotifyPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion INotifyPropertyChanged
    }
}