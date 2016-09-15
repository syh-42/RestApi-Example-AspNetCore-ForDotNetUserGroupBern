using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Swashbuckle.Swagger.Model;
using Swashbuckle.SwaggerGen.Generator;

namespace RestApi.Swagger
{
    public class ExamplesOperationFilter : IOperationFilter
    {
        public void Apply(Operation operation, OperationFilterContext context)
        {
            var swaggerResponseActions =
                context.ApiDescription.GetActionAttributes().OfType<SwaggerResponseExamplesAttribute>();

            foreach (var attr in swaggerResponseActions)
            {
                var schema = context.SchemaRegistry.GetOrRegister(attr.ResponseType);

                var response = operation.Responses.FirstOrDefault(x => x.Value.Schema.Type == schema.Type 
                                                                    && x.Value.Schema.Ref == schema.Ref).Value;

                if (response != null)
                {
                    var provider = (IProvideExamples)Activator.CreateInstance(attr.ExamplesType);
                    response.Examples = FormatAsJson(provider);
                }
            }

        }
        private static object FormatAsJson(IProvideExamples provider)
        {
            var examples = new Dictionary<string, object>
            {
                {
                    "application/json", provider.GetExamples()
                }
            };
  
            return ConvertToCamelCase(examples);
        }

        private static object ConvertToCamelCase(Dictionary<string, object> examples)
        {
            var jsonString = JsonConvert.SerializeObject(examples, 
                 new JsonSerializerSettings() {ContractResolver = new CamelCasePropertyNamesContractResolver()});
            return JsonConvert.DeserializeObject(jsonString);
        }
    }
}