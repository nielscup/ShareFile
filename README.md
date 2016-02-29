# Custom Camera Plugin for Xamarin

Custom Camera for Xamarin.iOS and Xamarin.Android. This plugin will let you add a custom camera view to your Xamarin project. Easily select either front or rear facing camera from within your code. 

#### Setup
* Coming soon to NuGet
* Or run the following command to create a nuget package:
```
nuget pack Plugin.ImageCrop.nuspec
```

**Supports**
* Xamarin.iOS (Unified)
* Xamarin.Android

### Usage

Use the CustomCameraView in your Android or iOS Project

**Custom Camera Android**

See the test project for a working example.
```
<plugin.customcamera.android.CustomCameraView
    android:id="@+id/customCameraView"
	android:layout_width="match_parent"
	android:layout_height="match_parent" />

```
```
_customCameraView = FindViewById<CustomCameraView>(Resource.Id.customCameraView);
_customCameraView.SelectedCamera = CameraSelection.Back;
_customCameraView.SelectedCamera = CameraSelection.Front;
```
**Custom Camera iOS**
```
TODO
```
