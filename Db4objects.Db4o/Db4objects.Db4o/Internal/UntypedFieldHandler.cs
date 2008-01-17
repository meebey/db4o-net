/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Ext;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Activation;
using Db4objects.Db4o.Internal.Handlers;
using Db4objects.Db4o.Internal.Marshall;
using Db4objects.Db4o.Marshall;

namespace Db4objects.Db4o.Internal
{
	public class UntypedFieldHandler : ClassMetadata, IBuiltinTypeHandler
	{
		public UntypedFieldHandler(ObjectContainerBase container) : base(container, container
			._handlers.IclassObject)
		{
		}

		public override void CascadeActivation(Transaction trans, object onObject, IActivationDepth
			 depth)
		{
			ClassMetadata classMetadata = ForObject(trans, onObject, false);
			if (classMetadata != null)
			{
				classMetadata.CascadeActivation(trans, onObject, depth);
			}
		}

		/// <exception cref="Db4oIOException"></exception>
		public override void Delete(IDeleteContext context)
		{
			int payLoadOffset = context.ReadInt();
			if (context.IsLegacyHandlerVersion())
			{
				context.DefragmentRecommended();
				return;
			}
			if (payLoadOffset <= 0)
			{
				return;
			}
			int linkOffset = context.Offset();
			context.Seek(payLoadOffset);
			int classMetadataID = context.ReadInt();
			ClassMetadata classMetadata = ((ObjectContainerBase)context.ObjectContainer()).ClassMetadataForId
				(classMetadataID);
			if (classMetadata != null)
			{
				classMetadata.Delete(context);
			}
			context.Seek(linkOffset);
		}

		public override int GetID()
		{
			return Handlers4.UntypedId;
		}

		public override bool HasField(ObjectContainerBase a_stream, string a_path)
		{
			return a_stream.ClassCollection().FieldExists(a_path);
		}

		public override bool HasClassIndex()
		{
			return false;
		}

		public override bool HoldsAnyClass()
		{
			return true;
		}

		public override bool IsStrongTyped()
		{
			return false;
		}

		public override ITypeHandler4 ReadArrayHandler(Transaction a_trans, MarshallerFamily
			 mf, BufferImpl[] a_bytes)
		{
			return mf._untyped.ReadArrayHandler(a_trans, a_bytes);
		}

		public override ObjectID ReadObjectID(IInternalReadContext context)
		{
			int payloadOffset = context.ReadInt();
			if (payloadOffset == 0)
			{
				return ObjectID.IsNull;
			}
			ClassMetadata classMetadata = ReadClassMetadata(context, payloadOffset);
			if (classMetadata == null)
			{
				return ObjectID.IsNull;
			}
			SeekSecondaryOffset(context, classMetadata);
			return classMetadata.ReadObjectID(context);
		}

		public override void Defragment(IDefragmentContext context)
		{
			int payLoadOffSet = context.ReadInt();
			if (payLoadOffSet == 0)
			{
				return;
			}
			int linkOffSet = context.Offset();
			context.Seek(payLoadOffSet);
			int classMetadataID = context.CopyIDReturnOriginalID();
			ClassMetadata classMetadata = context.ClassMetadataForId(classMetadataID);
			if (classMetadata != null)
			{
				classMetadata.Defragment(context);
			}
			context.Seek(linkOffSet);
		}

		private bool IsArray(ITypeHandler4 handler)
		{
			if (handler is ClassMetadata)
			{
				return ((ClassMetadata)handler).IsArray();
			}
			return handler is ArrayHandler;
		}

		public override object Read(IReadContext readContext)
		{
			IInternalReadContext context = (IInternalReadContext)readContext;
			int payloadOffset = context.ReadInt();
			if (payloadOffset == 0)
			{
				return null;
			}
			int savedOffSet = context.Offset();
			ClassMetadata classMetadata = ReadClassMetadata(context, payloadOffset);
			if (classMetadata == null)
			{
				context.Seek(savedOffSet);
				return null;
			}
			SeekSecondaryOffset(context, classMetadata);
			object obj = classMetadata.Read(context);
			context.Seek(savedOffSet);
			return obj;
		}

		private ClassMetadata ReadClassMetadata(IInternalReadContext context, int payloadOffset
			)
		{
			context.Seek(payloadOffset);
			ClassMetadata classMetadata = Container().ClassMetadataForId(context.ReadInt());
			return classMetadata;
		}

		private void SeekSecondaryOffset(IInternalReadContext context, ClassMetadata classMetadata
			)
		{
			if (classMetadata is PrimitiveFieldHandler && classMetadata.IsArray())
			{
				// unnecessary secondary offset, consistent with old format
				context.Seek(context.ReadInt());
			}
		}

		public override void Write(IWriteContext context, object obj)
		{
			if (obj == null)
			{
				context.WriteInt(0);
				return;
			}
			MarshallingContext marshallingContext = (MarshallingContext)context;
			ITypeHandler4 handler = ClassMetadata.ForObject(context.Transaction(), obj, true);
			if (handler == null)
			{
				context.WriteInt(0);
				return;
			}
			MarshallingContextState state = marshallingContext.CurrentState();
			marshallingContext.CreateChildBuffer(false, false);
			int id = marshallingContext.Container().Handlers().HandlerID(handler);
			context.WriteInt(id);
			if (IsArray(handler))
			{
				// TODO: This indirection is unneccessary, but it is required by the 
				// current old reading format. 
				// Remove in the next version of UntypedFieldHandler  
				marshallingContext.PrepareIndirectionOfSecondWrite();
			}
			else
			{
				marshallingContext.DoNotIndirectWrites();
			}
			handler.Write(context, obj);
			marshallingContext.RestoreState(state);
		}
	}
}
