using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Live
{
	public partial class LiveConnectSession
	{
		public IEnumerable<string> Scopes
		{
			get { return ScopesInternal.ToEnumerable<string>(); }
		}
	}
}