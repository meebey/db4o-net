/* Copyright (C) 2004   db4objects Inc.   http://www.db4o.com */

using System;

namespace Sharpen.Lang
{
	public class Thread : IRunnable
	{
		public const int MinPriority = 1;
		public const int MaxPriority = 10;

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

		private static readonly LocalDataStoreSlot _current = System.Threading.Thread.AllocateDataSlot();

		public static Thread CurrentThread()
		{
			Thread current = (Thread)System.Threading.Thread.GetData(_current);
			if (current == null)
			{
				current = new Thread(System.Threading.Thread.CurrentThread);
				System.Threading.Thread.SetData(_current, current);
			}
			return current;
		}

		public virtual void Run()
		{
		}

		public void SetName(string name)
		{
			this._name = name;
#if !CF_2_0
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
#if !CF_2_0
			return _thread != null ? _thread.Name : _name;
#else
			return "";
#endif
		}

		public void SetPriority(int priority)
		{
			// java priority is between 1 and 10, ThreadPriority is between 0 and 5
			if (priority < MinPriority || priority > MaxPriority)
			{
				string message = string.Format("Thread priority must be between {0} and {1}", MinPriority, MaxPriority);
#if !CF_2_0
				throw new ArgumentOutOfRangeException("priority", priority, message);
#else
				throw new ArgumentOutOfRangeException(message);
#endif
			}

			_thread.Priority = (System.Threading.ThreadPriority) (priority / 2);
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
			_thread.IsBackground = _isDaemon;
			if (_name != null)
			{
				SetName(_name);
			}
			_thread.Start();
		}

		public void Join() 
		{
			_thread.Join();
		}
		
		public void SetDaemon(bool isDaemon)
		{
			_isDaemon = isDaemon;
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
