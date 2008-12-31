/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

namespace Db4objects.Db4o.Diagnostic
{
	/// <summary>
	/// Marker interface for Diagnostic messages<br /><br />
	/// Diagnostic system can be enabled on a running db4o database
	/// to notify a user about possible problems or misconfigurations.
	/// </summary>
	/// <remarks>
	/// Marker interface for Diagnostic messages<br /><br />
	/// Diagnostic system can be enabled on a running db4o database
	/// to notify a user about possible problems or misconfigurations. Diagnostic
	/// messages must implement this interface and are usually derived from
	/// <see cref="Db4objects.Db4o.Diagnostic.DiagnosticBase">Db4objects.Db4o.Diagnostic.DiagnosticBase
	/// 	</see>
	/// class. A separate Diagnostic implementation
	/// should be used for each problem.
	/// </remarks>
	/// <seealso cref="Db4objects.Db4o.Diagnostic.DiagnosticBase">Db4objects.Db4o.Diagnostic.DiagnosticBase
	/// 	</seealso>
	/// <seealso cref="Db4objects.Db4o.Diagnostic.IDiagnosticConfiguration">Db4objects.Db4o.Diagnostic.IDiagnosticConfiguration
	/// 	</seealso>
	public interface IDiagnostic
	{
	}
}
