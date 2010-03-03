/* Copyright (C) 2004 - 2009  Versant Inc.  http://www.db4o.com */

using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Fileheader;

namespace Db4objects.Db4o.Internal.Fileheader
{
	/// <exclude></exclude>
	public class FileHeader2 : FileHeader1
	{
		protected override byte Version()
		{
			return (byte)2;
		}

		protected override FileHeader1 CreateNew()
		{
			return new FileHeader2();
		}

		protected override FileHeaderVariablePart1 CreateVariablePart(LocalObjectContainer
			 file, int id)
		{
			return new FileHeaderVariablePart2(file, id, file.SystemData());
		}

		public override FileHeader Convert(LocalObjectContainer file)
		{
			return this;
		}
	}
}
