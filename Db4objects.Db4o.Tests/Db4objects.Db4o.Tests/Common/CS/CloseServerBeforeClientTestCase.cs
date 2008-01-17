/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System;
using Db4oUnit.Extensions;
using Db4objects.Db4o.Ext;
using Db4objects.Db4o.Tests.Common.CS;

namespace Db4objects.Db4o.Tests.Common.CS
{
	public class CloseServerBeforeClientTestCase : Db4oClientServerTestCase
	{
		public static void Main(string[] arguments)
		{
			new CloseServerBeforeClientTestCase().RunClientServer();
		}

		/// <exception cref="Exception"></exception>
		public virtual void Test()
		{
			IExtObjectContainer client = OpenNewClient();
			try
			{
				ClientServerFixture().Server().Close();
			}
			finally
			{
				try
				{
					client.Close();
				}
				catch (Db4oException)
				{
				}
				// database may have been closed
				try
				{
					Fixture().Close();
				}
				catch (Db4oException)
				{
				}
			}
		}
	}
}
