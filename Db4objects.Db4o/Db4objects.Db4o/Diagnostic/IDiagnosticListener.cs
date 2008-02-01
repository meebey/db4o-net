/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Diagnostic;

namespace Db4objects.Db4o.Diagnostic
{
	/// <summary>listens to Diagnostic messages.</summary>
	/// <remarks>
	/// listens to Diagnostic messages.
	/// &lt;br&gt;&lt;br&gt;Create a class that implements this listener interface and add
	/// the listener by calling Db4o.configure().diagnostic().addListener().
	/// </remarks>
	/// <seealso cref="IDiagnosticConfiguration">IDiagnosticConfiguration</seealso>
	public interface IDiagnosticListener
	{
		/// <summary>this method will be called with Diagnostic messages.</summary>
		/// <remarks>this method will be called with Diagnostic messages.</remarks>
		void OnDiagnostic(IDiagnostic d);
	}
}
