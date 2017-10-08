using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Metadata;
using System.Text;

namespace AssemblyGenerator
{
    public partial class AssemblyGenerator
    {
        BindingFlags _defaultPropertiesBindingFlags = 
            BindingFlags.NonPublic | BindingFlags.Public | 
            BindingFlags.Instance | BindingFlags.Static;

        private BlobHandle GetPropertySignature(PropertyInfo propertyInfo)
        {
            var parameters = propertyInfo.GetIndexParameters();
            var countParameters = parameters.Length;
            var retType = propertyInfo.PropertyType;

            var blob = BuildSignature(x => x.PropertySignature()
                .Parameters(
                countParameters,
                r => r.FromSystemType(retType, this),
                p => {
                    foreach (var par in parameters)
                    {
                        var parEncoder = p.AddParameter();
                        parEncoder.Type().FromSystemType(par.ParameterType, this);
                    }
                }));
            return GetBlob(blob);

        }

        public void CreatePropertiesForType(TypeDefinitionHandle typeHandle, PropertyInfo[] propertyInfo)
        {
            if (propertyInfo.Length == 0) return;

            var handle = default(PropertyDefinitionHandle);
            foreach (var prop in propertyInfo)
            {
                var signature = GetPropertySignature(prop);
                var tmp = _metadataBuilder.AddProperty(
                    prop.Attributes, 
                    GetString(prop.Name),
                    signature);

                if (handle == default(PropertyDefinitionHandle))
                    handle = tmp;
            }

            _metadataBuilder.AddPropertyMap(typeHandle, handle);
        }
    }
}
