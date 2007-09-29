/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com

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
namespace Db4objects.Drs.Tests
{
	public class SimpleArrayHolder
	{
		private string name;

		private Db4objects.Drs.Tests.SimpleArrayContent[] arr;

		public SimpleArrayHolder()
		{
		}

		public SimpleArrayHolder(string name)
		{
			this.name = name;
		}

		public virtual Db4objects.Drs.Tests.SimpleArrayContent[] GetArr()
		{
			return arr;
		}

		public virtual void SetArr(Db4objects.Drs.Tests.SimpleArrayContent[] arr)
		{
			this.arr = arr;
		}

		public virtual string GetName()
		{
			return name;
		}

		public virtual void SetName(string name)
		{
			this.name = name;
		}

		public virtual void Add(Db4objects.Drs.Tests.SimpleArrayContent sac)
		{
			if (arr == null)
			{
				arr = new Db4objects.Drs.Tests.SimpleArrayContent[] { sac };
				return;
			}
			Db4objects.Drs.Tests.SimpleArrayContent[] temp = arr;
			arr = new Db4objects.Drs.Tests.SimpleArrayContent[temp.Length + 1];
			System.Array.Copy(temp, 0, arr, 0, temp.Length);
			arr[temp.Length] = sac;
		}
	}
}
