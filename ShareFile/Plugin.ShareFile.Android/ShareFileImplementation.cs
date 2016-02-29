using Android.App;
using Android.Content;
using Android.Database;
using Android.Graphics;
using System;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using Plugin.ShareFile.Abstractions;


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
      public void ShareLocalFile(string localFilePath, string title = "")
      {
          if (string.IsNullOrEmpty(localFilePath))
              return;

          if (!localFilePath.StartsWith("file://"))
              localFilePath = string.Format("file://{0}", localFilePath);

          var fileUri = Android.Net.Uri.Parse(localFilePath);

          var intent = new Intent();
          intent.SetFlags(ActivityFlags.ClearTop);
          intent.SetFlags(ActivityFlags.NewTask);
          intent.SetAction(Intent.ActionSend);
          intent.SetType("*/*");
          intent.PutExtra(Intent.ExtraStream, fileUri);
          intent.AddFlags(ActivityFlags.GrantReadUriPermission);

          var chooserIntent = Intent.CreateChooser(intent, title);
          chooserIntent.SetFlags(ActivityFlags.ClearTop);
          chooserIntent.SetFlags(ActivityFlags.NewTask);
          Android.App.Application.Context.StartActivity(chooserIntent);
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
                //return true;
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
			var localFolder = Android.OS.Environment.ExternalStorageDirectory.AbsolutePath;
			string localPath = System.IO.Path.Combine (localFolder, fileName);
			File.WriteAllBytes (localPath, bytes); // write to local storage

            return string.Format("file://{0}/{1}", localFolder, fileName);
		}
  }
}