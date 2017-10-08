using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;

namespace AssemblyGenerator
{
    public partial class AssemblyGenerator
    {
        private BlobHandle GetMethodSignature(MethodInfo methodInfo)
        {
            var retType = methodInfo.ReturnType;
            var parameters = methodInfo.GetParameters();
            var countParameters = parameters.Length;

            var paramsEncoder = new ParametersEncoder(new BlobBuilder());

            // generate return types and parameters
            var returnType = new ReturnTypeEncoder(new BlobBuilder());
            returnType.Type().FromSystemType(retType, this);

			foreach (var par in parameters)
            {
                var parEncoder = paramsEncoder.AddParameter();
                parEncoder.Type().FromSystemType(par.ParameterType, this);
            }
			
            return GetBlob(
                BuildSignature(x => x.MethodSignature()
                    .Parameters(countParameters,
					out returnType,
					out paramsEncoder)));
        }

        private MethodDefinitionHandle CreateMethods(MethodInfo[] methodInfos)
        {
            var handle = default(MethodDefinitionHandle);
            foreach (var method in methodInfos)
            {
                if (method.IsHideBySig) // not sure
                    continue;

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

                if (handle != default(MethodDefinitionHandle))
                    handle = temp;
            }
            return handle;
        }
    }
}
