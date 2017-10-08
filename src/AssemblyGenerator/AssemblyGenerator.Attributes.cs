using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Metadata;

namespace AssemblyGenerator
{
    public partial class AssemblyGenerator
    {
        public void CreateCustomAttributes(EntityHandle parent, IEnumerable<CustomAttributeData> attributes)
        {
            foreach (var attr in attributes)
            {
                var ctor = CreateConstructor(attr.Constructor);
                var type = GetOrCreateType(attr.AttributeType);
                _metadataBuilder.AddCustomAttribute(parent, ctor, default(BlobHandle));
            }
        }
    }
}
