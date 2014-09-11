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

using Android.Util;

using Org.Json;

namespace LiveSDKSampleAndroid.SkyDrive
{
	public abstract class SkyDriveObject
	{
		public static SkyDriveObject Create(JSONObject skyDriveObject)
		{
			string type = skyDriveObject.OptString("type");

			if (type.Equals(SkyDriveFolder.ObjectType))
			{
				return new SkyDriveFolder(skyDriveObject);
			}
			else if (type.Equals(SkyDriveFile.ObjectType))
			{
				return new SkyDriveFile(skyDriveObject);
			}
			else if (type.Equals(SkyDriveAlbum.ObjectType))
			{
				return new SkyDriveAlbum(skyDriveObject);
			}
			else if (type.Equals(SkyDrivePhoto.ObjectType))
			{
				return new SkyDrivePhoto(skyDriveObject);
			}
			else if (type.Equals(SkyDriveVideo.ObjectType))
			{
				return new SkyDriveVideo(skyDriveObject);
			}
			else if (type.Equals(SkyDriveAudio.ObjectType))
			{
				return new SkyDriveAudio(skyDriveObject);
			}

			string name = skyDriveObject.OptString("name");
			Log.Error(typeof (SkyDriveObject).FullName, string.Format("Unknown SkyDriveObject type.  Name: {0}, Type {1}", name, type));

			return null;
		}

		protected readonly JSONObject Value;

		public SkyDriveObject(JSONObject value)
		{
			Value = value;
		}

		public abstract void Accept(ISkyDriveObjectVisitor visitor);

		public virtual string Id
		{
			get { return Value.OptString("id"); }
		}

		public virtual From From
		{
			get { return new From(Value.OptJSONObject("from")); }
		}

		public virtual string Name
		{
			get { return Value.OptString("name"); }
		}

		public virtual string ParentId
		{
			get { return Value.OptString("parent_id"); }
		}

		public virtual string Description
		{
			get { return Value.IsNull("description") ? null : Value.OptString("description"); }
		}

		public virtual string Type
		{
			get { return Value.OptString("type"); }
		}

		public virtual string Link
		{
			get { return Value.OptString("link"); }
		}

		public virtual string CreatedTime
		{
			get { return Value.OptString("created_time"); }
		}

		public virtual string UpdatedTime
		{
			get { return Value.OptString("updated_time"); }
		}

		public virtual string UploadLocation
		{
			get { return Value.OptString("upload_location"); }
		}

		public virtual SharedWith SharedWith
		{
			get { return new SharedWith(Value.OptJSONObject("shared_with")); }
		}

		public virtual JSONObject ToJson()
		{
			return Value;
		}
	}

	public interface ISkyDriveObjectVisitor
	{
		void Visit(SkyDriveAlbum album);
		void Visit(SkyDriveAudio audio);
		void Visit(SkyDrivePhoto photo);
		void Visit(SkyDriveFolder folder);
		void Visit(SkyDriveFile file);
		void Visit(SkyDriveVideo video);
	}

	public class From
	{
		private readonly JSONObject value;

		public From(JSONObject from)
		{
			value = from;
		}

		public virtual string Name
		{
			get { return value.OptString("name"); }
		}

		public virtual string Id
		{
			get { return value.OptString("id"); }
		}

		public virtual JSONObject ToJson()
		{
			return value;
		}
	}

	public class SharedWith
	{
		private readonly JSONObject value;

		public SharedWith(JSONObject sharedWith)
		{
			value = sharedWith;
		}

		public virtual string Access
		{
			get { return value.OptString("access"); }
		}

		public virtual JSONObject ToJson()
		{
			return value;
		}
	}
}
