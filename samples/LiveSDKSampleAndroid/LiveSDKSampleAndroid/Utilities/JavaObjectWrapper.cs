namespace LiveSDKSampleAndroid.Utilities
{
	public class JavaObjectWrapper : Java.Lang.Object
	{
		public object Value { get; set; }
	}

	public static class JavaObjectWrapperExtensions
	{
		public static JavaObjectWrapper AsJavaObject(this object obj)
		{
			return new JavaObjectWrapper { Value = obj };
		}

		public static object FromJavaObject(this Java.Lang.Object obj)
		{
			JavaObjectWrapper wrapper = obj as JavaObjectWrapper;
			if (wrapper != null)
				return wrapper.Value;
			return obj;
		}
	}
}