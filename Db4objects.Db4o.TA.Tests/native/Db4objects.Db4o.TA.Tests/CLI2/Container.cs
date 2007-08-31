namespace Db4objects.Db4o.TA.Tests.CLI2
{
	class Named : ActivatableImpl
	{
		private string _name;

		public Named(string name)
		{
			_name = name;
		}

		/// <summary>
		/// Activatable based implementation. Activates the
		/// object before field access.
		/// </summary>
		public string Name
		{
			get
			{
				Activate();
				return _name;
			}
		}

		public string PassThroughName
		{
			get { return _name; }
		}
	}

	class NullableContainer<T> : Named where T : struct
	{
		private T? _value;

		public NullableContainer(string name, T? value)
			: base(name)
		{
			_value = value;
		}

		/// <summary>
		/// Activatable based implementation. Activates the
		/// object before field access.
		/// </summary>
		public T? Value
		{
			get
			{
				Activate();
				return _value;
			}
		}

		/// <summary>
		/// Bypass activation and access the field directly.
		/// </summary>
		public T? PassThroughValue
		{
			get { return _value; }
		}
	}

	class Container<T> : Named where T: struct 
	{	
		private T _value;

		public Container(string name, T value) : base(name)
		{	
			_value = value;
		}

		/// <summary>
		/// Activatable based implementation. Activates the
		/// object before field access.
		/// </summary>
		public T Value
		{
			get
			{
				Activate();
				return _value;
			}
		}

		/// <summary>
		/// Bypass activation and access the field directly.
		/// </summary>
		public T PassThroughValue
		{
			get { return _value; }
		}
	}
}