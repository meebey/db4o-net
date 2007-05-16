/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System;
using Db4objects.Db4o;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Types;

namespace Db4objects.Db4o.Internal
{
	/// <exclude>TODO: Split into separate enums with defined range and values.</exclude>
	public sealed class Const4
	{
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

		public const byte INTEGER_ARRAY = (byte)'I';

		public const int IDENTIFIER_LENGTH = (Deploy.debug && Deploy.identifiers) ? 1 : 0;

		public const int BRACKETS_BYTES = (Deploy.debug && Deploy.brackets) ? 1 : 0;

		public const int BRACKETS_LENGTH = BRACKETS_BYTES * 2;

		public const int LEADING_LENGTH = IDENTIFIER_LENGTH + BRACKETS_BYTES;

		public const int ADDED_LENGTH = IDENTIFIER_LENGTH + BRACKETS_LENGTH;

		public const int SHORT_BYTES = 2;

		public const int INTEGER_BYTES = (Deploy.debug && Deploy.debugLong) ? 11 : 4;

		public const int LONG_BYTES = (Deploy.debug && Deploy.debugLong) ? 20 : 8;

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

		public const int SERVER_SOCKET_TIMEOUT = Debug.longTimeOuts ? 1000000 : 5000;

		public const int CLIENT_SOCKET_TIMEOUT = 300000;

		public const int CLIENT_EMBEDDED_TIMEOUT = 300000;

		public const int CONNECTION_TIMEOUT = Debug.longTimeOuts ? 1000000 : 180000;

		public const int MAXIMUM_BLOCK_SIZE = 70000000;

		public const int MAXIMUM_ARRAY_ENTRIES = 7000000;

		public const int MAXIMUM_ARRAY_ENTRIES_PRIMITIVE = MAXIMUM_ARRAY_ENTRIES * 100;

		public static readonly Type CLASS_COMPARE = typeof(ICompare);

		public static readonly Type CLASS_DB4OTYPE = typeof(IDb4oType);

		public static readonly Type CLASS_DB4OTYPEIMPL = typeof(IDb4oTypeImpl);

		public static readonly Type CLASS_INTERNAL = typeof(IInternal4);

		public static readonly Type CLASS_UNVERSIONED = typeof(IUnversioned);

		public static readonly Type CLASS_OBJECT = new object().GetType();

		public static readonly Type CLASS_OBJECTCONTAINER = typeof(IObjectContainer);

		public static readonly Type CLASS_REPLICATIONRECORD = new ReplicationRecord().GetType
			();

		public static readonly Type CLASS_STATICFIELD = new StaticField().GetType();

		public static readonly Type CLASS_STATICCLASS = new StaticClass().GetType();

		public static readonly Type CLASS_TRANSIENTCLASS = typeof(ITransientClass);

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

		public static readonly UnicodeStringIO stringIO = new UnicodeStringIO();

		public static readonly Type[] ESSENTIAL_CLASSES = new Type[] { CLASS_STATICFIELD, 
			CLASS_STATICCLASS };

		public static readonly string VIRTUAL_FIELD_PREFIX = "v4o";

		public const int MAX_STACK_DEPTH = 20;
	}
}
