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
using Sharpen;

namespace Db4objects.Drs.Tests
{
	public class Replicated
	{
		private string name;

		private Db4objects.Drs.Tests.Replicated link;

		public Replicated()
		{
		}

		public Replicated(string name)
		{
			this.SetName(name);
		}

		public override string ToString()
		{
			return GetName() + ", hashcode = " + GetHashCode() + ", identity = " + Runtime.IdentityHashCode
				(this);
		}

		public virtual string GetName()
		{
			return name;
		}

		public virtual void SetName(string name)
		{
			this.name = name;
		}

		public virtual Db4objects.Drs.Tests.Replicated GetLink()
		{
			return link;
		}

		public virtual void SetLink(Db4objects.Drs.Tests.Replicated link)
		{
			this.link = link;
		}

		public override bool Equals(object o)
		{
			if (o == null)
			{
				return false;
			}
			if (!(o is Db4objects.Drs.Tests.Replicated))
			{
				return false;
			}
			return ((Db4objects.Drs.Tests.Replicated)o).name.Equals(name);
		}

		public override int GetHashCode()
		{
			if (name == null)
			{
				return 0;
			}
			return name.GetHashCode();
		}
	}
}
