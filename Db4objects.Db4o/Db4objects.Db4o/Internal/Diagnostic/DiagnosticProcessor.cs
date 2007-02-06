namespace Db4objects.Db4o.Internal.Diagnostic
{
	/// <exclude>FIXME: remove me from the core and make me a facade over Events</exclude>
	public class DiagnosticProcessor : Db4objects.Db4o.Diagnostic.IDiagnosticConfiguration
		, Db4objects.Db4o.Foundation.IDeepClone
	{
		private Db4objects.Db4o.Foundation.Collection4 _listeners;

		public DiagnosticProcessor()
		{
		}

		private DiagnosticProcessor(Db4objects.Db4o.Foundation.Collection4 listeners)
		{
			_listeners = listeners;
		}

		public virtual void AddListener(Db4objects.Db4o.Diagnostic.IDiagnosticListener listener
			)
		{
			if (_listeners == null)
			{
				_listeners = new Db4objects.Db4o.Foundation.Collection4();
			}
			_listeners.Add(listener);
		}

		public virtual void CheckClassHasFields(Db4objects.Db4o.Internal.ClassMetadata yc
			)
		{
			Db4objects.Db4o.Internal.FieldMetadata[] fields = yc.i_fields;
			if (fields != null && fields.Length == 0)
			{
				string name = yc.GetName();
				string[] ignoredPackages = new string[] { "java.util." };
				for (int i = 0; i < ignoredPackages.Length; i++)
				{
					if (name.IndexOf(ignoredPackages[i]) == 0)
					{
						return;
					}
				}
				if (IsDb4oClass(yc))
				{
					return;
				}
				OnDiagnostic(new Db4objects.Db4o.Diagnostic.ClassHasNoFields(name));
			}
		}

		public virtual void CheckUpdateDepth(int depth)
		{
			if (depth > 1)
			{
				OnDiagnostic(new Db4objects.Db4o.Diagnostic.UpdateDepthGreaterOne(depth));
			}
		}

		public virtual object DeepClone(object context)
		{
			return new Db4objects.Db4o.Internal.Diagnostic.DiagnosticProcessor(CloneListeners
				());
		}

		private Db4objects.Db4o.Foundation.Collection4 CloneListeners()
		{
			return _listeners != null ? new Db4objects.Db4o.Foundation.Collection4(_listeners
				) : null;
		}

		public virtual bool Enabled()
		{
			return _listeners != null;
		}

		private bool IsDb4oClass(Db4objects.Db4o.Internal.ClassMetadata yc)
		{
			return Db4objects.Db4o.Internal.Platform4.IsDb4oClass(yc.GetName());
		}

		public virtual void LoadedFromClassIndex(Db4objects.Db4o.Internal.ClassMetadata yc
			)
		{
			if (IsDb4oClass(yc))
			{
				return;
			}
			OnDiagnostic(new Db4objects.Db4o.Diagnostic.LoadedFromClassIndex(yc.GetName()));
		}

		public virtual void DescendIntoTranslator(Db4objects.Db4o.Internal.ClassMetadata 
			parent, string fieldName)
		{
			OnDiagnostic(new Db4objects.Db4o.Diagnostic.DescendIntoTranslator(parent.GetName(
				), fieldName));
		}

		public virtual void NativeQueryUnoptimized(Db4objects.Db4o.Query.Predicate predicate
			)
		{
			OnDiagnostic(new Db4objects.Db4o.Diagnostic.NativeQueryNotOptimized(predicate));
		}

		private void OnDiagnostic(Db4objects.Db4o.Diagnostic.IDiagnostic d)
		{
			if (_listeners == null)
			{
				return;
			}
			System.Collections.IEnumerator i = _listeners.GetEnumerator();
			while (i.MoveNext())
			{
				((Db4objects.Db4o.Diagnostic.IDiagnosticListener)i.Current).OnDiagnostic(d);
			}
		}

		public virtual void RemoveAllListeners()
		{
			_listeners = null;
		}
	}
}
