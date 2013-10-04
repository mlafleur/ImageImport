using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Image_Importer.Model
{
    public class ImportOptions : System.ComponentModel.INotifyPropertyChanged
    {

        Windows.Storage.ApplicationDataContainer appSettings;

        private object GetSetting(string key, object defaultValue)
        {
            if (appSettings.Values[key] == null) appSettings.Values[key] = defaultValue;
            return appSettings.Values[key];
        }

        private void SetSetting(string key, object value)
        {
            appSettings.Values[key] = value;

        }

        public class CustomKeyValuePair<TKey, TValue>
        {
            public TKey Key { get; set; }
            public TValue Value { get; set; }
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

        public string OrganizationRule
        {
            get { return (string)GetSetting("OrganizationRule", "%DateTaken%"); }
            set { SetSetting("OrganizationRule", value); NotifyPropertyChanged(); }
        }

        public bool RemoveSourceFile
        {
            get { return (bool)GetSetting("RemoveSourceFile", "true"); }
            set { SetSetting("RemoveSourceFile", value); NotifyPropertyChanged(); }
        }


        public ImportOptions()
        {
            appSettings = Windows.Storage.ApplicationData.Current.RoamingSettings;           
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
