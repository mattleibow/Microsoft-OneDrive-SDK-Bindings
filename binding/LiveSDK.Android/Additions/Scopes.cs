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

namespace Microsoft.Live
{
	/// <summary>
	/// See http://msdn.microsoft.com/en-us/library/hh243646.aspx for more information.
	/// </summary>
	public static class Scopes
	{
		/* Core Scopes */
		public const string Basic = "wl.basic";
		public const string OfflineAccess = "wl.offline_access";
		public const string Signin = "wl.signin";

		/* Extended Scopes */
		public const string Birthday = "wl.birthday";
		public const string Calendars = "wl.calendars";
		public const string CalendarsUpdate = "wl.calendars_update";
		public const string ContactsBirthday = "wl.contacts_birthday";
		public const string ContactsCreate = "wl.contacts_create";
		public const string ContactsCalendars = "wl.contacts_calendars";
		public const string ContactsPhotos = "wl.contacts_photos";
		public const string ContactsSkydrive = "wl.contacts_skydrive";
		public const string Emails = "wl.emails";
		public const string EventsCreate = "wl.events_create";
		public const string PhoneNumbers = "wl.phone_numbers";
		public const string Photos = "wl.photos";
		public const string PostalAddresses = "wl.postal_addresses";
		public const string Share = "wl.share";
		public const string Skydrive = "wl.skydrive";
		public const string SkydriveUpdate = "wl.skydrive_update";
		public const string WorkProfile = "wl.work_profile";

		/* Developer Scopes */
		public const string Applications = "wl.applications";
		public const string ApplicationsCreate = "wl.applications_create";

		public static readonly string[] All =
		{
			Basic, OfflineAccess, Signin, Birthday, ContactsBirthday, ContactsPhotos,
			Emails, EventsCreate, PhoneNumbers, Photos, PostalAddresses, Share, WorkProfile, Applications, ApplicationsCreate
		};
	}
}