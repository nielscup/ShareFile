using CoreGraphics;
using Plugin.ShareFile;
using System;
using System.Net;
using UIKit;

namespace ShareFileTest.iOS
{
    public partial class ViewController : UIViewController
    {
        public int yPos = 40;
        int xPos;
        private const int defaultHeight = 60;
        private const int defaultWidth = 300;

        UIButton shareLocalFileButton;
        string testFilePath;
        const string remoteFileUrl = "https://cupitcontent.blob.core.windows.net/images/cup-it.png";
        const string testFileName = "testfile.png";

        public ViewController(IntPtr handle): base(handle) { }
                
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            // Perform any additional setup after loading the view, typically from a nib.

            var shareRemoteFileButton = AddButton("Share Remote File");
            shareRemoteFileButton.TouchUpInside += shareRemoteFileButton_TouchUpInside;

            shareLocalFileButton = AddButton("Share Local File");
            shareLocalFileButton.Hidden = true;
            shareLocalFileButton.TouchUpInside += shareLocalFileButton_TouchUpInside;

            DownloadTestFile();
        }
                
        async void shareRemoteFileButton_TouchUpInside(object sender, EventArgs e)
        {
            await CrossShareFile.Current.ShareRemoteFile(remoteFileUrl, testFileName, "Share remote file");
        }

        void shareLocalFileButton_TouchUpInside(object sender, EventArgs e)
        {
            CrossShareFile.Current.ShareLocalFile(testFilePath);
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }

        public UIButton AddButton(string title, int width = defaultWidth)
        {
            var height = 40;            

            var button = new UIButton(GetFrame(height, width));
            button.SetTitle(title, UIControlState.Normal);
            button.SetTitleColor(new UIColor(1, 0, 0, 1), UIControlState.Normal);
            Add(button);

            yPos += height;

            return button;
        }

        private CGRect GetFrame(int height, int width)
        {
            var rect = new CGRect(20, yPos, width, height);
            return rect;
        }

        private async void DownloadTestFile()
        {
            using (var webClient = new WebClient())
            {
                var uri = new System.Uri(remoteFileUrl);
                var bytes = await webClient.DownloadDataTaskAsync(uri);
                testFilePath = WriteFile(testFileName, bytes);
            }

            shareLocalFileButton.Hidden = false;
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