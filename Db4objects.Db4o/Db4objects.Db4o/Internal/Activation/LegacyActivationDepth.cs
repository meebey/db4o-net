/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Activation;

namespace Db4objects.Db4o.Internal.Activation
{
	/// <summary>
	/// Activates an object graph to a specific depth respecting any
	/// activation configuration settings that might be in effect.
	/// </summary>
	/// <remarks>
	/// Activates an object graph to a specific depth respecting any
	/// activation configuration settings that might be in effect.
	/// </remarks>
	public class LegacyActivationDepth : ActivationDepthImpl
	{
		private readonly int _depth;

		public LegacyActivationDepth(int depth) : this(depth, ActivationMode.Activate)
		{
		}

		public LegacyActivationDepth(int depth, ActivationMode mode) : base(mode)
		{
			_depth = depth;
		}

		public override IActivationDepth Descend(ClassMetadata metadata)
		{
			if (null == metadata)
			{
				throw new ArgumentNullException();
			}
			return new Db4objects.Db4o.Internal.Activation.LegacyActivationDepth(DescendDepth
				(metadata), _mode);
		}

		private int DescendDepth(ClassMetadata metadata)
		{
			int depth = ConfiguredActivationDepth(metadata) - 1;
			if (metadata.IsValueType())
			{
				// 	We also have to instantiate structs completely every time.
				return Math.Max(1, depth);
			}
			return depth;
		}

		private int ConfiguredActivationDepth(ClassMetadata metadata)
		{
			Config4Class config = metadata.ConfigOrAncestorConfig();
			if (config != null && _mode.IsActivate())
			{
				return config.AdjustActivationDepth(_depth);
			}
			return _depth;
		}

		public override bool RequiresActivation()
		{
			return _depth > 0;
		}
	}
}
