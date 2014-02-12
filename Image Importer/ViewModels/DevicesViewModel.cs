using Image_Importer.Common.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Devices.Portable;
using Windows.Storage;
using Windows.Storage.BulkAccess;
using Windows.UI.ApplicationSettings;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

namespace Image_Importer.ViewModels
{
    public class DevicesViewModel : System.ComponentModel.INotifyPropertyChanged
    {
        public DevicesViewModel()
        {

        }

        private Devices _activeDevices = Common.Models.Devices.Current;

        public Devices ActiveDevices
        {
            get { return _activeDevices; }
            set { _activeDevices = value; }
        }

        public void LoadState()
        {
            SettingsPane.GetForCurrentView().CommandsRequested += OnCommandsRequested;
        }


        public void SaveState()
        {
            SettingsPane.GetForCurrentView().CommandsRequested -= OnCommandsRequested;
        }

        #region Settings Page

        public void OnCommandsRequested(SettingsPane settingsPane, SettingsPaneCommandsRequestedEventArgs eventArgs)
        {
            SettingsCommand aboutCommand = new SettingsCommand("deviceSettings", "Device Settings", (x) =>
            {
                SettingsFlyout flyout = new SettingsFlyout();
                flyout.Content = new SettingControls.DeviceSettings();
                flyout.Title = "Device Settings";
                flyout.Background = new Windows.UI.Xaml.Media.SolidColorBrush(Windows.UI.Colors.DarkGray);
                flyout.Show();
            });
            eventArgs.Request.ApplicationCommands.Add(aboutCommand);
        }

        #endregion Settings Page

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