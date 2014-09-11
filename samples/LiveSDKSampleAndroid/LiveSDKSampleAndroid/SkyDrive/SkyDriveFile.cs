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

using Org.Json;

namespace LiveSDKSampleAndroid.SkyDrive
{
	public class SkyDriveFile : SkyDriveObject
	{
		public const string ObjectType = "file";

		public SkyDriveFile(JSONObject file) 
			: base(file)
		{
		}

		public override void Accept(ISkyDriveObjectVisitor visitor)
		{
			visitor.Visit(this);
		}

		public virtual long Size
		{
			get { return Value.OptLong("size"); }
		}

		public virtual int CommentsCount
		{
			get { return Value.OptInt("comments_count"); }
		}

		public virtual bool CommentsEnabled
		{
			get { return Value.OptBoolean("comments_enabled"); }
		}

		public virtual string Source
		{
			get { return Value.OptString("source"); }
		}

		public virtual bool IsEmbeddable
		{
			get { return Value.OptBoolean("is_embeddable"); }
		}
	}
}
