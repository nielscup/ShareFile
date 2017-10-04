# Share File Plugin for Xamarin and Windows

Share File for Xamarin.iOS and Xamarin.Android. This plugin will let you share files in Xamarin.Android and Xamarin.iOS. 

#### Setup
* Available on nuget: https://www.nuget.org/packages/Plugin.ShareFile/
* Install into your PCL project and Client projects.

**Supports**
* Xamarin.iOS (Unified)
* Xamarin.Android
* Windows (UWP)

### Usage

```
CrossShareFile.Current.ShareLocalFile (filePath, "Share file") ;
await CrossShareFile.Current.ShareRemoteFile ("https://developer.xamarin.com/recipes/android/data/adapters/offline.pdf", "offline.pdf");
```
