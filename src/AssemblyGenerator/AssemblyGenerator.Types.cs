using System;
using System.Collections.Generic;
using System.Reflection.Metadata;

namespace AssemblyGenerator
{
    public partial class AssemblyGenerator
    {
        private Dictionary<Type, EntityHandle> _typeHandles = new Dictionary<Type, EntityHandle>();

        private void CreateTypes(Handle parent, Type[] types)
        {
            foreach (var type in types)
            {
                GetOrCreateType(type);
            }
        }

        private EntityHandle GetResolutionScopeForType(Type type)
        {
            return GetReferencedAssemblyForType(type);
        }

        internal EntityHandle GetOrCreateType(Type type)
        {
            if (_typeHandles.ContainsKey(type))
                return _typeHandles[type];

            var baseType = default(EntityHandle);
            if (type.BaseType != null)
            {
                var bsType = type.BaseType;
                if (bsType.Assembly != _currentAssembly)
                {
                    // todo, also maybe in Module, ModuleRef, AssemblyRef and TypeRef
                    // ECMA-335 page 273-274
                    var scope = GetResolutionScopeForType(bsType);
                    var bsTypeRef = _metadataBuilder.AddTypeReference(
                        scope,
                        GetString(bsType.Namespace),
                        GetString(bsType.Name));

                    _typeHandles[bsType] = bsTypeRef;
                }
                else
                {
                    baseType = GetOrCreateType(bsType);
                }
            }

            var methods = CreateMethods(type.GetMethods());
            var fields = CreateFields(type.GetFields());


            return _metadataBuilder.AddTypeDefinition(
                type.Attributes,
                GetString(type.Namespace),
                GetString(type.Name),
                baseType,
                fields, // todo
                methods);
        }
    }
}
