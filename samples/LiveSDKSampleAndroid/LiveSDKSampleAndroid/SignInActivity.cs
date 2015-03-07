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
using Android.Content.PM;
using Android.Net;
using Android.OS;
using Android.Views;
using Android.Widget;
using Java.Util;
using Microsoft.Live;
using Android.Support.V7.App;

namespace LiveSDKSampleAndroid
{
	[Activity(Label = "Sign In", Icon = "@drawable/ic_launcher", Theme = "@style/Theme.AppCompat.Light.DarkActionBar", MainLauncher = true, ConfigurationChanges = ConfigChanges.Orientation)]
	public class SignInActivity : ActionBarActivity
	{
		private LiveAuthClient mAuthClient;
		private ProgressDialog mInitializeDialog;
		private Button mSignInButton;
		private TextView mBeginTextView;
		private Button mNeedIdButton;
		private TextView mBeginTextViewNeedId;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			SetContentView(Resource.Layout.signin);

			mAuthClient = new LiveAuthClient(SampleApplication.Current, Config.ClientId);
			SampleApplication.Current.AuthClient = mAuthClient;
			mInitializeDialog = ProgressDialog.Show(this, "", "Initializing. Please wait...", true);

			mBeginTextView = FindViewById<TextView>(Resource.Id.beginTextView);
			mSignInButton = FindViewById<Button>(Resource.Id.signInButton);

			mBeginTextViewNeedId = FindViewById<TextView>(Resource.Id.beginTextViewNeedId);
			mNeedIdButton = FindViewById<Button>(Resource.Id.needIdButton);

			// Check to see if the CLIENT_ID has been changed. 
			if (string.IsNullOrWhiteSpace(Config.ClientId))
			{
				mNeedIdButton.Visibility = ViewStates.Visible;
				mBeginTextViewNeedId.Visibility = ViewStates.Visible;
				mNeedIdButton.Click += delegate
				{
					Intent intent = new Intent(Intent.ActionView);
					intent.SetData(Uri.Parse(BaseContext.GetString(Resource.String.AndroidSignInHelpLink)));
					StartActivity(intent);
				};
			}

			mSignInButton.Click += async delegate
			{
				try
				{
					AuthCompleteResult result = await mAuthClient.LoginAsync(this, new ArrayList(Config.RequiredScopes));
					if (result.Status == LiveStatus.Connected)
					{
						LaunchMainActivity(result.Session);
					}
					else
					{
						ShowToast("Login did not connect. Status is " + result.Status + ".");
					}
				}
				catch (AuthErrorException ex)
				{
					ShowToast(ex.Message);
				}
			};
		}

		protected async override void OnStart()
		{
			base.OnStart();

			try
			{
				var result = await mAuthClient.InitializeAsync((Java.Lang.IIterable)new ArrayList(Config.RequiredScopes));

				mInitializeDialog.Dismiss();
				if (result.Status == LiveStatus.Connected)
				{
					LaunchMainActivity(result.Session);
				}
				else
				{
					ShowSignIn();
				}
			}
			catch (AuthErrorException ex)
			{
				mInitializeDialog.Dismiss();
				ShowSignIn();
				ShowToast(ex.Message);
			}
		}

		private void LaunchMainActivity(LiveConnectSession session)
		{
			SampleApplication.Current.Session = session;
			SampleApplication.Current.ConnectClient = new LiveConnectClient(session);
			StartActivity(new Intent(ApplicationContext, typeof(MainActivity)));
		}

		private void ShowToast(string message)
		{
			Toast.MakeText(this, message, ToastLength.Long).Show();
		}

		private void ShowSignIn()
		{
			mSignInButton.Visibility = ViewStates.Visible;
			mBeginTextView.Visibility = ViewStates.Visible;
		}
	}
}
