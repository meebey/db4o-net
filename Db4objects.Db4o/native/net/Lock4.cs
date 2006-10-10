/* Copyright (C) 2004   db4objects Inc.   http://www.db4o.com */

using System;
using System.Threading;
namespace Db4objects.Db4o.Foundation {

    internal class Lock4 {
    
        public void Awake() {
            Monitor.Pulse(this);
        }

        public Object Run(Closure4 closure) {
            lock (this) {
                return closure.Run();
            }
        }
    
        public void Snooze(long timeout) {
            Monitor.Wait(this, (int)timeout);
        }
    }
}