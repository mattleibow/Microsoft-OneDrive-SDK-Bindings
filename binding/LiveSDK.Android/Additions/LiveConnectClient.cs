using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

using Android.App;
using Java.IO;
using Org.Json;

namespace Microsoft.Live
{
	public partial class LiveConnectClient
	{
		public virtual Task<OperationCompleteResult> CopyAsync(string path, string destination)
		{
			return LiveOperationListenerAsync.PerformOperationAsync(this, listener => CopyAsync(path, destination, listener), CancellationToken.None);
		}
		public virtual Task<OperationCompleteResult> CopyAsync(string path, string destination, CancellationToken ct)
		{
			return LiveOperationListenerAsync.PerformOperationAsync(this, listener => CopyAsync(path, destination, listener), ct);
		}
		public virtual Task<OperationCompleteResult> CopyAsync(string path, string destination, Java.Lang.Object userState)
		{
			return LiveOperationListenerAsync.PerformOperationAsync(this, listener => CopyAsync(path, destination, listener, userState), CancellationToken.None);
		}
		public virtual Task<OperationCompleteResult> CopyAsync(string path, string destination, Java.Lang.Object userState, CancellationToken ct)
		{
			return LiveOperationListenerAsync.PerformOperationAsync(this, listener => CopyAsync(path, destination, listener, userState), ct);
		}
		
		public virtual Task<OperationCompleteResult> DeleteAsync(string path)
		{
			return LiveOperationListenerAsync.PerformOperationAsync(this, listener => DeleteAsync(path, listener), CancellationToken.None);
		}
		public virtual Task<OperationCompleteResult> DeleteAsync(string path, CancellationToken ct)
		{
			return LiveOperationListenerAsync.PerformOperationAsync(this, listener => DeleteAsync(path, listener), ct);
		}
		public virtual Task<OperationCompleteResult> DeleteAsync(string path, Java.Lang.Object userState)
		{
			return LiveOperationListenerAsync.PerformOperationAsync(this, listener => DeleteAsync(path, listener, userState), CancellationToken.None);
		}
		public virtual Task<OperationCompleteResult> DeleteAsync(string path, Java.Lang.Object userState, CancellationToken ct)
		{
			return LiveOperationListenerAsync.PerformOperationAsync(this, listener => DeleteAsync(path, listener, userState), ct);
		}

		public virtual Task<DownloadOperationCompleteResult> DownloadAsync(string path)
		{
			return LiveOperationListenerAsync.PerformOperationAsync(this, listener => DownloadAsync(path, listener), CancellationToken.None, null);
		}
		public virtual Task<DownloadOperationCompleteResult> DownloadAsync(string path, CancellationToken ct, IProgress<DownloadOperationProgress> progress)
		{
			return LiveOperationListenerAsync.PerformOperationAsync(this, listener => DownloadAsync(path, listener), ct, progress);
		}
		public virtual Task<DownloadOperationCompleteResult> DownloadAsync(string path, Java.Lang.Object userState)
		{
			return LiveOperationListenerAsync.PerformOperationAsync(this, listener => DownloadAsync(path, listener, userState), CancellationToken.None, null);
		}
		public virtual Task<DownloadOperationCompleteResult> DownloadAsync(string path, Java.Lang.Object userState, CancellationToken ct, IProgress<DownloadOperationProgress> progress)
		{
			return LiveOperationListenerAsync.PerformOperationAsync(this, listener => DownloadAsync(path, listener, userState), ct, progress);
		}
		public virtual Task<DownloadOperationCompleteResult> DownloadAsync(string path, Java.IO.File file)
		{
			return LiveOperationListenerAsync.PerformOperationAsync(this, listener => DownloadAsync(path, file, listener), CancellationToken.None, null);
		}
		public virtual Task<DownloadOperationCompleteResult> DownloadAsync(string path, Java.IO.File file, CancellationToken ct, IProgress<DownloadOperationProgress> progress)
		{
			return LiveOperationListenerAsync.PerformOperationAsync(this, listener => DownloadAsync(path, file, listener), ct, progress);
		}
		public virtual Task<DownloadOperationCompleteResult> DownloadAsync(string path, Java.IO.File file, Java.Lang.Object userState)
		{
			return LiveOperationListenerAsync.PerformOperationAsync(this, listener => DownloadAsync(path, file, listener, userState), CancellationToken.None, null);
		}
		public virtual Task<DownloadOperationCompleteResult> DownloadAsync(string path, Java.IO.File file, Java.Lang.Object userState, CancellationToken ct, IProgress<DownloadOperationProgress> progress)
		{
			return LiveOperationListenerAsync.PerformOperationAsync(this, listener => DownloadAsync(path, file, listener, userState), ct, progress);
		}

