// ------------------------------------------------------------------------------
// Copyright (c) 2014 Microsoft Corporation
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
//  of this software and associated documentation files (the "Software"), to deal
//  in the Software without restriction, including without limitation the rights
//  to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//  copies of the Software, and to permit persons to whom the Software is
//  furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
//  all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//  OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
//  THE SOFTWARE.
// ------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Media;
using Android.OS;
using Android.Views;
using Android.Widget;
using Java.IO;
using Java.Lang;
using LiveSDKSampleAndroid.Utilities;
using Microsoft.Live;
using Org.Json;
using Environment = Android.OS.Environment;
using Math = System.Math;

namespace LiveSDKSampleAndroid.SkyDrive
{
	[Activity]
	public class SkyDriveActivity : ListActivity
	{
		private const int DialogDownloadId = 0;
		private const string HomeFolder = "me/skydrive";

		private SkyDriveListAdapter photoAdapter;
		private string currentFolderId;
		private Stack<string> prevFolderIds;

		protected override async void OnActivityResult(int requestCode, Result resultCode, Intent data)
		{
			if (requestCode == FilePicker.PickFileRequest && resultCode == Result.Ok)
			{
				string filePath = data.GetStringExtra(FilePicker.ExtraFilePath);

				if (string.IsNullOrEmpty(filePath))
				{
					ShowToast("No file was choosen.");
					return;
				}

				File file = new File(filePath);

				ProgressDialog uploadProgressDialog = new ProgressDialog(this);
				uploadProgressDialog.SetProgressStyle(ProgressDialogStyle.Horizontal);
				uploadProgressDialog.SetMessage("Uploading...");
				uploadProgressDialog.SetCancelable(true);
				uploadProgressDialog.Show();

				var cts = new CancellationTokenSource();
				uploadProgressDialog.CancelEvent += delegate
				{
					cts.Cancel();
				};

				try
				{
					UploadOperationCompleteResult uploadResult = await SampleApplication.Current.ConnectClient.UploadAsync(
						currentFolderId, file.Name, file, cts.Token,
						new Progress<UploadOperationProgress>(
							progress =>
							{
								int percentCompleted = ComputePercentCompleted(progress.TotalBytes, progress.RemainingBytes);
								uploadProgressDialog.Progress = percentCompleted;
							}));
					uploadProgressDialog.Dismiss();

					JSONObject result = uploadResult.Operation.Result;
					if (result.Has(JsonKeys.Error))
					{
						JSONObject error = result.OptJSONObject(JsonKeys.Error);
						string message = error.OptString(JsonKeys.Message);
						string code = error.OptString(JsonKeys.Code);
						ShowToast(code + ": " + message);
						return;
					}

					LoadFolder(currentFolderId);
				}
				catch (OperationErrorException ex)
				{
					uploadProgressDialog.Dismiss();
					ShowToast(ex.Message);
				}
				catch (OperationCanceledException)
				{
					uploadProgressDialog.Dismiss();
					ShowToast("Downloaded canceled.");
				}
			}
		}

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			SetContentView(Resource.Layout.skydrive);

			prevFolderIds = new Stack<string>();

			ListView lv = ListView;
			lv.TextFilterEnabled = true;
			lv.ItemClick += (sender, args) =>
			{
				SkyDriveObject skyDriveObj = (SkyDriveObject) args.Parent.GetItemAtPosition(args.Position).FromJavaObject();

				var video = skyDriveObj as SkyDriveVideo;
				if (video != null)
				{
					PlayVideoDialog dialog = new PlayVideoDialog(this, video);
					dialog.Show();
				}

				var file = skyDriveObj as SkyDriveFile;
				if (file != null)
				{
					Bundle b = new Bundle();
					b.PutString(JsonKeys.Id, file.Id);
					b.PutString(JsonKeys.Name, file.Name);
					ShowDialog(DialogDownloadId, b);
				}

				var folder = skyDriveObj as SkyDriveFolder;
				if (folder != null)
				{
					prevFolderIds.Push(currentFolderId);
					LoadFolder(folder.Id);
				}

				var photo = skyDriveObj as SkyDrivePhoto;
				if (photo != null)
				{
					ViewPhotoDialog dialog = new ViewPhotoDialog(this, photo);
					dialog.Show();
				}

				var album = skyDriveObj as SkyDriveAlbum;
				if (album != null)
				{
					prevFolderIds.Push(currentFolderId);
					LoadFolder(album.Id);
				}

				var audio = skyDriveObj as SkyDriveAudio;
				if (audio != null)
				{
					PlayAudioDialog audioDialog = new PlayAudioDialog(this, audio);
					audioDialog.Show();
				}
			};

