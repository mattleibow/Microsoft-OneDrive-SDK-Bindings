using System;
using System.Threading.Tasks;

using Android.App;

namespace Microsoft.Live
{
	internal static class LiveAuthListenerAsync
	{
		internal static Task<AuthCompleteResult> PerformOperationAsync(object sender, Action<ILiveAuthListener> operation)
		{
			TaskCompletionSource<AuthCompleteResult> tcs = new TaskCompletionSource<AuthCompleteResult>();

			ILiveAuthListenerImplementor listener = new ILiveAuthListenerImplementor(sender);
			listener.OnAuthCompleteHandler += (s, e) => { tcs.TrySetResult(new AuthCompleteResult(e)); };
			listener.OnAuthErrorHandler += (s, e) => { tcs.TrySetException(new AuthErrorException(e)); };

			operation(listener);

			return tcs.Task;
		}
	}

	public class AuthCompleteResult
	{
		public AuthCompleteResult(LiveStatus status, LiveConnectSession session, Java.Lang.Object userState)
		{
			Status = status;
			Session = session;
			UserState = userState;
		}

		public AuthCompleteResult(AuthCompleteEventArgs args)
			: this(args.P0, args.P1, args.P2)
		{
		}

		public LiveStatus Status { get; private set; }

		public LiveConnectSession Session { get; private set; }

		public Java.Lang.Object UserState { get; private set; }
	}

	public class AuthErrorException : Exception
	{
		public AuthErrorException(LiveAuthException exception, Java.Lang.Object userState)
			: base(exception.Message, exception)
		{
			UserState = userState;
		}

		public AuthErrorException(AuthErrorEventArgs args)
			: this(args.P0, args.P1)
		{
		}

		public Java.Lang.Object UserState { get; private set; }
	}

}