		public virtual Task<OperationCompleteResult> GetAsync(string path)
		{
			return LiveOperationListenerAsync.PerformOperationAsync(this, listener => GetAsync(path, listener), CancellationToken.None);
		}
		public virtual Task<OperationCompleteResult> GetAsync(string path, CancellationToken ct)
		{
			return LiveOperationListenerAsync.PerformOperationAsync(this, listener => GetAsync(path, listener), ct);
		}
		public virtual Task<OperationCompleteResult> GetAsync(string path, Java.Lang.Object userState)
		{
			return LiveOperationListenerAsync.PerformOperationAsync(this, listener => GetAsync(path, listener, userState), CancellationToken.None);
		}
		public virtual Task<OperationCompleteResult> GetAsync(string path, Java.Lang.Object userState, CancellationToken ct)
		{
			return LiveOperationListenerAsync.PerformOperationAsync(this, listener => GetAsync(path, listener, userState), ct);
		}

		public virtual Task<OperationCompleteResult> MoveAsync(string path, string destination)
		{
			return LiveOperationListenerAsync.PerformOperationAsync(this, listener => MoveAsync(path, destination, listener), CancellationToken.None);
		}
		public virtual Task<OperationCompleteResult> MoveAsync(string path, string destination, CancellationToken ct)
		{
			return LiveOperationListenerAsync.PerformOperationAsync(this, listener => MoveAsync(path, destination, listener), ct);
		}
		public virtual Task<OperationCompleteResult> MoveAsync(string path, string destination, Java.Lang.Object userState)
		{
			return LiveOperationListenerAsync.PerformOperationAsync(this, listener => MoveAsync(path, destination, listener, userState), CancellationToken.None);
		}
		public virtual Task<OperationCompleteResult> MoveAsync(string path, string destination, Java.Lang.Object userState, CancellationToken ct)
		{
			return LiveOperationListenerAsync.PerformOperationAsync(this, listener => MoveAsync(path, destination, listener, userState), ct);
		}

		public virtual Task<OperationCompleteResult> PostAsync(string path, string body)
		{
			return LiveOperationListenerAsync.PerformOperationAsync(this, listener => PostAsync(path, body, listener), CancellationToken.None);
		}
		public virtual Task<OperationCompleteResult> PostAsync(string path, string body, CancellationToken ct)
		{
			return LiveOperationListenerAsync.PerformOperationAsync(this, listener => PostAsync(path, body, listener), ct);
		}
		public virtual Task<OperationCompleteResult> PostAsync(string path, string body, Java.Lang.Object userState)
		{
			return LiveOperationListenerAsync.PerformOperationAsync(this, listener => PostAsync(path, body, listener, userState), CancellationToken.None);
		}
		public virtual Task<OperationCompleteResult> PostAsync(string path, string body, Java.Lang.Object userState, CancellationToken ct)
		{
			return LiveOperationListenerAsync.PerformOperationAsync(this, listener => PostAsync(path, body, listener, userState), ct);
		}
		public virtual Task<OperationCompleteResult> PostAsync(string path, JSONObject body)
		{
			return LiveOperationListenerAsync.PerformOperationAsync(this, listener => PostAsync(path, body, listener), CancellationToken.None);
		}
		public virtual Task<OperationCompleteResult> PostAsync(string path, JSONObject body, CancellationToken ct)
		{
			return LiveOperationListenerAsync.PerformOperationAsync(this, listener => PostAsync(path, body, listener), ct);
		}
		public virtual Task<OperationCompleteResult> PostAsync(string path, JSONObject body, Java.Lang.Object userState)
		{
			return LiveOperationListenerAsync.PerformOperationAsync(this, listener => PostAsync(path, body, listener, userState), CancellationToken.None);
		}
		public virtual Task<OperationCompleteResult> PostAsync(string path, JSONObject body, Java.Lang.Object userState, CancellationToken ct)
		{
			return LiveOperationListenerAsync.PerformOperationAsync(this, listener => PostAsync(path, body, listener, userState), ct);
		}

		public virtual Task<OperationCompleteResult> PutAsync(string path, string body)
		{
			return LiveOperationListenerAsync.PerformOperationAsync(this, listener => PutAsync(path, body, listener), CancellationToken.None);
		}
		public virtual Task<OperationCompleteResult> PutAsync(string path, string body, CancellationToken ct)
		{
			return LiveOperationListenerAsync.PerformOperationAsync(this, listener => PutAsync(path, body, listener), ct);
		}
		public virtual Task<OperationCompleteResult> PutAsync(string path, string body, Java.Lang.Object userState)
		{
			return LiveOperationListenerAsync.PerformOperationAsync(this, listener => PutAsync(path, body, listener, userState), CancellationToken.None);
		}
		public virtual Task<OperationCompleteResult> PutAsync(string path, string body, Java.Lang.Object userState, CancellationToken ct)
		{
			return LiveOperationListenerAsync.PerformOperationAsync(this, listener => PutAsync(path, body, listener, userState), ct);
		}
		public virtual Task<OperationCompleteResult> PutAsync(string path, JSONObject body)
		{
			return LiveOperationListenerAsync.PerformOperationAsync(this, listener => PutAsync(path, body, listener), CancellationToken.None);
		}
		public virtual Task<OperationCompleteResult> PutAsync(string path, JSONObject body, CancellationToken ct)
		{
			return LiveOperationListenerAsync.PerformOperationAsync(this, listener => PutAsync(path, body, listener), ct);
		}
		public virtual Task<OperationCompleteResult> PutAsync(string path, JSONObject body, Java.Lang.Object userState)
		{
			return LiveOperationListenerAsync.PerformOperationAsync(this, listener => PutAsync(path, body, listener, userState), CancellationToken.None);
		}
		public virtual Task<OperationCompleteResult> PutAsync(string path, JSONObject body, Java.Lang.Object userState, CancellationToken ct)
		{
			return LiveOperationListenerAsync.PerformOperationAsync(this, listener => PutAsync(path, body, listener, userState), ct);
		}

