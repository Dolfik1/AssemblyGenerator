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
                var type = attr.AttributeType;
                GetOrCreateType(type); // create type

                var ctor = GetTypeConstructor(type);
                _metadataBuilder.AddCustomAttribute(parent, ctor, default(BlobHandle));
            }
        }
    }
}
