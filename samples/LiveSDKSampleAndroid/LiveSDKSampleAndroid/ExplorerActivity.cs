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
using Android.OS;
using Android.Views;
using Android.Widget;
using Microsoft.Live;
using Org.Json;
using Android.Support.V7.App;

namespace LiveSDKSampleAndroid
{
	[Activity(Label = "Explorer", Icon = "@drawable/ic_launcher", Theme = "@style/Theme.AppCompat.Light.DarkActionBar")]
	public class ExplorerActivity : ActionBarActivity
	{
		private static readonly string[] HttpMethods = {"GET", "DELETE", "PUT", "POST"};
		private class HttpMethodIds
		{
			public const int Get = 0;
			public const int Delete = 1;
			public const int Put = 2;
			public const int Post = 3;
		}

		private EditText responseBodyText;
		private EditText pathText;
		private EditText requestBodyText;
		private TextView requestBodyTextView;
		private ProgressDialog progressDialog;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			SetContentView(Resource.Layout.explorer);

			responseBodyText = FindViewById<EditText>(Resource.Id.responseBodyText);
			pathText = FindViewById<EditText>(Resource.Id.pathText);
			requestBodyText = FindViewById<EditText>(Resource.Id.requestBodyText);
			requestBodyTextView = FindViewById<TextView>(Resource.Id.requestBodyTextView);

			ArrayAdapter<string> adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerItem, HttpMethods);
			adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
			Spinner httpMethodSpinner = FindViewById<Spinner>(Resource.Id.httpMethodSpinner);
			httpMethodSpinner.Adapter = adapter;
			httpMethodSpinner.ItemSelected += (sender, args) =>
			{
				switch (args.Position)
				{
					case HttpMethodIds.Get:
					case HttpMethodIds.Delete:
						HideRequestBody();
						break;
					case HttpMethodIds.Post:
					case HttpMethodIds.Put:
						ShowRequestBody();
						break;
					default:
						MakeToast("Unknown HTTP method selected: " + httpMethodSpinner.SelectedItem.ToString());
						break;
				}
			};

			FindViewById(Resource.Id.submitButton).Click += async delegate
			{
				string path = pathText.Text.ToString();
				string bodyString = requestBodyText.Text.ToString();

				if (string.IsNullOrEmpty(path))
				{
					MakeToast("Path must not be empty.");
					return;
				}

				int selectedPosition = httpMethodSpinner.SelectedItemPosition;
				bool httpMethodRequiresBody = selectedPosition == HttpMethodIds.Post || selectedPosition == HttpMethodIds.Put;
				if (httpMethodRequiresBody && string.IsNullOrEmpty(bodyString))
				{
					MakeToast("Request body must not be empty.");
					return;
				}

				OperationCompleteResult result = null;
				progressDialog = ShowProgressDialog("Loading. Please wait...");
				try
				{
					switch (selectedPosition)
					{
						case HttpMethodIds.Get:
							result = await SampleApplication.Current.ConnectClient.GetAsync(path);
							break;
						case HttpMethodIds.Delete:
							result = await SampleApplication.Current.ConnectClient.DeleteAsync(path);
							break;
						case HttpMethodIds.Post:
							result = await SampleApplication.Current.ConnectClient.PostAsync(path, bodyString);
							break;
						case HttpMethodIds.Put:
							result = await SampleApplication.Current.ConnectClient.PutAsync(path, bodyString);
							break;
						default:
							MakeToast(string.Format("Unknown HTTP method selected: {0}", httpMethodSpinner.SelectedItem));
							break;
					}
				}
				catch (OperationErrorException ex)
				{
					DismissProgressDialog();
					MakeToast(ex.Message);
				}
				DismissProgressDialog();
				try
				{
					if (result != null)
					{
						responseBodyText.Text = result.Operation.Result.ToString(2);
						responseBodyText.RequestFocus();
					}
				}
				catch (JSONException e)
				{
					MakeToast(e.Message);
				}
			};
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

		private void ShowRequestBody()
		{
			requestBodyText.Visibility = ViewStates.Visible;
			requestBodyTextView.Visibility = ViewStates.Visible;
		}

		private void HideRequestBody()
		{
			requestBodyText.Visibility = ViewStates.Gone;
			requestBodyTextView.Visibility = ViewStates.Gone;
		}

		private void DismissProgressDialog()
		{
			if (progressDialog != null && progressDialog.IsShowing)
			{
				progressDialog.Dismiss();
			}
		}

		private void MakeToast(string message)
		{
			Toast.MakeText(this, message, ToastLength.Long).Show();
		}

		private ProgressDialog ShowProgressDialog(string message)
		{
			return ProgressDialog.Show(this, "", message);
		}
	}
}