/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System;
using System.Collections;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Fieldindex;
using Db4objects.Db4o.Internal.Query.Processor;

namespace Db4objects.Db4o.Internal.Fieldindex
{
	public class IndexedPath : IndexedNodeBase
	{
		public static IIndexedNode NewParentPath(IIndexedNode next, QCon constraint)
		{
			if (!CanFollowParent(constraint))
			{
				return null;
			}
			return new Db4objects.Db4o.Internal.Fieldindex.IndexedPath((QConObject)constraint
				.Parent(), next);
		}

		private static bool CanFollowParent(QCon con)
		{
			QCon parent = con.Parent();
			FieldMetadata parentField = GetYapField(parent);
			if (null == parentField)
			{
				return false;
			}
			FieldMetadata conField = GetYapField(con);
			if (null == conField)
			{
				return false;
			}
			return parentField.HasIndex() && parentField.HandlerClassMetadata(con.Transaction
				().Container()).IsAssignableFrom(conField.ContainingClass());
		}

		private static FieldMetadata GetYapField(QCon con)
		{
			QField field = con.GetField();
			if (null == field)
			{
				return null;
			}
			return field.GetYapField();
		}

		private IIndexedNode _next;

		public IndexedPath(QConObject parent, IIndexedNode next) : base(parent)
		{
			_next = next;
		}

		public override IEnumerator GetEnumerator()
		{
			return new IndexedPathIterator(this, _next.GetEnumerator());
		}

		public override int ResultSize()
		{
			throw new NotSupportedException();
		}
	}
}
