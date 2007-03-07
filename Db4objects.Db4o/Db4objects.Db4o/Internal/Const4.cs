namespace Db4objects.Db4o.Internal
{
	/// <exclude>TODO: Split into separate enums with defined range and values.</exclude>
	public sealed class Const4
	{
		public static readonly object initMe = Init();

		public const byte YAPFILEVERSION = 4;

		public const byte YAPBEGIN = (byte)'{';

		public const byte YAPFILE = (byte)'Y';

		public const byte YAPID = (byte)'#';

		public const byte YAPPOINTER = (byte)'>';

		public const byte YAPCLASSCOLLECTION = (byte)'A';

		public const byte YAPCLASS = (byte)'C';

		public const byte YAPFIELD = (byte)'F';

		public const byte YAPOBJECT = (byte)'O';

		public const byte YAPARRAY = (byte)'N';

		public const byte YAPARRAYN = (byte)'Z';

		public const byte YAPINDEX = (byte)'X';

		public const byte YAPSTRING = (byte)'S';

		public const byte YAPLONG = (byte)'l';

		public const byte YAPINTEGER = (byte)'i';

		public const byte YAPBOOLEAN = (byte)'=';

		public const byte YAPDOUBLE = (byte)'d';

		public const byte YAPBYTE = (byte)'b';

		public const byte YAPSHORT = (byte)'s';

		public const byte YAPCHAR = (byte)'c';

		public const byte YAPFLOAT = (byte)'f';

		public const byte YAPEND = (byte)'}';

		public const byte YAPNULL = (byte)'0';

		public const byte BTREE = (byte)'T';

		public const byte BTREE_NODE = (byte)'B';

		public const byte HEADER = (byte)'H';

		public const int IDENTIFIER_LENGTH = (Db4objects.Db4o.Deploy.debug && Db4objects.Db4o.Deploy
			.identifiers) ? 1 : 0;

		public const int BRACKETS_BYTES = (Db4objects.Db4o.Deploy.debug && Db4objects.Db4o.Deploy
			.brackets) ? 1 : 0;

		public const int BRACKETS_LENGTH = BRACKETS_BYTES * 2;

		public const int LEADING_LENGTH = IDENTIFIER_LENGTH + BRACKETS_BYTES;

		public const int ADDED_LENGTH = IDENTIFIER_LENGTH + BRACKETS_LENGTH;

		public const int SHORT_BYTES = 2;

		public const int INTEGER_BYTES = (Db4objects.Db4o.Deploy.debug && Db4objects.Db4o.Deploy
			.debugLong) ? 11 : 4;

		public const int LONG_BYTES = (Db4objects.Db4o.Deploy.debug && Db4objects.Db4o.Deploy
			.debugLong) ? 20 : 8;

		public const int CHAR_BYTES = 2;

		public const int UNSPECIFIED = int.MinValue + 100;

		public const int INT_LENGTH = INTEGER_BYTES + ADDED_LENGTH;

		public const int ID_LENGTH = INT_LENGTH;

		public const int LONG_LENGTH = LONG_BYTES + ADDED_LENGTH;

		public const int WRITE_LOOP = (INTEGER_BYTES - 1) * 8;

		public const int OBJECT_LENGTH = ADDED_LENGTH;

		public const int POINTER_LENGTH = (INT_LENGTH * 2) + ADDED_LENGTH;

		public const int MESSAGE_LENGTH = INT_LENGTH * 2 + 1;

		public const byte SYSTEM_TRANS = (byte)'s';

		public const byte USER_TRANS = (byte)'u';

		public const byte XBYTE = (byte)'X';

		public const int IGNORE_ID = -99999;

		public const int PRIMITIVE = -2000000000;

		public const int TYPE_SIMPLE = 1;

		public const int TYPE_CLASS = 2;

		public const int TYPE_ARRAY = 3;

		public const int TYPE_NARRAY = 4;

		public const int NONE = 0;

		public const int STATE = 1;

		public const int ACTIVATION = 2;

		public const int TRANSIENT = -1;

		public const int ADD_MEMBERS_TO_ID_TREE_ONLY = 0;

		public const int ADD_TO_ID_TREE = 1;

		public const byte ISO8859 = (byte)1;

		public const byte UNICODE = (byte)2;

		public const int LOCK_TIME_INTERVAL = 1000;

		public const int SERVER_SOCKET_TIMEOUT = Db4objects.Db4o.Debug.longTimeOuts ? 1000000
			 : 5000;

		public const int CLIENT_SOCKET_TIMEOUT = 300000;

		public const int CLIENT_EMBEDDED_TIMEOUT = 3000;

		public const int CONNECTION_TIMEOUT = Db4objects.Db4o.Debug.longTimeOuts ? 1000000
			 : 180000;

		public const int MAXIMUM_BLOCK_SIZE = 70000000;

		public const int MAXIMUM_ARRAY_ENTRIES = 7000000;

		public const int MAXIMUM_ARRAY_ENTRIES_PRIMITIVE = MAXIMUM_ARRAY_ENTRIES * 100;

		public static System.Type CLASS_COMPARE;

		public static System.Type CLASS_DB4OTYPE;

		public static System.Type CLASS_DB4OTYPEIMPL;

		public static System.Type CLASS_INTERNAL;

		public static System.Type CLASS_UNVERSIONED;

		public static System.Type CLASS_OBJECT;

		public static System.Type CLASS_OBJECTCONTAINER;

		public static System.Type CLASS_REPLICATIONRECORD;

		public static System.Type CLASS_STATICFIELD;

		public static System.Type CLASS_STATICCLASS;

		public static System.Type CLASS_TRANSIENTCLASS;

		public static readonly string EMBEDDED_CLIENT_USER = "embedded client";

		public const int CLEAN = 0;

		public const int ACTIVE = 1;

		public const int PROCESSING = 2;

		public const int CACHED_DIRTY = 3;

		public const int CONTINUE = 4;

		public const int STATIC_FIELDS_STORED = 5;

		public const int CHECKED_CHANGES = 6;

		public const int DEAD = 7;

		public const int READING = 8;

		public const int OLD = -1;

		public const int NEW = 1;

		public static readonly Db4objects.Db4o.Internal.UnicodeStringIO stringIO = new Db4objects.Db4o.Internal.UnicodeStringIO
			();

		private static object Init()
		{
			CLASS_OBJECT = new object().GetType();
			CLASS_COMPARE = typeof(Db4objects.Db4o.Config.ICompare);
			CLASS_DB4OTYPE = typeof(Db4objects.Db4o.Types.IDb4oType);
			CLASS_DB4OTYPEIMPL = typeof(Db4objects.Db4o.Internal.IDb4oTypeImpl);
			CLASS_INTERNAL = typeof(Db4objects.Db4o.IInternal4);
			CLASS_UNVERSIONED = typeof(Db4objects.Db4o.Types.IUnversioned);
			CLASS_OBJECTCONTAINER = typeof(Db4objects.Db4o.IObjectContainer);
			CLASS_REPLICATIONRECORD = new Db4objects.Db4o.ReplicationRecord().GetType();
			CLASS_STATICFIELD = new Db4objects.Db4o.StaticField().GetType();
			CLASS_STATICCLASS = new Db4objects.Db4o.StaticClass().GetType();
			CLASS_TRANSIENTCLASS = typeof(Db4objects.Db4o.Types.ITransientClass);
			return null;
		}

		public static readonly System.Type[] ESSENTIAL_CLASSES = { CLASS_STATICFIELD, CLASS_STATICCLASS
			 };

		public static readonly string VIRTUAL_FIELD_PREFIX = "v4o";

		public const int MAX_STACK_DEPTH = 20;
	}
}
