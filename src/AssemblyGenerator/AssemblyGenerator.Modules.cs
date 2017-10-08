using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Metadata;
using System.Text;

namespace AssemblyGenerator
{
    public partial class AssemblyGenerator
    {
        public void CreateModules(Module[] moduleInfo)
        {

            foreach (var module in moduleInfo)
            {
                var moduleHandle = _metadataBuilder.AddModule(
                    0, // reserved in ECMA
                    GetString(module.Name),
                    GetGuid(module.ModuleVersionId),
                    default(GuidHandle), // reserved in ECMA
                    default(GuidHandle)); // reserved in ECMA

                CreateFields(module.GetFields());
                CreateTypes(module.GetTypes());
                CreateMethods(module.GetMethods(_defaultMethodsBindingFlags));
            }
        }
    }
}
