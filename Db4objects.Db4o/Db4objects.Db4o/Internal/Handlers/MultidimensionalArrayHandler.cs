/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System.Collections;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Handlers;
using Db4objects.Db4o.Internal.Query.Processor;
using Db4objects.Db4o.Marshall;
using Db4objects.Db4o.Reflect;

namespace Db4objects.Db4o.Internal.Handlers
{
	/// <summary>n-dimensional array</summary>
	/// <exclude></exclude>
	public class MultidimensionalArrayHandler : ArrayHandler
	{
		public MultidimensionalArrayHandler(ITypeHandler4 a_handler, bool a_isPrimitive) : 
			base(a_handler, a_isPrimitive)
		{
		}

		public MultidimensionalArrayHandler()
		{
		}

		// required for reflection cloning
		public sealed override IEnumerator AllElements(ObjectContainerBase container, object
			 array)
		{
			return AllElements(ArrayReflector(container), array);
		}

		public static IEnumerator AllElements(IReflectArray reflectArray, object array)
		{
			// TODO: replace array copying code with iteration
			int[] dim = reflectArray.Dimensions(array);
			object[] flat = new object[ElementCount(dim)];
			reflectArray.Flatten(array, dim, 0, flat, 0);
			return new ArrayIterator4(flat);
		}

		public sealed override int ElementCount(Transaction trans, IReadBuffer buffer)
		{
			return ElementCount(ReadDimensions(trans, buffer, ReflectClassByRef.Ignored));
		}

		protected static int ElementCount(int[] a_dim)
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
			return Const4.Yaparrayn;
		}

		public virtual int OwnLength(ObjectContainerBase container, object obj)
		{
			int[] dim = ArrayReflector(container).Dimensions(obj);
			return Const4.ObjectLength + (Const4.IntLength * (2 + dim.Length));
		}

		protected override int ReadElementsDefrag(IDefragmentContext context)
		{
			int numDimensions = base.ReadElementsDefrag(context);
			int[] dimensions = new int[numDimensions];
			for (int i = 0; i < numDimensions; i++)
			{
				dimensions[i] = context.ReadInt();
			}
			return ElementCount(dimensions);
		}

		public override void ReadSubCandidates(int handlerVersion, ByteArrayBuffer reader
			, QCandidates candidates)
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

		protected virtual object ReadCreate(Transaction trans, IReadBuffer buffer, IntArrayByRef
			 dimensions)
		{
			ReflectClassByRef classByRef = new ReflectClassByRef();
			dimensions.value = ReadDimensions(trans, buffer, classByRef);
			IReflectClass clazz = NewInstanceReflectClass(trans.Reflector(), classByRef);
			if (clazz == null)
			{
				return null;
			}
			return ArrayReflector(Container(trans)).NewInstance(clazz, dimensions.value);
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

		public override object Read(IReadContext context)
		{
			IntArrayByRef dimensions = new IntArrayByRef();
			object array = ReadCreate(context.Transaction(), context, dimensions);
			if (array != null)
			{
				object[] objects = new object[ElementCount(dimensions.value)];
				for (int i = 0; i < objects.Length; i++)
				{
					objects[i] = context.ReadObject(DelegateTypeHandler());
				}
				ArrayReflector(Container(context)).Shape(objects, 0, array, dimensions.value, 0);
			}
			return array;
		}

		public override void Write(IWriteContext context, object obj)
		{
			int classID = ClassID(Container(context), obj);
			context.WriteInt(classID);
			int[] dim = ArrayReflector(Container(context)).Dimensions(obj);
			context.WriteInt(dim.Length);
			for (int i = 0; i < dim.Length; i++)
			{
				context.WriteInt(dim[i]);
			}
			IEnumerator objects = AllElements(Container(context), obj);
			while (objects.MoveNext())
			{
				context.WriteObject(DelegateTypeHandler(), objects.Current);
			}
		}

		public override ITypeHandler4 GenericTemplate()
		{
			return new Db4objects.Db4o.Internal.Handlers.MultidimensionalArrayHandler();
		}
	}
}