		public virtual Task<UploadOperationCompleteResult> UploadAsync(string path, string filename, Java.IO.File file)
		{
			return LiveOperationListenerAsync.PerformOperationAsync(this, listener => UploadAsync(path, filename, file, listener), CancellationToken.None, null);
		}
		public virtual Task<UploadOperationCompleteResult> UploadAsync(string path, string filename, Java.IO.File file, CancellationToken ct, IProgress<UploadOperationProgress> progress)
		{
			return LiveOperationListenerAsync.PerformOperationAsync(this, listener => UploadAsync(path, filename, file, listener), ct, progress);
		}
		public virtual Task<UploadOperationCompleteResult> UploadAsync(string path, string filename, Java.IO.File file, Java.Lang.Object userState)
		{
			return LiveOperationListenerAsync.PerformOperationAsync(this, listener => UploadAsync(path, filename, file, listener, userState), CancellationToken.None, null);
		}
		public virtual Task<UploadOperationCompleteResult> UploadAsync(string path, string filename, Java.IO.File file, Java.Lang.Object userState, CancellationToken ct, IProgress<UploadOperationProgress> progress)
		{
			return LiveOperationListenerAsync.PerformOperationAsync(this, listener => UploadAsync(path, filename, file, listener, userState), ct, progress);
		}
		public virtual Task<UploadOperationCompleteResult> UploadAsync(string path, string filename, Java.IO.File file, OverwriteOption overwrite, Java.Lang.Object userState)
		{
			return LiveOperationListenerAsync.PerformOperationAsync(this, listener => UploadAsync(path, filename, file, overwrite, listener, userState), CancellationToken.None, null);
		}
		public virtual Task<UploadOperationCompleteResult> UploadAsync(string path, string filename, Java.IO.File file, OverwriteOption overwrite, Java.Lang.Object userState, CancellationToken ct, IProgress<UploadOperationProgress> progress)
		{
			return LiveOperationListenerAsync.PerformOperationAsync(this, listener => UploadAsync(path, filename, file, overwrite, listener, userState), ct, progress);
		}
		public virtual Task<UploadOperationCompleteResult> UploadAsync(string path, string filename, Stream input)
		{
			return LiveOperationListenerAsync.PerformOperationAsync(this, listener => UploadAsync(path, filename, input, listener), CancellationToken.None, null);
		}
		public virtual Task<UploadOperationCompleteResult> UploadAsync(string path, string filename, Stream input, CancellationToken ct, IProgress<UploadOperationProgress> progress)
		{
			return LiveOperationListenerAsync.PerformOperationAsync(this, listener => UploadAsync(path, filename, input, listener), ct, progress);
		}
		public virtual Task<UploadOperationCompleteResult> UploadAsync(string path, string filename, Stream input, Java.Lang.Object userState)
		{
			return LiveOperationListenerAsync.PerformOperationAsync(this, listener => UploadAsync(path, filename, input, listener, userState), CancellationToken.None, null);
		}
		public virtual Task<UploadOperationCompleteResult> UploadAsync(string path, string filename, Stream input, Java.Lang.Object userState, CancellationToken ct, IProgress<UploadOperationProgress> progress)
		{
			return LiveOperationListenerAsync.PerformOperationAsync(this, listener => UploadAsync(path, filename, input, listener, userState), ct, progress);
		}
		public virtual Task<UploadOperationCompleteResult> UploadAsync(string path, string filename, Stream input, OverwriteOption overwrite, Java.Lang.Object userState)
		{
			return LiveOperationListenerAsync.PerformOperationAsync(this, listener => UploadAsync(path, filename, input, overwrite, listener, userState), CancellationToken.None, null);
		}
		public virtual Task<UploadOperationCompleteResult> UploadAsync(string path, string filename, Stream input, OverwriteOption overwrite, Java.Lang.Object userState, CancellationToken ct, IProgress<UploadOperationProgress> progress)
		{
			return LiveOperationListenerAsync.PerformOperationAsync(this, listener => UploadAsync(path, filename, input, overwrite, listener, userState), ct, progress);
		}
	}
}