using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using System.Runtime.Versioning;
using System.Text;

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

                var args = attr.ConstructorArguments;
                var text = string.Join(",", args.Select(x =>
                {
                    //var val = x.Value;
                    return $"\"{x.Value}\"";
                }));

                var ctor = GetTypeConstructor(type);
                _metadataBuilder.AddCustomAttribute(parent, ctor, GetBlobString(text));
            }
        }
    }
}
