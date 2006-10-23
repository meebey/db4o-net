namespace Db4objects.Db4o.Config
{
	/// <summary>
	/// a simple Alias for a single Class or Type, using #equals() on
	/// the names in the resolve method.
	/// </summary>
	/// <remarks>
	/// a simple Alias for a single Class or Type, using #equals() on
	/// the names in the resolve method.
	/// <br /><br />See
	/// <see cref="Db4objects.Db4o.Config.IAlias">Db4objects.Db4o.Config.IAlias</see>
	/// for concrete examples.
	/// </remarks>
	public class TypeAlias : Db4objects.Db4o.Config.IAlias
	{
		private readonly string _storedType;

		private readonly string _runtimeType;

		public TypeAlias(string storedType, string runtimeType)
		{
			if (null == storedType || null == runtimeType)
			{
				throw new System.ArgumentException();
			}
			_storedType = storedType;
			_runtimeType = runtimeType;
		}

		/// <summary>checking if both names are equal.</summary>
		/// <remarks>checking if both names are equal.</remarks>
		public virtual string Resolve(string runtimeType)
		{
			return _runtimeType.Equals(runtimeType) ? _storedType : null;
		}
	}
}
