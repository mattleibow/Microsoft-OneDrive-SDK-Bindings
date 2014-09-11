using System;
using System.Threading.Tasks;

using Android.App;
using Java.Util;

namespace Microsoft.Live
{
	public partial class LiveAuthClient
	{
		public virtual Task<AuthCompleteResult> InitializeAsync()
		{
			return LiveAuthListenerAsync.PerformOperationAsync(this, listener => Initialize(listener));
		}

		public virtual Task<AuthCompleteResult> InitializeAsync(Java.Lang.Object userState)
		{
			return LiveAuthListenerAsync.PerformOperationAsync(this, listener => Initialize(listener, userState));
		}

		public virtual Task<AuthCompleteResult> InitializeAsync(Java.Lang.IIterable scopes)
		{
			return LiveAuthListenerAsync.PerformOperationAsync(this, listener => Initialize(scopes, listener));
		}

		public virtual Task<AuthCompleteResult> InitializeAsync(Java.Lang.IIterable scopes, Java.Lang.Object userState)
		{
			return LiveAuthListenerAsync.PerformOperationAsync(this, listener => Initialize(scopes, listener, userState));
		}

		public virtual Task<AuthCompleteResult> InitializeAsync(string[] scopes)
		{
			return LiveAuthListenerAsync.PerformOperationAsync(this, listener => Initialize(new ArrayList(scopes), listener));
		}

		public virtual Task<AuthCompleteResult> InitializeAsync(string[] scopes, Java.Lang.Object userState)
		{
			return LiveAuthListenerAsync.PerformOperationAsync(this, listener => Initialize(new ArrayList(scopes), listener, userState));
		}


		//public virtual void Login(Activity activity, Java.Lang.IIterable scopes, Action<object, AuthCompleteEventArgs> onAuthComplete, Action<object, AuthErrorEventArgs> onAuthError)
		//{
		//	ILiveAuthListenerImplementor listener = new ILiveAuthListenerImplementor(this);
		//	if (onAuthComplete != null)
		//		listener.OnAuthCompleteHandler += new EventHandler<AuthCompleteEventArgs>(onAuthComplete);
		//	if (onAuthError != null)
		//		listener.OnAuthErrorHandler += new EventHandler<AuthErrorEventArgs>(onAuthError);
		//	Login(activity, scopes, listener);
		//}

		public virtual Task<AuthCompleteResult> LoginAsync(Activity activity, Java.Lang.IIterable scopes)
		{
			return LiveAuthListenerAsync.PerformOperationAsync(this, listener => Login(activity, scopes, listener));
		}

		public virtual Task<AuthCompleteResult> LoginAsync(Activity activity, Java.Lang.IIterable scopes, Java.Lang.Object userState)
		{
			return LiveAuthListenerAsync.PerformOperationAsync(this, listener => Login(activity, scopes, listener, userState));
		}

		public virtual Task<AuthCompleteResult> LoginAsync(Activity activity, string[] scopes)
		{
			return LiveAuthListenerAsync.PerformOperationAsync(this, listener => Login(activity, new ArrayList(scopes), listener));
		}

		public virtual Task<AuthCompleteResult> LoginAsync(Activity activity, string[] scopes, Java.Lang.Object userState)
		{
			return LiveAuthListenerAsync.PerformOperationAsync(this, listener => Login(activity, new ArrayList(scopes), listener, userState));
		}


		public virtual Task<AuthCompleteResult> LogoutAsync()
		{
			return LiveAuthListenerAsync.PerformOperationAsync(this, listener => Logout(listener));
		}

		public virtual Task<AuthCompleteResult> LogoutAsync(Java.Lang.Object userState)
		{
			return LiveAuthListenerAsync.PerformOperationAsync(this, listener => Logout(listener, userState));
		}
	}
}