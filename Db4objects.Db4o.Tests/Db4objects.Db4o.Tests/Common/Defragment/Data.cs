namespace Db4objects.Db4o.Tests.Common.Defragment
{
	public class Data
	{
		public int _id;

		public string _name;

		public Db4objects.Db4o.Tests.Common.Defragment.Data _previous;

		public Db4objects.Db4o.Tests.Common.Defragment.Data[] _payload;

		public Data(int id, string name, Db4objects.Db4o.Tests.Common.Defragment.Data previous
			, Db4objects.Db4o.Tests.Common.Defragment.Data[] payload)
		{
			this._id = id;
			this._name = name;
			this._previous = previous;
			this._payload = payload;
		}

		public override string ToString()
		{
			return _id + ":" + _name + "," + (_payload == null ? "-" : _payload.Length.ToString
				() + "[" + _payload[1] + "]");
		}
	}
}
