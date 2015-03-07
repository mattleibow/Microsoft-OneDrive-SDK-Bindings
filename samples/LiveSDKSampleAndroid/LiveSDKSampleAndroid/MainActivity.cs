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

using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;

using Microsoft.Live;

using LiveSDKSampleAndroid.Hotmail;
using LiveSDKSampleAndroid.Identity;
using LiveSDKSampleAndroid.SkyDrive;

namespace LiveSDKSampleAndroid
{
	[Activity(Label = "Main", Icon = "@drawable/ic_launcher", Theme = "@style/Theme.AppCompat.Light.DarkActionBar")]
	public class MainActivity : TabActivity
	{
		private const int DialogLogoutId = 0;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			SetContentView(Resource.Layout.main);

			TabHost tabHost = TabHost;
			TabHost.TabSpec spec;
			Intent intent;

			intent = new Intent(this, typeof(ViewProfileActivity));
			spec = tabHost.NewTabSpec("profile").SetIndicator("Profile").SetContent(intent);
			tabHost.AddTab(spec);

			intent = new Intent(this, typeof(ContactsActivity));
			spec = tabHost.NewTabSpec("contacts").SetIndicator("Contacts").SetContent(intent);
			tabHost.AddTab(spec);

			intent = new Intent(this, typeof(SkyDriveActivity));
			spec = tabHost.NewTabSpec("skydrive").SetIndicator("SkyDrive").SetContent(intent);
			tabHost.AddTab(spec);

			intent = new Intent(this, typeof(ExplorerActivity));
			spec = tabHost.NewTabSpec("explorer").SetIndicator("Explorer").SetContent(intent);
			tabHost.AddTab(spec);

			tabHost.CurrentTab = 0;
		}

		public override bool OnKeyDown(Keycode keyCode, KeyEvent @event)
		{
			if (keyCode == Keycode.Back)
			{
				ShowDialog(DialogLogoutId);
				return true;
			}
			else
			{
				return base.OnKeyDown(keyCode, @event);
			}
		}

		protected override Dialog OnCreateDialog(int id)
		{
			Dialog dialog = null;
			if (id == DialogLogoutId)
			{
				AlertDialog.Builder builder = new AlertDialog.Builder(this)
					.SetTitle("Logout")
					.SetMessage("The Live Connect Session will be cleared.")
					.SetPositiveButton("OK", async delegate
					{
						try
						{
							await SampleApplication.Current.AuthClient.LogoutAsync();

							SampleApplication.Current.Session = null;
							SampleApplication.Current.ConnectClient = null;

							Finish();
						}
						catch (AuthErrorException ex)
						{
							ShowToast(ex.Message);
						}
					})
					.SetNegativeButton("Cancel", delegate
					{
						dialog.Cancel();
					});
				dialog = builder.Create();
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

		private void ShowToast(string message)
		{
			Toast.MakeText(this, message, ToastLength.Long).Show();
		}
	}
}
