using ASM_Repositories.Models.LoginDTO;
using ASM_Services.Interfaces;
using ASM_Services.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ASM.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _service;

        public AuthController(IAuthService service)
        {
            _service = service;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            var result = await _service.LoginAsync(request);
            if (result == null)
                return Unauthorized(new { message = "Invalid email or password" });

            // Set JWT to cookie for subsequent requests
            Response.Cookies.Append("authToken", result.Token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTimeOffset.UtcNow.AddHours(6),
                Path = "/"
            });

            return Ok(result);
        }
        
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequest request)
        {
            try
            {
                var result = await _service.RegisterAsync(request);
                return CreatedAtAction(nameof(Register), new { id = result.UserId }, result);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        [HttpPost("bulk-register")]
        public async Task<IActionResult> BulkRegister(IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                    return BadRequest(new { message = "File is required" });

                if (!file.FileName.EndsWith(".xlsx", StringComparison.OrdinalIgnoreCase) &&
                    !file.FileName.EndsWith(".xls", StringComparison.OrdinalIgnoreCase))
                    return BadRequest(new { message = "Only Excel files (.xlsx, .xls) are allowed" });

                var registerRequests = new List<RegisterRequestWithRow>();

                using (var stream = new MemoryStream())
                {
                    await file.CopyToAsync(stream);
                    stream.Position = 0;

                    using (var package = new ExcelPackage(stream))
                    {
                        var worksheet = package.Workbook.Worksheets[0];
                        if (worksheet == null)
                            return BadRequest(new { message = "Excel file is empty or invalid" });

                        int rowCount = worksheet.Dimension?.Rows ?? 0;
                        if (rowCount < 2)
                            return BadRequest(new { message = "Excel file must have at least a header row and one data row" });

                        // Read header row (row 1) to find column indices
                        int emailCol = -1, passwordCol = -1, fullnameCol = -1, roleCol = -1, deptCol = -1;

                        for (int col = 1; col <= worksheet.Dimension.End.Column; col++)
                        {
                            var headerValue = worksheet.Cells[1, col].Value?.ToString()?.Trim().ToLower();
                            if (headerValue == null) continue;

                            if (headerValue == "email")
                                emailCol = col;
                            else if (headerValue == "password")
                                passwordCol = col;
                            else if (headerValue == "fullname" || headerValue == "full name")
                                fullnameCol = col;
                            else if (headerValue == "role")
                                roleCol = col;
                            else if (headerValue == "dept" || headerValue == "department")
                                deptCol = col;
                        }

                        if (emailCol == -1 || passwordCol == -1 || fullnameCol == -1 || roleCol == -1)
                            return BadRequest(new { message = "Excel file must have columns: Email, Password, Fullname, Role. Dept is optional." });

                        // Read data rows (starting from row 2)
                        for (int row = 2; row <= rowCount; row++)
                        {
                            var email = worksheet.Cells[row, emailCol].Value?.ToString()?.Trim();
                            var password = worksheet.Cells[row, passwordCol].Value?.ToString()?.Trim();
                            var fullname = worksheet.Cells[row, fullnameCol].Value?.ToString()?.Trim();
                            var role = worksheet.Cells[row, roleCol].Value?.ToString()?.Trim();
                            var deptValue = deptCol > 0 ? worksheet.Cells[row, deptCol].Value?.ToString()?.Trim() : null;

                            // Skip empty rows
                            if (string.IsNullOrWhiteSpace(email) || 
                                string.IsNullOrWhiteSpace(password) || 
                                string.IsNullOrWhiteSpace(fullname) || 
                                string.IsNullOrWhiteSpace(role))
                                continue;

                            int? deptId = null;
                            if (!string.IsNullOrWhiteSpace(deptValue) && int.TryParse(deptValue, out int deptIdParsed))
                            {
                                deptId = deptIdParsed;
                            }

                            registerRequests.Add(new RegisterRequestWithRow
                            {
                                Request = new RegisterRequest
                                {
                                    Email = email,
                                    Password = password,
                                    FullName = fullname,
                                    RoleName = role,
                                    DeptId = deptId
                                },
                                RowNumber = row
                            });
                        }
                    }
                }

                if (registerRequests.Count == 0)
                    return BadRequest(new { message = "No valid data rows found in Excel file" });

                var result = await _service.BulkRegisterAsync(registerRequests);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while processing the file", error = ex.Message });
            }
        }
        
    }
}
