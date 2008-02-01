/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Diagnostic;

namespace Db4objects.Db4o.Diagnostic
{
	/// <summary>prints Diagnostic messsages to the Console.</summary>
	/// <remarks>
	/// prints Diagnostic messsages to the Console.
	/// Install this
	/// <see cref="IDiagnosticListener">IDiagnosticListener</see>
	/// with: &lt;br&gt;
	/// &lt;code&gt;Db4o.configure().diagnostic().addListener(new DiagnosticToConsole());&lt;/code&gt;&lt;br&gt;
	/// </remarks>
	/// <seealso cref="IDiagnosticConfiguration">IDiagnosticConfiguration</seealso>
	public class DiagnosticToConsole : IDiagnosticListener
	{
		/// <summary>redirects Diagnostic messages to the Console.</summary>
		/// <remarks>redirects Diagnostic messages to the Console.</remarks>
		public virtual void OnDiagnostic(IDiagnostic d)
		{
			Sharpen.Runtime.Out.WriteLine(d.ToString());
		}
	}
}
