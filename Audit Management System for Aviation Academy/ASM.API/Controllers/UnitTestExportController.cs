using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace ASM.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UnitTestExportController : ControllerBase
    {
        [HttpGet("export-excel")]
        public IActionResult ExportUnitTests()
        {
            try
            {
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                using var package = new ExcelPackage();

                // Get all controllers
                var controllers = GetControllers();
                
                foreach (var controller in controllers)
                {
                    foreach (var endpoint in controller.Endpoints)
                    {
                        var sheetName = $"{controller.Name.Replace("Controller", "")}_{endpoint.Name}";
                        if (sheetName.Length > 31) // Excel sheet name limit
                            sheetName = sheetName.Substring(0, 31);
                        
                        // Check if sheet name already exists (handle duplicates)
                        int counter = 1;
                        string originalSheetName = sheetName;
                        while (package.Workbook.Worksheets.Any(ws => ws.Name == sheetName))
                        {
                            sheetName = $"{originalSheetName.Substring(0, Math.Min(originalSheetName.Length, 28))}_{counter}";
                            counter++;
                        }
                        
                        var worksheet = package.Workbook.Worksheets.Add(sheetName);
                        CreateUnitTestSheet(worksheet, controller, endpoint);
                    }
                }

                var stream = new MemoryStream();
                package.SaveAs(stream);
                stream.Position = 0;

                var fileName = $"UnitTests_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while generating unit test Excel file", error = ex.Message });
            }
        }

        private void CreateUnitTestSheet(ExcelWorksheet worksheet, ControllerInfo controller, EndpointInfo endpoint)
        {
            // Header row
            var headers = new[] { "Test ID", "Test Name", "Description", "Preconditions", "Test Steps", "Expected Result", "Actual Result", "Status", "Priority", "Module", "Function" };
            
            for (int i = 0; i < headers.Length; i++)
            {
                worksheet.Cells[1, i + 1].Value = headers[i];
                worksheet.Cells[1, i + 1].Style.Font.Bold = true;
                worksheet.Cells[1, i + 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[1, i + 1].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                worksheet.Cells[1, i + 1].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[1, i + 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            }

            int row = 2;
            int testIdCounter = 1;

            var testCases = GenerateTestCases(controller.Name, endpoint);
            
            foreach (var testCase in testCases)
            {
                worksheet.Cells[row, 1].Value = $"UT-{testIdCounter++:000}";
                worksheet.Cells[row, 2].Value = testCase.TestName;
                worksheet.Cells[row, 3].Value = testCase.Description;
                worksheet.Cells[row, 4].Value = testCase.Preconditions;
                worksheet.Cells[row, 5].Value = testCase.TestSteps;
                worksheet.Cells[row, 6].Value = testCase.ExpectedResult;
                worksheet.Cells[row, 7].Value = ""; // Actual Result - empty for template
                worksheet.Cells[row, 8].Value = "Not Executed"; // Status
                worksheet.Cells[row, 9].Value = testCase.Priority;
                worksheet.Cells[row, 10].Value = controller.Name.Replace("Controller", "");
                worksheet.Cells[row, 11].Value = endpoint.Name;

                // Apply borders and wrap text for description columns
                for (int col = 1; col <= headers.Length; col++)
                {
                    worksheet.Cells[row, col].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    if (col == 3 || col == 4 || col == 5 || col == 6 || col == 7) // Description, Preconditions, Test Steps, Expected Result, Actual Result
                    {
                        worksheet.Cells[row, col].Style.WrapText = true;
                    }
                }

                row++;
            }

            // Set column widths
            worksheet.Column(1).Width = 10; // Test ID
            worksheet.Column(2).Width = 40; // Test Name
            worksheet.Column(3).Width = 50; // Description
            worksheet.Column(4).Width = 40; // Preconditions
            worksheet.Column(5).Width = 50; // Test Steps
            worksheet.Column(6).Width = 40; // Expected Result
            worksheet.Column(7).Width = 40; // Actual Result
            worksheet.Column(8).Width = 15; // Status
            worksheet.Column(9).Width = 12; // Priority
            worksheet.Column(10).Width = 20; // Module
            worksheet.Column(11).Width = 30; // Function
            
            // Set row height for header
            worksheet.Row(1).Height = 25;
            
            // Freeze header row
            worksheet.View.FreezePanes(2, 1);
        }

        private List<TestCaseInfo> GenerateTestCases(string controllerName, EndpointInfo endpoint)
        {
            var testCases = new List<TestCaseInfo>();
            var method = endpoint.HttpMethod.ToUpper();
            var functionName = endpoint.Name;

            // Common test cases for all endpoints
            if (method == "GET")
            {
                // GetById test cases
                if (functionName.Contains("GetById") || functionName.Contains("Get") && endpoint.Parameters.Any(p => p.Name.ToLower().Contains("id")))
                {
                    testCases.Add(new TestCaseInfo
                    {
                        TestName = $"{functionName}_ValidId_ReturnsEntity",
                        Description = $"Verify that {functionName} returns the correct entity when provided with a valid ID",
                        Preconditions = "Entity exists in database with valid ID",
                        TestSteps = $"1. Call {functionName} with valid ID\n2. Verify response status is 200 OK\n3. Verify returned entity matches expected data",
                        ExpectedResult = "Returns 200 OK with entity data",
                        Priority = "High"
                    });

                    testCases.Add(new TestCaseInfo
                    {
                        TestName = $"{functionName}_InvalidId_ReturnsNotFound",
                        Description = $"Verify that {functionName} returns 404 when provided with invalid ID",
                        Preconditions = "Entity does not exist in database",
                        TestSteps = $"1. Call {functionName} with invalid ID\n2. Verify response status is 404 Not Found",
                        ExpectedResult = "Returns 404 Not Found with error message",
                        Priority = "High"
                    });

                    testCases.Add(new TestCaseInfo
                    {
                        TestName = $"{functionName}_EmptyId_ReturnsBadRequest",
                        Description = $"Verify that {functionName} returns 400 when provided with empty ID",
                        Preconditions = "None",
                        TestSteps = $"1. Call {functionName} with empty/null ID\n2. Verify response status is 400 Bad Request",
                        ExpectedResult = "Returns 400 Bad Request with validation error",
                        Priority = "Medium"
                    });
                }
                else // GetAll test cases
                {
                    testCases.Add(new TestCaseInfo
                    {
                        TestName = $"{functionName}_ValidRequest_ReturnsList",
                        Description = $"Verify that {functionName} returns a list of entities",
                        Preconditions = "Entities exist in database",
                        TestSteps = $"1. Call {functionName}\n2. Verify response status is 200 OK\n3. Verify returned list is not null",
                        ExpectedResult = "Returns 200 OK with list of entities",
                        Priority = "High"
                    });

                    testCases.Add(new TestCaseInfo
                    {
                        TestName = $"{functionName}_EmptyDatabase_ReturnsEmptyList",
                        Description = $"Verify that {functionName} returns empty list when no entities exist",
                        Preconditions = "No entities in database",
                        TestSteps = $"1. Call {functionName}\n2. Verify response status is 200 OK\n3. Verify returned list is empty",
                        ExpectedResult = "Returns 200 OK with empty list",
                        Priority = "Medium"
                    });
                }
            }
            else if (method == "POST")
            {
                testCases.Add(new TestCaseInfo
                {
                    TestName = $"{functionName}_ValidData_CreatesEntity",
                    Description = $"Verify that {functionName} successfully creates a new entity with valid data",
                    Preconditions = "Valid request data provided",
                    TestSteps = $"1. Prepare valid request data\n2. Call {functionName}\n3. Verify response status is 201 Created\n4. Verify entity is created in database",
                    ExpectedResult = "Returns 201 Created with created entity data",
                    Priority = "High"
                });

                testCases.Add(new TestCaseInfo
                {
                    TestName = $"{functionName}_InvalidData_ReturnsBadRequest",
                    Description = $"Verify that {functionName} returns 400 when provided with invalid data",
                    Preconditions = "Invalid request data provided",
                    TestSteps = $"1. Prepare invalid request data\n2. Call {functionName}\n3. Verify response status is 400 Bad Request",
                    ExpectedResult = "Returns 400 Bad Request with validation errors",
                    Priority = "High"
                });

                testCases.Add(new TestCaseInfo
                {
                    TestName = $"{functionName}_MissingRequiredFields_ReturnsBadRequest",
                    Description = $"Verify that {functionName} returns 400 when required fields are missing",
                    Preconditions = "Request data missing required fields",
                    TestSteps = $"1. Prepare request data without required fields\n2. Call {functionName}\n3. Verify response status is 400 Bad Request",
                    ExpectedResult = "Returns 400 Bad Request with field validation errors",
                    Priority = "High"
                });

                testCases.Add(new TestCaseInfo
                {
                    TestName = $"{functionName}_DuplicateData_ReturnsConflict",
                    Description = $"Verify that {functionName} returns 409 when trying to create duplicate entity",
                    Preconditions = "Entity with same unique identifier already exists",
                    TestSteps = $"1. Prepare request data for duplicate entity\n2. Call {functionName}\n3. Verify response status is 409 Conflict",
                    ExpectedResult = "Returns 409 Conflict with error message",
                    Priority = "Medium"
                });
            }
            else if (method == "PUT")
            {
                testCases.Add(new TestCaseInfo
                {
                    TestName = $"{functionName}_ValidData_UpdatesEntity",
                    Description = $"Verify that {functionName} successfully updates an existing entity",
                    Preconditions = "Entity exists in database with valid ID",
                    TestSteps = $"1. Prepare valid update data\n2. Call {functionName} with valid ID\n3. Verify response status is 200 OK\n4. Verify entity is updated in database",
                    ExpectedResult = "Returns 200 OK with updated entity data",
                    Priority = "High"
                });

                testCases.Add(new TestCaseInfo
                {
                    TestName = $"{functionName}_InvalidId_ReturnsNotFound",
                    Description = $"Verify that {functionName} returns 404 when provided with invalid ID",
                    Preconditions = "Entity does not exist in database",
                    TestSteps = $"1. Prepare valid update data\n2. Call {functionName} with invalid ID\n3. Verify response status is 404 Not Found",
                    ExpectedResult = "Returns 404 Not Found with error message",
                    Priority = "High"
                });

                testCases.Add(new TestCaseInfo
                {
                    TestName = $"{functionName}_InvalidData_ReturnsBadRequest",
                    Description = $"Verify that {functionName} returns 400 when provided with invalid data",
                    Preconditions = "Entity exists in database",
                    TestSteps = $"1. Prepare invalid update data\n2. Call {functionName} with valid ID\n3. Verify response status is 400 Bad Request",
                    ExpectedResult = "Returns 400 Bad Request with validation errors",
                    Priority = "High"
                });
            }
            else if (method == "DELETE")
            {
                testCases.Add(new TestCaseInfo
                {
                    TestName = $"{functionName}_ValidId_DeletesEntity",
                    Description = $"Verify that {functionName} successfully deletes an entity",
                    Preconditions = "Entity exists in database with valid ID",
                    TestSteps = $"1. Call {functionName} with valid ID\n2. Verify response status is 200 OK or 204 No Content\n3. Verify entity is deleted or marked as inactive in database",
                    ExpectedResult = "Returns 200 OK or 204 No Content, entity deleted or inactive",
                    Priority = "High"
                });

                testCases.Add(new TestCaseInfo
                {
                    TestName = $"{functionName}_InvalidId_ReturnsNotFound",
                    Description = $"Verify that {functionName} returns 404 when provided with invalid ID",
                    Preconditions = "Entity does not exist in database",
                    TestSteps = $"1. Call {functionName} with invalid ID\n2. Verify response status is 404 Not Found",
                    ExpectedResult = "Returns 404 Not Found with error message",
                    Priority = "High"
                });
            }

            // Add authentication/authorization test cases
            testCases.Add(new TestCaseInfo
            {
                TestName = $"{functionName}_Unauthorized_ReturnsUnauthorized",
                Description = $"Verify that {functionName} returns 401 when called without authentication",
                Preconditions = "No authentication token provided",
                TestSteps = $"1. Call {functionName} without authentication token\n2. Verify response status is 401 Unauthorized",
                ExpectedResult = "Returns 401 Unauthorized",
                Priority = "High"
            });

            return testCases;
        }

        private List<ControllerInfo> GetControllers()
        {
            var controllers = new List<ControllerInfo>();
            var assembly = Assembly.GetExecutingAssembly();
            var controllerTypes = assembly.GetTypes()
                .Where(t => t.IsSubclassOf(typeof(ControllerBase)) && t.Name.EndsWith("Controller"))
                .ToList();

            foreach (var controllerType in controllerTypes)
            {
                var controllerInfo = new ControllerInfo
                {
                    Name = controllerType.Name,
                    Type = controllerType,
                    Endpoints = new List<EndpointInfo>()
                };

                var methods = controllerType.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                    .Where(m => m.IsPublic && !m.IsSpecialName && 
                                (m.GetCustomAttribute<HttpPostAttribute>() != null ||
                                 m.GetCustomAttribute<HttpGetAttribute>() != null ||
                                 m.GetCustomAttribute<HttpPutAttribute>() != null ||
                                 m.GetCustomAttribute<HttpDeleteAttribute>() != null))
                    .ToList();

                foreach (var method in methods)
                {
                    var httpMethod = "GET";
                    if (method.GetCustomAttribute<HttpPostAttribute>() != null) httpMethod = "POST";
                    else if (method.GetCustomAttribute<HttpPutAttribute>() != null) httpMethod = "PUT";
                    else if (method.GetCustomAttribute<HttpDeleteAttribute>() != null) httpMethod = "DELETE";

                    var routeAttr = method.GetCustomAttribute<RouteAttribute>();
                    var route = routeAttr?.Template ?? method.Name;

                    var endpoint = new EndpointInfo
                    {
                        Name = method.Name,
                        HttpMethod = httpMethod,
                        Route = route,
                        Parameters = method.GetParameters().Select(p => new ParameterInfo
                        {
                            Name = p.Name,
                            Type = p.ParameterType.Name
                        }).ToList()
                    };

                    controllerInfo.Endpoints.Add(endpoint);
                }

                if (controllerInfo.Endpoints.Any())
                {
                    controllers.Add(controllerInfo);
                }
            }

            return controllers;
        }

        private class ControllerInfo
        {
            public string Name { get; set; }
            public Type Type { get; set; }
            public List<EndpointInfo> Endpoints { get; set; }
        }

        private class EndpointInfo
        {
            public string Name { get; set; }
            public string HttpMethod { get; set; }
            public string Route { get; set; }
            public List<ParameterInfo> Parameters { get; set; }
        }

        private class ParameterInfo
        {
            public string Name { get; set; }
            public string Type { get; set; }
        }

        private class TestCaseInfo
        {
            public string TestName { get; set; }
            public string Description { get; set; }
            public string Preconditions { get; set; }
            public string TestSteps { get; set; }
            public string ExpectedResult { get; set; }
            public string Priority { get; set; }
        }
    }
}

