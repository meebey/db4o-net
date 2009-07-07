/* Copyright (C) 2004 - 2008  Versant Inc.  http://www.db4o.com */

using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Convert;
using Db4objects.Db4o.Reflect;

namespace Db4objects.Db4o.Internal.Convert.Conversions
{
	/// <exclude></exclude>
	public class DropEnumClassIndexes_7_10 : Conversion
	{
		public const int Version = 9;

		public override void Convert(ConversionStage.SystemUpStage stage)
		{
			LocalObjectContainer file = stage.File();
			IReflector reflector = file.Reflector();
			ClassMetadataIterator i = file.ClassCollection().Iterator();
			while (i.MoveNext())
			{
				ClassMetadata classmetadata = i.CurrentClass();
				if (Platform4.IsEnum(reflector, classmetadata.ClassReflector()))
				{
					classmetadata.DropClassIndex();
				}
			}
		}
	}
}
