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
namespace Db4objects.Drs.Test
{
	/// <summary>
	/// Design of this case is copied from
	/// com.db4o.db4ounit.common.types.arrays.ByteArrayTestCase.
	/// </summary>
	/// <remarks>
	/// Design of this case is copied from
	/// com.db4o.db4ounit.common.types.arrays.ByteArrayTestCase.
	/// </remarks>
	public class ByteArrayTest : Db4objects.Drs.Test.DrsTestCase
	{
		internal const int ARRAY_LENGTH = 5;

		internal static byte[] initial = CreateByteArray();

		internal static byte[] modInB = new byte[] { 2, 3, 5, 68, 69 };

		internal static byte[] modInA = new byte[] { 15, 36, 55, 8, 9, 28, 65 };

		public virtual void Test()
		{
			StoreInA();
			Replicate();
			ModifyInB();
			Replicate2();
			ModifyInA();
			Replicate3();
		}

		private void StoreInA()
		{
			Db4objects.Drs.Test.IIByteArrayHolder byteArrayHolder = new Db4objects.Drs.Test.ByteArrayHolder
				(CreateByteArray());
			A().Provider().StoreNew(byteArrayHolder);
			A().Provider().Commit();
			EnsureNames(A(), initial);
		}

		private void Replicate()
		{
			ReplicateAll(A().Provider(), B().Provider());
			EnsureNames(A(), initial);
			EnsureNames(B(), initial);
		}

		private void ModifyInB()
		{
			Db4objects.Drs.Test.IIByteArrayHolder c = GetTheObject(B());
			c.SetBytes(modInB);
			B().Provider().Update(c);
			B().Provider().Commit();
			EnsureNames(B(), modInB);
		}

		private void Replicate2()
		{
			ReplicateAll(B().Provider(), A().Provider());
			EnsureNames(A(), modInB);
			EnsureNames(B(), modInB);
		}

		private void ModifyInA()
		{
			Db4objects.Drs.Test.IIByteArrayHolder c = GetTheObject(A());
			c.SetBytes(modInA);
			A().Provider().Update(c);
			A().Provider().Commit();
			EnsureNames(A(), modInA);
		}

		private void Replicate3()
		{
			ReplicateAll(A().Provider(), B().Provider());
			EnsureNames(A(), modInA);
			EnsureNames(B(), modInA);
		}

		private void EnsureNames(Db4objects.Drs.Test.IDrsFixture fixture, byte[] bs)
		{
			EnsureOneInstance(fixture, typeof(Db4objects.Drs.Test.IIByteArrayHolder));
			Db4objects.Drs.Test.IIByteArrayHolder c = GetTheObject(fixture);
			Db4oUnit.ArrayAssert.AreEqual(c.GetBytes(), bs);
		}

		private Db4objects.Drs.Test.IIByteArrayHolder GetTheObject(Db4objects.Drs.Test.IDrsFixture
			 fixture)
		{
			return (Db4objects.Drs.Test.IIByteArrayHolder)GetOneInstance(fixture, typeof(Db4objects.Drs.Test.IIByteArrayHolder)
				);
		}

		internal static byte[] CreateByteArray()
		{
			byte[] bytes = new byte[ARRAY_LENGTH];
			for (byte i = 0; i < bytes.Length; ++i)
			{
				bytes[i] = i;
			}
			return bytes;
		}
	}
}
