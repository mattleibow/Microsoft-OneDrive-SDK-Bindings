using System;
using System.Threading;
using System.Threading.Tasks;

using Android.App;

namespace Microsoft.Live
{
	internal static class LiveOperationListenerAsync
	{
		internal static Task<OperationCompleteResult> PerformOperationAsync(object sender, Func<ILiveOperationListener, LiveOperation> operation, CancellationToken ct)
		{
			TaskCompletionSource<OperationCompleteResult> tcs = new TaskCompletionSource<OperationCompleteResult>();

			ILiveOperationListenerImplementor listener = new ILiveOperationListenerImplementor(sender);
			listener.OnCompleteHandler += (s, e) => { tcs.TrySetResult(new OperationCompleteResult(e)); };
			listener.OnErrorHandler += (s, e) => { tcs.TrySetException(new OperationErrorException(e)); };

			var liveOperation = operation(listener);

			ct.Register(() =>
			{
				liveOperation.Cancel();
				tcs.TrySetCanceled();
			});

			return tcs.Task;
		}

		internal static Task<DownloadOperationCompleteResult> PerformOperationAsync(object sender, Func<ILiveDownloadOperationListener, LiveDownloadOperation> operation, CancellationToken ct, IProgress<DownloadOperationProgress> progress)
		{
			TaskCompletionSource<DownloadOperationCompleteResult> tcs = new TaskCompletionSource<DownloadOperationCompleteResult>();

			ILiveDownloadOperationListenerImplementor listener = new ILiveDownloadOperationListenerImplementor(sender);
			listener.OnDownloadCompletedHandler += (s, e) => { tcs.TrySetResult(new DownloadOperationCompleteResult(e)); };
			listener.OnDownloadFailedHandler += (s, e) => { tcs.TrySetException(new DownloadOperationErrorException(e)); };
			if (progress != null)
				listener.OnDownloadProgressHandler += (s, e) => { progress.Report(new DownloadOperationProgress(e)); };

			var liveOperation = operation(listener);

			ct.Register(() =>
			{
				liveOperation.Cancel();
				tcs.TrySetCanceled();
			});

			return tcs.Task;
		}

		internal static Task<UploadOperationCompleteResult> PerformOperationAsync(object sender, Func<ILiveUploadOperationListener, LiveOperation> operation, CancellationToken ct, IProgress<UploadOperationProgress> progress)
		{
			TaskCompletionSource<UploadOperationCompleteResult> tcs = new TaskCompletionSource<UploadOperationCompleteResult>();

			ILiveUploadOperationListenerImplementor listener = new ILiveUploadOperationListenerImplementor(sender);
			listener.OnUploadCompletedHandler += (s, e) => { tcs.TrySetResult(new UploadOperationCompleteResult(e)); };
			listener.OnUploadFailedHandler += (s, e) => { tcs.TrySetException(new UploadOperationErrorException(e)); };
			if (progress != null)
				listener.OnUploadProgressHandler += (s, e) => { progress.Report(new UploadOperationProgress(e)); };

			var liveOperation = operation(listener);

			ct.Register(() =>
			{
				liveOperation.Cancel();
				tcs.TrySetCanceled();
			});

			return tcs.Task;
		}
	}


	public class OperationCompleteResult
	{
		public OperationCompleteResult(LiveOperation operation)
		{
			Operation = operation;
		}

		public OperationCompleteResult(CompleteEventArgs args)
			: this(args.P0)
		{
		}

		public LiveOperation Operation { get; private set; }
	}

	public class OperationErrorException : Exception
	{
		public OperationErrorException(LiveOperationException exception, LiveOperation operation)
			: base(exception.Message, exception)
		{
			Operation = operation;
		}

		public OperationErrorException(ErrorEventArgs args)
			: this(args.P0, args.P1)
		{
		}

		public LiveOperation Operation { get; private set; }
	}


	public class DownloadOperationCompleteResult
	{
		public DownloadOperationCompleteResult(LiveDownloadOperation operation)
		{
			Operation = operation;
		}

		public DownloadOperationCompleteResult(DownloadCompletedEventArgs args)
			: this(args.P0)
		{
		}

		public LiveDownloadOperation Operation { get; private set; }
	}

	public class DownloadOperationErrorException : Exception
	{
		public DownloadOperationErrorException(LiveOperationException exception, LiveDownloadOperation operation)
			: base(exception.Message, exception)
		{
			Operation = operation;
		}

		public DownloadOperationErrorException(DownloadFailedEventArgs args)
			: this(args.P0, args.P1)
		{
		}

		public LiveDownloadOperation Operation { get; private set; }
	}
	
	public class DownloadOperationProgress
	{
		public DownloadOperationProgress(int totalBytes, int remainingBytes, LiveDownloadOperation operation)
		{
			TotalBytes = totalBytes;
			RemainingBytes = remainingBytes;
			Operation = operation;
		}

		public DownloadOperationProgress(DownloadProgressEventArgs args)
			: this(args.P0, args.P1, args.P2)
		{
		}

		public int TotalBytes { get; private set; }
		public int RemainingBytes { get; private set; }
		public LiveDownloadOperation Operation { get; private set; }
	}


	public class UploadOperationCompleteResult
	{
		public UploadOperationCompleteResult(LiveOperation operation)
		{
			Operation = operation;
		}

		public UploadOperationCompleteResult(UploadCompletedEventArgs args)
			: this(args.P0)
		{
		}

		public LiveOperation Operation { get; private set; }
	}

	public class UploadOperationErrorException : Exception
	{
		public UploadOperationErrorException(LiveOperationException exception, LiveOperation operation)
			: base(exception.Message, exception)
		{
			Operation = operation;
		}

		public UploadOperationErrorException(UploadFailedEventArgs args)
			: this(args.P0, args.P1)
		{
		}

		public LiveOperation Operation { get; private set; }
	}
	
	public class UploadOperationProgress
	{
		public UploadOperationProgress(int totalBytes, int remainingBytes, LiveOperation operation)
		{
			TotalBytes = totalBytes;
			RemainingBytes = remainingBytes;
			Operation = operation;
		}

		public UploadOperationProgress(UploadProgressEventArgs args)
			: this(args.P0, args.P1, args.P2)
		{
		}

		public int TotalBytes { get; private set; }
		public int RemainingBytes { get; private set; }
		public LiveOperation Operation { get; private set; }
	}

}