# Share File Plugin for Xamarin and Windows (UWP)

Share File for Xamarin.iOS and Xamarin.Android. This plugin will let you share files in Xamarin.Android and Xamarin.iOS. 

#### Setup
* Coming soon to NuGet
* Or run the following command to create a nuget package:
```
nuget pack Plugin.ShareFile.nuspec
```

**Supports**
* Xamarin.iOS (Unified)
* Xamarin.Android

### Usage

```
CrossShareFile.Current.ShareLocalFile (filePath, "Share file") ;
await CrossShareFile.Current.ShareRemoteFile ("https://developer.xamarin.com/recipes/android/data/adapters/offline.pdf", "offline.pdf");
```
