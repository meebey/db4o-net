/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Internal.Activation;

namespace Db4objects.Db4o.Internal.Activation
{
	public sealed class ActivationMode
	{
		public static readonly ActivationMode Activate = new ActivationMode();

		public static readonly ActivationMode Deactivate = new ActivationMode();

		public static readonly ActivationMode Peek = new ActivationMode();

		public static readonly ActivationMode Prefetch = new ActivationMode();

		public static readonly ActivationMode Refresh = new ActivationMode();

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
			return this == Deactivate;
		}

		public bool IsActivate()
		{
			return this == Activate;
		}

		public bool IsPeek()
		{
			return this == Peek;
		}

		public bool IsPrefetch()
		{
			return this == Prefetch;
		}

		public bool IsRefresh()
		{
			return this == Refresh;
		}
	}
}
