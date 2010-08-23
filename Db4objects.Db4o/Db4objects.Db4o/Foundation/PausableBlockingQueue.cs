/* Copyright (C) 2004 - 2009  Versant Inc.  http://www.db4o.com */

using System;
using Db4objects.Db4o.Foundation;

namespace Db4objects.Db4o.Foundation
{
	public class PausableBlockingQueue : BlockingQueue, IPausableBlockingQueue4
	{
		private volatile bool _paused = false;

		public virtual void Pause()
		{
			_paused = true;
		}

		public virtual void Resume()
		{
			lock (this)
			{
				_paused = false;
				Sharpen.Runtime.Notify(this);
			}
		}

		/// <exception cref="Db4objects.Db4o.Foundation.BlockingQueueStoppedException"></exception>
		public override object Next()
		{
			WaitForNext();
			if (_paused)
			{
				lock (this)
				{
					if (_paused)
					{
						try
						{
							Sharpen.Runtime.Wait(this);
						}
						catch (Exception)
						{
						}
					}
				}
			}
			return base.Next();
		}

		/// <exception cref="Db4objects.Db4o.Foundation.BlockingQueueStoppedException"></exception>
		public override object Next(long timeout)
		{
			if (!WaitForNext(timeout))
			{
				return null;
			}
			if (_paused)
			{
				lock (this)
				{
					if (_paused)
					{
						try
						{
							Sharpen.Runtime.Wait(this);
						}
						catch (Exception)
						{
						}
					}
				}
			}
			return base.Next(timeout);
		}

		public override void Stop()
		{
			if (_paused)
			{
				lock (this)
				{
					if (_paused)
					{
						Sharpen.Runtime.NotifyAll(this);
					}
				}
			}
			base.Stop();
		}

		public virtual bool IsPaused()
		{
			return _paused;
		}

		public virtual object TryNext()
		{
			return _lock.Run(new _IClosure4_65(this));
		}

		private sealed class _IClosure4_65 : IClosure4
		{
			public _IClosure4_65(PausableBlockingQueue _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public object Run()
			{
				return this._enclosing.IsPaused() ? null : this._enclosing.HasNext() ? this._enclosing
					.Next() : null;
			}

			private readonly PausableBlockingQueue _enclosing;
		}
	}
}
