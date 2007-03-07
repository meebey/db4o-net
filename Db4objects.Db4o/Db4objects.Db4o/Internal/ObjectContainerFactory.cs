namespace Db4objects.Db4o.Internal
{
	public class ObjectContainerFactory
	{
		public static Db4objects.Db4o.IObjectContainer OpenObjectContainer(Db4objects.Db4o.Config.IConfiguration
			 config, string databaseFileName)
		{
			Db4objects.Db4o.IObjectContainer oc = null;
			try
			{
				oc = new Db4objects.Db4o.Internal.IoAdaptedObjectContainer(config, databaseFileName
					);
			}
			catch (Db4objects.Db4o.Ext.DatabaseFileLockedException e)
			{
				throw;
			}
			catch (Db4objects.Db4o.Ext.ObjectNotStorableException e)
			{
				throw;
			}
			catch (Db4objects.Db4o.Ext.Db4oException e)
			{
				throw;
			}
			catch (System.Exception ex)
			{
				Db4objects.Db4o.Internal.Messages.LogErr(Db4objects.Db4o.Db4oFactory.Configure(), 
					4, databaseFileName, ex);
				return null;
			}
			Db4objects.Db4o.Internal.Platform4.PostOpen(oc);
			Db4objects.Db4o.Internal.Messages.LogMsg(Db4objects.Db4o.Db4oFactory.Configure(), 
				5, databaseFileName);
			return oc;
		}
	}
}
