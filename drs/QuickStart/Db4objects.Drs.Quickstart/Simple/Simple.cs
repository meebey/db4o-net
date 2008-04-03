/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com

This file is part of the db4o open source object database.

db4o is free software; you can redistribute it and/or modify it under
the terms of version 2 of the GNU General Public License as published
by the Free Software Foundation and as clarified by db4objects' GPL 
interpretation policy, available at
http://www.db4o.com/about/company/legalpolicies/gplinterpretation/
Alternatively you can write to db4objects, Inc., 1900 S Norfolk Street,
Suite 350, San Mateo, CA 94403, USA.

db4o is distributed in the hope that it will be useful, but WITHOUT ANY
WARRANTY; without even the implied warranty of MERCHANTABILITY or
FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License
for more details.

You should have received a copy of the GNU General Public License along
with this program; if not, write to the Free Software Foundation, Inc.,
59 Temple Place - Suite 330, Boston, MA  02111-1307, USA. */
namespace Db4objects.Drs.Quickstart.Simple
{
	public class Simple
	{
		public static void Main(string[] args)
		{
			new Db4objects.Drs.Quickstart.Simple.Simple().DoOneWayReplcation();
			new Db4objects.Drs.Quickstart.Simple.Simple().DoBiDirectionalReplication();
			new Db4objects.Drs.Quickstart.Simple.Simple().DoSelectiveReplication();
		}

		public virtual void DoSelectiveReplication()
		{
			ConfigureDb4oForReplication();
			Db4objects.Db4o.IObjectContainer handheld = OpenDb("handheld.yap");
			StoreSomePilots(handheld);
			Db4objects.Db4o.IObjectContainer desktop = OpenDb("desktop.yap");
			DisplayContents("Selective Replication", "Before", handheld, desktop);
			Db4objects.Drs.IReplicationSession replication = Db4objects.Drs.Replication.Begin
				(handheld, desktop);
			Db4objects.Db4o.IObjectSet changed = replication.ProviderA().ObjectsChangedSinceLastReplication
				();
			while (changed.HasNext())
			{
				Db4objects.Drs.Quickstart.Simple.Pilot p = (Db4objects.Drs.Quickstart.Simple.Pilot
					)changed.Next();
				if (p._name.StartsWith("S"))
				{
					replication.Replicate(p);
				}
			}
			replication.Commit();
			DisplayContents(string.Empty, "After", handheld, desktop);
			CloseDb(handheld);
			CloseDb(desktop);
		}

		private void DoBiDirectionalReplication()
		{
			ConfigureDb4oForReplication();
			Db4objects.Db4o.IObjectContainer handheld = OpenDb("handheld.yap");
			StoreSomePilots(handheld);
			Db4objects.Db4o.IObjectContainer desktop = OpenDb("desktop.yap");
			StoreSomeMorePilots(desktop);
			DisplayContents("Bi-Directional", "Before", handheld, desktop);
			Db4objects.Drs.IReplicationSession replication = Db4objects.Drs.Replication.Begin
				(handheld, desktop);
			Db4objects.Db4o.IObjectSet changed = replication.ProviderA().ObjectsChangedSinceLastReplication
				();
			while (changed.HasNext())
			{
				replication.Replicate(changed.Next());
			}
			changed = replication.ProviderB().ObjectsChangedSinceLastReplication();
			while (changed.HasNext())
			{
				replication.Replicate(changed.Next());
			}
			replication.Commit();
			DisplayContents(string.Empty, "After", handheld, desktop);
			CloseDb(handheld);
			CloseDb(desktop);
		}

		private void DisplayContents(string methodname, string pointintime, Db4objects.Db4o.IObjectContainer
			 handheld, Db4objects.Db4o.IObjectContainer desktop)
		{
			if (methodname != string.Empty)
			{
				Sharpen.Runtime.Out.WriteLine(methodname + " Replication");
				Sharpen.Runtime.Out.WriteLine();
			}
			Sharpen.Runtime.Out.WriteLine(pointintime + " Replication");
			Sharpen.Runtime.Out.WriteLine();
			DisplayContentsOf("Contents of Handheld", handheld);
			DisplayContentsOf("Contents of Desktop", desktop);
		}

		private void StoreSomeMorePilots(Db4objects.Db4o.IObjectContainer db)
		{
			db.Set(new Db4objects.Drs.Quickstart.Simple.Pilot("Peter van der Merwe", 37));
			db.Set(new Db4objects.Drs.Quickstart.Simple.Pilot("Albert Kwan", 30));
		}

		private void DisplayContentsOf(string heading, Db4objects.Db4o.IObjectContainer db
			)
		{
			Sharpen.Runtime.Out.WriteLine(heading);
			Sharpen.Runtime.Out.WriteLine();
			Db4objects.Db4o.IObjectSet result = db.Get(new Db4objects.Drs.Quickstart.Simple.Pilot
				());
			ListResult(result);
		}

		private void CloseDb(Db4objects.Db4o.IObjectContainer db)
		{
			db.Close();
		}

		private Db4objects.Db4o.IObjectContainer OpenDb(string dbname)
		{
			new Sharpen.IO.File(dbname).Delete();
			Db4objects.Db4o.IObjectContainer db = Db4objects.Db4o.Db4oFactory.OpenFile(dbname
				);
			return db;
		}

		private void ConfigureDb4oForReplication()
		{
			Db4objects.Db4o.Db4oFactory.Configure().GenerateUUIDs(int.MaxValue);
			Db4objects.Db4o.Db4oFactory.Configure().GenerateVersionNumbers(int.MaxValue);
		}

		private void DoOneWayReplcation()
		{
			ConfigureDb4oForReplication();
			Db4objects.Db4o.IObjectContainer handheld = OpenDb("handheld.yap");
			StoreSomePilots(handheld);
			Db4objects.Db4o.IObjectContainer desktop = OpenDb("desktop.yap");
			DisplayContents("One-way Replication", "Before", handheld, desktop);
			Db4objects.Drs.IReplicationSession replication = Db4objects.Drs.Replication.Begin
				(handheld, desktop);
			Db4objects.Db4o.IObjectSet changed = replication.ProviderA().ObjectsChangedSinceLastReplication
				();
			while (changed.HasNext())
			{
				replication.Replicate(changed.Next());
			}
			replication.Commit();
			DisplayContents(string.Empty, "After", handheld, desktop);
			CloseDb(handheld);
			CloseDb(desktop);
		}

		private void StoreSomePilots(Db4objects.Db4o.IObjectContainer db)
		{
			db.Set(new Db4objects.Drs.Quickstart.Simple.Pilot("Scott Felton", 52));
			db.Set(new Db4objects.Drs.Quickstart.Simple.Pilot("Frank Green", 45));
		}

		public virtual void ListResult(Db4objects.Db4o.IObjectSet result)
		{
			while (result.HasNext())
			{
				Sharpen.Runtime.Out.WriteLine(result.Next());
			}
			Sharpen.Runtime.Out.WriteLine();
		}
	}
}
