/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System;

namespace Db4objects.Db4o.Internal.Marshall
{
	/// <exclude></exclude>
	public class AspectType
	{
		public readonly byte _id;

		public static readonly Db4objects.Db4o.Internal.Marshall.AspectType Field = new Db4objects.Db4o.Internal.Marshall.AspectType
			((byte)1);

		public static readonly Db4objects.Db4o.Internal.Marshall.AspectType Translator = 
			new Db4objects.Db4o.Internal.Marshall.AspectType((byte)2);

		public static readonly Db4objects.Db4o.Internal.Marshall.AspectType Typehandler = 
			new Db4objects.Db4o.Internal.Marshall.AspectType((byte)3);

		private AspectType(byte id)
		{
			_id = id;
		}

		public static Db4objects.Db4o.Internal.Marshall.AspectType ForByte(byte b)
		{
			switch (b)
			{
				case 1:
				{
					return Field;
				}

				case 2:
				{
					return Translator;
				}

				case 3:
				{
					return Typehandler;
				}

				default:
				{
					throw new ArgumentException();
					break;
				}
			}
		}

		public virtual bool IsFieldMetadata()
		{
			return this == Db4objects.Db4o.Internal.Marshall.AspectType.Field || this == Db4objects.Db4o.Internal.Marshall.AspectType
				.Translator;
		}
	}
}
