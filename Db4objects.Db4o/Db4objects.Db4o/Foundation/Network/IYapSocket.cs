namespace Db4objects.Db4o.Foundation.Network
{
	public interface IYapSocket
	{
		void Close();

		void Flush();

		bool IsConnected();

		int Read();

		int Read(byte[] a_bytes, int a_offset, int a_length);

		void SetSoTimeout(int timeout);

		void Write(byte[] bytes);

		void Write(byte[] bytes, int off, int len);

		void Write(int i);

		Db4objects.Db4o.Foundation.Network.IYapSocket OpenParalellSocket();
	}
}
