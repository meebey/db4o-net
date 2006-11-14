namespace Db4oUnit.Extensions
{
	public interface IDb4oFixture
	{
		string GetLabel();

		void Open();

		void Close();

		void Reopen();

		void Clean();

		Db4objects.Db4o.Ext.IExtObjectContainer Db();

		Db4objects.Db4o.Config.IConfiguration Config();

		bool Accept(System.Type clazz);
	}
}
