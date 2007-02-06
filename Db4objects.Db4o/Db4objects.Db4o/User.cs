namespace Db4objects.Db4o
{
	/// <exclude></exclude>
	/// <persistent></persistent>
	public class User : Db4objects.Db4o.IInternal4
	{
		public string name;

		public string password;

		public User()
		{
		}

		public User(string name_, string password_)
		{
			name = name_;
			password = password_;
		}
	}
}
