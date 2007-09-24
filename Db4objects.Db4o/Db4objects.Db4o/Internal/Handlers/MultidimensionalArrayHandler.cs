/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Handlers;
using Db4objects.Db4o.Internal.Marshall;
using Db4objects.Db4o.Internal.Query.Processor;
using Db4objects.Db4o.Marshall;
using Db4objects.Db4o.Reflect;

namespace Db4objects.Db4o.Internal.Handlers
{
	/// <summary>n-dimensional array</summary>
	/// <exclude></exclude>
	public class MultidimensionalArrayHandler : ArrayHandler
	{
		public MultidimensionalArrayHandler(ObjectContainerBase stream, ITypeHandler4 a_handler
			, bool a_isPrimitive) : base(stream, a_handler, a_isPrimitive)
		{
		}

		protected MultidimensionalArrayHandler(ITypeHandler4 template) : base(template)
		{
		}

		public sealed override object[] AllElements(object array)
		{
			return AllElements(ArrayReflector(), array);
		}

		public static object[] AllElements(IReflectArray reflectArray, object array)
		{
			int[] dim = reflectArray.Dimensions(array);
			object[] flat = new object[ElementCount(dim)];
			reflectArray.Flatten(array, dim, 0, flat, 0);
			return flat;
		}

		public int ElementCount(Transaction a_trans, Db4objects.Db4o.Internal.Buffer a_bytes
			)
		{
			return ElementCount(ReadDimensions(a_trans, a_bytes, ReflectClassByRef.IGNORED));
		}

		private static int ElementCount(int[] a_dim)
		{
			int elements = a_dim[0];
			for (int i = 1; i < a_dim.Length; i++)
			{
				elements = elements * a_dim[i];
			}
			return elements;
		}

		public sealed override byte Identifier()
		{
			return Const4.YAPARRAYN;
		}

		public override int OwnLength(object obj)
		{
			int[] dim = ArrayReflector().Dimensions(obj);
			return Const4.OBJECT_LENGTH + (Const4.INT_LENGTH * (2 + dim.Length));
		}

		public sealed override object Read1(MarshallerFamily mf, StatefulBuffer reader)
		{
			IntArrayByRef dimensions = new IntArrayByRef();
			object arr = ReadCreate(reader.GetTransaction(), reader, dimensions);
			if (arr != null)
			{
				object[] objects = new object[ElementCount(dimensions.value)];
				for (int i = 0; i < objects.Length; i++)
				{
					objects[i] = _handler.Read(mf, reader, true);
				}
				ArrayReflector().Shape(objects, 0, arr, dimensions.value, 0);
			}
			return arr;
		}

		protected override int ReadElementsDefrag(BufferPair readers)
		{
			int numDimensions = base.ReadElementsDefrag(readers);
			int[] dimensions = new int[numDimensions];
			for (int i = 0; i < numDimensions; i++)
			{
				dimensions[i] = readers.ReadInt();
			}
			return ElementCount(dimensions);
		}

		public override void ReadSubCandidates(int handlerVersion, Db4objects.Db4o.Internal.Buffer
			 reader, QCandidates candidates)
		{
			IntArrayByRef dimensions = new IntArrayByRef();
			object arr = ReadCreate(candidates.i_trans, reader, dimensions);
			if (arr == null)
			{
				return;
			}
			ReadSubCandidates(handlerVersion, reader, candidates, ElementCount(dimensions.value
				));
		}

		private object ReadCreate(Transaction trans, IReadBuffer buffer, IntArrayByRef dimensions
			)
		{
			ReflectClassByRef clazz = new ReflectClassByRef();
			dimensions.value = ReadDimensions(trans, buffer, clazz);
			if (_isPrimitive)
			{
				return ArrayReflector().NewInstance(PrimitiveClassReflector(), dimensions.value);
			}
			if (clazz.value != null)
			{
				return ArrayReflector().NewInstance(clazz.value, dimensions.value);
			}
			return null;
		}

		private int[] ReadDimensions(Transaction trans, IReadBuffer buffer, ReflectClassByRef
			 clazz)
		{
			int[] dim = new int[ReadElementsAndClass(trans, buffer, clazz)];
			for (int i = 0; i < dim.Length; i++)
			{
				dim[i] = buffer.ReadInt();
			}
			return dim;
		}

		private object Element(object a_array, int a_position)
		{
			try
			{
				return ArrayReflector().Get(a_array, a_position);
			}
			catch (Exception)
			{
				return null;
			}
		}

		public override object Read(IReadContext context)
		{
			IntArrayByRef dimensions = new IntArrayByRef();
			object array = ReadCreate(context.Transaction(), context, dimensions);
			if (array != null)
			{
				object[] objects = new object[ElementCount(dimensions.value)];
				for (int i = 0; i < objects.Length; i++)
				{
					objects[i] = context.ReadObject(_handler);
				}
				ArrayReflector().Shape(objects, 0, array, dimensions.value, 0);
			}
			return array;
		}

		public override void Write(IWriteContext context, object obj)
		{
			int classID = ClassID(obj);
			context.WriteInt(classID);
			int[] dim = ArrayReflector().Dimensions(obj);
			context.WriteInt(dim.Length);
			for (int i = 0; i < dim.Length; i++)
			{
				context.WriteInt(dim[i]);
			}
			object[] objects = AllElements(obj);
			for (int i = 0; i < objects.Length; i++)
			{
				context.WriteObject(_handler, Element(objects, i));
			}
		}
	}
}
