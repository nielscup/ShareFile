using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Plugin.ShareFile;
using System.Threading.Tasks;
using System.Net;
using System.IO;

namespace ShareFileTest.Droid
{
    [Activity(Label = "ShareFileTest.Droid", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        Button shareLocalFileButton;
        string testFilePath;
        const string remoteFileUrl = "https://cupitcontent.blob.core.windows.net/images/cup-it.png";
        const string testFileName = "testfile.png";

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            // Get our button from the layout resource,
            // and attach an event to it
            shareLocalFileButton = FindViewById<Button>(Resource.Id.shareLocalFileButton);
            shareLocalFileButton.Click += shareLocalFileButton_Click;

            Button shareRemoteFileButton = FindViewById<Button>(Resource.Id.shareRemoteFileButton);
            shareRemoteFileButton.Click += shareRemoteFileButton_Click;
            DownloadTestFile();
        }

        void shareLocalFileButton_Click(object sender, EventArgs e)
        {
            CrossShareFile.Current.ShareLocalFile(testFilePath);
        }

        async void shareRemoteFileButton_Click(object sender, EventArgs e)
        {
            await CrossShareFile.Current.ShareRemoteFile(remoteFileUrl, testFileName, "Share remote file");
        }

        private async void DownloadTestFile()
        {
            using (var webClient = new WebClient())
            {
                var uri = new System.Uri(remoteFileUrl);
                var bytes = await webClient.DownloadDataTaskAsync(uri);
                testFilePath = WriteFile(testFileName, bytes);
            }

            shareLocalFileButton.Visibility = ViewStates.Visible;
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
            string localPath = System.IO.Path.Combine(localFolder, fileName);
            File.WriteAllBytes(localPath, bytes); // write to local storage

            return string.Format("file://{0}/{1}", localFolder, fileName);
        }
    }
}

