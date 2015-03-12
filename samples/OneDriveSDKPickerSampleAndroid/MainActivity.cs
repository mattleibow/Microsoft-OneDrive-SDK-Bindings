using System;
using System.Net;
using System.Threading.Tasks;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Views;
using Android.Widget;

using Microsoft.OneDrive.Pickers;

[assembly: UsesPermission (Android.Manifest.Permission.Internet)]

namespace OneDriveSDKPickerSampleAndroid
{
	/// <summary>
	/// Activity that shows how the OneDrive SDK can be used for file picking
	/// </summary>
	[Activity (Label = "@string/app_name", MainLauncher = true, Icon = "@drawable/ic_launcher")]
	public class MainActivity : Activity
	{
		/// <summary>
		/// Registered Application id for OneDrive {@link http
		/// ://go.microsoft.com/fwlink/p/?LinkId=193157}
		/// </summary>
		private const string OneDriveAppId = "48122D4E";

		/// <summary>
		/// The OneDrive picker instance used by this activity
		/// </summary>
		private IPicker picker;

		/// <summary>
		/// The picked file's download URI
		/// </summary>
		private Android.Net.Uri downloadUrl;

		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);
			SetContentView (Resource.Layout.MainLayout);

			// Create the picker instance
			picker = Picker.CreatePicker (OneDriveAppId);

			// Add the start picker listener
			FindViewById<Button> (Resource.Id.startPickerButton).Click += delegate {
				// Clear out any previous results
				ClearResultTable ();

				// Determine the link type that was selected
				LinkType linkType;
				if (FindViewById<RadioButton> (Resource.Id.radioWebViewLink).Checked) {
					linkType = LinkType.WebViewLink;
				} else if (FindViewById<RadioButton> (Resource.Id.radioDownloadLink).Checked) {
					linkType = LinkType.DownloadLink;
				} else {
					throw new Exception ("Invalid Radio Button Choosen.");
				}

				// Start the picker
				picker.StartPicking (this, linkType);
			};

			// Add the save as listener for download links
			FindViewById<Button> (Resource.Id.saveAsButton).Click += delegate {
				if (downloadUrl != null) {
					var downloadManager = DownloadManager.FromContext (this);
					var request = new DownloadManager.Request (downloadUrl);
					request.SetNotificationVisibility (DownloadVisibility.VisibleNotifyCompleted);
					downloadManager.Enqueue (request);
				}
			};
		}

		protected override void OnActivityResult (int requestCode, Result resultCode, Intent data)
		{
			// Get the results from the from the picker
			//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
			//ORIGINAL LINE: final IPickerResult result = mPicker.getPickerResult(requestCode, resultCode, data);
			var result = picker.GetPickerResult (requestCode, resultCode, data);

			// Handle the case if nothing was picked
			if (result != null) {
				// Update the UI with the picker results
				UpdateResultTable (result);
			} else {
				Toast.MakeText (this, "Did not get a file from the picker!", ToastLength.Long).Show ();
				return;
			}
		}

		/// <summary>
		/// Updates the results table with details from an <seealso cref="IPickerResult"/>
		/// </summary>
		/// <param name="result"> The results of the picker </param>
		private void UpdateResultTable (IPickerResult result)
		{
			FindViewById<TextView> (Resource.Id.nameResult).Text = result.Name;
			FindViewById<TextView> (Resource.Id.linkTypeResult).Text = result.LinkType.ToString ();
			FindViewById<TextView> (Resource.Id.linkResult).Text = result.Link.ToString ();
			FindViewById<TextView> (Resource.Id.fileSizeResult).Text = result.Size.ToString ();

			var thumbnailSmall = result.ThumbnailLinks ["small"].ToString ();
			FindViewById<TextView> (Resource.Id.thumbnail_small_uri).Text = thumbnailSmall;
			CreateUpdateThumbnail (FindViewById<ImageView> (Resource.Id.thumbnail_small), thumbnailSmall);

			var thumbnailMedium = result.ThumbnailLinks ["medium"].ToString ();
			FindViewById<TextView> (Resource.Id.thumbnail_medium_uri).Text = thumbnailMedium;
			CreateUpdateThumbnail (FindViewById<ImageView> (Resource.Id.thumbnail_medium), thumbnailMedium);

			var thumbnailLarge = result.ThumbnailLinks ["large"].ToString ();
			FindViewById<TextView> (Resource.Id.thumbnail_large_uri).Text = thumbnailLarge;
			CreateUpdateThumbnail (FindViewById<ImageView> (Resource.Id.thumbnail_large), thumbnailLarge);

			FindViewById (Resource.Id.thumbnails).Visibility = ViewStates.Visible;

			if (result.LinkType == LinkType.DownloadLink) {
				FindViewById (Resource.Id.saveAsArea).Visibility = ViewStates.Visible;
				downloadUrl = result.Link;
			}
		}

		/// <summary>
		/// Clears out all picker results
		/// </summary>
		private void ClearResultTable ()
		{
			FindViewById<TextView> (Resource.Id.nameResult).Text = "";
			FindViewById<TextView> (Resource.Id.linkTypeResult).Text = "";
			FindViewById<TextView> (Resource.Id.linkResult).Text = "";
			FindViewById<TextView> (Resource.Id.fileSizeResult).Text = "";
			FindViewById (Resource.Id.thumbnails).Visibility = ViewStates.Invisible;
			FindViewById (Resource.Id.saveAsArea).Visibility = ViewStates.Invisible;
			FindViewById<ImageView> (Resource.Id.thumbnail_small).SetImageBitmap (null);
			FindViewById<TextView> (Resource.Id.thumbnail_small_uri).Text = "";
			FindViewById<ImageView> (Resource.Id.thumbnail_medium).SetImageBitmap (null);
			FindViewById<TextView> (Resource.Id.thumbnail_medium_uri).Text = "";
			FindViewById<ImageView> (Resource.Id.thumbnail_large).SetImageBitmap (null);
			FindViewById<TextView> (Resource.Id.thumbnail_large_uri).Text = "";
			downloadUrl = null;
		}

		/// <summary>
		/// Download the thumbnails for display
		/// </summary>
		/// <param name="imageView"> The image view that should be updated </param>
		/// <param name="imageSource"> The uri of the image that should be put on the image view </param>
		private async Task CreateUpdateThumbnail (ImageView imageView, string imageSource)
		{
			if (imageSource != null) {
				try {
					var result = await WebRequest.CreateDefault (new Uri (imageSource)).GetResponseAsync ();
					using (var inputStream = result.GetResponseStream ())
					using (var bitmap = await BitmapFactory.DecodeStreamAsync (inputStream)) {
						imageView.SetImageBitmap (bitmap);
					}
				} catch (Exception ex) {
					Toast.MakeText (this, "Download Error: " + ex.Message, ToastLength.Long).Show ();
				}
			}
		}
	}
}
