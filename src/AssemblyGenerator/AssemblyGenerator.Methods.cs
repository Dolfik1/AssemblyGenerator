using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;

namespace AssemblyGenerator
{
    public partial class AssemblyGenerator
    {
        BindingFlags _defaultMethodsBindingFlags = 
            BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | 
            BindingFlags.DeclaredOnly | BindingFlags.CreateInstance |
            BindingFlags.Instance;

        private BlobHandle GetMethodSignature(MethodInfo methodInfo)
        {
            var retType = methodInfo.ReturnType;
            var parameters = methodInfo.GetParameters();
            var countParameters = parameters.Length;

            var blob = BuildSignature(x => x.MethodSignature()
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

        private MethodDefinitionHandle CreateMethods(MethodInfo[] methodInfos)
        {
            var handle = default(MethodDefinitionHandle);
            foreach (var method in methodInfos)
            {
                var offset = _ilBuilder.Count; // take an offset
                var body = method.GetMethodBody();
                // If body exists, we write it in IL body stream
                if (body != null)
                {
                    var methodBody = body.GetILAsByteArray();
                    _ilBuilder.WriteBytes(methodBody);
                }

                var signature = GetMethodSignature(method);
                var parameters = CreateParameters(method.GetParameters());
                
                var temp = _metadataBuilder.AddMethodDefinition(
                    method.Attributes,
                    method.MethodImplementationFlags,
                    GetString(method.Name),
					signature,
                    offset,
					parameters);

                if (handle == default(MethodDefinitionHandle))
                    handle = temp;
            }
            return handle;
        }
    }
}
