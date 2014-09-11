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

using System.Diagnostics;

using Org.Json;

namespace LiveSDKSampleAndroid.Hotmail
{
	public class Contact
	{
		private readonly JSONObject mContact;

		public Contact(JSONObject contact)
		{
			Debug.Assert(contact != null);
			mContact = contact;
		}

		public virtual string Id
		{
			get { return mContact.OptString("id"); }
		}

		public virtual string FirstName
		{
			get { return mContact.IsNull("first_name") ? null : mContact.OptString("first_name"); }
		}

		public virtual string LastName
		{
			get { return mContact.IsNull("last_name") ? null : mContact.OptString("last_name"); }
		}

		public virtual string Name
		{
			get { return mContact.OptString("name"); }
		}

		public virtual string Gender
		{
			get { return mContact.IsNull("gender") ? null : mContact.OptString("gender"); }
		}

		public virtual bool IsFriend
		{
			get { return mContact.OptBoolean("is_friend"); }
		}

		public virtual bool IsFavorite
		{
			get { return mContact.OptBoolean("is_favorite"); }
		}

		public virtual string UserId
		{
			get { return mContact.IsNull("user_id") ? null : mContact.OptString("user_id"); }
		}

		public virtual string UpdatedTime
		{
			get { return mContact.OptString("updated_time"); }
		}

		public virtual JSONObject ToJson()
		{
			return mContact;
		}
	}
}