/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System;
using System.Collections;
using Db4objects.Db4o.Foundation;

namespace Db4objects.Db4o.Foundation
{
	public class Collections4
	{
		public static ISequence4 UnmodifiableList(ISequence4 orig)
		{
			return new Collections4.UnmodifiableSequence4(orig);
		}

		private class UnmodifiableSequence4 : ISequence4
		{
			private ISequence4 _sequence;

			public UnmodifiableSequence4(ISequence4 sequence)
			{
				_sequence = sequence;
			}

			public virtual void Add(object element)
			{
				throw new InvalidOperationException();
			}

			public virtual bool IsEmpty()
			{
				return _sequence.IsEmpty();
			}

			public virtual IEnumerator GetEnumerator()
			{
				return _sequence.GetEnumerator();
			}

			public virtual object Get(int index)
			{
				return _sequence.Get(index);
			}
		}
	}
}
