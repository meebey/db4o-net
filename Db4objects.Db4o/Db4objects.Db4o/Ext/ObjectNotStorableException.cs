namespace Db4objects.Db4o.Ext
{
	/// <summary>
	/// this Exception is thrown, if objects can not be stored and if
	/// db4o is configured to throw Exceptions on storage failures.
	/// </summary>
	/// <remarks>
	/// this Exception is thrown, if objects can not be stored and if
	/// db4o is configured to throw Exceptions on storage failures.
	/// </remarks>
	/// <seealso cref="Db4objects.Db4o.Config.IConfiguration.ExceptionsOnNotStorable">Db4objects.Db4o.Config.IConfiguration.ExceptionsOnNotStorable
	/// 	</seealso>
	[System.Serializable]
	public class ObjectNotStorableException : System.Exception
	{
		public ObjectNotStorableException(Db4objects.Db4o.Reflect.IReflectClass a_class) : 
			base(Db4objects.Db4o.Messages.Get(a_class.IsPrimitive() ? 59 : 45, a_class.GetName
			()))
		{
		}

		public ObjectNotStorableException(string message) : base(message)
		{
		}
	}
}
