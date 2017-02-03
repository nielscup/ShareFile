using Plugin.ShareFile.Abstractions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;
using Windows.Storage.Streams;

namespace Plugin.ShareFile
{
    public class ShareFileImplementation : IShareFile
    {
        private string _chosenFile;
        private string _title;
        private IStorageFile _remoteFile;

        public void ShareLocalFile(string localFilePath, string title = "Shared File")
        {
            try
            {
                if (string.IsNullOrWhiteSpace(localFilePath))
                {
                    Debug.WriteLine("Plugin.ShareFile: ShareLocalFile Warning: localFilePath null or empty");
                    return;
                }

                _chosenFile = localFilePath;
                _title = title;

                DataTransferManager.GetForCurrentView().DataRequested += MainPage_DataRequested;
                DataTransferManager.ShowShareUI();
            }
            catch (Exception ex)
            {
                if (ex != null && !string.IsNullOrWhiteSpace(ex.Message))
                    Debug.WriteLine("Plugin.ShareFile: ShareLocalFile Exception: {0}", ex);
            }
        }

        public async Task ShareRemoteFile(string fileUri, string fileName, string title = "Shared File")
        {
            try
            {
                if (string.IsNullOrWhiteSpace(fileUri))
                {
                    Debug.WriteLine("Plugin.ShareFile: ShareLocalFile Warning: localFilePath null or empty");
                    return;
                }

                // download remote file
                var uri = new Uri(fileUri);
                var fileNameToUse = fileName ?? Path.GetFileName(fileUri);
                var thumbnail = RandomAccessStreamReference.CreateFromUri(uri);
                var remoteFIle = await StorageFile.CreateStreamedFileFromUriAsync(fileNameToUse, uri, thumbnail);
                _remoteFile = await remoteFIle.CopyAsync(ApplicationData.Current.TemporaryFolder, fileNameToUse, NameCollisionOption.ReplaceExisting);

                _chosenFile = _remoteFile.Path;
                _title = title;

                DataTransferManager.GetForCurrentView().DataRequested += MainPage_RemoteDataRequested;
                DataTransferManager.ShowShareUI();
            }
            catch (Exception ex)
            {
                if (ex != null && !string.IsNullOrWhiteSpace(ex.Message))
                    Debug.WriteLine("Plugin.ShareFile: ShareLocalFile Exception: {0}", ex);
            }
        }

        private void MainPage_RemoteDataRequested(DataTransferManager sender, DataRequestedEventArgs args)
        {
            List<IStorageFile> files = new List<IStorageFile>();
            files.Add(_remoteFile);

            args.Request.Data.Properties.Title = "Shared File for Exact";
            args.Request.Data.SetStorageItems(files);
        }

        private async void MainPage_DataRequested(DataTransferManager sender, DataRequestedEventArgs args)
        {
            List<IStorageFile> files = new List<IStorageFile>();
            files.Add(await StorageFile.GetFileFromPathAsync(_chosenFile));

            args.Request.Data.Properties.Title = "Shared File for Exact";
            args.Request.Data.SetStorageItems(files);
        }
    }
}
