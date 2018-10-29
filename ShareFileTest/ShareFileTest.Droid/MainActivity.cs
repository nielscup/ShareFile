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
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;

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
            Button shareRemoteFileButton = FindViewById<Button>(Resource.Id.shareRemoteFileButton);
            shareRemoteFileButton.Click += shareRemoteFileButton_Click;

            shareLocalFileButton = FindViewById<Button>(Resource.Id.shareLocalFileButton);
            shareLocalFileButton.Click += shareLocalFileButton_Click;
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        async void shareRemoteFileButton_Click(object sender, EventArgs e)
        {
            if (!await RequestStoragePermission()) return;
            await CrossShareFile.Current.ShareRemoteFile(remoteFileUrl, testFileName, "Share remote file");
        }

        async void shareLocalFileButton_Click(object sender, EventArgs e)
        {
            if (!await RequestStoragePermission()) return;
            await DownloadTestFile();
            CrossShareFile.Current.ShareLocalFile(testFilePath);
        }

        async Task<bool> RequestStoragePermission()
        {
            // Request Storage permission.
            var permission = Permission.Storage;
            var status = await CrossPermissions.Current.CheckPermissionStatusAsync(permission);
            if (status != PermissionStatus.Granted)
            {
                if (await CrossPermissions.Current.ShouldShowRequestPermissionRationaleAsync(permission))
                {
                    
                }

                var results = await CrossPermissions.Current.RequestPermissionsAsync(permission);
                //Best practice to always check that the key exists
                if (results.ContainsKey(permission))
                    status = results[permission];
            }

            if (status == PermissionStatus.Granted)
            {
                return true;
            }
            else
            {
                var dlgAlert = (new AlertDialog.Builder(this)).Create();

                dlgAlert.SetTitle("Storage permission");
                dlgAlert.SetMessage("This app needs storage permission");

                dlgAlert.SetButton("OK", HandlerDialogButton);
                dlgAlert.Show();
            }

            return false;
        }

        private async Task DownloadTestFile()
        {
            using (var webClient = new WebClient())
            {
                var uri = new System.Uri(remoteFileUrl);
                var bytes = await webClient.DownloadDataTaskAsync(uri);
                testFilePath = WriteFile(testFileName, bytes);
            }

            //shareLocalFileButton.Visibility = ViewStates.Visible;
        }

        void HandlerDialogButton(object sender, DialogClickEventArgs e)
        {
            if (sender is AlertDialog objAlertDialog) objAlertDialog.Dismiss();
        }

        /// <summary>
        /// Writes the file to local storage.
        /// </summary>
        /// <returns>The file.</returns>
        /// <param name="fileName">File name.</param>
        /// <param name="bytes">Bytes.</param>
        private string WriteFile(string fileName, byte[] bytes)
        {
            var localFolder = Application.Context.CacheDir.AbsolutePath;

            string localPath = System.IO.Path.Combine(localFolder, fileName);
            File.WriteAllBytes(localPath, bytes); // write to local storage

            return localPath;
        }
    }
}

