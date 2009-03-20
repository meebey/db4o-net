/* Copyright (C) 2004   db4objects Inc.   http://www.db4o.com */

using System;

namespace Sharpen.Lang
{
	public class Thread : IRunnable
	{
		private IRunnable _target;

		private string _name;

		private System.Threading.Thread _thread;

		private bool _isDaemon;

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
#if !CF
			if (_thread != null && name != null)
			{
				try
				{
					_thread.Name = _name;
				}
				catch
				{
				}
			}
#endif
		}

		public string GetName()
		{
#if !CF
			return _thread != null ? _thread.Name : _name;
#else
			return "";
#endif
		}

		public static void Sleep(long milliseconds)
		{
			System.Threading.Thread.Sleep((int)milliseconds);
		}

		public void Start()
		{
			_thread = new System.Threading.Thread(new System.Threading.ThreadStart(EntryPoint));
			_thread.IsBackground = _isDaemon;
			if (_name != null)
			{
				SetName(_name);
			}
			_thread.Start();
		}

		public void Join() 
		{
			if (_thread == null)
			{
				string message = "A thread must be available to join";
				throw new InvalidOperationException(message);
			}
			_thread.Join();
		}
		
		public void SetDaemon(bool isDaemon)
		{
			_isDaemon = isDaemon;
		}

		public override bool Equals(object obj)
		{
			Thread other = (obj as Thread);
			if (other == null)
				return false;
			if (other == this)
				return true;
			if (_thread == null)
				return false;
			return _thread == other._thread;
		}

		public override int GetHashCode()
		{
			return _thread == null ? 37 : _thread.GetHashCode();
		}

		private void EntryPoint()
		{
			try
			{
				_target.Run();
			}
			catch (Exception e)
			{
				// don't let an unhandled exception bring
				// the process down
				Sharpen.Runtime.PrintStackTrace(e);
			}
		}
	}
}
