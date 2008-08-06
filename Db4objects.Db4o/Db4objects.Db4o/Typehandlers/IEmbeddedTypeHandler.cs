/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Typehandlers;

namespace Db4objects.Db4o.Typehandlers
{
	/// <summary>
	/// marker interface to mark TypeHandlers that marshall
	/// objects to the parent slot and do not create objects
	/// with own identity.
	/// </summary>
	/// <remarks>
	/// marker interface to mark TypeHandlers that marshall
	/// objects to the parent slot and do not create objects
	/// with own identity.
	/// </remarks>
	public interface IEmbeddedTypeHandler : ITypeHandler4
	{
	}
}
