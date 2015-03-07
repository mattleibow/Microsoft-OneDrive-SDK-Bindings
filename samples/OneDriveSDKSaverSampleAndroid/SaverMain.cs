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

namespace com.example.onedrivesdk.saversample
{

    using Activity = android.app.Activity;
    using Intent = android.content.Intent;
    using Uri = android.net.Uri;
    using android.os;
    using View = android.view.View;
    using OnClickListener = android.view.View.OnClickListener;
    using android.widget;

    using com.microsoft.onedrivesdk.saver;

    /// <summary>
    /// Activity that shows how the OneDrive SDK can be used for file saving
    /// 
    /// @author pnied
    /// </summary>
    public class SaverMain : Activity
    {

        /// <summary>
        /// The default file size
        /// </summary>
        private const int DEFAULT_FILE_SIZE_KB = 100;

        /// Registered Application id for OneDrive {<seealso cref= http://go.microsoft.com/fwlink/p/?LinkId=193157} </seealso>
        private const string ONEDRIVE_APP_ID = "48122D4E";

        /// <summary>
        /// The onClickListener that will start the OneDrive Picker
        /// </summary>
        private readonly View.OnClickListener mStartPickingListener = new OnClickListenerAnonymousInnerClassHelper();

        private class OnClickListenerAnonymousInnerClassHelper : View.OnClickListener
        {
            public OnClickListenerAnonymousInnerClassHelper()
            {
            }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: @Override public void onClick(final android.view.View v)
            public override void OnClick(View v)
            {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final android.app.Activity activity = (android.app.Activity) v.getContext();
                Activity activity = (Activity) v.Context;
                activity.findViewById(R.id.result_table).Visibility = View.INVISIBLE;

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String filename = ((EditText)activity.findViewById(R.id.file_name_edit_text)).getText().toString();
                string filename = ((EditText)activity.findViewById(R.id.file_name_edit_text)).Text.ToString();
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String fileSizeString = ((EditText)activity.findViewById(R.id.file_size_edit_text)).getText().toString();
                string fileSizeString = ((EditText)activity.findViewById(R.id.file_size_edit_text)).Text.ToString();
                int size;
                try
                {
                    size = int.Parse(fileSizeString);
                }
//JAVA TO C# CONVERTER WARNING: 'final' catch parameters are not available in C#:
//ORIGINAL LINE: catch (final NumberFormatException nfe)
                catch (mberFormatException)
                {
                    size = DEFAULT_FILE_SIZE_KB;
                }

                // Create a file
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final File f = createExternalSdCardFile(filename, size);
                File f = outerInstance.CreateExternalSdCardFile(filename, size);

                // Start the saver
                outerInstance.mSaver.startSaving(activity, filename, Uri.parse("file://" + f.AbsolutePath));
            }
        }

        /// <summary>
        /// The OneDrive saver instance used by this activity
        /// </summary>
        private ISaver mSaver;

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: @Override protected void onCreate(final Bundle savedInstanceState)
        protected internal override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            ContentView = R.layout.activity_saver_main;

            // Create the picker instance
            mSaver = Saver.createSaver(ONEDRIVE_APP_ID);

            // Add the start saving listener
            findViewById(R.id.startSaverButton).OnClickListener = mStartPickingListener;
        }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: @Override protected void onActivityResult(final int requestCode, final int resultCode, final android.content.Intent data)
        protected internal override void OnActivityResult(int requestCode, int resultCode, Intent data)
        {
            // Check that we were able to save the file on OneDrive
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final TextView overallResult = (TextView) findViewById(R.id.overall_result);
            TextView overallResult = (TextView) findViewById(R.id.overall_result);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final TextView errorResult = (TextView) findViewById(R.id.error_type_result);
            TextView errorResult = (TextView) findViewById(R.id.error_type_result);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final TextView debugErrorResult = (TextView) findViewById(R.id.debug_error_result);
            TextView debugErrorResult = (TextView) findViewById(R.id.debug_error_result);

            try
            {
                mSaver.handleSave(requestCode, resultCode, data);
                overallResult.Text = getString(R.@string.overall_result_success);
                errorResult.Text = getString(R.@string.error_message_none);
                debugErrorResult.Text = getString(R.@string.error_message_none);
            }
//JAVA TO C# CONVERTER WARNING: 'final' catch parameters are not available in C#:
//ORIGINAL LINE: catch (final SaverException e)
            catch (SaverException e)
            {
                overallResult.Text = getString(R.@string.overall_result_failure);
                errorResult.Text = e.ErrorType.ToString();
                debugErrorResult.Text = e.DebugErrorInfo;
            }
            findViewById(R.id.result_table).Visibility = View.VISIBLE;
        }

        /// <summary>
        /// Creates an file on the SDCard </summary>
        /// <param name="filename"> The name of the file to create </param>
        /// <param name="size"> The size in KB to make the file </param>
        /// <returns> The <seealso cref="File"/> object that was created </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: private File createExternalSdCardFile(final String filename, final int size)
        private File CreateExternalSdCardFile(string filename, int size)
        {
            const int bufferSize = 1024;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int alphabetRange = 'z' - 'a';
            int alphabetRange = 'z' - 'a';
            File file = null;
            try
            {
                file = new File(Environment.ExternalStorageDirectory, filename);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final FileOutputStream fos = new FileOutputStream(file);
                System.IO.FileStream fos = new System.IO.FileStream(file, System.IO.FileMode.Create, System.IO.FileAccess.Write);

                // Create a 1 kb size buffer to use in writing the temp file
                sbyte[] buffer = new sbyte[bufferSize];
                for (int i = 0; i < buffer.Length; i++)
                {
                    buffer[i] = (sbyte)('a' + i % alphabetRange);
                }

                // Write out the file, 1 kb at a time
                for (int i = 0; i < size; i++)
                {
                    fos.Write(buffer, 0, buffer.Length);
                }

                fos.Close();
            }
//JAVA TO C# CONVERTER WARNING: 'final' catch parameters are not available in C#:
//ORIGINAL LINE: catch (final IOException e)
            catch (IOException e)
            {
                Toast.makeText(this, "Error when creating the file: " + e.Message, Toast.LENGTH_LONG).show();
            }
            return file;
        }
    }

}