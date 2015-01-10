using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Image_Importer.Common.ImageHandlers
{
    public class Importer : System.ComponentModel.INotifyPropertyChanged
    {
        private int _importingCount = 0;
        private int _importingIndex = 0;
        private bool _isImporting = false;

        private string _statusMessage = "Not Importing";

        public int ImportingCount
        {
            get { return _importingCount; }
            set { _importingCount = value; NotifyPropertyChanged(); }
        }

        public int ImportingIndex
        {
            get { return _importingIndex; }
            set { _importingIndex = value; NotifyPropertyChanged(); }
        }

        public bool IsImporting
        {
            get { return _isImporting; }
            set { _isImporting = value; NotifyPropertyChanged(); }
        }

        public string StatusMessage
        {
            get { return _statusMessage; }
            set { _statusMessage = value; NotifyPropertyChanged(); }
        }

        public async Task Execute(IList<object> importItems)
        {
            StatusMessage = "";
            IsImporting = true;
            ImportingCount = importItems.Count;
            ImportingIndex = 0;

            Common.Models.AppSettings appSettings = new Common.Models.AppSettings();
            Windows.Storage.StorageFolder destinationFolder = await appSettings.GetDestinationFolder();

            foreach (var item in importItems)
            {
                var fileInfo = item as Windows.Storage.BulkAccess.FileInformation;
                var file = Windows.Storage.StorageFile.GetFileFromPathAsync(fileInfo.Path);
                var props = await fileInfo.Properties.GetImagePropertiesAsync();

                // Path
                var importPath = appSettings.ImportExpresion;

                try
                {
                    importPath = importPath.Replace("[YEAR]", fileInfo.ImageProperties.DateTaken.Year.ToString());
                }
                catch
                {
                    importPath = importPath.Replace("[YEAR]", fileInfo.DateCreated.Year.ToString());
                }

                try
                {
                    importPath = importPath.Replace("[DATE]", fileInfo.ImageProperties.DateTaken.ToString("yyyy'-'MM'-'dd"));
                }
                catch
                {
                    importPath = importPath.Replace("[DATE]", fileInfo.DateCreated.ToString("yyyy'-'MM'-'dd"));
                }

                importPath = importPath.Replace("[EXT]", fileInfo.FileType.Substring(1));
                importPath = importPath.Replace("[IMPORT_YEAR]", DateTime.Now.Year.ToString());
                importPath = importPath.Replace("[IMPORT_DATE]", DateTime.Now.ToString("yyyy'-'MM'-'dd"));

                Windows.Storage.StorageFolder importFolder;
                if (importPath == "[NONE]")
                    importFolder = destinationFolder;
                else
                    importFolder = await destinationFolder.CreateFolderAsync(importPath, Windows.Storage.CreationCollisionOption.OpenIfExists);

                try
                {
                    if (appSettings.MoveSourceFiles)
                    {
                        StatusMessage = string.Format("Moving {0} to {1} ({2} or {3})", importFolder, fileInfo.Name, ImportingIndex, ImportingCount);
                        await fileInfo.MoveAsync(importFolder, fileInfo.Name, Windows.Storage.NameCollisionOption.GenerateUniqueName);
                    }
                    else
                    {
                        StatusMessage = string.Format("Copying {0} to {1} ({2} or {3})", importFolder, fileInfo.Name, ImportingIndex, ImportingCount);
                        await fileInfo.CopyAsync(importFolder, fileInfo.Name, Windows.Storage.NameCollisionOption.GenerateUniqueName);
                    }

                    ImportingIndex++;
                }
                catch (Exception ex)
                {
                    StatusMessage = string.Format("Error {0} : {1}", ex.HResult, ex.Message);
                    continue;
                }
            }

            ImportingCount = 0;
            ImportingIndex = 0;
            IsImporting = false;

            //Device.Refresh();
            //photoGridInstance.SelectedIndex = -1;
            //await viewModel.Refresh();
            //this.DefaultViewModel["Items"] = viewModel.Items;
            //ImportProgressPanel.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
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