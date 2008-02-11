/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Foundation;

namespace Db4objects.Db4o.Foundation
{
	/// <exclude></exclude>
	public class Visitor4Dispatch : IVisitor4
	{
		public readonly IVisitor4 _target;

		public Visitor4Dispatch(IVisitor4 visitor)
		{
			_target = visitor;
		}

		public virtual void Visit(object a_object)
		{
			((IVisitor4)a_object).Visit(_target);
		}
	}
}
