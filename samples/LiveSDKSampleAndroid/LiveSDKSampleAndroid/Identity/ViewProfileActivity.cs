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

using System.Threading.Tasks;
using Android.App;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Views;
using Android.Widget;
using LiveSDKSampleAndroid.Utilities;
using Microsoft.Live;
using Org.Json;

namespace LiveSDKSampleAndroid.Identity
{
	[Activity]
	public class ViewProfileActivity : Activity
	{
		private TextView mNameTextView;

		protected async override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			SetContentView(Resource.Layout.view_profile);

			mNameTextView = FindViewById<TextView>(Resource.Id.nameTextView);

			LiveAuthClient authClient = SampleApplication.Current.AuthClient;
			LiveConnectClient connectClient = SampleApplication.Current.ConnectClient;

			FindViewById(Resource.Id.signOutButton).Click += async delegate 
			{
				try
				{
					await authClient.LogoutAsync();
					SampleApplication.Current.Session = null;
					SampleApplication.Current.ConnectClient = null;
					Finish();
				}
				catch (OperationErrorException ex)
				{
					ShowToast(ex.Message);
				}
			};

			try
			{
				OperationCompleteResult getResult = await connectClient.GetAsync("me");
				JSONObject result = getResult.Operation.Result;

				if (result.Has(JsonKeys.Error))
				{
					JSONObject error = result.OptJSONObject(JsonKeys.Error);
					string code = error.OptString(JsonKeys.Code);
					string message = error.OptString(JsonKeys.Message);

					ShowToast(code + ": " + message);
				}
				else
				{
					User user = new User(result);
					mNameTextView.Text = "Hello, " + user.Name + "!";
				}
			}
			catch (OperationErrorException ex)
			{
				ShowToast(ex.Message);
			}

			try
			{
				OperationCompleteResult getResult = await connectClient.GetAsync("me/picture");
				JSONObject result = getResult.Operation.Result;

				if (result.Has(JsonKeys.Error))
				{
					JSONObject error = result.OptJSONObject(JsonKeys.Error);
					string code = error.OptString(JsonKeys.Code);
					string message = error.OptString(JsonKeys.Message);
					ShowToast(code + ": " + message);
					return;
				}

				string location = result.OptString(JsonKeys.Location);

				DownloadOperationCompleteResult downloadResult = await connectClient.DownloadAsync(location);
				BitmapDrawable bitmapDrawable = await Task.Run(() => new BitmapDrawable(Resources, downloadResult.Operation.Stream));
				mNameTextView.SetCompoundDrawablesWithIntrinsicBounds(bitmapDrawable, null, null, null);
			}
			catch (OperationErrorException ex)
			{
				ShowToast(ex.Message);
			}
		}

		public override bool OnKeyDown(Keycode keyCode, KeyEvent @event)
		{
			// Since this activity is part of a TabView we want to send
			// the back button to the TabView activity.
			if (keyCode == Keycode.Back)
			{
				return false;
			}
			else
			{
				return base.OnKeyDown(keyCode, @event);
			}
		}

		private void ShowToast(string message)
		{
			Toast.MakeText(this, message, ToastLength.Long).Show();
		}
	}
}