			LinearLayout layout = new LinearLayout(this);
			Button newFolderButton = new Button(this);
			newFolderButton.Text = "New Folder";
			newFolderButton.Click += delegate
			{
				NewFolderDialog dialog = new NewFolderDialog(this);
				dialog.Show();
			};

			layout.AddView(newFolderButton);

			Button uploadFileButton = new Button(this);
			uploadFileButton.Text = "Upload File";
			uploadFileButton.Click += delegate
			{
				Intent intent = new Intent(ApplicationContext, typeof (FilePicker));
				StartActivityForResult(intent, FilePicker.PickFileRequest);
			};

			layout.AddView(uploadFileButton);
			lv.AddHeaderView(layout);

			photoAdapter = new SkyDriveListAdapter(this);
			ListAdapter = photoAdapter;
		}

		public override bool OnKeyDown(Keycode keyCode, KeyEvent @event)
		{
			if (keyCode == Keycode.Back)
			{
				// if prev folders is empty, send the back button to the TabView activity.
				if (prevFolderIds.Count == 0)
				{
					return false;
				}

				LoadFolder(prevFolderIds.Pop());
				return true;
			}
			else
			{
				return base.OnKeyDown(keyCode, @event);
			}
		}

		protected override Dialog OnCreateDialog(int id, Bundle bundle)
		{
			Dialog dialog = null;
			switch (id)
			{
				case DialogDownloadId:
				{
					AlertDialog.Builder builder = new AlertDialog.Builder(this);
					builder.SetTitle("Download")
						.SetMessage("This file will be downloaded to the SD Card.")
						.SetPositiveButton("OK", async delegate
						{
							ProgressDialog downloadProgressDialog = new ProgressDialog(this);
							downloadProgressDialog.SetProgressStyle(ProgressDialogStyle.Horizontal);
							downloadProgressDialog.SetMessage("Downloading...");
							downloadProgressDialog.SetCancelable(true);
							downloadProgressDialog.Show();

							string fileId = bundle.GetString(JsonKeys.Id);
							string name = bundle.GetString(JsonKeys.Name);

							File file = new File(Environment.ExternalStorageDirectory, name);


							var cts = new CancellationTokenSource();
							downloadProgressDialog.CancelEvent += delegate
							{
								cts.Cancel();
							};
							try
							{
								await SampleApplication.Current.ConnectClient.DownloadAsync(
									fileId + "/content", file, cts.Token, 
									new Progress<DownloadOperationProgress>(
									progress =>
									{
										int percentCompleted = ComputePercentCompleted(progress.TotalBytes, progress.RemainingBytes);
										downloadProgressDialog.Progress = percentCompleted;
									}));
								downloadProgressDialog.Dismiss();
								ShowToast("File downloaded.");
							}
							catch (OperationErrorException ex)
							{
								downloadProgressDialog.Dismiss();
								ShowToast(ex.Message);
							}
							catch (OperationCanceledException)
							{
								downloadProgressDialog.Dismiss();
								ShowToast("Downloaded canceled.");
							}
						})
						.SetNegativeButton("Cancel", delegate
						{
							dialog.Cancel();
						});

					dialog = builder.Create();
					break;
				}
			}

			if (dialog != null)
			{
				dialog.DismissEvent += delegate
				{
					RemoveDialog(id);
				};
			}

			return dialog;
		}

		protected override void OnStart()
		{
			base.OnStart();
			LoadFolder(HomeFolder);
		}

