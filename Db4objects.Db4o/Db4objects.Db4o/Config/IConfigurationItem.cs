using Db4objects.Db4o.Internal;

namespace Db4objects.Db4o.Config
{
	/// <summary>
	/// Implement this interface for configuration items that are to be applied
	/// to ObjectContainers after they are opened.
	/// </summary>
	/// <remarks>
	/// Implement this interface for configuration items that are to be applied
	/// to ObjectContainers after they are opened.
	/// </remarks>
	public interface IConfigurationItem
	{
		/// <summary>
		/// Implement this interface to to apply a configuration item
		/// to an ObjectContainerBase after it is opened.
		/// </summary>
		/// <remarks>
		/// Implement this interface to to apply a configuration item
		/// to an ObjectContainerBase after it is opened.
		/// </remarks>
		/// <param name="container">the ObjectContainerBase</param>
		void Apply(ObjectContainerBase container);
	}
}
