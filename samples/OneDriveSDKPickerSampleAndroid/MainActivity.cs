using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

[assembly: UsesPermission(Android.Manifest.Permission.Internet)]

namespace OneDriveSDKSampleAndroid
{
	/// <summary>
	/// Activity that shows how the OneDrive SDK can be used for file picking
	/// </summary>
	[Activity (Label = "OneDriveSDKSampleAndroid", MainLauncher = true, Icon = "@drawable/icon")]
	public class MainActivity : Activity
	{

		/// <summary>
		/// Registered Application id for OneDrive {@link http
		/// ://go.microsoft.com/fwlink/p/?LinkId=193157}
		/// </summary>
		private const string ONEDRIVE_APP_ID = "48122D4E";

		/// <summary>
		/// The onClickListener that will start the OneDrive Picker
		/// </summary>
		private readonly OnClickListener mStartPickingListener = new OnClickListenerAnonymousInnerClassHelper();

		private class OnClickListenerAnonymousInnerClassHelper : OnClickListener
		{
			public OnClickListenerAnonymousInnerClassHelper()
			{
			}

			public override void OnClick(View v)
			{
				// Clear out any previous results
				outerInstance.ClearResultTable();

				// Determine the link type that was selected
				LinkType linkType;
				if (((RadioButton)findViewById(R.id.radioWebViewLink)).Checked)
				{
					linkType = LinkType.WebViewLink;
				}
				else if (((RadioButton)findViewById(R.id.radioDownloadLink)).Checked)
				{
					linkType = LinkType.DownloadLink;
				}
				else
				{
					throw new Exception("Invalid Radio Button Choosen.");
				}

				// Start the picker
				outerInstance.mPicker.startPicking((Activity)v.Context, linkType);
			}
		}

		/// <summary>
		/// Saves the picked file from OneDrive to the device
		/// </summary>
		private readonly OnClickListener mSaveLocally = new OnClickListenerAnonymousInnerClassHelper2();

		private class OnClickListenerAnonymousInnerClassHelper2 : OnClickListener
		{
			public OnClickListenerAnonymousInnerClassHelper2()
			{
			}

			public override void OnClick(View v)
			{
				if (outerInstance.mDownloadUrl == null)
				{
					return;
				}

				DownloadManager downloadManager = (DownloadManager)v.Context.getSystemService(DOWNLOAD_SERVICE);
				Request request = new Request(outerInstance.mDownloadUrl);
				request.NotificationVisibility = Request.VISIBILITY_VISIBLE_NOTIFY_COMPLETED;
				downloadManager.enqueue(request);
			}
		}

		/// <summary>
		/// The OneDrive picker instance used by this activity
		/// </summary>
		private IPicker mPicker;

		/// <summary>
		/// The picked file's download URI
		/// </summary>
		private Uri mDownloadUrl;

		protected internal override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			ContentView = R.layout.activity_picker_main;

			// Create the picker instance
			mPicker = Picker.createPicker(ONEDRIVE_APP_ID);

			// Add the start picker listener
			((Button)findViewById(R.id.startPickerButton)).OnClickListener = mStartPickingListener;

			// Add the save as listener for download links
			((Button)findViewById(R.id.saveAsButton)).OnClickListener = mSaveLocally;
		}

		protected internal override void OnActivityResult(int requestCode, int resultCode, Intent data)
		{
			// Get the results from the from the picker
			//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
			//ORIGINAL LINE: final IPickerResult result = mPicker.getPickerResult(requestCode, resultCode, data);
			IPickerResult result = mPicker.getPickerResult(requestCode, resultCode, data);

			// Handle the case if nothing was picked
			if (result == null)
			{
				Toast.makeText(this, "Did not get a file from the picker!", Toast.LENGTH_LONG).show();
				return;
			}

			// Update the UI with the picker results
			UpdateResultTable(result);
		}

