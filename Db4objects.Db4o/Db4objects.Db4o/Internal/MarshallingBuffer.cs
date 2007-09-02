/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System.Collections;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Marshall;
using Sharpen;

namespace Db4objects.Db4o.Internal
{
	/// <exclude></exclude>
	public class MarshallingBuffer : IWriteBuffer
	{
		private const int SIZE_NEEDED = Const4.LONG_LENGTH;

		private const int LINK_LENGTH = Const4.INT_LENGTH + Const4.ID_LENGTH;

		private const int NO_PARENT = -1;

		private Db4objects.Db4o.Internal.Buffer _delegate;

		private int _lastOffSet;

		private int _addressInParent = NO_PARENT;

		private List4 _children;

		private List4 _indexEntries;

		public virtual int Length()
		{
			return Offset();
		}

		public virtual int Offset()
		{
			if (_delegate == null)
			{
				return 0;
			}
			return _delegate.Offset();
		}

		public virtual void WriteByte(byte b)
		{
			PrepareWrite();
			_delegate.WriteByte(b);
		}

		public virtual void WriteBytes(byte[] bytes)
		{
			PrepareWrite(bytes.Length);
			_delegate.WriteBytes(bytes);
		}

		public virtual void WriteInt(int i)
		{
			PrepareWrite();
			_delegate.WriteInt(i);
		}

		public virtual void WriteLong(long l)
		{
			PrepareWrite();
			_delegate.WriteLong(l);
		}

		private void PrepareWrite()
		{
			PrepareWrite(SIZE_NEEDED);
		}

		private void PrepareWrite(int sizeNeeded)
		{
			if (_delegate == null)
			{
				_delegate = new Db4objects.Db4o.Internal.Buffer(sizeNeeded);
			}
			_lastOffSet = _delegate.Offset();
			if (RemainingSize() < sizeNeeded)
			{
				Resize(sizeNeeded);
			}
		}

		private int RemainingSize()
		{
			return _delegate.Length() - _delegate.Offset();
		}

		private void Resize(int sizeNeeded)
		{
			int newSize = _delegate.Length() * 2;
			if (newSize - _lastOffSet < sizeNeeded)
			{
				newSize += sizeNeeded;
			}
			Db4objects.Db4o.Internal.Buffer temp = new Db4objects.Db4o.Internal.Buffer(newSize
				);
			temp.Offset(_lastOffSet);
			_delegate.CopyTo(temp, 0, 0, _lastOffSet);
			_delegate = temp;
		}

		public virtual void TransferLastWriteTo(MarshallingBuffer other)
		{
			other._addressInParent = _lastOffSet;
			int length = _delegate.Offset() - _lastOffSet;
			other.PrepareWrite(length);
			int otherOffset = other._delegate.Offset();
			System.Array.Copy(_delegate._buffer, _lastOffSet, other._delegate._buffer, otherOffset
				, length);
			_delegate.Offset(_lastOffSet);
			other._delegate.Offset(otherOffset + length);
			other._lastOffSet = otherOffset;
		}

		public virtual void TransferContentTo(Db4objects.Db4o.Internal.Buffer buffer)
		{
			System.Array.Copy(_delegate._buffer, 0, buffer._buffer, buffer._offset, Length());
			buffer._offset += Length();
		}

		public virtual Db4objects.Db4o.Internal.Buffer TestDelegate()
		{
			return _delegate;
		}

		public virtual MarshallingBuffer AddChild(bool reserveLinkSpace)
		{
			MarshallingBuffer child = new MarshallingBuffer();
			child._addressInParent = Offset();
			_children = new List4(_children, child);
			if (reserveLinkSpace)
			{
				ReserveChildLinkSpace();
			}
			return child;
		}

		public virtual void ReserveChildLinkSpace()
		{
			PrepareWrite(LINK_LENGTH);
			_delegate.IncrementOffset(LINK_LENGTH);
		}

		public virtual void MergeChildren(int linkOffset)
		{
			MergeChildren(this, this, linkOffset);
		}

		private static void MergeChildren(MarshallingBuffer writeBuffer, MarshallingBuffer
			 parentBuffer, int linkOffset)
		{
			if (parentBuffer._children == null)
			{
				return;
			}
			IEnumerator i = new Iterator4Impl(parentBuffer._children);
			while (i.MoveNext())
			{
				Merge(writeBuffer, parentBuffer, (MarshallingBuffer)i.Current, linkOffset);
			}
		}

		private static void Merge(MarshallingBuffer writeBuffer, MarshallingBuffer parentBuffer
			, MarshallingBuffer childBuffer, int linkOffset)
		{
			int childLength = childBuffer.Length();
			int childPosition = writeBuffer.Offset();
			writeBuffer.Reserve(childLength);
			MergeChildren(writeBuffer, childBuffer, linkOffset);
			int savedWriteBufferOffset = writeBuffer.Offset();
			writeBuffer.Seek(childPosition);
			childBuffer.TransferContentTo(writeBuffer._delegate);
			writeBuffer.Seek(savedWriteBufferOffset);
			parentBuffer.WriteLink(childBuffer, childPosition + linkOffset, childLength);
		}

		public virtual void Seek(int offset)
		{
			_delegate.Offset(offset);
		}

		private void Reserve(int length)
		{
			PrepareWrite(length);
			_delegate.Offset(_delegate.Offset() + length);
		}

		private void WriteLink(MarshallingBuffer child, int position, int length)
		{
			int offset = Offset();
			_delegate.Offset(child._addressInParent);
			_delegate.WriteInt(position);
			_delegate.WriteInt(length);
			_delegate.Offset(offset);
		}

		public virtual void DebugDecrementLastOffset(int count)
		{
			_lastOffSet -= count;
		}

		public virtual bool HasParent()
		{
			return _addressInParent != NO_PARENT;
		}

		public virtual void AddIndexEntry(FieldMetadata fieldMetadata)
		{
		}
	}
}
