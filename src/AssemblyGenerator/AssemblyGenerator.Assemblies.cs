using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Metadata;

namespace AssemblyGenerator
{
    public partial class AssemblyGenerator
    {
        // Saved assembly references handles
        private Dictionary<byte[], AssemblyReferenceHandle> _assemblyReferenceHandles =
            new Dictionary<byte[], AssemblyReferenceHandle>(new ByteArrayEqualityComparer());

        private AssemblyReferenceHandle GetReferencedAssemblyForType(Type type)
        {
            var asm = type.Assembly.GetName();

            var token = asm.GetPublicKeyToken();

            if(_assemblyReferenceHandles.ContainsKey(token))
                return _assemblyReferenceHandles[token];

            var key = asm.GetPublicKey();
            if (_assemblyReferenceHandles.ContainsKey(key))
                return _assemblyReferenceHandles[key];

            throw new Exception($"Referenced Assembly not found! ({asm.FullName})");
        }

        private bool CheckReferencedAssembliesKeyAndTokenExists(byte[] token, byte[] key)
        {
            if (token != null && _assemblyReferenceHandles.ContainsKey(token))
            {
                if (key != null && !_assemblyReferenceHandles.ContainsKey(key))
                {
                    var tok = _assemblyReferenceHandles[token];
                    _assemblyReferenceHandles.Add(key, tok);
                }
                return true;
            }

            if (key != null && _assemblyReferenceHandles.ContainsKey(key))
            {
                if (token != null && !_assemblyReferenceHandles.ContainsKey(token))
                {
                    var k = _assemblyReferenceHandles[key];
                    _assemblyReferenceHandles.Add(token, k);
                }
                return true;
            }
            return false;
        }

        private void CreateReferencedAssemblies(AssemblyName[] assemblies)
        {
            foreach (var asm in assemblies)
            {
                var token = asm.GetPublicKeyToken();
                var key = asm.GetPublicKey();

                var hashOrToken = token == null ? GetBlob(key) : GetBlob(token);
                if (CheckReferencedAssembliesKeyAndTokenExists(token, key))
                    return;

                var handle = _metadataBuilder.AddAssemblyReference(
                    GetString(asm.FullName),
                    asm.Version,
                    GetString(asm.CultureName),
                    hashOrToken,
                    _assemblyNameFlagsConvert(asm.Flags),
                    default(BlobHandle)); // Null is allowed

            }
        }
    }
}
