namespace Db4oUnit.Extensions
{
	public interface IDb4oFixture
	{
		void Open();

		void Close();

		void Reopen();

		void Clean();

		Db4objects.Db4o.Ext.IExtObjectContainer Db();

		Db4objects.Db4o.Config.IConfiguration Config();
	}
}
