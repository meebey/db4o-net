/* Copyright (C) 2007 - 2008  db4objects Inc.  http://www.db4o.com */

using System;
using System.Collections.Generic;
using Db4objects.Db4o.Query;

namespace Db4objects.Db4o.Linq.Internals
{
	internal class QueryBuilderContext
	{
		private IQuery _root;
		private IQuery _query;
		private Stack<IConstraint> _constraints = new Stack<IConstraint>();
        private Type _descendigFieldEnum;

		public IQuery RootQuery
		{
			get { return _root; }
		}

		public IQuery CurrentQuery
		{
			get { return _query; }
		}

		public QueryBuilderContext(IQuery root)
		{
			_root = root;
			_query = _root;
		}

		public void PushQuery(IQuery query)
		{
			_query = query;
		}

        internal void PushDescendigFieldEnumType(Type descendigFieldEnum)
        {
            _descendigFieldEnum = descendigFieldEnum;
        }

        private Type PopDescendigFieldEnumType()
        {
            Type returnType = _descendigFieldEnum;
            _descendigFieldEnum = null;

            return returnType;
        }

        public void PushConstraint(IConstraint constraint)
		{
			_constraints.Push(constraint);
		}

		public IConstraint PopConstraint()
		{
			return _constraints.Pop();
		}

		public void ApplyConstraint(Func<IConstraint, IConstraint> constraint)
		{
			PushConstraint(constraint(PopConstraint()));
		}

        internal object ResolveValue(object value)
        {
            Type type = PopDescendigFieldEnumType();            
            return (type != null) ? Enum.ToObject(type, value) : value;
        }
    }
}
