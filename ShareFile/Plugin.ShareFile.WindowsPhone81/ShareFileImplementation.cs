using Plugin.ShareFile.Abstractions;
using System;


namespace Plugin.ShareFile
{
  /// <summary>
  /// Implementation for ShareFile
  /// </summary>
  public class ShareFileImplementation : IShareFile
  {
      public void ShareLocalFile(string localFilePath, string title = "", object view = null)
      {
          throw new NotImplementedException();
      }

      public System.Threading.Tasks.Task ShareRemoteFile(string fileUri, string fileName, string title = "")
      {
          throw new NotImplementedException();
      }
  }
}