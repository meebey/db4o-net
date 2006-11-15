namespace Db4objects.Db4o.Config
{
	/// <exclude></exclude>
	public class TNull : Db4objects.Db4o.Config.IObjectTranslator
	{
		public virtual object OnStore(Db4objects.Db4o.IObjectContainer con, object @object
			)
		{
			return null;
		}

		public virtual void OnActivate(Db4objects.Db4o.IObjectContainer con, object @object
			, object members)
		{
		}

		public virtual System.Type StoredClass()
		{
			return typeof(object);
		}
	}
}
