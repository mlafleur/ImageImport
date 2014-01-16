using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Image_Importer.Model
{
    public class AppSettings : INotifyPropertyChanged
    {
        private Windows.Storage.ApplicationDataContainer ad;

        public AppSettings()
        {
            ad = Windows.Storage.ApplicationData.Current.RoamingSettings;
        }

        private object GetSetting(string key, object defaultValue)
        {
            if (ad.Values[key] == null)
            {
                if (defaultValue == null) return null; // don't store it if it is null
                ad.Values[key] = defaultValue;
            }
            return ad.Values[key];
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public string OrgRule
        {
            get { return (string)GetSetting("OrgRule", "%DateTaken%"); }
            set
            {
                ad.Values["OrgRule"] = value;
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

        public bool MoveSourceFiles
        {
            get { return Convert.ToBoolean(GetSetting("MoveSourceFiles", true)); }
            set
            {
                ad.Values["MoveSourceFiles"] = value;
                NotifyPropertyChanged();
            }
        }

        public bool ShowAllDevices
        {
            get { return Convert.ToBoolean(GetSetting("ShowAllDeices", false)); }
            set
            {
                ad.Values["ShowAllDeices"] = value;
                NotifyPropertyChanged();
            }
        }

        public class CustomKeyValuePair<TKey, TValue>
        {
            public TKey Key { get; set; }

            public TValue Value { get; set; }
        }
    }
}