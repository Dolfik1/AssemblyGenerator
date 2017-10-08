using System.Reflection;
using System.Reflection.Metadata;

namespace AssemblyGenerator
{
    public partial class AssemblyGenerator
    {
        public ParameterHandle CreateParameters(ParameterInfo[] parameters)
        {
            var handle = default(ParameterHandle);
            for (var i = 0; i < parameters.Length; i++)
            {
                var param = parameters[i];
                var tmp = _metadataBuilder.AddParameter(
                    param.Attributes, 
                    GetString(param.Name),
                    i);

                CreateCustomAttributes(tmp, param.GetCustomAttributesData());
                if (handle == default(ParameterHandle))
                    handle = tmp;
            }

            return handle;
        }
    }
}
