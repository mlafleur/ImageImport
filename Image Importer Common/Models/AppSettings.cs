using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Windows.Storage.AccessCache;

namespace Image_Importer.Common.Models
{
    public class AppSettings : INotifyPropertyChanged
    {
        private Windows.Storage.ApplicationDataContainer localSettings;
        private Windows.Storage.ApplicationDataContainer roamingSettings;

        public AppSettings()
        {
            roamingSettings = Windows.Storage.ApplicationData.Current.RoamingSettings;
            localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            var ignore = GetDestinationFolder(); // Does a prefetch for settings display latter
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public bool MoveSourceFiles
        {
            get { return Convert.ToBoolean(GetSetting("MoveSourceFiles", true)); }
            set
            {
                roamingSettings.Values["MoveSourceFiles"] = value;
                NotifyPropertyChanged();
            }
        }

        public string OrgRule
        {
            get { return (string)GetSetting("OrgRule", "%DateTaken%"); }
            set
            {
                roamingSettings.Values["OrgRule"] = value;
                NotifyPropertyChanged();
            }
        }

        public List<CustomKeyValuePair<string, string>> OrgRuleOptions
        {
            get
            {
                Dictionary<string, string> ops = new Dictionary<string, string>();
                ops.Add("%DateTaken%", "Date each photo was taken");
                ops.Add("%DateImported%", "Date photos were imported");
                return ops.Select(kvp => new CustomKeyValuePair<string, string> { Key = kvp.Key, Value = kvp.Value }).ToList();
            }
        }

        public bool ShowAllDevices
        {
            get { return Convert.ToBoolean(GetSetting("ShowAllDeices", false)); }
            set
            {
                roamingSettings.Values["ShowAllDeices"] = value;
                NotifyPropertyChanged();
            }
        }

        private string folderPath;

        public string FolderPath
        {
            get { return folderPath; }
            set { folderPath = value; }
        }

        public string ImportExpresion
        {
            get { return (string)GetSetting("ImportExpresion", @"[YEAR]\[DATE]"); }
            set
            {
                roamingSettings.Values["ImportExpresion"] = value;
                NotifyPropertyChanged();
            }
        }

        public List<CustomKeyValuePair<string, string>> ImportExpresionList
        {
            get
            {
                string date = DateTime.Now.AddDays(-35).ToString("yyyy'-'MM'-'dd");
                string import = DateTime.Now.ToString("yyyy'-'MM'-'dd");
                string year = DateTime.Now.Year.ToString();

                Dictionary<string, string> list = new Dictionary<string, string>();

                list.Add(@"[YEAR]\[DATE]", string.Format(@"[YEAR]\[DATE] ({0}\{1}\picture.jpg)", year, date));
                list.Add(@"[YEAR]\[DATE]\[EXT]", string.Format(@"[YEAR]\[DATE]\[EXT] ({0}\{1}\JPG\picture.jpg)", year, date));
                list.Add(@"[DATE]", string.Format(@"[DATE] ({0}\picture.jpg)", date));
                list.Add(@"[IMPORT_YEAR]\[IMPORT_DATE]", string.Format(@"[IMPORT_YEAR]\[IMPORT_DATE] ({0}\{1}\picture.jpg)", year, import));
                list.Add(@"[IMPORT_YEAR]\[IMPORT_DATE]\[EXT]", string.Format(@"[IMPORT_YEAR]\[IMPORT_DATE]\[EXT] ({0}\{1}\JPG\picture.jpg)", year, import));
                list.Add(@"[IMPORT_DATE]", string.Format(@"[IMPORT_DATE] ({0}\picture.jpg)", import));
                list.Add(@"[NONE]", string.Format(@"[NONE] ({0}\picture.jpg)", FolderPath));

                return list.Select(kvp => new CustomKeyValuePair<string, string> { Key = kvp.Key, Value = kvp.Value }).ToList();
            }
        }

        public async Task<Windows.Storage.StorageFolder> GetDestinationFolder()
        {
            Windows.Storage.StorageFolder folder = Windows.Storage.KnownFolders.PicturesLibrary;

            // If we don't have a token somewhere, use the default
            try
            {
                folder = await StorageApplicationPermissions.FutureAccessList.GetFolderAsync("DestinationFolder");
            }
            catch
            {
                // It wasn't valid for some reason, use the default
                folder = Windows.Storage.KnownFolders.PicturesLibrary;
            }

            if (string.IsNullOrEmpty(folder.Path)) FolderPath = folder.DisplayName;
            else FolderPath = folder.Path;

            return folder;
        }

        private object GetSetting(string key, object defaultValue)
        {
            if (roamingSettings.Values[key] == null)
            {
                if (defaultValue == null) return null; // don't store it if it is null
                roamingSettings.Values[key] = defaultValue;
            }
            return roamingSettings.Values[key];
        }

        private object GetSettingL(string key, object defaultValue)
        {
            if (localSettings.Values[key] == null)
            {
                if (defaultValue == null) return null; // don't store it if it is null
                localSettings.Values[key] = defaultValue;
            }
            return localSettings.Values[key];
        }

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public class CustomKeyValuePair<TKey, TValue>
        {
            public TKey Key { get; set; }

            public TValue Value { get; set; }
        }
    }
}