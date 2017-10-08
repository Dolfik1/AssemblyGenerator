using System.Reflection;
using System.Reflection.Metadata;

namespace AssemblyGenerator
{
    public partial class AssemblyGenerator
    {
        internal BlobHandle GetConstructorSignature(ConstructorInfo constructorInfo)
        {
            var parameters = constructorInfo.GetParameters();
            var countParameters = parameters.Length;

            var blob = BuildSignature(x => x.MethodSignature(isInstanceMethod: true)
                .Parameters(
                countParameters,
                r => r.Void(),
                p => {
                    foreach (var par in parameters)
                    {
                        var parEncoder = p.AddParameter();
                        parEncoder.Type().FromSystemType(par.ParameterType, this);
                    }
                }));
            return GetBlob(blob);
        }

        internal MethodDefinitionHandle CreateConstructor(ConstructorInfo constructorInfo)
        {
            var parameters = CreateParameters(constructorInfo.GetParameters());
            var bodyOffset = _ilBuilder.Count;

            var body = constructorInfo.GetMethodBody();
            if (body != null)
            {
                _ilBuilder.WriteBytes(body.GetILAsByteArray());
            }

            return _metadataBuilder.AddMethodDefinition(constructorInfo.Attributes,
                constructorInfo.MethodImplementationFlags,
                GetString(constructorInfo.Name),
                GetConstructorSignature(constructorInfo),
                bodyOffset,
                parameters);
        }

		internal MethodDefinitionHandle CreateConstructors(ConstructorInfo[] constructorInfo)
        {
            var handle = default(MethodDefinitionHandle);
            foreach (var ctor in constructorInfo)
            {
                var temp = CreateConstructor(ctor);
                if (handle == default(MethodDefinitionHandle))
                    handle = temp;
            }

            return handle;
        }
    }
}
