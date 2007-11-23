/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

namespace Db4objects.Db4o.Internal.Activation
{
	public sealed class ActivationMode
	{
		public static readonly Db4objects.Db4o.Internal.Activation.ActivationMode ACTIVATE
			 = new Db4objects.Db4o.Internal.Activation.ActivationMode();

		public static readonly Db4objects.Db4o.Internal.Activation.ActivationMode DEACTIVATE
			 = new Db4objects.Db4o.Internal.Activation.ActivationMode();

		public static readonly Db4objects.Db4o.Internal.Activation.ActivationMode PEEK = 
			new Db4objects.Db4o.Internal.Activation.ActivationMode();

		public static readonly Db4objects.Db4o.Internal.Activation.ActivationMode PREFETCH
			 = new Db4objects.Db4o.Internal.Activation.ActivationMode();

		public static readonly Db4objects.Db4o.Internal.Activation.ActivationMode REFRESH
			 = new Db4objects.Db4o.Internal.Activation.ActivationMode();

		private ActivationMode()
		{
		}

		public override string ToString()
		{
			if (IsActivate())
			{
				return "ACTIVATE";
			}
			if (IsDeactivate())
			{
				return "DEACTIVATE";
			}
			if (IsPrefetch())
			{
				return "PREFETCH";
			}
			if (IsRefresh())
			{
				return "REFRESH";
			}
			return "PEEK";
		}

		public bool IsDeactivate()
		{
			return this == DEACTIVATE;
		}

		public bool IsActivate()
		{
			return this == ACTIVATE;
		}

		public bool IsPeek()
		{
			return this == PEEK;
		}

		public bool IsPrefetch()
		{
			return this == PREFETCH;
		}

		public bool IsRefresh()
		{
			return this == REFRESH;
		}
	}
}
