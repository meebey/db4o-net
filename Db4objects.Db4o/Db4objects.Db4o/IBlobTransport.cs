namespace Db4objects.Db4o
{
	/// <exclude></exclude>
	public interface IBlobTransport
	{
		void WriteBlobTo(Db4objects.Db4o.Internal.Transaction trans, Db4objects.Db4o.Internal.BlobImpl
			 blob, Sharpen.IO.File file);

		void ReadBlobFrom(Db4objects.Db4o.Internal.Transaction trans, Db4objects.Db4o.Internal.BlobImpl
			 blob, Sharpen.IO.File file);
	}
}
