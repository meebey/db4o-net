/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Instrumentation.Api;

namespace Db4objects.Db4o.Instrumentation.Api
{
	/// <summary>A reference to a field.</summary>
	/// <remarks>
	/// A reference to a field. See
	/// <see cref="ReferenceProvider#forField">ReferenceProvider#forField</see>
	/// .
	/// </remarks>
	public interface IFieldRef
	{
		ITypeRef Type
		{
			get;
		}

		string Name
		{
			get;
		}
	}
}
