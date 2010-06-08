/* Copyright (C) 2004 - 2009  Versant Inc.  http://www.db4o.com */

using System;
using Sharpen;

namespace Db4objects.Db4o.Foundation
{
	/// <exclude></exclude>
	public class TimeStampIdGenerator
	{
		private const int BitsReservedForCounter = 15;

		private const int BitsReservedForCounterIn48bitId = 6;

		private const int CounterLimit = 64;

		private long _counter;

		private long _lastTime;

		public static long IdToMilliseconds(long id)
		{
			return id >> BitsReservedForCounter;
		}

		public static long MillisecondsToId(long milliseconds)
		{
			return milliseconds << BitsReservedForCounter;
		}

		public TimeStampIdGenerator(long minimumNext)
		{
			InternalSetMinimumNext(minimumNext);
		}

		public TimeStampIdGenerator() : this(0)
		{
		}

		public virtual long Generate()
		{
			long t = Runtime.CurrentTimeMillis();
			if (t > _lastTime)
			{
				_lastTime = t;
				_counter = 0;
				return MillisecondsToId(t);
			}
			UpdateTimeOnCounterLimitOverflow();
			_counter++;
			UpdateTimeOnCounterLimitOverflow();
			return Last();
		}

		private void UpdateTimeOnCounterLimitOverflow()
		{
			if (_counter < CounterLimit)
			{
				return;
			}
			long timeIncrement = _counter / CounterLimit;
			_lastTime += timeIncrement;
			_counter -= (timeIncrement * CounterLimit);
		}

		public virtual long Last()
		{
			return MillisecondsToId(_lastTime) + _counter;
		}

		public virtual bool SetMinimumNext(long newMinimum)
		{
			if (newMinimum <= Last())
			{
				return false;
			}
			InternalSetMinimumNext(newMinimum);
			return true;
		}

		private void InternalSetMinimumNext(long newNext)
		{
			_lastTime = IdToMilliseconds(newNext);
			long timePart = MillisecondsToId(_lastTime);
			_counter = newNext - timePart;
			UpdateTimeOnCounterLimitOverflow();
		}

		public static long Convert64BitIdTo48BitId(long id)
		{
			return Convert(id, BitsReservedForCounter, BitsReservedForCounterIn48bitId);
		}

		public static long Convert48BitIdTo64BitId(long id)
		{
			return Convert(id, BitsReservedForCounterIn48bitId, BitsReservedForCounter);
		}

		private static long Convert(long id, int shiftBitsFrom, int shiftBitsTo)
		{
			long creationTimeInMillis = id >> shiftBitsFrom;
			long timeStampPart = creationTimeInMillis << shiftBitsFrom;
			long counterPerMillisecond = id - timeStampPart;
			if (counterPerMillisecond >= CounterLimit)
			{
				throw new InvalidOperationException("ID can't be converted");
			}
			return (creationTimeInMillis << shiftBitsTo) + counterPerMillisecond;
		}
	}
}
