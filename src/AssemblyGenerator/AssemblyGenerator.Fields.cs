using System.Reflection;
using System.Reflection.Metadata;

namespace AssemblyGenerator
{
    public partial class AssemblyGenerator
    {
        BindingFlags _defaultFieldsBindingFlags =
            BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic |
            BindingFlags.DeclaredOnly | BindingFlags.CreateInstance |
            BindingFlags.Instance;

        /// <summary>
        /// Signature of field
        /// </summary>
        /// <param name="fieldInfo"></param>
        /// <returns></returns>
        private BlobHandle GetFieldSignature(FieldInfo fieldInfo)
        {
            var type = fieldInfo.FieldType;
            return GetBlob(
                BuildSignature(x => x.FieldSignature()
                .FromSystemType(type, this)));
        }

        private FieldDefinitionHandle CreateFields(FieldInfo[] fieldInfos)
        {
            var handle = default(FieldDefinitionHandle);
            foreach (var field in fieldInfos)
            {
                var signature = GetFieldSignature(field);
                var temp = _metadataBuilder.AddFieldDefinition(
                    field.Attributes,
                    GetString(field.Name),
                    GetFieldSignature(field));

                // It seems that we only need the first method
                if (handle == default(FieldDefinitionHandle))
                    handle = temp;
            }

            return handle;
        }
    }
}
