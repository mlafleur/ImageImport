using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Devices.Portable;
using Windows.UI.Xaml.Media.Imaging;

namespace Image_Importer.Common.Models
{
    public class Devices : System.ComponentModel.INotifyPropertyChanged
    {
        private static Devices _current;
        private bool _isRefreshing = false;
        private ObservableCollection<ImageDevice> _items = new ObservableCollection<ImageDevice>();

        private Devices()
        {
            if (Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
                for (int i = 0; i < 10; i++)
                {
                    this.Items.Add(ImageDevice.GetDesignTime("Device " + i.ToString(), i.ToString(), i.ToString() + " Files", new BitmapImage(new Uri("ms-appx://Assets/SplashScreen.scale-100.png"))));
                }
            }
        }

        public static Devices Current
        {
            get
            {
                if (_current == null) _current = new Devices();

                return _current;
            }
        }

        public bool IsRefreshing
        {
            get { return _isRefreshing; }
            set
            {
                _isRefreshing = value;
                NotifyPropertyChanged();
            }
        }

        public ObservableCollection<ImageDevice> Items
        {
            get { return _items; }
            set { _items = value; }
        }

        public async Task Refresh()
        {
            try
            {
                IsRefreshing = true;

                Common.Models.AppSettings appSettings = new Common.Models.AppSettings();

                // Get a collection of storage devices
                DeviceInformationCollection dic = await DeviceInformation.FindAllAsync(StorageDevice.GetDeviceSelector());

                // The above query returens a lot of stuff that isn't valid.
                // A SD card being used for ReadyBoost for example. We'll weed down the list a bit more
                // by cross-referensing it with a list of the MTP devices that are valid storeage media
                DeviceInformationCollection mtpDevices = await DeviceInformation.FindAllAsync(ServiceDevice.GetDeviceSelectorFromServiceId(new Guid("6BDD1FC6-810F-11D0-BEC7-08002BE2092F")));

                // Get a hash of the MTP devices to quickly validate in the next section
                var mtpHash = new HashSet<String>(from device in mtpDevices select device.Name as string);

                foreach (var item in dic)
                {
                    if (Items.Any(c => c.DeviceId == item.Id)) continue; // We've already added this

                    // Cross-reference this device name with the mtpHash to make sure it is worth listing
                    // We also support an option to skip this step via settings (just in case).
                    if (!mtpHash.Contains(item.Name as string) && !appSettings.ShowAllDevices) continue;

                    var device = ImageDevice.FromDeviceId(item.Id);

                    // Now we grab the glyph from the device. This helps the user figure out
                    // which device they want to import from (in case there is multiple such as camera and phone)
                    BitmapImage bitmapImage = new BitmapImage();
                    var thumb = await item.GetGlyphThumbnailAsync();
                    bitmapImage.SetSource(thumb);
                    device.Image = bitmapImage;

                    // Add everything to our ViewModel to work from going forward.
                    Items.Add(device);
                }

                foreach (var item in Items)
                {
                    item.Refresh();
                }
            }
            finally
            {
                IsRefreshing = false;
            }
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