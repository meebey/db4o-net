namespace Db4objects.Db4o.Internal
{
	public class Sessions : Db4objects.Db4o.Foundation.Collection4
	{
		private static readonly Db4objects.Db4o.Internal.Sessions _instance = new Db4objects.Db4o.Internal.Sessions
			();

		private Sessions()
		{
		}

		public virtual void ForEach(Db4objects.Db4o.Foundation.IVisitor4 visitor)
		{
			lock (Db4objects.Db4o.Internal.Global4.Lock)
			{
				System.Collections.IEnumerator i = GetEnumerator();
				while (i.MoveNext())
				{
					visitor.Visit(i.Current);
				}
			}
		}

		public static Db4objects.Db4o.IObjectContainer Open(Db4objects.Db4o.Config.IConfiguration
			 config, string databaseFileName)
		{
			return _instance.OpenSession(config, databaseFileName);
		}

		private Db4objects.Db4o.IObjectContainer OpenSession(Db4objects.Db4o.Config.IConfiguration
			 config, string databaseFileName)
		{
			lock (Db4objects.Db4o.Internal.Global4.Lock)
			{
				Db4objects.Db4o.IObjectContainer oc = null;
				Db4objects.Db4o.Internal.Session newSession = new Db4objects.Db4o.Internal.Session
					(databaseFileName);
				Db4objects.Db4o.Internal.Session oldSession = (Db4objects.Db4o.Internal.Session)Get
					(newSession);
				if (oldSession != null)
				{
					oc = oldSession.SubSequentOpen();
					if (oc == null)
					{
						Remove(oldSession);
					}
					return oc;
				}
				try
				{
					oc = new Db4objects.Db4o.Internal.IoAdaptedObjectContainer(config, newSession);
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
				catch (System.Exception t)
				{
					Db4objects.Db4o.Internal.Messages.LogErr(Db4objects.Db4o.Db4oFactory.Configure(), 
						4, databaseFileName, t);
					return null;
				}
				newSession.i_stream = (Db4objects.Db4o.Internal.ObjectContainerBase)oc;
				Add(newSession);
				Db4objects.Db4o.Internal.Platform4.PostOpen(oc);
				Db4objects.Db4o.Internal.Messages.LogMsg(Db4objects.Db4o.Db4oFactory.Configure(), 
					5, databaseFileName);
				return oc;
			}
		}

		public override object Remove(object obj)
		{
			lock (Db4objects.Db4o.Internal.Global4.Lock)
			{
				return base.Remove(obj);
			}
		}

		internal static void ForEachSession(Db4objects.Db4o.Foundation.IVisitor4 visitor)
		{
			_instance.ForEach(visitor);
		}

		internal static void SessionStopped(Db4objects.Db4o.Internal.Session a_session)
		{
			_instance.Remove(a_session);
		}
	}
}
