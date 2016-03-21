using CoreGraphics;
using Foundation;
using Plugin.ShareFile.Abstractions;
using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using UIKit;
using System.Collections.Generic;

namespace Plugin.ShareFile
{
    /// <summary>
    /// Implementation for ShareFile
    /// </summary>
    public class ShareFileImplementation : IShareFile
    {
        public void ShareLocalFile(string localFilePath, string title = "")
        {
            try
            {
                if (string.IsNullOrWhiteSpace(localFilePath))
                {
                    Console.WriteLine("Plugin.ShareFile: ShareLocalFile Warning: localFilePath null or empty");
                    return;
                }

                var rootController = UIApplication.SharedApplication.KeyWindow.RootViewController;
                var sharedItems = new System.Collections.Generic.List<NSObject>();
                var fileName = Path.GetFileName(localFilePath);
                
                // Image
                //UIImage uiImage = UIImage.FromFile(localFilePath);
                //sharedItems.Add(uiImage);
                
                // File => Attachhment name incorrect
                //var fileData = NSData.FromFile(localFilePath);                
                //sharedItems.Add(fileData);

                // 
                var fileUrl = NSUrl.FromFilename(localFilePath);
                sharedItems.Add(fileUrl);

                //// Text
                //string theText = "the shared text message";
                //var messageNSStr = new NSString(theText);
                //sharedItems.Add(messageNSStr);

                UIActivity[] applicationActivities = null;
                var activityViewController = new UIActivityViewController(sharedItems.ToArray(), applicationActivities);

                // Subject
                if (!string.IsNullOrWhiteSpace(title))
                    activityViewController.SetValueForKey(NSObject.FromObject(title), new NSString("subject"));

                rootController.PresentViewController(activityViewController, true, null);
            }
            catch (Exception ex)
            {
                if (ex != null && !string.IsNullOrWhiteSpace(ex.Message))
                    Console.WriteLine("Plugin.ShareFile: ShareLocalFile Exception: {0}", ex);
            }
        }

        /// <summary>
        /// Share a file from a remote resource on compatible services
        /// </summary>
        /// <param name="fileUri">uri to external file</param>
        /// <param name="fileName">name of the file</param>
        /// <param name="title">Title of popup on share (not included in message)</param>
        /// <returns>awaitable bool</returns>
        public async Task ShareRemoteFile(string fileUri, string fileName, string title = "")
        {
            try
            {
                using (var webClient = new WebClient())
                {
                    var uri = new System.Uri(fileUri);
                    var bytes = await webClient.DownloadDataTaskAsync(uri);
                    var filePath = WriteFile(fileName, bytes);
                    ShareLocalFile(filePath, title);
                }
            }
            catch (Exception ex)
            {
                if (ex != null && !string.IsNullOrWhiteSpace(ex.Message))
                    Console.WriteLine("Plugin.ShareFile: ShareRemoteFile Exception: {0}", ex.Message);
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
                    Console.WriteLine("Plugin.ShareFile: ShareRemoteFile Exception: {0}", ex);
            }

            return localPath;
        }

        ///// <summary>
        ///// Share a local file on compatible services
        ///// </summary>
        ///// <param name="localFilePath">path to local file</param>
        ///// <param name="title">Title of popup on share (not included in message)</param>
        ///// <returns>awaitable Task</returns>
        //public void ShareLocalFile2(string localFilePath, string title = "")
        //{
        //    try
        //    {
        //        if (string.IsNullOrWhiteSpace(localFilePath))
        //        {
        //            Console.WriteLine("Plugin.ShareFile: ShareLocalFile Warning: localFilePath null or empty");
        //            return;
        //        }

        //        //var controller = UIApplication.SharedApplication.KeyWindow.RootViewController;
        //        var view = UIApplication.SharedApplication.KeyWindow.RootViewController.View;

        //        //var path = Path.Combine(NSBundle.MainBundle.BundlePath, "Documents/test123.png");
        //        //localFilePath = path; //"./Documents/test123.png";
        //        var interactionController = UIDocumentInteractionController.FromUrl(NSUrl.FromFilename(localFilePath.Trim()));


        //        //_openInWindow.ViewControllerForPreview = c => controller;
        //        //_openInWindow.ViewForPreview = c => view;
        //        //_openInWindow.RectangleForPreview = c => view.Bounds;
        //        var result = interactionController.PresentOpenInMenu(view.Bounds, view, true);

        //        if (!result)
        //        {
        //            Console.WriteLine("Plugin.ShareFile: Unable to share file");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        if (ex != null && !string.IsNullOrWhiteSpace(ex.Message))
        //            Console.WriteLine("Plugin.ShareFile: ShareLocalFile Exception: {0}", ex);
        //    }
        //}
    }
}