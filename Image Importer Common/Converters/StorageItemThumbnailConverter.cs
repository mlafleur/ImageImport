using System;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media.Imaging;

namespace Image_Importer.Common.Converters
{
    public class StorageItemThumbnailConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            BitmapImage imagen = new BitmapImage();
            try
            {
                imagen.SetSource((Windows.Storage.FileProperties.StorageItemThumbnail)value);
            }
            catch { }
            return imagen;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}