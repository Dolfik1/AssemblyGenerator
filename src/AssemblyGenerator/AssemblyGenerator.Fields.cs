﻿using System.Reflection;
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
                BuildSignature(x => 
                    x.FieldSignature()
                .FromSystemType(type, this)));
        }

        FieldDefinitionHandle _defaultField = default(FieldDefinitionHandle);

        private FieldDefinitionHandle CreateFields(FieldInfo[] fieldInfos)
        {
            var handle = _defaultField;
            foreach (var field in fieldInfos)
            {
                var signature = GetFieldSignature(field);
                var temp = _metadataBuilder.AddFieldDefinition(
                    field.Attributes,
                    GetString(field.Name),
                    GetFieldSignature(field));

                CreateCustomAttributes(temp, field.GetCustomAttributesData());
                
                // It seems that we only need the first method
                if (handle == _defaultField)
                    handle = temp;
            }

            return handle;
        }
    }
}