		private async void LoadFolder(string folderId)
		{
			currentFolderId = folderId;

			ProgressDialog progressDialog = ProgressDialog.Show(this, "", "Loading. Please wait...", true);

			try
			{
				OperationCompleteResult getResult = await SampleApplication.Current.ConnectClient.GetAsync(folderId + "/files");
				progressDialog.Dismiss();

				JSONObject result = getResult.Operation.Result;
				if (result.Has(JsonKeys.Error))
				{
					JSONObject error = result.OptJSONObject(JsonKeys.Error);
					string message = error.OptString(JsonKeys.Message);
					string code = error.OptString(JsonKeys.Code);
					ShowToast(code + ": " + message);
					return;
				}

				List<SkyDriveObject> skyDriveObjs = photoAdapter.SkyDriveObjs;
				skyDriveObjs.Clear();

				JSONArray data = result.OptJSONArray(JsonKeys.Data);
				for (int i = 0; i < data.Length(); i++)
				{
					SkyDriveObject skyDriveObj = SkyDriveObject.Create(data.OptJSONObject(i));
					if (skyDriveObj != null)
					{
						skyDriveObjs.Add(skyDriveObj);
					}
				}

				photoAdapter.NotifyDataSetChanged();
			}
			catch (OperationErrorException ex)
			{
				progressDialog.Dismiss();
				ShowToast(ex.Message);
			}
		}

		private void ShowToast(string message)
		{
			Toast.MakeText(this, message, ToastLength.Long).Show();
		}

		private int ComputePercentCompleted(int totalBytes, int bytesRemaining)
		{
			return (int) (((float) (totalBytes - bytesRemaining))/totalBytes*100);
		}

		private ProgressDialog ShowProgressDialog(string title, string message, bool indeterminate)
		{
			return ProgressDialog.Show(this, title, message, indeterminate);
		}


		private class NewFolderDialog : Dialog
		{
			private readonly SkyDriveActivity activity;

			public NewFolderDialog(SkyDriveActivity activity)
				: base(activity)
			{
				this.activity = activity;
			}

			protected override void OnCreate(Bundle savedInstanceState)
			{
				base.OnCreate(savedInstanceState);
				SetContentView(Resource.Layout.create_folder);

				SetTitle("New Folder");

				EditText name = FindViewById<EditText>(Resource.Id.nameEditText);
				EditText description = FindViewById<EditText>(Resource.Id.descriptionEditText);

				FindViewById(Resource.Id.saveButton).Click += async delegate
				{
					Dictionary<string, string> folder = new Dictionary<string, string>();
					folder[JsonKeys.Name] = name.Text;
					folder[JsonKeys.Description] = description.Text;

					ProgressDialog progressDialog = activity.ShowProgressDialog("", "Saving. Please wait...", true);
					progressDialog.Show();

					try
					{
						var postResult = await SampleApplication.Current.ConnectClient.PostAsync(activity.currentFolderId, new JSONObject(folder));
						progressDialog.Dismiss();

						JSONObject result = postResult.Operation.Result;
						if (result.Has(JsonKeys.Error))
						{
							JSONObject error = result.OptJSONObject(JsonKeys.Error);
							string message = error.OptString(JsonKeys.Message);
							string code = error.OptString(JsonKeys.Code);
							activity.ShowToast(code + ":" + message);
						}
						else
						{
							Dismiss();
							activity.LoadFolder(activity.currentFolderId);
						}
					}
					catch (OperationErrorException ex)
					{
						progressDialog.Dismiss();
						activity.ShowToast(ex.Message);
					}
				};

				FindViewById(Resource.Id.cancelButton).Click += delegate
				{
					Dismiss();
				};
			}
		}

		private class PlayAudioDialog : Dialog
		{
			private readonly SkyDriveActivity activity;
			private readonly SkyDriveAudio audio;
			private MediaPlayer player;
			private TextView playerStatus;

			public PlayAudioDialog(SkyDriveActivity activity, SkyDriveAudio audio)
				: base(activity)
			{
				this.activity = activity;
				this.audio = audio;

				player = new MediaPlayer();
			}

			protected override void OnCreate(Bundle savedInstanceState)
			{
				base.OnCreate(savedInstanceState);
				SetTitle(audio.Name);

				playerStatus = new TextView(Context);
				playerStatus.Text = "Buffering...";
				AddContentView(playerStatus,
					new ViewGroup.LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent));

