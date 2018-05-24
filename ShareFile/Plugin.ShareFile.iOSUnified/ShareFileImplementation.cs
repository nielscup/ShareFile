using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Foundation;
using Plugin.ShareFile.Abstractions;
using UIKit;

namespace Plugin.ShareFile
{
    /// <summary>
    /// Implementation for ShareFile
    /// </summary>
    public class ShareFileImplementation : IShareFile
    {
        public void ShareLocalFile(string localFilePath, string title = "", object view = null)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(localFilePath))
                {
                    Console.WriteLine("Plugin.ShareFile: ShareLocalFile Warning: localFilePath null or empty");
                    return;
                }

                var rootController = UIApplication.SharedApplication.KeyWindow.RootViewController;
                var sharedItems = new List<NSObject>();
                var fileName = Path.GetFileName(localFilePath);

                var fileUrl = NSUrl.FromFilename(localFilePath);
                sharedItems.Add(fileUrl);

                UIActivity[] applicationActivities = null;
                var activityViewController = new UIActivityViewController(sharedItems.ToArray(), applicationActivities);

                // Subject
                if (!string.IsNullOrWhiteSpace(title))
                    activityViewController.SetValueForKey(NSObject.FromObject(title), new NSString("subject"));

                if(UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone)
                {
                    rootController.PresentViewController(activityViewController, true, null);
                }
                else
                {
                    var shareView = view as UIView;
                    if (shareView != null)
                    {
                        UIPopoverController popCont = new UIPopoverController(activityViewController);
                        popCont.PresentFromRect(shareView.Frame, shareView, UIPopoverArrowDirection.Any, true);
                    }
                    else
                    {
                        throw new Exception("view is null: for iPad you must pass the view paramater. The view parameter should be the view that triggers the share action, i.e. the share button.");
                    }
                }
            }
            catch (Exception ex)
            {
                if (ex != null && !string.IsNullOrWhiteSpace(ex.Message))
                    Console.WriteLine("Exception in Plugin.ShareFile: ShareLocalFile Exception: {0}", ex);
            }
        }

        /// <summary>
        /// Share a file from a remote resource on compatible services
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
                    ShareLocalFile(filePath, title, view);
                }
            }
            catch (Exception ex)
            {
                if (ex != null && !string.IsNullOrWhiteSpace(ex.Message))
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
                string localFolder = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);
                localPath = System.IO.Path.Combine(localFolder, fileName);
                System.IO.File.WriteAllBytes(localPath, bytes); // write to local storage
            }
            catch (Exception ex)
            {
                if (ex != null && !string.IsNullOrWhiteSpace(ex.Message))
                    Console.WriteLine("Exception in Plugin.ShareFile: ShareRemoteFile Exception: {0}", ex);
            }

            return localPath;
        }
    }
}