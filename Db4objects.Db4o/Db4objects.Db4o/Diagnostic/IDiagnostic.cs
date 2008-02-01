/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Diagnostic;

namespace Db4objects.Db4o.Diagnostic
{
	/// <summary>
	/// Marker interface for Diagnostic messages&lt;br&gt;&lt;br&gt;
	/// Diagnostic system can be enabled on a running db4o database
	/// to notify a user about possible problems or misconfigurations.
	/// </summary>
	/// <remarks>
	/// Marker interface for Diagnostic messages&lt;br&gt;&lt;br&gt;
	/// Diagnostic system can be enabled on a running db4o database
	/// to notify a user about possible problems or misconfigurations. Diagnostic
	/// messages must implement this interface and are usually derived from
	/// <see cref="DiagnosticBase">DiagnosticBase</see>
	/// class. A separate Diagnostic implementation
	/// should be used for each problem.
	/// </remarks>
	/// <seealso cref="DiagnosticBase">DiagnosticBase</seealso>
	/// <seealso cref="IDiagnosticConfiguration">IDiagnosticConfiguration</seealso>
	public interface IDiagnostic
	{
	}
}
