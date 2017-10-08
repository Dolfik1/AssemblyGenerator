using System;
using System.Collections.Generic;
using System.Reflection.Metadata;

namespace AssemblyGenerator
{
    public partial class AssemblyGenerator
    {
        private Dictionary<Guid, EntityHandle> _typeHandles = new Dictionary<Guid, EntityHandle>();

        private void CreateTypes(Type[] types)
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

        internal EntityHandle CreateReferencedType(Type type)
        {
            var scope = GetResolutionScopeForType(type);
            var refType = _metadataBuilder.AddTypeReference(
                scope,
                GetString(type.Namespace),
                GetString(type.Name));

            _typeHandles.Add(type.GUID, refType);
            CreateConstructorForReferencedType(type);

            return refType;
        }

        internal bool IsReferencedType(Type type)
        {
            // todo, also maybe in Module, ModuleRef, AssemblyRef and TypeRef
            // ECMA-335 page 273-274
            return type.Assembly != _currentAssembly;
        }
        
        internal EntityHandle GetOrCreateType(Type type)
        {
            if (_typeHandles.ContainsKey(type.GUID))
                return _typeHandles[type.GUID];
            
            var baseType = default(EntityHandle);

            if (IsReferencedType(type))
                return CreateReferencedType(type);

            if (type.BaseType != null)
            {
                var bsType = type.BaseType;
                if (bsType.Assembly != _currentAssembly)
                {
                    var bsTypeRef = CreateReferencedType(bsType);
                    _typeHandles[bsType.GUID] = bsTypeRef;
                    baseType = bsTypeRef;
                }
                else
                {
                    baseType = GetOrCreateType(bsType);
                }
            }

            var methods = CreateMethods(type.GetMethods(_defaultMethodsBindingFlags));
            CreateConstructors(type.GetConstructors(), type);

            var fields = CreateFields(type.GetFields(_defaultFieldsBindingFlags));
            


            var def = _metadataBuilder.AddTypeDefinition(
                type.Attributes,
                GetString(type.Namespace),
                GetString(type.Name),
                baseType,
                fields,
                methods);

            _typeHandles[type.GUID] = def;

            return def;
        }
    }
}
