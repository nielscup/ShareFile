using Android.App;
using Android.Content;
using Android.Database;
using Android.Graphics;
using System;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using Android.Support.V4.App;
using Plugin.ShareFile.Abstractions;
using Android.Support.V4.Content;
using Plugin.CurrentActivity;

[assembly: Permission(Name = "android.permission.READ_EXTERNAL_STORAGE")]
[assembly: Permission(Name = "android.permission.WRITE_EXTERNAL_STORAGE")]
namespace Plugin.ShareFile
{
    /// <summary>
    /// Implementation for Feature
    /// </summary> 
    public class ShareFileImplementation : IShareFile
    {
        /// <summary>
        /// Simply share a local file on compatible services
        /// </summary>
        /// <param name="localFilePath">path to local file</param>
        /// <param name="title">Title of popup on share (not included in message)</param>
        /// <returns>awaitable Task</returns>
        public void ShareLocalFile(string localFilePath, string title = "", object view = null)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(localFilePath))
                {
                    Console.WriteLine("Plugin.ShareFile: ShareLocalFile Warning: localFilePath null or empty");
                    return;
                }

                Android.Net.Uri fileUri = FileProvider.GetUriForFile(Application.Context, $"{Application.Context.PackageName}.fileprovider", new Java.IO.File(localFilePath));

                var builder =
                    ShareCompat.IntentBuilder.From(CrossCurrentActivity.Current.Activity).SetType(CrossCurrentActivity.Current.Activity.ContentResolver.GetType(fileUri)).SetText(title).AddStream(fileUri);
                var chooserIntent = builder.CreateChooserIntent();
                chooserIntent.SetFlags(ActivityFlags.ClearTop);
                chooserIntent.SetFlags(ActivityFlags.NewTask);
                chooserIntent.AddFlags(ActivityFlags.GrantReadUriPermission);
                CrossCurrentActivity.Current.Activity.StartActivity(chooserIntent);
            }
            catch (Exception ex)
            {
                if (!string.IsNullOrWhiteSpace(ex.Message))
                    Console.WriteLine("Exception in Plugin.ShareFile: ShareLocalFile Exception: {0}", ex);
            }
        }
        /// <summary>
        /// Simply share a file from a remote resource on compatible services
        /// </summary>
        /// <param name="fileUri">uri to external file</param>
        /// <param name="fileName">name of the file</param>
        /// <param name="title">Title of popup on share (not included in message)</param>
        /// <returns>awaitable bool</returns>
        public async Task ShareRemoteFile(string fileUri, string fileName, string title = "", object view = null)
        {
            try
            {
                using (var webClient = new WebClient())
                {
                    var uri = new System.Uri(fileUri);
                    var bytes = await webClient.DownloadDataTaskAsync(uri);
                    var filePath = WriteFile(fileName, bytes);
                    ShareLocalFile(filePath, title);
                    //return true;
                }
            }
            catch (Exception ex)
            {
                if (!string.IsNullOrWhiteSpace(ex.Message))
                    Console.WriteLine("Exception in Plugin.ShareFile: ShareRemoteFile Exception: {0}", ex.Message);
            }
        }

        /// <summary>
        /// Writes the file to local storage.
        /// </summary>
        /// <returns>The file.</returns>
        /// <param name="fileName">File name.</param>
        /// <param name="bytes">Bytes.</param>
        private string WriteFile(string fileName, byte[] bytes)
        {
            string localPath = "";

            try
            {
                var localFolder = Application.Context.CacheDir.AbsolutePath;
                localPath = System.IO.Path.Combine(localFolder, fileName);
                File.WriteAllBytes(localPath, bytes); // write to local storage

                return localPath;
            }
            catch (Exception ex)
            {
                if (!string.IsNullOrWhiteSpace(ex.Message))
                    Console.WriteLine("Exception in Plugin.ShareFile: ShareRemoteFile Exception: {0}", ex);
            }

            return localPath;
        }
    }
}