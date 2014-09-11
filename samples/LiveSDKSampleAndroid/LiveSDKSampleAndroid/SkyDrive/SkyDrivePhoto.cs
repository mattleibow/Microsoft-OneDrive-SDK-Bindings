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
	public class SkyDrivePhoto : SkyDriveObject
	{
		public const string ObjectType = "photo";

		public SkyDrivePhoto(JSONObject photo) 
			: base(photo)
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

		public virtual int TagsCount
		{
			get { return Value.OptInt("tags_count"); }
		}

		public virtual bool TagsEnabled
		{
			get { return Value.OptBoolean("tags_enabled"); }
		}

		public virtual string Picture
		{
			get { return Value.OptString("picture"); }
		}

		public virtual Image[] Images
		{
			get
			{
				JSONArray images = Value.OptJSONArray("images");
				Image[] imgs = new Image[images.Length()];
				for (int i = 0; i < images.Length(); i++)
				{
					imgs[i] = new Image(images.OptJSONObject(i));
				}

				return imgs;
			}
		}

		public virtual string WhenTaken
		{
			get { return Value.IsNull("when_taken") ? null : Value.OptString("when_taken"); }
		}

		public virtual int Height
		{
			get { return Value.OptInt("height"); }
		}

		public virtual int Width
		{
			get { return Value.OptInt("width"); }
		}
	}

	public class Image
	{
		private readonly JSONObject value;

		public Image(JSONObject image)
		{
			value = image;
		}

		public virtual int Height
		{
			get { return value.OptInt("height"); }
		}

		public virtual int Width
		{
			get { return value.OptInt("width"); }
		}

		public virtual string Source
		{
			get { return value.OptString("source"); }
		}

		public virtual string Type
		{
			get { return value.OptString("type"); }
		}

		public virtual JSONObject ToJson()
		{
			return value;
		}
	}
}
