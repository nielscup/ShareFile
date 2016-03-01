using CoreGraphics;
using Foundation;
using Plugin.ShareFile.Abstractions;
using System;
using System.Net;
using System.Threading.Tasks;
using UIKit;


namespace Plugin.ShareFile
{
  /// <summary>
  /// Implementation for ShareFile
  /// </summary>
  public class ShareFileImplementation : IShareFile
  {
      /// <summary>
      /// Simply share a local file on compatible services
      /// </summary>
      /// <param name="localFilePath">path to local file</param>
      /// <param name="title">Title of popup on share (not included in message)</param>
      /// <returns>awaitable Task</returns>
      public void ShareLocalFile(string localFilePath, string title = "")
      {
          var _openInWindow = UIDocumentInteractionController.FromUrl(NSUrl.FromFilename(localFilePath.Trim()));
          _openInWindow.PresentOpenInMenu(new CGRect(0, 260, 320, 320), UIApplication.SharedApplication.KeyWindow.RootViewController.View, false);
      }

      /// <summary>
      /// Simply share a file from a remote resource on compatible services
      /// </summary>
      /// <param name="fileUri">uri to external file</param>
      /// <param name="fileName">name of the file</param>
      /// <param name="title">Title of popup on share (not included in message)</param>
      /// <returns>awaitable bool</returns>
      public async Task ShareRemoteFile(string fileUri, string fileName, string title = "")
      {
            using (var webClient = new WebClient())
            {
                var uri = new System.Uri(fileUri);
                var bytes = await webClient.DownloadDataTaskAsync(uri);
                var filePath = WriteFile(fileName, bytes);
                ShareLocalFile(filePath, title);
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
          string localFolder = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
          string localPath = System.IO.Path.Combine(localFolder, fileName);
          System.IO.File.WriteAllBytes(localPath, bytes); // write to local storage

          return localPath;
      }
  }
}