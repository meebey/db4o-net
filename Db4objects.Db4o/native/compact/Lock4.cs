/* Copyright (C) 2004   db4objects Inc.   http://www.db4o.com */

using System;
using System.Threading;

namespace Db4objects.Db4o.Foundation
{
#if CF_1_0 || CF_2_0
    public class Lock4
    {
        private volatile Thread lockedByThread;

        private volatile Thread waitReleased;
        private volatile Thread closureReleased;

        AutoResetEvent waitEvent = new AutoResetEvent(false);
        AutoResetEvent closureEvent = new AutoResetEvent(false);

        public Object Run(IClosure4 closure4)
        {
            EnterClosure();
            Object ret;
            try
            {
                ret = closure4.Run();
            }
            catch (Exception e)
            {
                AwakeClosure();
                throw;
            }
            AwakeClosure();
            return ret;
        }

        public void Snooze(long l)
        {
            AwakeClosure();
            WaitWait();
            EnterClosure();
        }

        public void Awake()
        {
            AwakeWait();
        }

        private void AwakeWait()
        {
            lock (this)
            {
                waitReleased = Thread.CurrentThread;
                waitEvent.Set();
                Thread.Sleep(0);
                if (waitReleased == Thread.CurrentThread)
                {
                    waitEvent.Reset();
                }
            }
        }

        private void AwakeClosure()
        {
            lock (this)
            {
                RemoveLock();
                closureReleased = Thread.CurrentThread;
                closureEvent.Set();
                Thread.Sleep(0);
                if (closureReleased == Thread.CurrentThread)
                {
                    closureEvent.Reset();
                }
            }
        }

        private void WaitWait()
        {
            waitEvent.WaitOne();
            waitReleased = Thread.CurrentThread;
        }

        private void WaitClosure()
        {
            closureEvent.WaitOne();
            closureReleased = Thread.CurrentThread;
        }

        private void EnterClosure()
        {
            while (lockedByThread != Thread.CurrentThread)
            {
                while (!SetLock())
                {
                    WaitClosure();
                }
            }
        }

        private bool SetLock()
        {
            lock (this)
            {
                if (lockedByThread == null)
                {
                    lockedByThread = Thread.CurrentThread;
                    return true;
                }
                return false;
            }
        }

        private void RemoveLock()
        {
            lock (this)
            {
                if (lockedByThread == Thread.CurrentThread)
                {
                    lockedByThread = null;
                }
            }
        }
    }
#endif
}