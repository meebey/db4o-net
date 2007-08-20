using Mono.Cecil;

namespace Db4oAdmin.Core
{
    public interface ITypeFilter
    {
        bool Accept(TypeDefinition typeDef);
    }
}