using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ASM.API.Swagger
{
    public class FileUploadOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var methodParams = context.MethodInfo.GetParameters();
            
            // Find all IFormFile parameters (with or without [FromForm])
            var fileParameters = methodParams
                .Where(p => p.ParameterType == typeof(IFormFile) || 
                           p.ParameterType == typeof(IFormFileCollection))
                .ToList();

            // Find DTOs with [FromForm]
            var formParameters = methodParams
                .Where(p => p.GetCustomAttributes(typeof(FromFormAttribute), false).Any() &&
                           p.ParameterType != typeof(IFormFile) &&
                           p.ParameterType != typeof(IFormFileCollection))
                .ToList();

            // Check if action consumes multipart/form-data
            var consumesMultipart = context.MethodInfo.GetCustomAttributes(typeof(Microsoft.AspNetCore.Mvc.ConsumesAttribute), false)
                .Cast<Microsoft.AspNetCore.Mvc.ConsumesAttribute>()
                .Any(a => a.ContentTypes.Any(ct => ct.Contains("multipart/form-data")));

            // Process if we have file/form parameters or consumes multipart/form-data
            if (fileParameters.Any() || formParameters.Any() || consumesMultipart)
            {
                // Remove IFormFile parameters from parameters list
                var fileParamsToRemove = operation.Parameters
                    .Where(p => fileParameters.Any(fp => fp.Name == p.Name))
                    .ToList();

                foreach (var param in fileParamsToRemove)
                {
                    operation.Parameters.Remove(param);
                }

                // Remove [FromForm] DTO parameters from parameters list
                var formParamsToRemove = operation.Parameters
                    .Where(p => formParameters.Any(fp => fp.Name == p.Name))
                    .ToList();

                foreach (var param in formParamsToRemove)
                {
                    operation.Parameters.Remove(param);
                }

                // Create or update RequestBody
                if (operation.RequestBody == null)
                {
                    operation.RequestBody = new OpenApiRequestBody
                    {
                        Content = new Dictionary<string, OpenApiMediaType>()
                    };
                }

                if (!operation.RequestBody.Content.ContainsKey("multipart/form-data"))
                {
                    operation.RequestBody.Content["multipart/form-data"] = new OpenApiMediaType
                    {
                        Schema = new OpenApiSchema
                        {
                            Type = "object",
                            Properties = new Dictionary<string, OpenApiSchema>(),
                            Required = new HashSet<string>()
                        }
                    };
                }

                var schema = operation.RequestBody.Content["multipart/form-data"].Schema;
                if (schema.Properties == null)
                {
                    schema.Properties = new Dictionary<string, OpenApiSchema>();
                }
                if (schema.Required == null)
                {
                    schema.Required = new HashSet<string>();
                }

                // Add form fields for DTOs - flatten the DTO properties into form fields
                foreach (var param in formParameters)
                {
                    var paramSchema = context.SchemaGenerator.GenerateSchema(param.ParameterType, context.SchemaRepository);
                    if (paramSchema.Properties != null && paramSchema.Properties.Count > 0)
                    {
                        foreach (var property in paramSchema.Properties)
                        {
                            // Skip if already exists
                            if (schema.Properties.ContainsKey(property.Key))
                                continue;

                            // Create a new schema for the property
                            var propertySchema = new OpenApiSchema();

                            // Copy basic properties
                            if (property.Value.Type != null)
                                propertySchema.Type = property.Value.Type;
                            
                            if (property.Value.Format != null)
                                propertySchema.Format = property.Value.Format;
                            
                            if (property.Value.Description != null)
                                propertySchema.Description = property.Value.Description;
                            
                            propertySchema.Nullable = property.Value.Nullable;
                            
                            if (property.Value.Default != null)
                                propertySchema.Default = property.Value.Default;
                            
                            if (property.Value.Example != null)
                                propertySchema.Example = property.Value.Example;

                            // Handle DateOnly type - ensure it's string format date
                            if (property.Value.Type == "string" && property.Value.Format == "date")
                            {
                                propertySchema.Type = "string";
                                propertySchema.Format = "date";
                            }

                            // Handle Guid type - ensure it's string format uuid
                            if (property.Value.Type == "string" && property.Value.Format == "uuid")
                            {
                                propertySchema.Type = "string";
                                propertySchema.Format = "uuid";
                            }

                            // Handle boolean type
                            if (property.Value.Type == "boolean")
                            {
                                propertySchema.Type = "boolean";
                            }

                            // Handle array types
                            if (property.Value.Items != null)
                            {
                                propertySchema.Items = property.Value.Items;
                            }

                            // Handle enum types
                            if (property.Value.Enum != null && property.Value.Enum.Count > 0)
                            {
                                propertySchema.Enum = property.Value.Enum;
                            }

                            // Add the property to schema
                            schema.Properties[property.Key] = propertySchema;
                            
                            // Add required fields
                            if (paramSchema.Required != null && paramSchema.Required.Contains(property.Key))
                            {
                                if (!schema.Required.Contains(property.Key))
                                {
                                    schema.Required.Add(property.Key);
                                }
                            }
                        }
                    }
                }

                // Add file parameters
                foreach (var fileParam in fileParameters)
                {
                    schema.Properties[fileParam.Name] = new OpenApiSchema
                    {
                        Type = "string",
                        Format = "binary",
                        Description = "File to upload"
                    };
                    schema.Required.Add(fileParam.Name);
                }
            }
        }
    }
}

