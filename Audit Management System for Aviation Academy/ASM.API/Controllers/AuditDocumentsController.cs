using ASM_Services.Interfaces;
using ASM_Services.Interfaces.SQAStaffInterfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ASM.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuditDocumentsController : ControllerBase
    {
        private readonly IAuditDocumentService _auditDocumentService;
        private readonly long _maxFileSizeBytes = 10 * 1024 * 1024;
        private readonly string[] _allowedFileTypes = new[]
        {
            "application/pdf",
            "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
            "image/jpeg",
            "image/png"
        };

        public AuditDocumentsController(IAuditDocumentService auditDocumentService)
        {
            _auditDocumentService = auditDocumentService;
        }

        [HttpPost("upload/{auditId:guid}")]
        public async Task<IActionResult> UploadAuditDocument(Guid auditId, IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("File is empty");

            if (file.Length > _maxFileSizeBytes)
                return BadRequest($"File size exceeds {_maxFileSizeBytes / (1024 * 1024)} MB limit.");

            if (!_allowedFileTypes.Contains(file.ContentType))
                return BadRequest("Invalid file type. Only PDF, DOCX, JPG, PNG are allowed.");

            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
                return Unauthorized("User not authenticated");

            try
            {
                var updatedDoc = await _auditDocumentService.UploadAndUpdateAuditDocumentAsync(auditId, file, Guid.Parse(userIdClaim));

                if (updatedDoc == null)
                    return NotFound($"No document found for AuditId {auditId}");
                var doc = await _auditDocumentService.GetAuditDocumentByAuditIdAsync(auditId);
                return Ok(doc);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("{auditId}")]
        public async Task<IActionResult> GetDocumentsByAuditId(Guid auditId)
        {
            try
            {
                var document = await _auditDocumentService.GetAuditDocumentByAuditIdAsync(auditId);

                if (document == null)
                    return NotFound(new { message = "No audit document found", auditId });

                return Ok(document);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Internal server error",
                    error = ex.Message
                });
            }
        }

        [HttpPost("upload-multiple/{auditId:guid}")]
        public async Task<IActionResult> UploadMultipleAuditDocuments(Guid auditId, List<IFormFile> files)
        {
            if (files == null || !files.Any())
                return BadRequest("No files uploaded");

            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
                return Unauthorized("User not authenticated");

            try
            {
                var result = await _auditDocumentService.UploadMultipleAsync(auditId, files, Guid.Parse(userIdClaim));
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

    }
}