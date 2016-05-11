
[![Build status][img]][link]

The picker is the easiset way to browse, select, open and save OneDrive files an Android app with OneDrive.

![Preview of the OneDrive Picker in Action](https://github.com/mattleibow/Microsoft-Live-SDK-Bindings/raw/master/images/android-picker-saver.png)

## Requirements

Before we can access the OneDrive, we will need to get an App ID. Register the app [here](https://account.live.com/developers/applications) to get an App ID (Client ID).

The OneDrive picker library is supported at runtime for [Android API revision 15](http://source.android.com/source/build-numbers.html) and greater.

The picker requires the OneDrive app to be installed, in order to function. If the OneDrive app is not installed, the user will be prompted to download the app when either the `StartPicking()` or `StartSaving()` method is invoked.

## Opening files

Our app needs to give the user a way to start opening files from OneDrive. Here we set up a click handler that launches the open picker:

```csharp
// keep a reference to the picker
private IPicker picker;

protected override void OnCreate(Bundle savedInstanceState)
{
    base.OnCreate(savedInstanceState);
    
    // ...

    // create a picker
    picker = Picker.CreatePicker("ONEDRIVE_APP_ID");
    
    button.Click += delegate {
        // start the picker activity
        picker.StartPicking(this, LinkType.DownloadLink);
    };
}

protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
{
    // get the results from the picker activity
    var result = picker.GetPickerResult(requestCode, resultCode, data);
    if (result != null) {
        // use the result
    } else {
        // continue as normal
        base.OnActivityResult(requestCode, resultCode, data);
    }
}
```

### Start Picking

The picker can be configured to return a URL for the selected file in one of these
formats:

 - `LinkType.DownloadLink` - the URL is valid for 1 hour and can be used to download a file
 - `LinkType.WebViewLink` - the readonly URL is valid until the user deletes the sharing link

### Picker Result Object
In addition to the filename and link for the file, you can access several other properties on the `IPickerResult` object that provide more details about the file selected:

```csharp
public interface IPickerResult
{
    Uri Link { get; }
    LinkType LinkType { get; }
    string Name { get; }
    long Size { get; }
    // keys: "small", "medium", and "large"
    IDictionary<string, Uri> ThumbnailLinks { get; }
}
```

## Saving Files

Similar to when opening files, we should provide a way for the user to save a file to OneDrive. Before we can upload a file, we need a filename for OneDrive and the path to the file on the local file system:

```csharp
private ISaver saver;

protected override void OnCreate (Bundle savedInstanceState)
{
    base.OnCreate (savedInstanceState);
    
    // ...

    // create the saver
    saver = Saver.createSaver("ONEDRIVE_APP_ID");
    
    button.Click += delegate {
        // launch the saver
        saver.startSaving(this, "FILENAME", "file://PATH/TO/FILE");
    }
};

protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
{
    try {
        // the file saved successfully
        saver.HandleSave (requestCode, resultCode, data);
    } catch (SaverException ex) {
        // there was an error
    }
}
```

### Start Saving

The saver currently supports the `content://` and `file://` file URI scheme. If a different URI scheme is used, the saver will return a `NoFileSpecified` error type.

### Saver Result

The error message provided by `DebugErrorInfo` is primarily for development and debugging and can change at any time. When handling errors, you can use `ErrorType` to determine the general cause of the error.


### Saver Error Types

When the saver is unable to complete saving a file and throws an exception, it provides a `SaverError` through the `ErrorType` property that indicates one of a set of possible error types:

```csharp
public enum SaverError {
    Unknown,
    Cancelled,
    // The OneDrive account did not have enough space
    OutOfQuota,
    InvalidFileName,
    NoNetworkConnectivity,
    // The Uri to the file could not be accessed
    CouldNotAccessFile,
    // No file was specified to be saved, 
    // or the file URI scheme was not supported
    NoFileSpecified
}
```

[img]: https://ci.appveyor.com/api/projects/status/7t4j8ac2b68m36hn/branch/master?svg=true
[link]: https://ci.appveyor.com/project/mattleibow/microsoft-onedrive-sdk-bindings/branch/master
