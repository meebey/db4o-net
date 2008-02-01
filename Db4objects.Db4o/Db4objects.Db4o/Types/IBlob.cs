/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System.IO;
using Db4objects.Db4o.Ext;
using Db4objects.Db4o.Types;

namespace Db4objects.Db4o.Types
{
	/// <summary>
	/// the db4o Blob type to store blobs independent of the main database
	/// file and allows to perform asynchronous upload and download operations.
	/// </summary>
	/// <remarks>
	/// the db4o Blob type to store blobs independent of the main database
	/// file and allows to perform asynchronous upload and download operations.
	/// &lt;br&gt;&lt;br&gt;
	/// &lt;b&gt;Usage:&lt;/b&gt;&lt;br&gt;
	/// - Define Blob fields on your user classes.&lt;br&gt;
	/// - As soon as an object of your class is stored, db4o automatically
	/// takes care that the Blob field is set.&lt;br&gt;
	/// - Call readFrom to read a blob file into the db4o system.&lt;br&gt;
	/// - Call writeTo to write a blob file from within the db4o system.&lt;br&gt;
	/// - getStatus may help you to determine, whether data has been
	/// previously stored. It may also help you to track the completion
	/// of the current process.
	/// &lt;br&gt;&lt;br&gt;
	/// db4o client/server carries out all blob operations in a separate
	/// thread on a specially dedicated socket. One socket is used for
	/// all blob operations and operations are queued. Your application
	/// may continue to access db4o while a blob is transferred in the
	/// background.
	/// </remarks>
	public interface IBlob : IDb4oType
	{
		/// <summary>returns the name of the file the blob was stored to.</summary>
		/// <remarks>
		/// returns the name of the file the blob was stored to.
		/// &lt;br&gt;&lt;br&gt;The method may return null, if the file was never
		/// stored.
		/// </remarks>
		/// <returns>String the name of the file.</returns>
		string GetFileName();

		/// <summary>returns the status after the last read- or write-operation.</summary>
		/// <remarks>
		/// returns the status after the last read- or write-operation.
		/// &lt;br&gt;&lt;br&gt;The status value returned may be any of the following:&lt;br&gt;
		/// Status.UNUSED  no data was ever stored to the Blob field.&lt;br&gt;
		/// Status.AVAILABLE available data was previously stored to the Blob field.&lt;br&gt;
		/// Status.QUEUED an operation was triggered and is waiting for it's turn in the Blob queue.&lt;br&gt;
		/// Status.COMPLETED the last operation on this field was completed successfully.&lt;br&gt;
		/// Status.PROCESSING for internal use only.&lt;br&gt;
		/// Status.ERROR the last operation failed.&lt;br&gt;
		/// or a double between 0 and 1 that signifies the current completion percentage of the currently
		/// running operation.&lt;br&gt;&lt;br&gt; the five STATUS constants defined in this interface or a double
		/// between 0 and 1 that signifies the completion of the currently running operation.&lt;br&gt;&lt;br&gt;
		/// </remarks>
		/// <returns>status - the current status</returns>
		/// <seealso cref="Status">STATUS constants</seealso>
		double GetStatus();

		/// <summary>reads a file into the db4o system and stores it as a blob.</summary>
		/// <remarks>
		/// reads a file into the db4o system and stores it as a blob.
		/// &lt;br&gt;&lt;br&gt;
		/// In Client/Server mode db4o will open an additional socket and
		/// process writing data in an additional thread.
		/// &lt;br&gt;&lt;br&gt;
		/// </remarks>
		/// <param name="file">the file the blob is to be read from.</param>
		/// <exception cref="IOException">in case of errors</exception>
		void ReadFrom(Sharpen.IO.File file);

		/// <summary>reads a file into the db4o system and stores it as a blob.</summary>
		/// <remarks>
		/// reads a file into the db4o system and stores it as a blob.
		/// &lt;br&gt;&lt;br&gt;
		/// db4o will use the local file system in Client/Server mode also.
		/// &lt;br&gt;&lt;br&gt;
		/// </remarks>
		/// <param name="file">the file the blob is to be read from.</param>
		/// <exception cref="IOException">in case of errors</exception>
		void ReadLocal(Sharpen.IO.File file);

		/// <summary>writes stored blob data to a file.</summary>
		/// <remarks>
		/// writes stored blob data to a file.
		/// &lt;br&gt;&lt;br&gt;
		/// db4o will use the local file system in Client/Server mode also.
		/// &lt;br&gt;&lt;br&gt;
		/// </remarks>
		/// <exception cref="IOException">
		/// in case of errors and in case no blob
		/// data was stored
		/// </exception>
		/// <param name="file">the file the blob is to be written to.</param>
		void WriteLocal(Sharpen.IO.File file);

		/// <summary>writes stored blob data to a file.</summary>
		/// <remarks>
		/// writes stored blob data to a file.
		/// &lt;br&gt;&lt;br&gt;
		/// In Client/Server mode db4o will open an additional socket and
		/// process writing data in an additional thread.
		/// &lt;br&gt;&lt;br&gt;
		/// </remarks>
		/// <exception cref="IOException">
		/// in case of errors and in case no blob
		/// data was stored
		/// </exception>
		/// <param name="file">the file the blob is to be written to.</param>
		void WriteTo(Sharpen.IO.File file);

		/// <summary>Deletes the current file stored in this BLOB.</summary>
		/// <remarks>Deletes the current file stored in this BLOB.</remarks>
		/// <exception cref="IOException">
		/// in case of errors and in case no
		/// data was stored
		/// </exception>
		void DeleteFile();
	}
}
