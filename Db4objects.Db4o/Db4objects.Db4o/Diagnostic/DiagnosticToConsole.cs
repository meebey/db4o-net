namespace Db4objects.Db4o.Diagnostic
{
	/// <summary>prints Diagnostic messsages to the Console.</summary>
	/// <remarks>
	/// prints Diagnostic messsages to the Console.
	/// Install this
	/// <see cref="Db4objects.Db4o.Diagnostic.IDiagnosticListener">Db4objects.Db4o.Diagnostic.IDiagnosticListener
	/// 	</see>
	/// with: <br />
	/// <code>Db4o.configure().diagnostic().addListener(new DiagnosticToConsole());</code><br />
	/// </remarks>
	/// <seealso cref="Db4objects.Db4o.Diagnostic.IDiagnosticConfiguration">Db4objects.Db4o.Diagnostic.IDiagnosticConfiguration
	/// 	</seealso>
	public class DiagnosticToConsole : Db4objects.Db4o.Diagnostic.IDiagnosticListener
	{
		/// <summary>redirects Diagnostic messages to the Console.</summary>
		/// <remarks>redirects Diagnostic messages to the Console.</remarks>
		public virtual void OnDiagnostic(Db4objects.Db4o.Diagnostic.IDiagnostic d)
		{
			System.Console.Out.WriteLine(d.ToString());
		}
	}
}
