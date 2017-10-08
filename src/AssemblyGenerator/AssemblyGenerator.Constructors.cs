using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;

namespace AssemblyGenerator
{
    public partial class AssemblyGenerator
    {
        internal Dictionary<Guid, List<EntityHandle>> _typeConstructors
            = new Dictionary<Guid, List<EntityHandle>>();

        internal BlobHandle GetConstructorSignature(ConstructorInfo constructorInfo)
        {
            var parameters = constructorInfo.GetParameters();
            var countParameters = parameters.Length;

            var blob = BuildSignature(x => x.MethodSignature(isInstanceMethod: true)
                .Parameters(
                countParameters,
                r => r.Void(),
                p =>
                {
                    foreach (var par in parameters)
                    {
                        var parEncoder = p.AddParameter();
                        parEncoder.Type().FromSystemType(par.ParameterType, this);
                    }
                }));
            return GetBlob(blob);
        }

        internal EntityHandle GetTypeConstructor(Type type)
        {
            return _typeConstructors[type.GUID].First();
        }

        internal MemberReferenceHandle CreateConstructorForReferencedType(Type type)
        {
            var ctors = type.GetConstructors();
            var typeHandle = GetOrCreateType(type);

            var handle = default(MemberReferenceHandle);

            foreach (var ctor in ctors)
            {
                var signature = GetConstructorSignature(ctor);
                var tmp = _metadataBuilder.AddMemberReference(typeHandle, GetString(ctor.Name), signature);

                if (handle == default(MemberReferenceHandle))
                    handle = tmp;

                if (_typeConstructors.ContainsKey(type.GUID))
                    _typeConstructors[type.GUID].Add(tmp);
                else
                    _typeConstructors.Add(type.GUID, new List<EntityHandle> { tmp });
            }

            return handle;
        }

        internal MethodDefinitionHandle CreateConstructor(ConstructorInfo constructorInfo, Type type)
        {
            var parameters = CreateParameters(constructorInfo.GetParameters());
            var bodyOffset = _ilBuilder.Count;

            var body = constructorInfo.GetMethodBody();
            if (body != null)
            {
                _ilBuilder.WriteBytes(body.GetILAsByteArray());
            }

            var method = _metadataBuilder.AddMethodDefinition(
                constructorInfo.Attributes,
                constructorInfo.MethodImplementationFlags,
                GetString(constructorInfo.Name),
                GetConstructorSignature(constructorInfo),
                bodyOffset,
                parameters);

            if (_typeConstructors.ContainsKey(type.GUID))
                _typeConstructors[type.GUID].Add(method);
            else
                _typeConstructors.Add(type.GUID, new List<EntityHandle> { method });

            return method;
        }

        internal MethodDefinitionHandle CreateConstructors(ConstructorInfo[] constructorInfo, Type type)
        {
            var handle = default(MethodDefinitionHandle);
            foreach (var ctor in constructorInfo)
            {
                var temp = CreateConstructor(ctor, type);
                if (handle == default(MethodDefinitionHandle))
                    handle = temp;
            }

            return handle;
        }
    }
}
