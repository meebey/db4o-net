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
using Db4objects.Drs.Inside;
using Db4objects.Drs.Tests;

namespace Db4objects.Drs.Tests
{
	internal class R0Linker
	{
		internal R0 r0;

		internal R1 r1;

		internal R2 r2;

		internal R3 r3;

		internal R4 r4;

		internal R0Linker()
		{
			r0 = new R0();
			r1 = new R1();
			r2 = new R2();
			r3 = new R3();
			r4 = new R4();
		}

		internal virtual void SetNames(string name)
		{
			r0.name = "0" + name;
			r1.name = "1" + name;
			r2.name = "2" + name;
			r3.name = "3" + name;
			r4.name = "4" + name;
		}

		internal virtual void LinkCircles()
		{
			LinkList();
			r1.circle1 = r0;
			r2.circle2 = r0;
			r3.circle3 = r0;
			r4.circle4 = r0;
		}

		internal virtual void LinkList()
		{
			r0.r1 = r1;
			r1.r2 = r2;
			r2.r3 = r3;
			r3.r4 = r4;
		}

		internal virtual void LinkThis()
		{
			r0.r0 = r0;
			r1.r1 = r1;
			r2.r2 = r2;
			r3.r3 = r3;
			r4.r4 = r4;
		}

		internal virtual void LinkBack()
		{
			r1.r0 = r0;
			r2.r1 = r1;
			r3.r2 = r2;
			r4.r3 = r3;
		}

		public virtual void Store(ITestableReplicationProviderInside provider)
		{
			provider.StoreNew(r4);
			provider.StoreNew(r3);
			provider.StoreNew(r2);
			provider.StoreNew(r1);
			provider.StoreNew(r0);
		}
	}
}