				player.Prepared += delegate
				{
					playerStatus.Text = "Playing...";
					player.Start();
				};

				player.Completion += delegate
				{
					playerStatus.Text = "Finished playing.";
				};

				try
				{
					player.SetDataSource(audio.Source);
					player.SetAudioStreamType(Stream.Music);
					player.PrepareAsync();
				}
				catch (System.ArgumentException e)
				{
					activity.ShowToast(e.Message);
				}
				catch (IllegalStateException e)
				{
					activity.ShowToast(e.Message);
				}
				catch (IOException e)
				{
					activity.ShowToast(e.Message);
				}
			}

			protected override void OnStop()
			{
				base.OnStop();
				player.Stop();
				player.Release();
				player = null;
			}
		}

		/// <summary>
		/// Supported media formats can be found
		/// <a href="http://developer.android.com/guide/appendix/media-formats.html">here</a>
		/// </summary>
		private class PlayVideoDialog : Dialog
		{
			private readonly SkyDriveActivity activity;
			private readonly SkyDriveVideo video;
			private VideoView videoView;

			public PlayVideoDialog(SkyDriveActivity activity, SkyDriveVideo video)
				: base(activity)
			{
				this.activity = activity;
				this.video = video;
			}

			protected override void OnCreate(Bundle savedInstanceState)
			{
				base.OnCreate(savedInstanceState);
				SetTitle(video.Name);

				videoView = new VideoView(Context);
				videoView.SetMediaController(new MediaController(Context));
				videoView.SetVideoURI(Android.Net.Uri.Parse(video.Source));
				AddContentView(videoView,
					new ViewGroup.LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent));
			}

			protected override void OnStart()
			{
				base.OnStart();
				videoView.Start();
			}
		}

		private class SkyDriveListAdapter : BaseAdapter
		{
			private readonly SkyDriveActivity activity;

			internal readonly LayoutInflater MInflater;
			internal readonly List<SkyDriveObject> MSkyDriveObjs;

			public SkyDriveListAdapter(SkyDriveActivity activity)
			{
				this.activity = activity;
				MInflater = (LayoutInflater) activity.GetSystemService(LayoutInflaterService);
				MSkyDriveObjs = new List<SkyDriveObject>();
			}

			/// <returns> The underlying array of the class. If changes are made to this object and you
			/// want them to be seen, call <seealso cref="#notifyDataSetChanged()"/>. </returns>
			public virtual List<SkyDriveObject> SkyDriveObjs
			{
				get { return MSkyDriveObjs; }
			}

			public override int Count
			{
				get { return MSkyDriveObjs.Count; }
			}

			public override Java.Lang.Object GetItem(int position)
			{
				return MSkyDriveObjs[position].AsJavaObject();
			}

			public override long GetItemId(int position)
			{
				return position;
			}

			// Note: This implementation of the ListAdapter.getView(...) forces a download of thumb-nails when retrieving
			// views, this is not a good solution in regards to CPU time and network band-width.
			public override View GetView(int position, View convertView, ViewGroup parent)
			{
				SkyDriveObject skyDriveObj = (SkyDriveObject) GetItem(position).FromJavaObject();
				View view = convertView ?? MInflater.Inflate(Resource.Layout.skydrive_list_item, parent, false);
				TextView name = view.FindViewById<TextView>(Resource.Id.nameTextView);
				TextView desc = view.FindViewById<TextView>(Resource.Id.descriptionTextView);
				ImageView imgView = (ImageView) view.FindViewById(Resource.Id.skyDriveItemIcon);

				name.Text = skyDriveObj.Name;
				desc.Text = skyDriveObj.Description ?? "No description.";

				int imgId = 0;
				var video = skyDriveObj as SkyDriveVideo;
				if (video != null)
				{
					imgId = Resource.Drawable.video_x_generic;
				}

				var file = skyDriveObj as SkyDriveFile;
				if (file != null)
				{
					imgId = Resource.Drawable.text_x_preview;
				}

				var folder = skyDriveObj as SkyDriveFolder;
				if (folder != null)
				{
					imgId = Resource.Drawable.folder;
				}

				var photo = skyDriveObj as SkyDrivePhoto;
				if (photo != null)
				{
					imgId = Resource.Drawable.image_x_generic;
					LoadThumbnailImage(photo, imgView);
				}

				var album = skyDriveObj as SkyDriveAlbum;
				if (album != null)
				{
					imgId = Resource.Drawable.folder_image;
				}

				var audio = skyDriveObj as SkyDriveAudio;
				if (audio != null)
				{
					imgId = Resource.Drawable.audio_x_generic;
				}

				imgView.SetImageResource(imgId);

				return view;
			}

			private async Task LoadThumbnailImage(SkyDrivePhoto photo, ImageView imgView)
			{
				// Try to find a smaller/thumbnail and use that source
				string thumbnailSource = null;
				string smallSource = null;
				foreach (Image image in photo.Images)
				{
					if (image.Type.Equals("small"))
						smallSource = image.Source;
					else if (image.Type.Equals("thumbnail"))
						thumbnailSource = image.Source;
				}
				string source = thumbnailSource ?? smallSource;

				// if we do not have a thumbnail or small image, just leave.
				if (source != null)
				{
					await Task.Run(async delegate
					{
						try
						{
							// Download the thumbnail image
							DownloadOperationCompleteResult downloadResult = await SampleApplication.Current.ConnectClient.DownloadAsync(source);

							// Make sure we don't burn up memory for all of these thumb nails that are transient
							Bitmap result = await BitmapFactory.DecodeStreamAsync(
								downloadResult.Operation.Stream,
								null,
								new BitmapFactory.Options {InPurgeable = true});

							imgView.SetImageBitmap(result);
						}
						catch (OperationErrorException ex)
						{
							activity.ShowToast(ex.Message);
						}
					});
				}
			}
		}

		private class ViewPhotoDialog : Dialog
		{
			private readonly SkyDriveActivity activity;
			private readonly SkyDrivePhoto photo;

			public ViewPhotoDialog(SkyDriveActivity activity, SkyDrivePhoto photo)
				: base(activity)
			{
				this.activity = activity;
				this.photo = photo;
			}

			protected override async void OnCreate(Bundle savedInstanceState)
			{
				base.OnCreate(savedInstanceState);
				SetTitle(photo.Name);

				ImageView imgView = new ImageView(Context);
				ViewGroup.LayoutParams layoutParams = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent);
				AddContentView(imgView, layoutParams);

				try
				{
					var res = await SampleApplication.Current.ConnectClient.DownloadAsync(photo.Source);
					Bitmap bitmap = null;
					await Task.Run(() => bitmap = ExtractScaledBitmap(res.Operation.Stream));
					imgView.SetImageBitmap(bitmap);
				}
				catch (OperationErrorException ex)
				{
					activity.ShowToast(ex.Message);
				}
			}

			/// <summary>
			/// Extract a photo from SkyDrive and creates a scaled bitmap according to the device resolution, this is needed to
			/// prevent memory over-allocation that can cause some devices to crash when opening high-resolution pictures
			/// 
			/// Note: this method should not be used for downloading photos, only for displaying photos on-screen
			/// </summary>
			/// <param name="photo"> The source photo to download </param>
			/// <param name="imageStream"> The stream that contains the photo </param>
			/// <returns> Scaled bitmap representation of the photo </returns>
			/// <seealso cref= http://stackoverflow.com/questions/477572/strange-out-of-memory-issue-while-loading-an-image-to-a-bitmap-object/823966#823966 </seealso>
			private Bitmap ExtractScaledBitmap(System.IO.Stream imageStream)
			{
				Display display = activity.WindowManager.DefaultDisplay;
				int imageMaxSize = Math.Max(display.Width, display.Height);

				int scale = 1;
				if (photo.Height > imageMaxSize || photo.Width > imageMaxSize)
				{
					int power = (int) Math.Ceiling(Math.Log(imageMaxSize/(double) Math.Max(photo.Height, photo.Width))/Math.Log(0.5));
					scale = (int) Math.Pow(2, power);
				}

				BitmapFactory.Options options = new BitmapFactory.Options {InPurgeable = true, InSampleSize = scale};
				return BitmapFactory.DecodeStream(imageStream, null, options);
			}
		}
	}
}
