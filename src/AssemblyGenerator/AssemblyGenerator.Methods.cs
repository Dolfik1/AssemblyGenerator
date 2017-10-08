using System;
using System.Collections.Generic;
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

        private Dictionary<MethodInfo, MethodDefinitionHandle> _methodsHandles =
            new Dictionary<MethodInfo, MethodDefinitionHandle>();

        private BlobHandle GetMethodSignature(MethodInfo methodInfo)
        {
            var retType = methodInfo.ReturnType;
            var parameters = methodInfo.GetParameters();
            var countParameters = parameters.Length;

            var blob = BuildSignature(x => x.MethodSignature()
                .Parameters(
                countParameters,
                r => r.FromSystemType(retType, this),
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

        private MethodDefinitionHandle GetMethodDefinitionHandle(MethodInfo methodInfo)
        {
            if (methodInfo == null)
                return default(MethodDefinitionHandle);

            return _methodsHandles[methodInfo];
        }

        private MethodDefinitionHandle GetOrCreateMethod(MethodInfo methodInfo)
        {
            if (_methodsHandles.ContainsKey(methodInfo))
                return _methodsHandles[methodInfo];

            var offset = _ilBuilder.Count; // take an offset
            var body = methodInfo.GetMethodBody();
            // If body exists, we write it in IL body stream
            if (body != null)
            {
                var methodBody = body.GetILAsByteArray();
                _ilBuilder.WriteBytes(methodBody);
            }

            var signature = GetMethodSignature(methodInfo);
            var parameters = CreateParameters(methodInfo.GetParameters());

            var handle = _metadataBuilder.AddMethodDefinition(
                methodInfo.Attributes,
                methodInfo.MethodImplementationFlags,
                GetString(methodInfo.Name),
                signature,
                offset,
                parameters);


            if (body != null && body.LocalVariables.Count > 0)
            {
                _metadataBuilder.AddStandaloneSignature
                    (GetBlob(
                        BuildSignature(x =>
                        {
                            var sig = x.LocalVariableSignature(body.LocalVariables.Count);
                            foreach (var vrb in body.LocalVariables)
                            {
                                sig.AddVariable().Type().FromSystemType(vrb.LocalType, this);
                            }

                        })));
            }

            /*
             FieldList and MethodList described in ECMA 335, page 270
             */

            _methodsHandles.Add(methodInfo, handle);

            CreateCustomAttributes(handle, methodInfo.GetCustomAttributesData());
            return handle;

        }

        private MethodDefinitionHandle CreateMethods(MethodInfo[] methodInfos)
        {
            var handle = default(MethodDefinitionHandle);
            foreach (var method in methodInfos)
            {
                var temp = GetOrCreateMethod(method);
                if (handle == default(MethodDefinitionHandle))
                    handle = temp;
            }
            return handle;
        }
    }
}
