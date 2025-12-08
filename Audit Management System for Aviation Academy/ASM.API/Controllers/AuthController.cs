using ASM_Repositories.DBContext;
using ASM_Repositories.Models.LoginDTO;
using ASM_Services.Interfaces;
using ASM_Services.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        private readonly AuditManagementSystemForAviationAcademyContext _dbContext;

        public AuthController(IAuthService service, AuditManagementSystemForAviationAcademyContext dbContext)
        {
            _service = service;
            _dbContext = dbContext;
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
            var validationErrors = new List<BulkRegisterError>();
            
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
                            else if (headerValue == "dept" || headerValue == "department" || headerValue == "department name")
                                deptCol = col;
                        }

                        if (emailCol == -1 || passwordCol == -1 || fullnameCol == -1 || roleCol == -1)
                            return BadRequest(new { message = "Excel file must have columns: Email, Password, Fullname, Role. Department Name is optional." });

                        // Load all departments into memory for faster lookup
                        var departments = await _dbContext.Departments
                            .Where(d => d.Status == "Active")
                            .ToListAsync();
                        var departmentDict = departments.ToDictionary(d => d.Name.Trim(), StringComparer.OrdinalIgnoreCase);

                        // Read data rows (starting from row 2)
                        for (int row = 2; row <= rowCount; row++)
                        {
                            var email = worksheet.Cells[row, emailCol].Value?.ToString()?.Trim();
                            var password = worksheet.Cells[row, passwordCol].Value?.ToString()?.Trim();
                            var fullname = worksheet.Cells[row, fullnameCol].Value?.ToString()?.Trim();
                            var role = worksheet.Cells[row, roleCol].Value?.ToString()?.Trim();
                            var deptName = deptCol > 0 ? worksheet.Cells[row, deptCol].Value?.ToString()?.Trim() : null;

                            // Validate required fields
                            var rowErrors = new List<string>();

                            if (string.IsNullOrWhiteSpace(email))
                                rowErrors.Add("Email is required");
                            else if (!IsValidEmail(email))
                                rowErrors.Add($"Email format is invalid: {email}");

                            if (string.IsNullOrWhiteSpace(password))
                                rowErrors.Add("Password is required");
                            else if (password.Length < 6)
                                rowErrors.Add("Password must be at least 6 characters long");

                            if (string.IsNullOrWhiteSpace(fullname))
                                rowErrors.Add("Fullname is required");

                            if (string.IsNullOrWhiteSpace(role))
                                rowErrors.Add("Role is required");

                            // Validate Department Name if provided and find DeptId
                            int? deptId = null;
                            if (!string.IsNullOrWhiteSpace(deptName))
                            {
                                // Trim deptName để đảm bảo so sánh chính xác
                                var trimmedDeptName = deptName.Trim();
                                
                                // Tìm department theo tên (không phân biệt hoa thường)
                                if (departmentDict.TryGetValue(trimmedDeptName, out var department))
                                {
                                    // Tìm thấy department, gán DeptId vào user
                                    deptId = department.DeptId;
                                }
                                else
                                {
                                    // Không tìm thấy department, báo lỗi
                                    rowErrors.Add($"Department '{deptName}' not found. Available departments: {string.Join(", ", departmentDict.Keys.Take(10))}");
                                }
                            }

                            // If there are validation errors for this row, add to error list
                            if (rowErrors.Any())
                            {
                                validationErrors.Add(new BulkRegisterError
                                {
                                    RowNumber = row,
                                    Email = email ?? "N/A",
                                    ErrorMessage = string.Join("; ", rowErrors)
                                });
                                continue;
                            }

                            // All validations passed, add to register requests
                            registerRequests.Add(new RegisterRequestWithRow
                            {
                                Request = new RegisterRequest
                                {
                                    Email = email!,
                                    Password = password!,
                                    FullName = fullname!,
                                    RoleName = role!,
                                    DeptId = deptId
                                },
                                RowNumber = row
                            });
                        }
                    }
                }

                if (registerRequests.Count == 0 && validationErrors.Count == 0)
                    return BadRequest(new { message = "No valid data rows found in Excel file" });

                // Process registration
                var result = await _service.BulkRegisterAsync(registerRequests);

                // Merge validation errors with registration errors
                result.ErrorItems.AddRange(validationErrors);
                result.FailureCount += validationErrors.Count;
                result.TotalRows = registerRequests.Count + validationErrors.Count;

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { 
                    message = "An error occurred while processing the file", 
                    error = ex.Message,
                    stackTrace = ex.StackTrace
                });
            }
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
        {
            try
            {
                if (request == null)
                    return BadRequest(new { message = "Request body is required" });

                if (string.IsNullOrWhiteSpace(request.Email))
                    return BadRequest(new { message = "Email is required" });

                // Validate email format
                if (!IsValidEmail(request.Email))
                    return BadRequest(new { message = $"Email format is invalid: {request.Email}" });

                // Validate new password if provided
                if (!string.IsNullOrWhiteSpace(request.NewPassword) && request.NewPassword.Length < 6)
                    return BadRequest(new { message = "New password must be at least 6 characters long" });

                var result = await _service.ResetPasswordAsync(request);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { 
                    message = "An error occurred while resetting password", 
                    error = ex.Message 
                });
            }
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
        
    }
}
