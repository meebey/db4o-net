/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4oUnit;
using Db4oUnit.Extensions;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Activation;
using Db4objects.Db4o.Internal.Marshall;
using Db4objects.Db4o.Internal.Slots;
using Db4objects.Db4o.Tests.Common.Internal;

namespace Db4objects.Db4o.Tests.Common.Internal
{
	public class MarshallingContextTestCase : AbstractDb4oTestCase
	{
		public static void Main(string[] arguments)
		{
			new MarshallingContextTestCase().RunSolo();
		}

		public class StringItem
		{
			public string _name;

			public StringItem(string name)
			{
				_name = name;
			}
		}

		public class StringIntItem
		{
			public string _name;

			public int _int;

			public StringIntItem(string name, int i)
			{
				_name = name;
				_int = i;
			}
		}

		public class StringIntBooleanItem
		{
			public string _name;

			public int _int;

			public bool _bool;

			public StringIntBooleanItem(string name, int i, bool @bool)
			{
				_name = name;
				_int = i;
				_bool = @bool;
			}
		}

		public virtual void TestStringItem()
		{
			MarshallingContextTestCase.StringItem writtenItem = new MarshallingContextTestCase.StringItem
				("one");
			MarshallingContextTestCase.StringItem readItem = (MarshallingContextTestCase.StringItem
				)WriteRead(writtenItem);
			Assert.AreEqual(writtenItem._name, readItem._name);
		}

		public virtual void TestStringIntItem()
		{
			MarshallingContextTestCase.StringIntItem writtenItem = new MarshallingContextTestCase.StringIntItem
				("one", 777);
			MarshallingContextTestCase.StringIntItem readItem = (MarshallingContextTestCase.StringIntItem
				)WriteRead(writtenItem);
			Assert.AreEqual(writtenItem._name, readItem._name);
			Assert.AreEqual(writtenItem._int, readItem._int);
		}

		public virtual void TestStringIntBooleanItem()
		{
			MarshallingContextTestCase.StringIntBooleanItem writtenItem = new MarshallingContextTestCase.StringIntBooleanItem
				("one", 777, true);
			MarshallingContextTestCase.StringIntBooleanItem readItem = (MarshallingContextTestCase.StringIntBooleanItem
				)WriteRead(writtenItem);
			Assert.AreEqual(writtenItem._name, readItem._name);
			Assert.AreEqual(writtenItem._int, readItem._int);
			Assert.AreEqual(writtenItem._bool, readItem._bool);
		}

		private object WriteRead(object obj)
		{
			int imaginativeID = 500;
			ObjectReference @ref = new ObjectReference(ClassMetadataForObject(obj), imaginativeID
				);
			@ref.SetObject(obj);
			ObjectMarshaller marshaller = MarshallerFamily.Current()._object;
			MarshallingContext marshallingContext = new MarshallingContext(Trans(), @ref, int.MaxValue
				, true);
			marshaller.Marshall(@ref.GetObject(), marshallingContext);
			Pointer4 pointer = marshallingContext.AllocateSlot();
			BufferImpl buffer = marshallingContext.ToWriteBuffer(pointer);
			buffer.Seek(0);
			UnmarshallingContext unmarshallingContext = new UnmarshallingContext(Trans(), @ref
				, Const4.AddToIdTree, false);
			unmarshallingContext.Buffer(buffer);
			unmarshallingContext.ActivationDepth(new LegacyActivationDepth(5));
			return unmarshallingContext.Read();
		}

		private ClassMetadata ClassMetadataForObject(object obj)
		{
			return Stream().ProduceClassMetadata(Reflector().ForObject(obj));
		}
	}
}
