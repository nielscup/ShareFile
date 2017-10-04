# Share File Plugin for Xamarin and Windows

Cross platform plugin to share files in Xamarin.Android, Xamarin.iOS and Windows.

#### Setup
* Available on nuget: https://www.nuget.org/packages/Plugin.ShareFile/
* Install into your PCL project and Client projects.

**Supports**
* Xamarin.iOS (Unified)
* Xamarin.Android
* Windows (UWP)

### Usage

Share a local file:
```
CrossShareFile.Current.ShareLocalFile (filePath, "Share file") ;
```

Share a remote file:
```
await CrossShareFile.Current.ShareRemoteFile ("https://developer.xamarin.com/recipes/android/data/adapters/offline.pdf", "offline.pdf");
```
