using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Devices.Portable;
using Windows.Storage;
using Windows.Storage.BulkAccess;
using Windows.UI.Xaml.Media.Imaging;

namespace Image_Importer.Common.Models
{
    public class ImageDevice : System.ComponentModel.INotifyPropertyChanged
    {
        private string _deviceId;
        private BitmapImage _image;
        private IReadOnlyList<FileInformation> _items;
        private string _subtitle;
        private string _title;

        private Windows.Storage.Search.StorageFileQueryResult query;

        private ImageDevice()
        {
            if (Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
            }
        }

        public string DeviceId
        {
            get { return _deviceId; }
            set { _deviceId = value; NotifyPropertyChanged(); }
        }

        public BitmapImage Image
        {
            get { return _image; }
            set { _image = value; NotifyPropertyChanged(); }
        }

        public IReadOnlyList<FileInformation> Items
        {
            get { return _items; }
            set { _items = value; NotifyPropertyChanged(); }
        }

        public string Subtitle
        {
            get { return _subtitle; }
            set { _subtitle = value; NotifyPropertyChanged(); }
        }

        public string Title
        {
            get { return _title; }
            set { _title = value; NotifyPropertyChanged(); }
        }

        public static ImageDevice FromDeviceId(string id)
        {
            var folder = StorageDevice.FromId(id);
            ImageDevice device = FromFolder(folder);
            device.DeviceId = id;
            return device;
        }

        public static ImageDevice FromFolder(StorageFolder folder)
        {
            ImageDevice device = new ImageDevice();

            Windows.Storage.Search.QueryOptions queryOptions = new Windows.Storage.Search.QueryOptions();
            queryOptions.FolderDepth = Windows.Storage.Search.FolderDepth.Deep;
            queryOptions.IndexerOption = Windows.Storage.Search.IndexerOption.DoNotUseIndexer;

            device.query = folder.CreateFileQueryWithOptions(queryOptions);

            device.Title = folder.DisplayName;
            return device;
        }

        public static ImageDevice GetDesignTime(string title, string id, string subtitle, BitmapImage image)
        {
            ImageDevice device = new ImageDevice();
            device.Title = title;
            device.DeviceId = id;
            device.Subtitle = subtitle;
            device.Image = image;
            return device;
        }

        public async void Refresh()
        {
            FileInformationFactory fileFactory = new FileInformationFactory(query, Windows.Storage.FileProperties.ThumbnailMode.PicturesView);
            Items = await fileFactory.GetFilesAsync(0, 2000);
            Subtitle = Items.Count.ToString() + " Images Found";
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