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
	public class ViewProfileFragment : Fragment, ISampleFragment
	{
		private TextView mNameTextView;
		private LiveAuthClient authClient;
		private LiveConnectClient connectClient;

		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			authClient = SampleApplication.Current.AuthClient;
			connectClient = SampleApplication.Current.ConnectClient;

			View rootView = inflater.Inflate (Resource.Layout.view_contacts, container, false);

			mNameTextView = rootView.FindViewById<TextView> (Resource.Id.nameTextView);

			rootView.FindViewById(Resource.Id.signOutButton).Click += async delegate 
			{
				try
				{
					await authClient.LogoutAsync();
					SampleApplication.Current.Session = null;
					SampleApplication.Current.ConnectClient = null;
				}
				catch (OperationErrorException ex)
				{
					Utils.ShowToast(Activity, ex.Message);
				}
			};

			return rootView;
		}

		public async void OnSelected()
		{
			try
			{
				OperationCompleteResult getResult = await connectClient.GetAsync("me");
				JSONObject result = getResult.Operation.Result;

				if (result.Has(JsonKeys.Error))
				{
					JSONObject error = result.OptJSONObject(JsonKeys.Error);
					string code = error.OptString(JsonKeys.Code);
					string message = error.OptString(JsonKeys.Message);

					Utils.ShowToast(Activity, code + ": " + message);
				}
				else
				{
					User user = new User(result);
					mNameTextView.Text = "Hello, " + user.Name + "!";
				}
			}
			catch (OperationErrorException ex)
			{
				Utils.ShowToast(Activity, ex.Message);
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
					Utils.ShowToast(Activity, code + ": " + message);
					return;
				}

				string location = result.OptString(JsonKeys.Location);

				DownloadOperationCompleteResult downloadResult = await connectClient.DownloadAsync(location);
				BitmapDrawable bitmapDrawable = await Task.Run(() => new BitmapDrawable(Resources, downloadResult.Operation.Stream));
				mNameTextView.SetCompoundDrawablesWithIntrinsicBounds(bitmapDrawable, null, null, null);
			}
			catch (OperationErrorException ex)
			{
				Utils.ShowToast(Activity, ex.Message);
			}
		}
	}
}
