namespace Db4objects.Db4o
{
	internal class Sessions : Db4objects.Db4o.Foundation.Collection4
	{
		internal virtual void ForEach(Db4objects.Db4o.Foundation.IVisitor4 visitor)
		{
			lock (Db4objects.Db4o.Inside.Global4.Lock)
			{
				System.Collections.IEnumerator i = GetEnumerator();
				while (i.MoveNext())
				{
					visitor.Visit(i.Current);
				}
			}
		}

		internal virtual Db4objects.Db4o.IObjectContainer Open(Db4objects.Db4o.Config.IConfiguration
			 config, string databaseFileName)
		{
			lock (Db4objects.Db4o.Inside.Global4.Lock)
			{
				Db4objects.Db4o.IObjectContainer oc = null;
				Db4objects.Db4o.Session newSession = new Db4objects.Db4o.Session(databaseFileName
					);
				Db4objects.Db4o.Session oldSession = (Db4objects.Db4o.Session)Get(newSession);
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
					oc = new Db4objects.Db4o.YapRandomAccessFile(config, newSession);
				}
				catch (Db4objects.Db4o.Ext.DatabaseFileLockedException e)
				{
					throw;
				}
				catch (Db4objects.Db4o.Ext.ObjectNotStorableException e)
				{
					throw;
				}
				catch (System.Exception t)
				{
					Db4objects.Db4o.Messages.LogErr(Db4objects.Db4o.Db4oFactory.i_config, 4, databaseFileName
						, t);
					return null;
				}
				newSession.i_stream = (Db4objects.Db4o.YapStream)oc;
				Add(newSession);
				Db4objects.Db4o.Platform4.PostOpen(oc);
				Db4objects.Db4o.Messages.LogMsg(Db4objects.Db4o.Db4oFactory.i_config, 5, databaseFileName
					);
				return oc;
			}
		}

		public override object Remove(object obj)
		{
			lock (Db4objects.Db4o.Inside.Global4.Lock)
			{
				return base.Remove(obj);
			}
		}
	}
}
