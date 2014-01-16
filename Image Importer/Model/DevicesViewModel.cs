using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Devices.Portable;
using Windows.Storage;
using Windows.Storage.BulkAccess;
using Windows.UI.Xaml.Media.Imaging;

namespace Image_Importer.Model
{
    public class DevicesViewModel
    {
        private List<ImageDevice> _items = new List<ImageDevice>();

        public List<ImageDevice> Items
        {
            get { return _items; }
            set { _items = value; }
        }

        private bool _isRefreshing;

        public bool IsRefreshing
        {
            get { return _isRefreshing; }
            set { _isRefreshing = value; }
        }

        public async Task Refresh()
        {
            IsRefreshing = true;
            Model.AppSettings appSettings = new Model.AppSettings();

            // Get a collection of storage devices
            DeviceInformationCollection dic = await DeviceInformation.FindAllAsync(StorageDevice.GetDeviceSelector());
            // The above query returens a lot of stuff that isn't valid.
            // A SD card being used for ReadyBoost for example. We'll weed down the list a bit more
            // by cross-referensing it with a list of the MTP devices that are valid storeage media
            DeviceInformationCollection mtpDevices = await DeviceInformation.FindAllAsync(ServiceDevice.GetDeviceSelectorFromServiceId(new Guid("6BDD1FC6-810F-11D0-BEC7-08002BE2092F")));

            // Get a hash of the MTP devices to quickly validate in the next section
            var mtpHash = new HashSet<String>(from device in mtpDevices select device.Name as string);

            Items.Clear();
            foreach (var item in dic)
            {
                // Cross-reference this device name with the mtpHash to make sure it is worth listing
                // We also support an option to skip this step via settings (just in case).
                if (!mtpHash.Contains(item.Name as string) && !appSettings.ShowAllDevices) continue;

                // Grab the storagefolder from the device id
                StorageFolder folder = StorageDevice.FromId(item.Id);

                // With some media readers (card readers), drives show up even without cards inserted.
                // To make sure we only show a valid list of targets, we'll use GetFodlersAsync().
                // If there it is a real location it will have at least 1 folder (the root folder).
                // We also support an option to skip this step via settings (just in case).
                var folders = await folder.GetFoldersAsync();
                if (folders.Count == 0 && !appSettings.ShowAllDevices) continue; // Tells us this is real

                // Now we grab the glyph from the device. This helps the user figure out
                // which device they want to import from (in case there is multiple such as camera and phone)
                BitmapImage bitmapImage = new BitmapImage();
                var thumb = await item.GetGlyphThumbnailAsync();
                bitmapImage.SetSource(thumb);

                var imageDevice = new ImageDevice() { Title = folder.DisplayName, Image = bitmapImage, DeviceId = item.Id, Folder = folder };
                await imageDevice.Refresh();
                imageDevice.Subtitle = imageDevice.Items.Count.ToString() + " Images Found";

                // Add everything to our ViewModel to work from going forward.
                Items.Add(imageDevice);
            }
            IsRefreshing = false;
        }

        public class ImageDevice
        {
            public string DeviceId { get; set; }

            public StorageFolder Folder { get; set; }

            public BitmapImage Image { get; set; }

            public IReadOnlyList<FileInformation> Items { get; set; }

            public string Subtitle { get; set; }

            public string Title { get; set; }

            public async Task LoadFromFolder(StorageFolder folder)
            {
                try
                {
                    Windows.Storage.Search.QueryOptions queryOptions = new Windows.Storage.Search.QueryOptions();
                    queryOptions.FolderDepth = Windows.Storage.Search.FolderDepth.Deep;
                    var query = folder.CreateFileQueryWithOptions(queryOptions);
                    var files = await folder.GetFilesAsync();
                    FileInformationFactory fileFactory = new FileInformationFactory(query, Windows.Storage.FileProperties.ThumbnailMode.PicturesView, 900);

                    Items = await fileFactory.GetFilesAsync(0, 500);
                }
                catch
                {
#if DEBUG
                    System.Diagnostics.Debugger.Launch();
#endif
                }
            }

            public async Task Refresh()
            {
                try
                {
                    if (Folder != null)
                        await LoadFromFolder(Folder);
                    else

                        await LoadFromFolder(StorageDevice.FromId(DeviceId));
                }
                catch
                {
#if DEBUG
                    System.Diagnostics.Debugger.Launch();
#endif
                }
                return;
            }
        }
    }

    public class DevicesViewModel_DesignTime : DevicesViewModel
    {
        public DevicesViewModel_DesignTime()
        {
            for (int i = 0; i < 10; i++)
            {
                this.Items.Add(new ImageDevice() { Title = "Device " + i.ToString(), DeviceId = i.ToString(), Subtitle = i.ToString() + " Files", Image = new BitmapImage(new Uri("ms-appx://Assets/SplashScreen.scale-100.png")) });
            }
        }
    }
}