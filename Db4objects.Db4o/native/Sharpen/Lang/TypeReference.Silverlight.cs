/* Copyright (C) 2009   Versant Inc.   http://www.db4o.com */

using System;
using System.Reflection;

namespace Sharpen.Lang
{
	public abstract partial class TypeReference
	{
#if SILVERLIGHT
		public static event EventHandler<AssemblyResolveArgs> AssemblyResolve;

		protected static Assembly RaiseAssemblyResolveEvent(object sender, string assemblyName)
		{
			EventHandler<AssemblyResolveArgs> handler = AssemblyResolve;
			if (handler == null)
			{
				throw new InvalidOperationException("Subscribe to TypeReference.AssemblyResolve event and return the requested assembly.");
			}

			AssemblyResolveArgs args = new AssemblyResolveArgs(assemblyName);
			handler(sender, args);
			
			return args.Assembly;
		}

#endif
	}

#if SILVERLIGHT
	public class AssemblyResolveArgs : EventArgs
	{
		public string Name
		{
			get { return _name; }
		}

		public Assembly Assembly
		{
			set { _assembly = value; }
			get { return _assembly; }
		}

		public AssemblyResolveArgs(string name)
		{
			_name = name;
		}

		private readonly string _name;
		private Assembly _assembly;
	}
#endif
}
