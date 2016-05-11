#tool nuget:?package=XamarinComponent

#addin nuget:?package=Cake.Xamarin
#addin nuget:?package=Cake.FileHelpers

//////////////////////////////////////////////////////////////////////
// ARGUMENTS
//////////////////////////////////////////////////////////////////////

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");

//////////////////////////////////////////////////////////////////////
// PREPARATION
//////////////////////////////////////////////////////////////////////

FilePath XamarinComponentPath = "./tools/XamarinComponent/tools/xamarin-component.exe";

DirectoryPath outDir = "./output/";
if (!DirectoryExists(outDir)) {
    CreateDirectory(outDir);
}

DirectoryPath extDir = "./externals/";
if (!DirectoryExists(extDir)) {
    CreateDirectory(extDir);
}

var Build = new Action<string>((solution) =>
{
    if (IsRunningOnWindows()) {
        MSBuild(solution, s => s.SetConfiguration(configuration).SetMSBuildPlatform(MSBuildPlatform.x86));
    } else {
        XBuild(solution, s => s.SetConfiguration(configuration));
    }
});

//////////////////////////////////////////////////////////////////////
// TASKS
//////////////////////////////////////////////////////////////////////

Task("Clean")
    .Does(() =>
{
    var dirs = new [] { 
        "./output",
        // source
        "./packages",
        "./source/*/bin", 
        "./source/*/obj", 
        // samples
        "./samples/*/*/packages",
        "./samples/*/*/bin",
        "./samples/*/*/obj",
    };
    foreach (var dir in dirs) {
        CleanDirectories(dir);
    }
});

Task("RestorePackages")
    .Does(() =>
{
    var solutions = new [] { 
        "./Microsoft.OneDriveSDK.sln", 
        "./samples/OneDriveSDKPickerSampleAndroid/OneDriveSDKPickerSampleAndroid.sln",
        "./samples/OneDriveSDKSaverSampleAndroid/OneDriveSDKSaverSampleAndroid.sln",
    };
    foreach (var solution in solutions) {
        Information("Restoring {0}...", solution);
        NuGetRestore(solution, new NuGetRestoreSettings {
            Source = new [] { IsRunningOnWindows () ? "https://api.nuget.org/v3/index.json" : "https://www.nuget.org/api/v2/" },
            Verbosity = NuGetVerbosity.Detailed
        });
    }
});

Task("Externals")
    .Does(() =>
{
    var native = "https://bintray.com/onedrive/Maven/download_file?file_path=com%2Fmicrosoft%2Fonedrivesdk%2Fonedrive-picker-android%2Fv2.0%2Fonedrive-picker-android-v2.0.aar";
    var dest = extDir.CombineWithFilePath ("onedrive-picker-android.aar");
    if (!FileExists (dest)) {
        DownloadFile (native, dest);
    }
});

Task("Build")
    .IsDependentOn("Externals")
    .IsDependentOn("RestorePackages")
    .Does(() =>
{
    var solutions = new [] { 
        "./Microsoft.OneDriveSDK.sln",
    };
    foreach (var solution in solutions) {
        Information("Building {0}...", solution);
        Build(solution);
    }
    
    var outputs = new Dictionary<string, string> {
        { "./source/OneDriveSDK.Android/bin/{0}/OneDriveSDK.Android.dll", "OneDriveSDK.Android.dll" }
    };
    foreach (var output in outputs) {
        var dest = outDir.CombineWithFilePath(string.Format(output.Value, configuration));
        var dir = dest.GetDirectory();
        if (!DirectoryExists(dir)) {
            CreateDirectory(dir);
        }
        CopyFile(string.Format(output.Key, configuration), dest);
    }
});

Task("BuildSamples")
    .IsDependentOn("RestorePackages")
    .Does(() =>
{
    var solutions = new List<string> { 
        "./samples/OneDriveSDKPickerSampleAndroid/OneDriveSDKPickerSampleAndroid.sln",
        "./samples/OneDriveSDKSaverSampleAndroid/OneDriveSDKSaverSampleAndroid.sln",
    };
    foreach (var solution in solutions) {
        Information("Building {0}...", solution);
        Build(solution);
    }
});

Task("PackageNuGet")
    .IsDependentOn("Build")
    .Does(() =>
{
    var nugets = new [] {
        "./nuget/OneDrivePicker.nuspec",
    };
    foreach (var nuget in nugets) {
        Information("Packing (NuGet) {0}...", nuget);
        NuGetPack(nuget, new NuGetPackSettings {
            OutputDirectory = outDir,
            Verbosity = NuGetVerbosity.Detailed,
            BasePath = IsRunningOnUnix() ? "././" : "./",
        });
    }
});

Task("PackageComponent")
    .IsDependentOn("Build")
    .IsDependentOn("PackageNuGet")
    .Does(() =>
{
    Information("Packing Component...");
        
    DeleteFiles("./component/*.xam");
    PackageComponent("./component/", new XamarinComponentSettings { ToolPath = XamarinComponentPath });
    
    DeleteFiles("./output/*.xam");
    MoveFiles("./component/*.xam", outDir);
});

Task("Package")
    .IsDependentOn("PackageNuGet")
    .IsDependentOn("PackageComponent")
    .Does(() =>
{
});

//////////////////////////////////////////////////////////////////////
// TASK TARGETS
//////////////////////////////////////////////////////////////////////

Task("Default")
    .IsDependentOn("Build")
    .IsDependentOn("Package")
    .IsDependentOn("BuildSamples");

//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

RunTarget(target);
