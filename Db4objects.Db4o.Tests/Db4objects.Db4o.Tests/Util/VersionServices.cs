/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System.IO;
using Db4objects.Db4o.Ext;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Activation;
using Db4objects.Db4o.Internal.Marshall;
using Sharpen.IO;

namespace Db4objects.Db4o.Tests.Util
{
	public class VersionServices
	{
		public const byte HEADER_30_40 = 123;

		public const byte HEADER_46_57 = 4;

		public const byte HEADER_60 = 100;

		/// <exception cref="IOException"></exception>
		public static byte FileHeaderVersion(string testFile)
		{
			RandomAccessFile raf = new RandomAccessFile(testFile, "r");
			byte[] bytes = new byte[1];
			raf.Read(bytes);
			byte db4oHeaderVersion = bytes[0];
			raf.Close();
			return db4oHeaderVersion;
		}

		public static int SlotHandlerVersion(IExtObjectContainer objectContainer, object 
			obj)
		{
			int id = (int)objectContainer.GetID(obj);
			IObjectInfo objectInfo = objectContainer.GetObjectInfo(obj);
			ObjectContainerBase container = (ObjectContainerBase)objectContainer;
			Transaction trans = container.Transaction();
			BufferImpl buffer = container.ReadReaderByID(trans, id);
			UnmarshallingContext context = new UnmarshallingContext(trans, (ObjectReference)objectInfo
				, Const4.TRANSIENT, false);
			context.Buffer(buffer);
			context.PersistentObject(obj);
			context.ActivationDepth(new LegacyActivationDepth(0));
			context.Read();
			return context.HandlerVersion();
		}
	}
}
