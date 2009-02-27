/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Convert;

namespace Db4objects.Db4o.Internal.Convert.Conversions
{
	/// <exclude></exclude>
	public partial class ReindexNetDateTime_7_8 : Conversion
	{
		public const int Version = 8;

		public override void Convert(ConversionStage.SystemUpStage stage)
		{
			ReindexDateTimeFields(stage);
		}

		private void ReindexDateTimeFields(ConversionStage.SystemUpStage stage)
		{
			ClassMetadataIterator i = stage.File().ClassCollection().Iterator();
			while (i.MoveNext())
			{
				ClassMetadata classmetadata = i.CurrentClass();
				classmetadata.ForEachDeclaredField(new _IProcedure4_26(this));
			}
		}

		private sealed class _IProcedure4_26 : IProcedure4
		{
			public _IProcedure4_26(ReindexNetDateTime_7_8 _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public void Apply(object field)
			{
				if (!((FieldMetadata)field).HasIndex())
				{
					return;
				}
				this._enclosing.ReindexDateTimeField(((FieldMetadata)field));
			}

			private readonly ReindexNetDateTime_7_8 _enclosing;
		}
		// do nothing, code is in partial class in .NET.
	}
}
