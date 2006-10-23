/* Copyright (C) 2004   db4objects Inc.   http://www.db4o.com */

using System;

namespace Sharpen.Lang
{
	public class Thread : IRunnable
	{
		public const int MIN_PRIORITY = 0;

		private IRunnable _target;

		private string _name;

		private System.Threading.Thread _thread;

		private bool _isDaemon;

		static int idGenerator = 1;

		public Thread()
		{
			_target = this;
		}

		public Thread(IRunnable target)
		{
			this._target = target;
		}

		public Thread(System.Threading.Thread thread)
		{
			this._thread = thread;
		}

		public static Thread CurrentThread()
		{
			return new Thread(System.Threading.Thread.CurrentThread);
		}

		public virtual void Run()
		{
		}

		public void SetName(string name)
		{
			this._name = name;
#if !CF_1_0 && !CF_2_0
			if (_thread != null && name != null)
			{
				try
				{
					_thread.Name = _name;
				}
				catch (Exception ignored)
				{
				}
			}
#endif
		}

		public string GetName()
		{
#if !CF_1_0 && !CF_2_0
			return _thread != null ? _thread.Name : _name;
#else
			return "";
#endif
		}

		public void SetPriority(int priority)
		{
			// TODO: how ?
		}

		public void SetPriority(System.Threading.ThreadPriority priority)
		{
			_thread.Priority = priority;
		}

		public static void Sleep(long milliseconds)
		{
			System.Threading.Thread.Sleep((int)milliseconds);
		}

		public void Start()
		{
			_thread = new System.Threading.Thread(new System.Threading.ThreadStart(EntryPoint));
#if !CF_1_0
			_thread.IsBackground = _isDaemon;
#endif
			if (_name != null)
			{
				SetName(_name);
			}
			_thread.Start();
		}

		public void SetDaemon(bool isDaemon)
		{
			_isDaemon = isDaemon;
		}

		private void EntryPoint()
		{
			_target.Run();
		}

	}
}
