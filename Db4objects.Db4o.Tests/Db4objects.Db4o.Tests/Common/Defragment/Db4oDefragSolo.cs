namespace Db4objects.Db4o.Tests.Common.Defragment
{
	public class Db4oDefragSolo : Db4oUnit.Extensions.Fixtures.Db4oSolo
	{
		public Db4oDefragSolo(Db4oUnit.Extensions.Fixtures.IConfigurationSource configSource
			) : base(configSource)
		{
		}

		protected override Db4objects.Db4o.IObjectContainer CreateDatabase(Db4objects.Db4o.Config.IConfiguration
			 config)
		{
			Sharpen.IO.File origFile = new Sharpen.IO.File(GetAbsolutePath());
			if (origFile.Exists())
			{
				try
				{
					string backupFile = GetAbsolutePath() + ".defrag.backup";
					Db4objects.Db4o.Defragment.IContextIDMapping mapping = new Db4objects.Db4o.Defragment.TreeIDMapping
						();
					Db4objects.Db4o.Defragment.DefragmentConfig defragConfig = new Db4objects.Db4o.Defragment.DefragmentConfig
						(GetAbsolutePath(), backupFile, mapping);
					defragConfig.ForceBackupDelete(true);
					Db4objects.Db4o.Config.IConfiguration clonedConfig = (Db4objects.Db4o.Config.IConfiguration
						)((Db4objects.Db4o.Foundation.IDeepClone)config).DeepClone(null);
					defragConfig.Db4oConfig(clonedConfig);
					Db4objects.Db4o.Defragment.Defragment.Defrag(defragConfig, new _AnonymousInnerClass32
						(this));
				}
				catch (System.IO.IOException e)
				{
					Sharpen.Runtime.PrintStackTrace(e);
				}
			}
			return base.CreateDatabase(config);
		}

		private sealed class _AnonymousInnerClass32 : Db4objects.Db4o.Defragment.IDefragmentListener
		{
			public _AnonymousInnerClass32(Db4oDefragSolo _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public void NotifyDefragmentInfo(Db4objects.Db4o.Defragment.DefragmentInfo info)
			{
				Sharpen.Runtime.Err.WriteLine(info);
			}

			private readonly Db4oDefragSolo _enclosing;
		}

		public override bool Accept(System.Type clazz)
		{
			return !typeof(Db4oUnit.Extensions.Fixtures.IOptOutDefragSolo).IsAssignableFrom(clazz
				);
		}
	}
}
