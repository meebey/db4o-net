namespace Db4objects.Db4o
{
	/// <exclude></exclude>
	public sealed class YapConst
	{
		internal static readonly object initMe = Init();

		public const byte YAPFILEVERSION = 4;

		public const byte YAPBEGIN = (byte)'{';

		public const byte YAPFILE = (byte)'Y';

		internal const byte YAPID = (byte)'#';

		internal const byte YAPPOINTER = (byte)'>';

		public const byte YAPCLASSCOLLECTION = (byte)'A';

		public const byte YAPCLASS = (byte)'C';

		internal const byte YAPFIELD = (byte)'F';

		public const byte YAPOBJECT = (byte)'O';

		internal const byte YAPARRAY = (byte)'N';

		internal const byte YAPARRAYN = (byte)'Z';

		public const byte YAPINDEX = (byte)'X';

		public const byte YAPSTRING = (byte)'S';

		internal const byte YAPLONG = (byte)'l';

		internal const byte YAPINTEGER = (byte)'i';

		internal const byte YAPBOOLEAN = (byte)'=';

		internal const byte YAPDOUBLE = (byte)'d';

		internal const byte YAPBYTE = (byte)'b';

		internal const byte YAPSHORT = (byte)'s';

		internal const byte YAPCHAR = (byte)'c';

		internal const byte YAPFLOAT = (byte)'f';

		internal const byte YAPEND = (byte)'}';

		internal const byte YAPNULL = (byte)'0';

		public const byte BTREE = (byte)'T';

		public const byte BTREE_NODE = (byte)'B';

		public const byte HEADER = (byte)'H';

		internal const int IDENTIFIER_LENGTH = (Db4objects.Db4o.Deploy.debug && Db4objects.Db4o.Deploy
			.identifiers) ? 1 : 0;

		public const int BRACKETS_BYTES = (Db4objects.Db4o.Deploy.debug && Db4objects.Db4o.Deploy
			.brackets) ? 1 : 0;

		internal const int BRACKETS_LENGTH = BRACKETS_BYTES * 2;

		public const int LEADING_LENGTH = IDENTIFIER_LENGTH + BRACKETS_BYTES;

		public const int ADDED_LENGTH = IDENTIFIER_LENGTH + BRACKETS_LENGTH;

		internal const int SHORT_BYTES = 2;

		internal const int INTEGER_BYTES = (Db4objects.Db4o.Deploy.debug && Db4objects.Db4o.Deploy
			.debugLong) ? 11 : 4;

		internal const int LONG_BYTES = (Db4objects.Db4o.Deploy.debug && Db4objects.Db4o.Deploy
			.debugLong) ? 20 : 8;

		internal const int CHAR_BYTES = 2;

		internal const int UNSPECIFIED = int.MinValue + 100;

		public const int INT_LENGTH = INTEGER_BYTES + ADDED_LENGTH;

		public const int ID_LENGTH = INT_LENGTH;

		public const int LONG_LENGTH = LONG_BYTES + ADDED_LENGTH;

		internal const int WRITE_LOOP = (INTEGER_BYTES - 1) * 8;

		public const int OBJECT_LENGTH = ADDED_LENGTH;

		public const int POINTER_LENGTH = (INT_LENGTH * 2) + ADDED_LENGTH;

		public const int MESSAGE_LENGTH = INT_LENGTH * 2 + 1;

		public const byte SYSTEM_TRANS = (byte)'s';

		public const byte USER_TRANS = (byte)'u';

		internal const byte XBYTE = (byte)'X';

		public const int IGNORE_ID = -99999;

		internal const int PRIMITIVE = -2000000000;

		internal const int TYPE_SIMPLE = 1;

		internal const int TYPE_CLASS = 2;

		internal const int TYPE_ARRAY = 3;

		internal const int TYPE_NARRAY = 4;

		internal const int NONE = 0;

		internal const int STATE = 1;

		internal const int ACTIVATION = 2;

		internal const int TRANSIENT = -1;

		internal const int ADD_MEMBERS_TO_ID_TREE_ONLY = 0;

		internal const int ADD_TO_ID_TREE = 1;

		internal const byte ISO8859 = (byte)1;

		internal const byte UNICODE = (byte)2;

		public const int LOCK_TIME_INTERVAL = 1000;

		internal static readonly int SERVER_SOCKET_TIMEOUT = Db4objects.Db4o.Debug.longTimeOuts
			 ? 1000000 : 5000;

		internal const int CLIENT_SOCKET_TIMEOUT = 300000;

		internal static readonly int CONNECTION_TIMEOUT = Db4objects.Db4o.Debug.longTimeOuts
			 ? 1000000 : 180000;

		public const int MAXIMUM_BLOCK_SIZE = 70000000;

		internal const int MAXIMUM_ARRAY_ENTRIES = 7000000;

		internal const int MAXIMUM_ARRAY_ENTRIES_PRIMITIVE = MAXIMUM_ARRAY_ENTRIES * 100;

		internal static System.Type CLASS_COMPARE;

		internal static System.Type CLASS_DB4OTYPE;

		internal static System.Type CLASS_DB4OTYPEIMPL;

		internal static System.Type CLASS_INTERNAL;

		internal static System.Type CLASS_UNVERSIONED;

		internal static System.Type CLASS_METACLASS;

		internal static System.Type CLASS_METAFIELD;

		internal static System.Type CLASS_METAINDEX;

		public static System.Type CLASS_OBJECT;

		internal static System.Type CLASS_OBJECTCONTAINER;

		internal static System.Type CLASS_REPLICATIONRECORD;

		internal static System.Type CLASS_STATICFIELD;

		internal static System.Type CLASS_STATICCLASS;

		internal static System.Type CLASS_TRANSIENTCLASS;

		public static readonly string EMBEDDED_CLIENT_USER = "embedded client";

		internal const int CLEAN = 0;

		internal const int ACTIVE = 1;

		internal const int PROCESSING = 2;

		internal const int CACHED_DIRTY = 3;

		internal const int CONTINUE = 4;

		internal const int STATIC_FIELDS_STORED = 5;

		internal const int CHECKED_CHANGES = 6;

		internal const int DEAD = 7;

		internal const int READING = 8;

		internal const int UNCHECKED = 0;

		public const int NO = -1;

		public const int YES = 1;

		public const int DEFAULT = 0;

		public const int UNKNOWN = 0;

		public const int OLD = -1;

		public const int NEW = 1;

		public static readonly Db4objects.Db4o.YapStringIOUnicode stringIO = new Db4objects.Db4o.YapStringIOUnicode
			();

		private static object Init()
		{
			CLASS_OBJECT = new object().GetType();
			CLASS_COMPARE = typeof(Db4objects.Db4o.Config.ICompare);
			CLASS_DB4OTYPE = typeof(Db4objects.Db4o.Types.IDb4oType);
			CLASS_DB4OTYPEIMPL = typeof(Db4objects.Db4o.IDb4oTypeImpl);
			CLASS_INTERNAL = typeof(Db4objects.Db4o.IInternal4);
			CLASS_UNVERSIONED = typeof(Db4objects.Db4o.Types.IUnversioned);
			CLASS_METACLASS = new Db4objects.Db4o.MetaClass().GetType();
			CLASS_METAFIELD = new Db4objects.Db4o.MetaField().GetType();
			CLASS_METAINDEX = new Db4objects.Db4o.MetaIndex().GetType();
			CLASS_OBJECTCONTAINER = typeof(Db4objects.Db4o.IObjectContainer);
			CLASS_REPLICATIONRECORD = new Db4objects.Db4o.ReplicationRecord().GetType();
			CLASS_STATICFIELD = new Db4objects.Db4o.StaticField().GetType();
			CLASS_STATICCLASS = new Db4objects.Db4o.StaticClass().GetType();
			CLASS_TRANSIENTCLASS = typeof(Db4objects.Db4o.Types.ITransientClass);
			return null;
		}

		internal static readonly System.Type[] ESSENTIAL_CLASSES = { CLASS_METAINDEX, CLASS_METAFIELD
			, CLASS_METACLASS, CLASS_STATICFIELD, CLASS_STATICCLASS };

		public static readonly string VIRTUAL_FIELD_PREFIX = "v4o";

		public const int MAX_STACK_DEPTH = 100;
	}
}
