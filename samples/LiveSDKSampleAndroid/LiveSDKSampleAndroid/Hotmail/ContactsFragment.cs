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

using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using LiveSDKSampleAndroid.Utilities;
using Org.Json;

using Microsoft.Live;

namespace LiveSDKSampleAndroid.Hotmail
{
	public class ContactsFragment : ListFragment, ISampleFragment
	{
		private ContactsListAdapter mAdapter;

		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			View rootView = inflater.Inflate(Resource.Layout.view_contacts, container, false);

			ListView lv = ListView;
			lv.TextFilterEnabled = true;
			lv.ItemClick += (sender, args) =>
			{
				Contact contact = (Contact)args.Parent.GetItemAtPosition(args.Position).FromJavaObject();
				ViewContactDialog dialog = new ViewContactDialog(this, contact);
				dialog.Show();
			};

			LinearLayout layout = new LinearLayout(this);
			Button newCalendarButton = new Button(this);
			newCalendarButton.Text = "New Contact";
			newCalendarButton.Click += delegate
			{
				CreateContactDialog dialog = new CreateContactDialog(this);
				dialog.Show();
			};

			layout.AddView(newCalendarButton);
			lv.AddHeaderView(layout);

			mAdapter = new ContactsListAdapter(this);
			ListAdapter = mAdapter;

			return rootView;
		}

		public void OnSelected()
		{
			LoadContacts();
		}

		private async void LoadContacts()
		{
			ProgressDialog progDialog = ProgressDialog.Show(Activity, "", "Loading. Please wait...", true);
			try
			{
				OperationCompleteResult getResult = await SampleApplication.Current.ConnectClient.GetAsync("me/contacts");

				JSONObject result = getResult.Operation.Result;
				if (result.Has(JsonKeys.Error))
				{
					JSONObject error = result.OptJSONObject(JsonKeys.Error);
					string message = error.OptString(JsonKeys.Message);
					string code = error.OptString(JsonKeys.Code);
					Utils.ShowToast(Activity, code + ": " + message);
					return;
				}

				List<Contact> contacts = mAdapter.Contacts;
				contacts.Clear();
				mAdapter.Contacts.AddRange(GetContacts(result).OrderBy(x => x.Name));

				mAdapter.NotifyDataSetChanged();
			}
			catch (OperationErrorException ex)
			{
				Utils.ShowToast(Activity, ex.Message);
			}
			progDialog.Dismiss();
		}

		private IEnumerable<Contact> GetContacts(JSONObject json)
		{
			JSONArray data = json.OptJSONArray(JsonKeys.Data);
			for (int i = 0; i < data.Length(); i++)
			{
				yield return new Contact(data.OptJSONObject(i));
			}
		}

		private class CreateContactDialog : Dialog
		{
			public CreateContactDialog(ContactsFragment fragment)
				: base(fragment)
			{
				SetContentView(Resource.Layout.create_contact);

				EditText firstName = FindViewById<EditText>(Resource.Id.firstNameEditText);
				EditText lastName = FindViewById<EditText>(Resource.Id.lastNameEditText);
				RadioGroup gender = FindViewById<RadioGroup>(Resource.Id.genderRadioGroup);

				FindViewById(Resource.Id.saveButton).Click += async delegate
				{
					ProgressDialog progDialog = ProgressDialog.Show(fragment.Activity, "", "Saving. Please wait...");

					JSONObject body = new JSONObject();
					try
					{
						body.Put(JsonKeys.FirstName, firstName.Text);
						body.Put(JsonKeys.LastName, lastName.Text);
						switch (gender.CheckedRadioButtonId)
						{
							case Resource.Id.maleRadio:
								body.Put(JsonKeys.Gender, "male");
								break;
							case Resource.Id.femaleRadio:
								body.Put(JsonKeys.Gender, "female");
								break;
						}
					}
					catch (JSONException e)
					{
						Utils.ShowToast(Context, e.Message);
						return;
					}

					try
					{
						OperationCompleteResult postResult = await SampleApplication.Current.ConnectClient.PostAsync("me/contacts", body);

						JSONObject result = postResult.Operation.Result;
						if (result.Has(JsonKeys.Error))
						{
							JSONObject error = result.OptJSONObject(JsonKeys.Error);
							string message = error.OptString(JsonKeys.Message);
							string code = error.OptString(JsonKeys.Code);
							Utils.ShowToast(Context, code + ": " + message);
						}
						else
						{
							fragment.LoadContacts();
							Dismiss();
						}
					}
					catch (OperationErrorException ex)
					{
						Utils.ShowToast(Context, ex.Message);
					}
					progDialog.Dismiss();
				};
			}
		}

		private class ContactsListAdapter : BaseAdapter
		{
			readonly LayoutInflater mInflater;
			readonly List<Contact> mContacts;

			public ContactsListAdapter(Context context)
			{
				mInflater = (LayoutInflater) context.GetSystemService(LayoutInflaterService);
				mContacts = new List<Contact>();
			}

			public virtual List<Contact> Contacts
			{
				get { return mContacts; }
			}

			public override int Count
			{
				get { return mContacts.Count; }
			}

			public override Java.Lang.Object GetItem(int position)
			{
				return mContacts[position].AsJavaObject();
			}

			public override long GetItemId(int position)
			{
				return position;
			}

			public override View GetView(int position, View convertView, ViewGroup parent)
			{
				View v = convertView ?? mInflater.Inflate(Resource.Layout.view_contacts_list_item, parent, false);

				TextView name = v.FindViewById<TextView>(Resource.Id.nameTextView);
				Contact contact = mContacts[position];
				name.Text = contact.Name;

				return v;
			}
		}

		private class ViewContactDialog : Dialog
		{
			private readonly Contact contact;

			public ViewContactDialog(Context context, Contact contact) 
				: base(context)
			{
				this.contact = contact;
			}

			protected override void OnCreate(Bundle savedInstanceState)
			{
				base.OnCreate(savedInstanceState);
				SetContentView(Resource.Layout.view_contact);

				FindViewById<TextView>(Resource.Id.idTextView).Text = "Id: " + contact.Id;
				FindViewById<TextView>(Resource.Id.nameTextView).Text = "Name: " + contact.Name;
				FindViewById<TextView>(Resource.Id.genderTextView).Text = "Gender: " + contact.Gender;
				FindViewById<TextView>(Resource.Id.isFriendTextView).Text = "Is Friend?: " + contact.IsFriend;
				FindViewById<TextView>(Resource.Id.isFavoriteTextView).Text = "Is Favorite?: " + contact.IsFavorite;
				FindViewById<TextView>(Resource.Id.userIdTextView).Text = "User Id: " + contact.UserId;
				FindViewById<TextView>(Resource.Id.updatedTimeTextView).Text = "Updated Time: " + contact.UpdatedTime;
			}
		}
	}
}
