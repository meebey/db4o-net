using Mono.Cecil;

namespace Db4oAdmin.Core
{
    class ByAttributeFilter : ITypeFilter
    {
        private string _attribute;

        public ByAttributeFilter(string attribute)
        {
            _attribute = attribute;
        }

        public bool Accept(TypeDefinition typeDef)
        {
            foreach (CustomAttribute attribute in typeDef.CustomAttributes)
            {
                if (_attribute == attribute.Constructor.DeclaringType.FullName)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
