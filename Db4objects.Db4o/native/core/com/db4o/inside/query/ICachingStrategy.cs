namespace Db4objects.Db4o.Inside.Query
{
	public interface ICachingStrategy
	{
		void Add(object key, object item);
		object Get(object key);
	}
}