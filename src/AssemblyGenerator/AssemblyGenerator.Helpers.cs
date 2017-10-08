using System;
using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using CfgAssemblyHashAlgorithm = System.Configuration.Assemblies.AssemblyHashAlgorithm;

namespace AssemblyGenerator
{
    public partial class AssemblyGenerator
    {
        private AssemblyFlags _assemblyNameFlagsConvert(AssemblyNameFlags flags)
        {
            switch (flags)
            {
                // we not support only AssemblyNameFlags.None flag
                // also i'm not sure about AssemblyNameFlags.EnableJITcompileOptimizer flag
                case AssemblyNameFlags.None:
                    return AssemblyFlags.PublicKey; // Possible wrong
                default:
                    return (AssemblyFlags)flags;
            }
        }

        private AssemblyHashAlgorithm _assemblyHashAlgorithmConvert(CfgAssemblyHashAlgorithm alg)
        {
            return (AssemblyHashAlgorithm)alg;
        }

        private BlobBuilder BuildSignature(Action<BlobEncoder> action)
        {
            var builder = new BlobBuilder();
            action(new BlobEncoder(builder));
            return builder;
        }

        private BlobBuilder BuildSignature(BlobBuilder builder, Action<BlobEncoder> action)
        {
            action(new BlobEncoder(builder));
            return builder;
        }

        private StringHandle GetString(string str)
        {
            return _metadataBuilder.GetOrAddString(str);
        }

        private BlobHandle GetBlob(byte[] bytes)
        {
            return _metadataBuilder.GetOrAddBlob(bytes);
        }

        private BlobHandle GetBlobString(string str)
        {
            if (string.IsNullOrEmpty(str))
                return default(BlobHandle);

            return _metadataBuilder.GetOrAddBlobUTF8(str);
        }

        private BlobHandle GetBlob(BlobBuilder builder)
        {
            return _metadataBuilder.GetOrAddBlob(builder);
        }

        private GuidHandle GetGuid(Guid guid)
        {
            return _metadataBuilder.GetOrAddGuid(guid);
        }
    }
}