		/// <summary>
		/// Updates the results table with details from an <seealso cref="IPickerResult"/>
		/// </summary>
		/// <param name="result"> The results of the picker </param>
		private void UpdateResultTable(IPickerResult result)
		{
			((TextView)findViewById(R.id.nameResult)).Text = result.Name;
			((TextView)findViewById(R.id.linkTypeResult)).Text = result.LinkType + "";
			((TextView)findViewById(R.id.linkResult)).Text = result.Link + "";
			((TextView)findViewById(R.id.fileSizeResult)).Text = result.Size + "";

			Uri thumbnailSmall = result.ThumbnailLinks.get("small");
			CreateUpdateThumbnail((ImageView)findViewById(R.id.thumbnail_small), thumbnailSmall).execute((Void)null);
			((TextView)findViewById(R.id.thumbnail_small_uri)).Text = thumbnailSmall + "";

			Uri thumbnailMedium = result.ThumbnailLinks.get("medium");
			CreateUpdateThumbnail((ImageView)findViewById(R.id.thumbnail_medium), thumbnailMedium).execute((Void)null);
			((TextView)findViewById(R.id.thumbnail_medium_uri)).Text = thumbnailMedium + "";

			Uri thumbnailLarge = result.ThumbnailLinks.get("large");
			CreateUpdateThumbnail((ImageView)findViewById(R.id.thumbnail_large), thumbnailLarge).execute((Void)null);
			((TextView)findViewById(R.id.thumbnail_large_uri)).Text = thumbnailLarge + "";

			findViewById(R.id.thumbnails).Visibility = View.VISIBLE;

			if (result.LinkType == LinkType.DownloadLink)
			{
				findViewById(R.id.saveAsArea).Visibility = View.VISIBLE;
				mDownloadUrl = result.Link;
			}
		}

		/// <summary>
		/// Clears out all picker results
		/// </summary>
		private void ClearResultTable()
		{
			((TextView)findViewById(R.id.nameResult)).Text = "";
			((TextView)findViewById(R.id.linkTypeResult)).Text = "";
			((TextView)findViewById(R.id.linkResult)).Text = "";
			((TextView)findViewById(R.id.fileSizeResult)).Text = "";
			findViewById(R.id.thumbnails).Visibility = View.INVISIBLE;
			findViewById(R.id.saveAsArea).Visibility = View.INVISIBLE;
			((ImageView)findViewById(R.id.thumbnail_small)).ImageBitmap = null;
			((TextView)findViewById(R.id.thumbnail_small_uri)).Text = "";
			((ImageView)findViewById(R.id.thumbnail_medium)).ImageBitmap = null;
			((TextView)findViewById(R.id.thumbnail_medium_uri)).Text = "";
			((ImageView)findViewById(R.id.thumbnail_large)).ImageBitmap = null;
			((TextView)findViewById(R.id.thumbnail_large_uri)).Text = "";
			mDownloadUrl = null;
		}

		/// <summary>
		/// Download the thumbnails for display
		/// </summary>
		/// <param name="uri"> The uri of the bitmap to retrieve from OneDrive </param>
		/// <returns> The image as a bitmap </returns>
		private Bitmap GetBitmap(Uri uri)
		{
			try
			{
				if (uri == null)
				{
					return null;
				}

				URL url = new URL(uri.ToString());
				System.IO.Stream inputStream = url.openConnection().InputStream;
				Bitmap bitmap = BitmapFactory.decodeStream(inputStream);
				inputStream.Close();
				return bitmap;
			}
			catch (ception)
			{
				return null;
			}
		}

		/// <summary>
		/// Creates a task that will update a thumbnail
		/// </summary>
		/// <param name="imageView"> The image view that should be updated </param>
		/// <param name="imageSource"> The uri of the image that should be put on the image view </param>
		/// <returns> The task that will perform this update </returns>
		private AsyncTask<Void, Void, Bitmap> CreateUpdateThumbnail(ImageView imageView, Uri imageSource)
		{
			return new AsyncTaskAnonymousInnerClassHelper(this, imageView, imageSource);
		}

		private class AsyncTaskAnonymousInnerClassHelper : AsyncTask<Void, Void, Bitmap>
		{
			private readonly PickerMain outerInstance;

			private ImageView imageView;
			private Uri imageSource;

			public AsyncTaskAnonymousInnerClassHelper(PickerMain outerInstance, ImageView imageView, Uri imageSource)
			{
				this.outerInstance = outerInstance;
				this.imageView = imageView;
				this.imageSource = imageSource;
			}

			protected internal override Bitmap DoInBackground(params Void[] @params)
			{
				return outerInstance.GetBitmap(imageSource);
			}

			protected internal override void OnPostExecute(Bitmap result)
			{
				imageView.ImageBitmap = result;
			}
		}
	}
}
