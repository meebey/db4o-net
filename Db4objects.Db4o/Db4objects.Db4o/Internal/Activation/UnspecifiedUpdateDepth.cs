/* Copyright (C) 2004 - 2009  Versant Inc.  http://www.db4o.com */

using System;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Activation;

namespace Db4objects.Db4o.Internal.Activation
{
	public class UnspecifiedUpdateDepth : IUpdateDepth
	{
		public static readonly Db4objects.Db4o.Internal.Activation.UnspecifiedUpdateDepth
			 Instance = new Db4objects.Db4o.Internal.Activation.UnspecifiedUpdateDepth();

		private UnspecifiedUpdateDepth()
		{
		}

		public virtual bool SufficientDepth()
		{
			return true;
		}

		public virtual bool Negative()
		{
			return true;
		}

		public override string ToString()
		{
			return GetType().FullName;
		}

		public virtual IUpdateDepth Adjust(ClassMetadata clazz)
		{
			FixedUpdateDepth depth = clazz.UpdateDepthFromConfig();
			//        depth = clazz.adjustCollectionDepthToBorders(depth);
			//        return depth.adjust(clazz);
			return depth.Descend();
		}

		public virtual IUpdateDepth AdjustUpdateDepthForCascade(bool isCollection)
		{
			throw new InvalidOperationException();
		}

		public virtual IUpdateDepth Descend()
		{
			throw new InvalidOperationException();
		}
	}
}
