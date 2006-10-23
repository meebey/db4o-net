namespace Db4objects.Db4o
{
	internal interface IYapType
	{
		object DefaultValue();

		int TypeID();

		void Write(object obj, byte[] bytes, int offset);

		object Read(byte[] bytes, int offset);

		int Compare(object compare, object with);

		bool IsEqual(object compare, object with);
	}
}
