/* Copyright (C) 2004   db4objects Inc.   http://www.db4o.com */

using System;
using System.Collections;

namespace Db4objects.Db4o
{
	internal class P2HashMapKeyIterator : P2HashMapIterator, IEnumerator
	{
		internal P2HashMapKeyIterator(P2HashMap hm) : base(hm){
		}

        public override Object Current{
            get{
                return Entry.Key;
            }
        }
	}
}
