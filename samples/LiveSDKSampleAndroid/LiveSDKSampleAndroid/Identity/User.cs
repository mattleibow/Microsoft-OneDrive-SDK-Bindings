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

namespace LiveSDKSampleAndroid.Identity
{
	public class User
	{
		private readonly JSONObject mUserObj;

		public User(JSONObject userObj)
		{
			mUserObj = userObj;
		}

		public virtual string Id
		{
			get { return mUserObj.OptString("id"); }
		}

		public virtual string Name
		{
			get { return mUserObj.OptString("name"); }
		}

		public virtual string FirstName
		{
			get { return mUserObj.OptString("first_name"); }
		}

		public virtual string LastName
		{
			get { return mUserObj.OptString("last_name"); }
		}

		public virtual string Link
		{
			get { return mUserObj.OptString("link"); }
		}

		public virtual int BirthDay
		{
			get { return mUserObj.OptInt("birth_day"); }
		}

		public virtual int BirthMonth
		{
			get { return mUserObj.OptInt("birth_month"); }
		}

		public virtual int BirthYear
		{
			get { return mUserObj.OptInt("birth_year"); }
		}

		public virtual string Gender
		{
			get { return mUserObj.IsNull("gender") ? null : mUserObj.OptString("gender"); }
		}

		public virtual string Locale
		{
			get { return mUserObj.OptString("locale"); }
		}

		public virtual string UpdatedTime
		{
			get { return mUserObj.OptString("updated_time"); }
		}

		public virtual JSONObject ToJson()
		{
			return mUserObj;
		}
	}
}
