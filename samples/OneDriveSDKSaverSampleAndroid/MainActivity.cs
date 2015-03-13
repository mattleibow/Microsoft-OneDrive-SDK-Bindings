using System;

using Android.App;
using Android.Content;
using Android.Views;
using Android.Widget;
using Android.OS;

using Microsoft.OneDrive.Savers;
using System.IO;
using System.Threading.Tasks;

[assembly: UsesPermission (Android.Manifest.Permission.Internet)]
[assembly: UsesPermission (Android.Manifest.Permission.WriteExternalStorage)]

namespace OneDriveSDKSaverSampleAndroid
{
	[Activity (Label = "@string/app_name", MainLauncher = true, Icon = "@drawable/ic_launcher")]
	public class MainActivity : Activity
	{
		/// <summary>
		/// The default file size
		/// </summary>
		private const int DefaultFileSizeKb = 100;

		/// Registered Application id for OneDrive {<seealso cref="http://go.microsoft.com/fwlink/p/?LinkId=193157" />}.
		private const string OneDriveAppId = "insert OneDrive App ID here";

		/// <summary>
		/// The OneDrive saver instance used by this activity
		/// </summary>
		private ISaver saver;

		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);
			SetContentView (Resource.Layout.MainLayout);

			// Create the picker instance
			saver = Saver.CreateSaver (OneDriveAppId);

			// Add the start saving listener
			FindViewById (Resource.Id.startSaverButton).Click += async delegate {
				FindViewById (Resource.Id.result_table).Visibility = ViewStates.Invisible;

				var filename = FindViewById<EditText> (Resource.Id.file_name_edit_text).Text;
				var fileSizeString = FindViewById<EditText> (Resource.Id.file_size_edit_text).Text;
				int size;
				if (!int.TryParse (fileSizeString, out size)) {
					size = DefaultFileSizeKb;
				}

				// Create a file
				var file = await CreateExternalSdCardFile (filename, size);

				// Start the saver
				saver.StartSaving (this, filename, Android.Net.Uri.Parse ("file://" + file));
			};
		}

		protected override void OnActivityResult (int requestCode, Result resultCode, Intent data)
		{
			// Check that we were able to save the file on OneDrive
			TextView overallResult = FindViewById<TextView> (Resource.Id.overall_result);
			TextView errorResult = FindViewById<TextView> (Resource.Id.error_type_result);
			TextView debugErrorResult = FindViewById<TextView> (Resource.Id.debug_error_result);

			try {
				saver.HandleSave (requestCode, resultCode, data);

				overallResult.Text = GetString (Resource.String.overall_result_success);
				errorResult.Text = GetString (Resource.String.error_message_none);
				debugErrorResult.Text = GetString (Resource.String.error_message_none);
			} catch (SaverException e) {
				overallResult.Text = GetString (Resource.String.overall_result_failure);
				errorResult.Text = e.ErrorType.ToString ();
				debugErrorResult.Text = e.DebugErrorInfo;
			}
			FindViewById (Resource.Id.result_table).Visibility = ViewStates.Visible;
		}

		/// <summary>
		/// Creates an file on the SDCard </summary>
		/// <param name="filename"> The name of the file to create </param>
		/// <param name="size"> The size in KB to make the file </param>
		/// <returns> The <seealso cref="File"/> object that was created </returns>
		private async Task<string> CreateExternalSdCardFile (string filename, int size)
		{
			const int bufferSize = 1024;
			var random = new Random ();

			// get the path
			var fullPath = Android.OS.Environment.ExternalStorageDirectory.AbsolutePath;
			fullPath = Path.Combine (fullPath, filename);

			// create the file
			try {
				using (var stream = File.OpenWrite (fullPath)) {
					// Create a 1 kb size buffer to use in writing the temp file
					var buffer = new byte[bufferSize];
					random.NextBytes (buffer);

					// Write out the file, 1 kb at a time
					for (int i = 0; i < size; i++) {
						await stream.WriteAsync (buffer, 0, buffer.Length);
					}
				}
			} catch (IOException ex) {
				Toast.MakeText (this, "Error when creating the file: " + ex.Message, ToastLength.Long).Show ();
			}

			return fullPath;
		}
	}
}
