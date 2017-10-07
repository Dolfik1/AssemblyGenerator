using System;
using System.IO;
using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Reflection.PortableExecutable;

namespace AssemblyGenerator
{
    public partial class AssemblyGenerator : IDisposable
    {

        private DebugDirectoryBuilder _debugDirectoryBuilder;
        private MemoryStream _peStream;
        private BlobBuilder _ilBuilder;
        private MetadataBuilder _metadataBuilder;
        private Assembly _currentAssembly;

        public AssemblyGenerator(Assembly assembly)
        {
            _debugDirectoryBuilder = new DebugDirectoryBuilder();
            _peStream = new MemoryStream();
            _ilBuilder = new BlobBuilder();
            _metadataBuilder = new MetadataBuilder();
            _currentAssembly = assembly;
        }

        public byte[] GenerateAssemblyBytes()
        {
            var name = _currentAssembly.GetName();

            var assemblyHandle = _metadataBuilder.AddAssembly(
                GetString(name.FullName),
                name.Version,
                GetString(name.CultureName),
                GetBlob(name.GetPublicKey()),
                _assemblyNameFlagsConvert(name.Flags),
                _assemblyHashAlgorithmConvert(name.HashAlgorithm));


            var refs = _currentAssembly.GetReferencedAssemblies();
            CreateReferencedAssemblies(refs);

            var modules = _currentAssembly.GetModules();
            foreach (var module in modules)
            {
                var moduleHandle = _metadataBuilder.AddModule(
                    0, // reserved in ECMA
                    GetString(module.Name),
                    GetGuid(module.ModuleVersionId),
                    default(GuidHandle), // reserved in ECMA
                    default(GuidHandle)); // reserved in ECMA

                CreateFields(module.GetFields());
                var methods = module.GetTypes();
                Console.WriteLine();
            }

            var types = _currentAssembly.GetTypes();
            CreateTypes(assemblyHandle, types);

            var metadataRootBuilder = new MetadataRootBuilder(_metadataBuilder);
            var header = PEHeaderBuilder.CreateLibraryHeader();

            var peBuilder = new ManagedPEBuilder(
                header,
                metadataRootBuilder,
                _ilBuilder,
                debugDirectoryBuilder: _debugDirectoryBuilder);

            var peImageBuilder = new BlobBuilder();
            peBuilder.Serialize(peImageBuilder);

            return peImageBuilder.ToArray();
        }

        public void GenerateAssembly(string path)
        {
            var bytes = GenerateAssemblyBytes();
            File.WriteAllBytes(path, bytes);
        }

        public void Dispose()
        {
            _peStream.Close();
        }
    }
}
