/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

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
