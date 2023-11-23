using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using ResourceStorageService.Application.Commands;
using ResourceStorageService.Application.DataTransferObjects;
using ResourceStorageService.Application.Models;
using ResourceStorageService.Application.Queries;
using ResourceStorageService.Application.Queries.GetResourceFromDB;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace ResourceStorageService.API.Swagger
{
    public class ResourceDataSet : ISchemaFilter
    {

        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            if (context.Type == typeof(ResourceDto))
                schema.Example = GetExampleOrNullFor(typeof(ResourceDto));
            else if (context.Type == typeof(ResourcePathDto))
                schema.Example = GetExampleOrNullFor(typeof(ResourcePathDto));
            else if (context.Type == typeof(FilterResourceDto))
                schema.Example = GetExampleOrNullFor(typeof(FilterResourceDto));
            else if (context.Type == typeof(GetResourceListQuery)) 
                schema.Example = GetExampleOrNullFor(typeof(GetResourceListQuery));
            else if (context.Type == typeof(GetResourceDetailsListQuery))
                schema.Example = GetExampleOrNullFor(typeof(GetResourceDetailsListQuery));
            else if (context.Type == typeof(DeleteMultiResourceCommand))
                schema.Example = GetExampleOrNullFor(typeof(GetResourceDetailsListQuery));
            else if (context.Type == typeof(DownloadMultipleResourcesQuery))
                schema.Example = GetExampleOrNullFor(typeof(GetResourceDetailsListQuery));
        }
        private static IOpenApiAny GetExampleOrNullFor(Type type)
        {
            OpenApiObject openApiObject = null;
            switch (type.Name)
            {
                case "ResourceDto":
                    openApiObject = GetResouceDto();
                    break;
                case "ResourcePathDto":
                    openApiObject = new OpenApiObject
                    {
                        ["resourceId"] = new OpenApiString(Guid.NewGuid().ToString()),
                        ["resourcePath"] = new OpenApiString("Abi/" + Guid.NewGuid().ToString() + "/Lion.pdf"),
                        ["fullResourcePath"] = new OpenApiString("FullAWSResourcePath"),
                        ["expiresAt"] = new OpenApiDateTime(DateTime.Now)
                    };
                    break;
                case "FilterResourceDto":
                    openApiObject = new OpenApiObject
                    {
                        ["offset"] = new OpenApiInteger(1),
                        ["limit"] = new OpenApiInteger(10),
                        ["totalPages"] = new OpenApiInteger(3),
                        ["totalCount"] = new OpenApiInteger(27),
                        ["resources"] = new OpenApiArray()
                        {
                            GetResouceDto()
                        }
                    };
                    break;
                case "GetResourceListQuery":
                    openApiObject = new OpenApiObject
                    {
                        ["offset"] = new OpenApiInteger(1),
                        ["limit"] = new OpenApiInteger(10),
                        ["searchResourceName"] = new OpenApiString("image"),
                        ["searchResourcePath"] = new OpenApiString("Karthick/resource"),
                        ["fromDate"] = new OpenApiDateTime(Convert.ToDateTime("2023-10-03")),
                        ["toDate"] = new OpenApiDateTime(Convert.ToDateTime("2023-10-04")),
                        ["resourceIds"] = new OpenApiArray()
                        {
                            new OpenApiString(Guid.NewGuid().ToString())
                        },
                        ["resourcePaths"] = new OpenApiArray()
                        {
                            new OpenApiString("Abi/" + Guid.NewGuid().ToString() + "/Lion.pdf")
                        }
                    };
                    break;
                case "GetResourceDetailsListQuery":
                    openApiObject = new OpenApiObject
                    {
                        ["resourceIds"] = new OpenApiArray()
                        {
                            new OpenApiString(Guid.NewGuid().ToString())
                        },
                        ["resourcePaths"] = new OpenApiArray()
                        {
                            new OpenApiString("Abi/" + Guid.NewGuid().ToString() + "/Lion.pdf")
                        }
                    };
                    break;
            }
            return openApiObject;
        }

        private static OpenApiObject GetResouceDto()
        {
            return new OpenApiObject
            {
                ["id"] = new OpenApiString("a391a187-7b32-4972-aa7b-fa459beb1aae"),
                ["path"] = new OpenApiString("resource/ui.txt"),
                ["name"] = new OpenApiString("ui.txt"),
                ["sizeInBytes"] = new OpenApiLong(5012),
                ["type"] = new OpenApiString(".txt"),
                ["createdAt"] = new OpenApiDateTime(DateTime.Now)
            };
        }
    }
}
