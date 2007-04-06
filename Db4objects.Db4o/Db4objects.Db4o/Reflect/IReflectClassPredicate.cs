using Db4objects.Db4o.Reflect;

namespace Db4objects.Db4o.Reflect
{
	public interface IReflectClassPredicate
	{
		bool Match(IReflectClass item);
	}
}
