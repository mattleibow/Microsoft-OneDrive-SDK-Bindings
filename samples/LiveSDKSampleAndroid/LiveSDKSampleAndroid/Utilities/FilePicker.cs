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

using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using Java.IO;
using Debug = System.Diagnostics.Debug;

namespace LiveSDKSampleAndroid.Utilities
{
	[Activity]
	public class FilePicker : ListActivity
	{
		public const int PickFileRequest = 0;
		public const string ExtraFilePath = "filePath";

		private File currentFolder;
		private Stack<File> prevFolders;
		private FilePickerListAdapter adapter;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			SetContentView(Resource.Layout.file_picker);

			prevFolders = new Stack<File>();

			ListView lv = ListView;
			lv.TextFilterEnabled = true;
			lv.ItemClick += (sender, args) =>
			{
				File file = (File) args.Parent.GetItemAtPosition(args.Position);

				if (file.IsDirectory)
				{
					prevFolders.Push(currentFolder);
					LoadFolder(file);
				}
				else
				{
					Intent data = new Intent();
					data.PutExtra(ExtraFilePath, file.Path);
					SetResult(Result.Ok, data);
					Finish();
				}
			};

			adapter = new FilePickerListAdapter(this);
			ListAdapter = adapter;
		}

		public override bool OnKeyDown(Keycode keyCode, KeyEvent e)
		{
			if (keyCode == Keycode.Back && prevFolders.Count > 0)
			{
				File folder = prevFolders.Pop();
				LoadFolder(folder);
				return true;
			}
			else
			{
				return base.OnKeyDown(keyCode, e);
			}
		}

		protected override void OnStart()
		{
			base.OnStart();

			LoadFolder(Environment.ExternalStorageDirectory);
		}

		private async void LoadFolder(File folder)
		{
			Debug.Assert(folder.IsDirectory);
			Title = folder.Name;

			currentFolder = folder;

			ProgressDialog progressDialog = ProgressDialog.Show(this, "", "Loading. Please wait...", true);
			List<File> adapterFiles = adapter.Files;
			adapterFiles.Clear();
			adapterFiles.AddRange(await folder.ListFilesAsync());
			adapter.NotifyDataSetChanged();

			progressDialog.Dismiss();
		}

		private class FilePickerListAdapter : BaseAdapter
		{
			private readonly LayoutInflater inflater;
			private readonly List<File> files;

			public FilePickerListAdapter(FilePicker picker)
			{
				inflater = (LayoutInflater)picker.GetSystemService(LayoutInflaterService);
				files = new List<File>();
			}

			public List<File> Files
			{
				get { return files; }
			}

			public override int Count
			{
				get { return files.Count; }
			}

			public override Java.Lang.Object GetItem(int position)
			{
				return files[position];
			}

			public override long GetItemId(int position)
			{
				return position;
			}

			public override View GetView(int position, View convertView, ViewGroup parent)
			{
				View v = convertView ?? inflater.Inflate(Resource.Layout.file_picker_list_item, parent, false);
				TextView name = v.FindViewById<TextView>(Resource.Id.nameTextView);
				TextView type = v.FindViewById<TextView>(Resource.Id.typeTextView);

				File file = (File) GetItem(position);
				name.Text = file.Name;
				type.Text = file.IsDirectory ? "Folder" : "File";

				return v;
			}
		}
	}
}
