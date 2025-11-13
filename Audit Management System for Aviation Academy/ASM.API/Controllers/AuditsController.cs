using ASM_Repositories.Entities;
using ASM_Repositories.Models.AuditDTO;
using ASM_Services.Interfaces.AdminInterfaces;
using ASM_Services.Interfaces.SQAStaffInterfaces;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Reflection.Metadata;
using System.Security.Claims;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;
using Font = System.Drawing.Font;

namespace ASM.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuditsController : ControllerBase
    {
        private readonly IAuditService _service;
        private readonly IWebHostEnvironment _env;
        private readonly IFindingService _findingService;
        private readonly IAttachmentService _attachmentService;
        public AuditsController(IAuditService service, IWebHostEnvironment env, IFindingService findingService, IAttachmentService attachmentService)
        {
            _service = service;
            _env = env;
            _findingService = findingService;
            _attachmentService = attachmentService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ViewAudit>>> GetAll()
        {
            try
            {
                var result = await _service.GetAllAuditAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving audits", error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ViewAudit>> GetById(Guid id)
        {
            try
            {
                if (id == Guid.Empty)
                {
                    return BadRequest(new { message = "Invalid audit ID" });
                }

                var result = await _service.GetAuditByIdAsync(id);
                if (result == null)
                {
                    return NotFound(new { message = $"Audit with ID {id} not found" });
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving the audit", error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<ActionResult<ViewAudit>> Create([FromBody] CreateAudit dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState
                        .Where(x => x.Value.Errors.Count > 0)
                        .SelectMany(x => x.Value.Errors.Select(e => new
                        {
                            Field = x.Key,
                            Message = e.ErrorMessage
                        }))
                        .ToList();

                    return BadRequest(new
                    {
                        message = "Validation failed",
                        errors = errors
                    });
                }

                if (string.IsNullOrWhiteSpace(dto.Title))
                {
                    return BadRequest(new { message = "Title is required" });
                }

                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                Guid? userId = null;
                if (userIdClaim != null && Guid.TryParse(userIdClaim.Value, out Guid parsedUserId))
                {
                    userId = parsedUserId;
                }

                var result = await _service.CreateAuditAsync(dto, userId);
                return CreatedAtAction(nameof(GetById), new { id = result.AuditId }, result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while creating the audit", error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ViewAudit>> Update(Guid id, [FromBody] UpdateAudit dto)
        {
            try
            {
                if (id == Guid.Empty)
                {
                    return BadRequest(new { message = "Invalid audit ID" });
                }

                if (!ModelState.IsValid)
                {
                    var errors = ModelState
                        .Where(x => x.Value.Errors.Count > 0)
                        .SelectMany(x => x.Value.Errors.Select(e => new
                        {
                            Field = x.Key,
                            Message = e.ErrorMessage
                        }))
                        .ToList();

                    return BadRequest(new
                    {
                        message = "Validation failed",
                        errors = errors
                    });
                }

                var result = await _service.UpdateAuditAsync(id, dto);
                if (result == null)
                {
                    return NotFound(new { message = $"Audit with ID {id} not found" });
                }

                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating the audit", error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(Guid id)
        {
            try
            {
                if (id == Guid.Empty)
                {
                    return BadRequest(new { message = "Invalid audit ID" });
                }

                var result = await _service.DeleteAuditAsync(id);
                if (!result)
                {
                    return NotFound(new { message = $"Audit with ID {id} not found" });
                }

                return Ok(new { message = "Audit deleted successfully (status changed to Inactive)" });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while deleting the audit", error = ex.Message });
            }
        }

        public class RejectPlanRequest
        {
            public string Comment { get; set; }
        }

        public class ApproveForwardDirectorRequest
        {
            public string Comment { get; set; }
        }

        public class ApprovePlanRequest
        {
            public string Comment { get; set; }
        }

        [HttpPost("{id}/submit-to-lead-auditor")]
        public async Task<ActionResult> SubmitToLeadAuditor(Guid id)
        {
            try
            {
                if (id == Guid.Empty)
                {
                    return BadRequest(new { message = "Invalid audit ID" });
                }

                var ok = await _service.SubmitToLeadAuditorAsync(id);
                if (!ok)
                {
                    return NotFound(new { message = $"Audit with ID {id} not found" });
                }

                return Ok(new { message = "Submitted to Lead Auditor successfully. Audit status set to PendingReview." });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while submitting the audit", error = ex.Message });
            }
        }

        [HttpPost("{id}/reject-plan-content")]
        public async Task<ActionResult> RejectPlanContent(Guid id, [FromBody] RejectPlanRequest request)
        {
            try
            {
                if (id == Guid.Empty)
                {
                    return BadRequest(new { message = "Invalid audit ID" });
                }

                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out Guid approverId))
                {
                    return Unauthorized(new { message = "Invalid or missing user token" });
                }

                var ok = await _service.RejectPlanContentAsync(id, approverId, request?.Comment);
                if (!ok)
                {
                    return NotFound(new { message = $"Audit with ID {id} not found" });
                }

                return Ok(new { message = "Plan content rejected. Audit status set to Rejected." });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while rejecting the plan content", error = ex.Message });
            }
        }

        [HttpPost("{id}/approve-forward-director")]
        public async Task<ActionResult> ApproveAndForwardToDirector(Guid id, [FromBody] ApproveForwardDirectorRequest request)
        {
            try
            {
                if (id == Guid.Empty)
                {
                    return BadRequest(new { message = "Invalid audit ID" });
                }

                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out Guid approverId))
                {
                    return Unauthorized(new { message = "Invalid or missing user token" });
                }

                var ok = await _service.ApproveAndForwardToDirectorAsync(id, approverId, request?.Comment);
                if (!ok)
                {
                    return NotFound(new { message = $"Audit with ID {id} not found" });
                }

                return Ok(new { message = "Approved and forwarded to Director. Audit status set to PendingDirectorApproval." });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while approving and forwarding the audit", error = ex.Message });
            }
        }

        [HttpPost("{id}/approve-plan")]
        public async Task<ActionResult> ApprovePlan(Guid id, [FromBody] ApprovePlanRequest request)
        {
            try
            {
                if (id == Guid.Empty)
                {
                    return BadRequest(new { message = "Invalid audit ID" });
                }

                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out Guid approverId))
                {
                    return Unauthorized(new { message = "Invalid or missing user token" });
                }

                var ok = await _service.ApprovePlanAsync(id, approverId, request?.Comment);
                if (!ok)
                {
                    return NotFound(new { message = $"Audit with ID {id} not found" });
                }

                return Ok(new { message = "Plan approved. Audit status set to Approve." });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while approving the plan", error = ex.Message });
            }
        }

        [HttpGet("Summary/{auditId:guid}")]
        public async Task<IActionResult> GetSummary(Guid auditId)
        {
            var dto = await _service.GetAuditSummaryAsync(auditId);
            if (dto == null) return NotFound("Audit not found");
            return Ok(dto);
        }

        [HttpPost("Submit/{auditId:guid}")]
        public async Task<IActionResult> Submit(Guid auditId)
        {
            await _service.SubmitAuditAsync(auditId);
            return Ok(new { message = "Audit submitted", auditId });
        }

        [HttpGet("{auditId:guid}/chart/line")]
        public async Task<IActionResult> GetFindingsByMonthChart(Guid auditId)
        {
            var data = await _findingService.GetFindingsByMonthAsync(auditId);
            if (data == null || !data.Any())
                return Ok(new List<object>());

            var result = data.Select(x => new
            {
                month = x.Date.ToString("yyyy-MM"),
                count = x.Count
            });

            return Ok(result);
        }

        [HttpGet("{auditId:guid}/chart/pie")]
        public async Task<IActionResult> GetSeverityPieChart(Guid auditId)
        {
            var summary = await _service.GetAuditSummaryAsync(auditId);
            if (summary?.SeverityBreakdown == null)
                return Ok(new List<object>());

            var result = summary.SeverityBreakdown.Select(kv => new
            {
                severity = kv.Key,
                count = kv.Value
            });

            return Ok(result);
        }

        [HttpGet("{auditId:guid}/chart/bar")]
        public async Task<IActionResult> GetDepartmentBarChart(Guid auditId)
        {
            var data = await _findingService.GetDepartmentFindingsInCurrentMonthAsync(auditId);
            if (data == null || !data.Any())
                return Ok(new List<object>());

            var result = data.Select(x => new
            {
                department = x.Department,
                count = x.Count
            });

            return Ok(result);
        }

        [HttpGet("ExportPdf/{auditId:guid}")]
        public async Task<IActionResult> ExportPdf(Guid auditId)
        {
            var summary = await _service.GetAuditSummaryAsync(auditId);
            if (summary == null) return NotFound("Audit not found");
            var data = await _findingService.GetDepartmentFindingsInCurrentMonthAsync(auditId);
            var findingsByMonth = await _findingService.GetFindingsByMonthAsync(auditId);
            var findings = await _findingService.GetFindingsAsync(auditId);
            var attachments = await _attachmentService.GetAttachmentsAsync(findings.Select(f => f.FindingId).ToList());
            byte[]? logo = GetLogoBytes("wwwroot/images/aviation_logo.png");

            var charts = new List<byte[]>{
                GenerateLineChartPng(findingsByMonth),
                GeneratePieChartPng(summary.SeverityBreakdown.Select(kv => (kv.Key, kv.Value)).ToList()),
                GenerateBarChartPng(data.Select(x => (x.Department, x.Count)).ToList()),
        
            };
            var pdfBytes = GenerateFullPdf(summary, findings, attachments, logo, charts);
            return File(pdfBytes, "application/pdf", $"Audit_{auditId}.pdf");
        }


        [HttpGet("ExportExcel/{auditId:guid}")]
        public async Task<IActionResult> ExportExcel(Guid auditId)
        {
            var summary = await _service.GetAuditSummaryAsync(auditId);
            if (summary == null) return NotFound("Audit not found");

            var findings = await _findingService.GetFindingsAsync(auditId);
            var bytes = GenerateExcelFile(summary, findings);
            return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"Audit_{auditId}.xlsx");
        }

        // ================= Helpers =================

        private byte[]? GetLogoBytes(string relativePath)
        {
            var path = Path.Combine(_env.ContentRootPath, relativePath);
            if (!System.IO.File.Exists(path)) return null;
            return System.IO.File.ReadAllBytes(path);
        }

        private byte[] GenerateLineChartPng(List<(DateTime Date, int Count)> rawData)
        {
            int width = 540, height = 300;
            using var bmp = new Bitmap(width, height);
            using var g = Graphics.FromImage(bmp);
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.Clear(Color.White);

            int margin = 50;
            int chartW = width - margin * 2;
            int chartH = height - margin * 2;

            // chuẩn hoá: lấy Count cho các tháng 1..12
            var months = Enumerable.Range(1, 12).Select(m =>
            {
                var match = rawData.FirstOrDefault(d => d.Date.Month == m);
                return (Month: m, Count: match.Count);
            }).ToList();

            int rawMax = months.Max(x => x.Count);
            int min = 0;

            // tính tick size an toàn: chia ra tickCount phần
            int tickCount = 5;
            int tickSize = Math.Max(1, (int)Math.Ceiling(rawMax / (double)tickCount));
            int displayMax = tickSize * tickCount;
            if (displayMax == 0) { displayMax = tickCount; tickSize = 1; } // trường hợp tất cả 0

            // Khoảng giữa hai điểm liên tiếp: có 11 khoảng giữa 12 điểm
            float stepX = chartW / 11f;
            float xOffset = stepX * 0.2f;

            using var font = new Font("Arial", 9);
            using var gridPen = new Pen(Color.FromArgb(230, 230, 230));
            using var axisPen = new Pen(Color.Black, 1);

            // Vẽ grid ngang & nhãn Y (0, tickSize, 2*tickSize, ... displayMax)
            for (int i = 0; i <= tickCount; i++)
            {
                int value = i * tickSize;
                float y = height - margin - (float)(value / (double)displayMax * chartH);
                g.DrawLine(gridPen, margin, y, width - margin, y);
                var lbl = value.ToString();
                var sz = g.MeasureString(lbl, font);
                g.DrawString(lbl, font, Brushes.Black, margin - sz.Width - 6, y - sz.Height / 2);
            }

            // Tính tọa độ điểm: đặt tháng 1 tại margin, tháng 12 tại margin + 11*stepX
            var points = months.Select(m =>
            {
                // nếu là tháng 1 thì dịch sang phải 1 xíu, nếu là tháng 12 thì giữ nguyên
                float extra = (m.Month == 1) ? stepX * 0.2f :
                              (m.Month == 12 ? -stepX * 0.1f : 0f);

                return new PointF(
                    margin + (m.Month - 1) * stepX + extra,
                    height - margin - (float)(m.Count / (double)displayMax * chartH)
                );
            }).ToArray();

            // Vẽ trục
            g.DrawLine(axisPen, margin, height - margin, width - margin, height - margin); // X
            g.DrawLine(axisPen, margin, margin, margin, height - margin); // Y

            // Vẽ đường nối
            if (points.Length > 1)
            {
                using var linePen = new Pen(Color.FromArgb(69, 123, 157), 2);
                g.DrawLines(linePen, points);
            }

            // 🔹 Vẽ đường thẳng từ trục X lên tới từng điểm
            using var guidePen = new Pen(Color.FromArgb(200, 220, 220, 220), 1);
            foreach (var pt in points)
            {
                g.DrawLine(guidePen, pt.X, height - margin, pt.X, pt.Y);
            }

            // Vẽ điểm & values
            using var valueFont = new Font("Arial", 8, FontStyle.Bold);
            for (int i = 0; i < points.Length; i++)
            {
                var pt = points[i];
                // điểm
                g.FillEllipse(Brushes.OrangeRed, pt.X - 3.5f, pt.Y - 3.5f, 7, 7);
                // số trên điểm
                string txt = months[i].Count.ToString();
                var txtSz = g.MeasureString(txt, valueFont);
                g.DrawString(txt, valueFont, Brushes.Black, pt.X - txtSz.Width / 2, pt.Y - txtSz.Height - 6);
            }

            // Vẽ nhãn X: tháng 1..12 (căn giữa mỗi điểm)
            for (int i = 0; i < 12; i++)
            {
                string lbl = (i + 1).ToString();
                var lblSz = g.MeasureString(lbl, font);

                // tương tự phần trên
                float extra = (i == 0) ? stepX * 0.2f :
                              (i == 11 ? -stepX * 0.1f : 0f);

                float x = margin + i * stepX + extra - lblSz.Width / 2;
                g.DrawString(lbl, font, Brushes.Black, x, height - margin + 6);
            }

            // Viền
            using var border = new Pen(Color.LightGray, 1);
            g.DrawRectangle(border, margin, margin, chartW, chartH);

            using var ms = new MemoryStream();
            bmp.Save(ms, ImageFormat.Png);
            return ms.ToArray();
        }
        private byte[] GeneratePieChartPng(List<(string Name, int Count)> groups)
        {
            int width = 360, height = 220;
            using var bmp = new Bitmap(width, height);
            using var g = Graphics.FromImage(bmp);
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.Clear(Color.White);

            var total = groups.Sum(x => x.Count);
            if (total == 0)
            {
                g.DrawString("No data", new Font("Arial", 14, FontStyle.Italic), Brushes.Gray, new PointF(120, 100));
            }
            else
            {
                var rect = new Rectangle(20, 20, 140, 140);
                float start = 0f;
                var palette = new[] {
            Color.FromArgb(230,57,70), Color.FromArgb(69,123,157),
            Color.FromArgb(168,218,220), Color.FromArgb(29,53,87),
            Color.FromArgb(241,196,15), Color.FromArgb(46,204,113)
        };

                using var font = new Font("Arial", 8);
                for (int i = 0; i < groups.Count; i++)
                {
                    var sweep = 360f * groups[i].Count / total;
                    using var brush = new SolidBrush(palette[i % palette.Length]);
                    g.FillPie(brush, rect, start, sweep);

                    // Hiển thị phần trăm
                    float mid = start + sweep / 2;
                    double rad = mid * Math.PI / 180;
                    float labelX = (float)(rect.X + rect.Width / 2 + Math.Cos(rad) * 45);
                    float labelY = (float)(rect.Y + rect.Height / 2 + Math.Sin(rad) * 45);
                    string label = $"{(groups[i].Count * 100 / total)}%";
                    g.DrawString(label, font, Brushes.Black, labelX - 10, labelY - 6);

                    start += sweep;
                }

                g.FillEllipse(Brushes.White, 60, 60, 60, 60);

                // Legend
                int lx = 180, ly = 30;
                for (int i = 0; i < groups.Count; i++)
                {
                    using var brush = new SolidBrush(palette[i % palette.Length]);
                    g.FillRectangle(brush, lx, ly + i * 18, 12, 12);
                    g.DrawString($"{groups[i].Name} ({groups[i].Count})", font, Brushes.Black, lx + 18, ly + i * 18 - 2);
                }
            }

            using var ms = new MemoryStream();
            bmp.Save(ms, ImageFormat.Png);
            return ms.ToArray();
        }
        private byte[] GenerateBarChartPng(List<(string Dept, int Count)> data)
        {
            int width = 600, height = 320;
            using var bmp = new Bitmap(width, height);
            using var g = Graphics.FromImage(bmp);
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.Clear(Color.White);

            int marginLeft = 60;
            int marginBottom = 60;
            int marginTop = 20;
            int marginRight = 20;
            int chartW = width - marginLeft - marginRight;
            int chartH = height - marginTop - marginBottom;

            using var axisPen = new Pen(Color.Black, 1);
            using var gridPen = new Pen(Color.FromArgb(220, 220, 220), 1);

            g.DrawLine(axisPen, marginLeft, height - marginBottom, width - marginRight, height - marginBottom); // X
            g.DrawLine(axisPen, marginLeft, marginTop, marginLeft, height - marginBottom); // Y

            if (data == null || data.Count == 0)
            {
                g.DrawString("No data", new Font("Arial", 14, FontStyle.Italic), Brushes.Gray, new PointF(width / 2 - 40, height / 2 - 10));
            }
            else
            {
                int n = data.Count;
                float barWidth = chartW / (float)(n * 1.5);
                float spacing = barWidth * 0.5f;

                int maxCount = data.Max(d => d.Count);
                if (maxCount == 0) maxCount = 1;

                using var yFont = new Font("Arial", 8);
                for (int yVal = 0; yVal <= maxCount; yVal++)
                {
                    float y = height - marginBottom - (float)yVal / maxCount * chartH;
                    g.DrawLine(gridPen, marginLeft, y, width - marginRight, y);
                    g.DrawString(yVal.ToString(), yFont, Brushes.Black, marginLeft - 30, y - 6);
                }

                Color barColor = Color.FromArgb(180, 110, 193, 228); 
                using var barBrush = new SolidBrush(barColor);
                using var font = new Font("Arial", 8);

                for (int i = 0; i < n; i++)
                {
                    var (dept, count) = data[i];
                    float x = marginLeft + 10 + i * (barWidth + spacing);
                    float barH = (float)count / maxCount * chartH;
                    float yTop = height - marginBottom - barH;

                    g.FillRectangle(barBrush, x, yTop, barWidth, barH);
                    g.DrawRectangle(Pens.Gray, x, yTop, barWidth, barH);

                    string shortName = dept.Length > 8 ? dept.Substring(0, 6) + "…" : dept;

                    var sf = new StringFormat() { Alignment = StringAlignment.Center };
                    g.TranslateTransform(x + barWidth / 2, height - marginBottom + 15);
                    g.RotateTransform(45);
                    g.DrawString(shortName, font, Brushes.Black, 0, 0, sf);
                    g.ResetTransform();

                    string valStr = count.ToString();
                    g.DrawString(valStr, font, Brushes.Black,
                        x + barWidth / 2 - g.MeasureString(valStr, font).Width / 2,
                        yTop - 14);
                }
            }

            using var ms = new MemoryStream();
            bmp.Save(ms, ImageFormat.Png);
            return ms.ToArray();
        }

        private byte[] GenerateFullPdf(ViewAuditSummary summary, List<Finding> findings, List<Attachment> attachments, byte[]? logo, List<byte[]> charts)
        {
            var pdf = QuestPDF.Fluent.Document.Create(container =>
            {
                // =============== 1️⃣ COVER PAGE ===============
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(40);
                    page.DefaultTextStyle(x => x.FontSize(11));

                    page.Content().AlignMiddle().AlignCenter().Column(col =>
                    {
                        if (logo != null)
                            col.Item().AlignCenter().Width(120).Image(logo).FitWidth();

                        col.Item().Height(30);
                        col.Item().Text(summary.Title)
                            .FontSize(22).Bold().FontColor("#1B4965").AlignCenter();

                        col.Item().Text($"{summary.Type} — {summary.Scope}")
                            .FontSize(13).FontColor("#5C677D").AlignCenter();

                        col.Item().Height(20);
                        col.Item().Text($"Audit Period: {summary.StartDate:dd MMM yyyy} - {summary.EndDate:dd MMM yyyy}")
                            .FontSize(11).AlignCenter();

                        col.Item().Height(20);
                        col.Item().Text("Prepared by Audit Department")
                            .FontSize(11).Italic().FontColor("#999999").AlignCenter();

                        col.Item().Height(60);
                        col.Item().Text("CONFIDENTIAL DOCUMENT")
                            .FontSize(10).FontColor("#D90429").Bold().AlignCenter();
                    });
                });

                // =============== 2️⃣ CHART PAGE ===============
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(24);
                    page.DefaultTextStyle(x => x.FontSize(10));
                    page.Header().Text(summary.Title).Bold().FontSize(12).AlignCenter();

                    // =============== CHART ===============

                    page.Content().Column(col =>
                    {
                        col.Spacing(10);
                        col.Item().Text("1. Charts Overview").FontSize(13).Bold().AlignCenter().FontColor("#1B4965");

                        col.Item().Column(chartCol =>
                        {
                            chartCol.Spacing(12);

                            chartCol.Item().Row(r =>
                            {
                                var chart1 = charts.ElementAtOrDefault(0);
                                if (chart1 != null)
                                    r.RelativeItem().Padding(4).Border(0.5f).BorderColor("#CCCCCC")
                                        .Column(c =>
                                        {
                                            c.Item().Text("1.1 Biểu đồ đường (Line Chart)").FontSize(10).Bold();
                                            c.Item().Image(chart1).FitWidth();
                                        });

                                var chart2 = charts.ElementAtOrDefault(1);
                                if (chart2 != null)
                                    r.RelativeItem().Padding(4).Border(0.5f).BorderColor("#CCCCCC")
                                        .Column(c =>
                                        {
                                            c.Item().Text("1.2 Biểu đồ tròn (Pie / Donut Chart)").FontSize(10).Bold();
                                            c.Item().Image(chart2).FitWidth();
                                        });
                            });

                            var chart3 = charts.ElementAtOrDefault(2);
                            if (chart3 != null)
                                chartCol.Item().Padding(4).Border(0.5f).BorderColor("#CCCCCC")
                                    .Column(c =>
                                    {
                                        c.Item().Text("1.5 Biểu đồ tần suất (Histogram)").FontSize(10).Bold();
                                        c.Item().Image(chart3).FitWidth();
                                    });

                        });
                    });

                    // =============== FOOTER ===============

                    page.Footer().AlignCenter().Text(x =>
                    {
                        x.Span("Page ");
                        x.CurrentPageNumber();
                        x.Span(" / ");
                        x.TotalPages();
                    });
                });

                // =============== 3️⃣ SUMMARY + FINDINGS ===============
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(24);
                    page.DefaultTextStyle(x => x.FontSize(10));
                    page.Header().Text(summary.Title).Bold().FontSize(12).AlignCenter();

                    page.Content().Column(col =>
                    {
                        col.Spacing(10);
                        var headerColor = "#E9ECEF";
                        var borderColor = "#999999";
                        var rowBorder = "#CCCCCC";

                        // ---- 2️⃣ Summary Tables ----
                        col.Item().Text("2. Summary Tables").FontSize(13).Bold().AlignCenter().FontColor("#1B4965");

                        col.Item().Column(section =>
                        {
                            section.Spacing(12);
                            var headerColor = "#E9ECEF";
                            var borderColor = "#999999";
                            var rowBorder = "#CCCCCC";

                            // 2.1 Severity
                            section.Item().Text("2.1 Findings by Severity").Bold().FontSize(11);
                            section.Item().Table(t =>
                            {
                                t.ColumnsDefinition(cd =>
                                {
                                    cd.RelativeColumn(3);
                                    cd.RelativeColumn(1);
                                });

                                t.Header(h =>
                                {
                                    h.Cell().Background(headerColor).Border(0.5f).BorderColor(borderColor).Padding(5).Text("Severity").Bold();
                                    h.Cell().Background(headerColor).Border(0.5f).BorderColor(borderColor).Padding(5).Text("Count").Bold();
                                });

                                foreach (var kv in summary.SeverityBreakdown.OrderByDescending(k => k.Value))
                                {
                                    t.Cell().Border(0.5f).BorderColor(rowBorder).Padding(5).Text(kv.Key);
                                    t.Cell().Border(0.5f).BorderColor(rowBorder).Padding(5).AlignLeft().Text(kv.Value.ToString());
                                }
                            });

                            // 2.2 Department
                            section.Item().Text("2.2 Findings by Department").Bold().FontSize(11);

                            var deptSummary = summary.ByDepartment
                                .GroupBy(d => d.DeptName)
                                .Select(g => new
                                {
                                    DeptName = g.Key,
                                    Count = g.Sum(x => x.Count)
                                })
                                .OrderByDescending(x => x.Count)
                                .ToList();

                            section.Item().Table(t =>
                            {
                                t.ColumnsDefinition(cd =>
                                {
                                    cd.RelativeColumn(3);
                                    cd.RelativeColumn(1);
                                });

                                t.Header(h =>
                                {
                                    h.Cell().Background(headerColor).Border(0.5f).BorderColor(borderColor).Padding(5)
                                        .Text("Department").Bold();
                                    h.Cell().Background(headerColor).Border(0.5f).BorderColor(borderColor).Padding(5)
                                        .Text("Count").Bold();
                                });

                                foreach (var d in deptSummary)
                                {
                                    t.Cell().Border(0.5f).BorderColor(rowBorder).Padding(5).Text(d.DeptName);
                                    t.Cell().Border(0.5f).BorderColor(rowBorder).Padding(5).Text(d.Count.ToString());
                                }
                            });


                            // 2.3 Root Cause
                            section.Item().Text("2.3 Findings by Root Cause").Bold().FontSize(11);
                            section.Item().Table(t =>
                            {
                                t.ColumnsDefinition(cd =>
                                {
                                    cd.RelativeColumn(3);
                                    cd.RelativeColumn(1);
                                });

                                t.Header(h =>
                                {
                                    h.Cell().Background(headerColor).Border(0.5f).BorderColor(borderColor).Padding(5).Text("Root Cause").Bold();
                                    h.Cell().Background(headerColor).Border(0.5f).BorderColor(borderColor).Padding(5).Text("Count").Bold();
                                });

                                foreach (var r in summary.ByRootCause)
                                {
                                    t.Cell().Border(0.5f).BorderColor(rowBorder).Padding(5).Text(r.RootCause);
                                    t.Cell().Border(0.5f).BorderColor(rowBorder).Padding(5).AlignLeft().Text(r.Count.ToString());
                                }
                            });
                        });



                        // --- Detailed Findings Section ---
                        col.Item().Text("3. Detailed Findings").Bold().FontSize(12);

                        col.Item().Table(t =>
                        {
                            t.ColumnsDefinition(cd =>
                            {
                                cd.ConstantColumn(28);
                                cd.RelativeColumn(4);
                                cd.RelativeColumn(2);
                                cd.RelativeColumn(1);
                                cd.RelativeColumn(1);
                                cd.RelativeColumn(1.5f); 
                            });

                            string headerColor = "#E9ECEF";
                            string borderColor = "#999999";

                            t.Header(h =>
                            {
                                h.Cell().Background(headerColor).Border(0.5f).BorderColor(borderColor).Padding(5).AlignCenter().Text("#").Bold();
                                h.Cell().Background(headerColor).Border(0.5f).BorderColor(borderColor).Padding(5).Text("Title").Bold();
                                h.Cell().Background(headerColor).Border(0.5f).BorderColor(borderColor).Padding(5).AlignCenter().Text("Dept").Bold();
                                h.Cell().Background(headerColor).Border(0.5f).BorderColor(borderColor).Padding(5).AlignCenter().Text("Severity").Bold();
                                h.Cell().Background(headerColor).Border(0.5f).BorderColor(borderColor).Padding(5).AlignCenter().Text("Status").Bold();
                                h.Cell().Background(headerColor).Border(0.5f).BorderColor(borderColor).Padding(5).AlignCenter().Text("Deadline").Bold();
                            });

                            int idx = 1;
                            bool alt = false;

                            foreach (var f in findings)
                            {
                                string border = "#CCCCCC";
                                string bg = alt ? "#F9FAFB" : "#FFFFFF";
                                alt = !alt;

                                t.Cell().Background(bg).Border(0.5f).BorderColor(border).Padding(4).AlignCenter().Text(idx++.ToString());
                                t.Cell().Background(bg).Border(0.5f).BorderColor(border).Padding(4).Text(f.Title ?? "-");
                                t.Cell().Background(bg).Border(0.5f).BorderColor(border).Padding(4).AlignCenter().Text(f.DeptId?.ToString() ?? "-");
                                t.Cell().Background(bg).Border(0.5f).BorderColor(border).Padding(4).AlignCenter().Text(f.Severity ?? "-");
                                t.Cell().Background(bg).Border(0.5f).BorderColor(border).Padding(4).AlignCenter().Text(f.Status ?? "-");
                                t.Cell().Background(bg).Border(0.5f).BorderColor(border).Padding(4).AlignCenter().Text(f.Deadline?.ToString("yyyy-MM-dd") ?? "-");
                            }
                        });
                    });

                    page.Footer().AlignCenter().Text(x =>
                    {
                        x.Span("Page ");
                        x.CurrentPageNumber();
                        x.Span(" / ");
                        x.TotalPages();
                    });
                });

                // =============== 4️⃣ SIGNATURE PAGE ===============
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(40);

                    page.Content().Column(col =>
                    {
                        col.Spacing(20);
                        col.Item().Text("Approval and Signatures").Bold().FontSize(14).AlignCenter();

                        col.Item().Row(row =>
                        {
                            row.RelativeItem().Border(1).Padding(20).Column(c =>
                            {
                                c.Item().Text("Prepared by:").Bold();
                                c.Item().Height(60);
                                c.Item().Text("Signature: ____________________________");
                                c.Item().Text("Name: ____________________________");
                                c.Item().Text("Date: ____________________________");
                            });

                            row.RelativeItem().Border(1).Padding(20).Column(c =>
                            {
                                c.Item().Text("Reviewed by (Lead Auditor):").Bold();
                                c.Item().Height(60);
                                c.Item().Text("Signature: ____________________________");
                                c.Item().Text("Name: ____________________________");
                                c.Item().Text("Date: ____________________________");
                            });
                        });

                        col.Item().Row(row =>
                        {
                            row.RelativeItem().Border(1).Padding(20).Column(c =>
                            {
                                c.Item().Text("Approved by (Director QA):").Bold();
                                c.Item().Height(60);
                                c.Item().Text("Signature: ____________________________");
                                c.Item().Text("Name: ____________________________");
                                c.Item().Text("Date: ____________________________");
                            });
                        });
                    });
                });
            });

            return pdf.GeneratePdf();
        }


        private byte[] GenerateExcelFile(ViewAuditSummary summary, List<Finding> findings)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using var pkg = new ExcelPackage();

            // ---------- Sheet 1: Audit Summary ----------
            var ws = pkg.Workbook.Worksheets.Add("Audit Summary");
            ws.Cells["A1"].Value = "Audit Title"; ws.Cells["B1"].Value = summary.Title;
            ws.Cells["A2"].Value = "Type"; ws.Cells["B2"].Value = summary.Type;
            ws.Cells["A3"].Value = "Scope"; ws.Cells["B3"].Value = summary.Scope;
            ws.Cells["A4"].Value = "Period"; ws.Cells["B4"].Value = $"{summary.StartDate:yyyy-MM-dd} - {summary.EndDate:yyyy-MM-dd}";
            ws.Cells["A6"].Value = "Total Findings"; ws.Cells["B6"].Value = summary.TotalFindings;

            // Severity summary
            int row = 8;
            ws.Cells[row, 1].Value = "Severity"; ws.Cells[row, 2].Value = "Count"; row++;
            foreach (var kv in summary.SeverityBreakdown.OrderByDescending(k => k.Value))
            {
                ws.Cells[row, 1].Value = kv.Key;
                ws.Cells[row, 2].Value = kv.Value;
                row++;
            }

            // Findings by Department
            row += 1;
            ws.Cells[row, 1].Value = "Department"; ws.Cells[row, 2].Value = "Count"; row++;
            foreach (var d in summary.ByDepartment)
            {
                ws.Cells[row, 1].Value = d.DeptName;
                ws.Cells[row, 2].Value = d.Count;
                row++;
            }

            // Findings by RootCause
            row += 1;
            ws.Cells[row, 1].Value = "Root Cause"; ws.Cells[row, 2].Value = "Count"; row++;
            foreach (var r in summary.ByRootCause)
            {
                ws.Cells[row, 1].Value = r.RootCause;
                ws.Cells[row, 2].Value = r.Count;
                row++;
            }

            // ---------- Sheet 2: Detailed Findings ----------
            var ws2 = pkg.Workbook.Worksheets.Add("Findings");
            ws2.Cells[1, 1].Value = "No";
            ws2.Cells[1, 2].Value = "Title";
            ws2.Cells[1, 3].Value = "DeptID";
            ws2.Cells[1, 4].Value = "Severity";
            ws2.Cells[1, 5].Value = "Status";
            ws2.Cells[1, 6].Value = "Deadline";

            int r2 = 2; int idx = 1;
            foreach (var f in findings)
            {
                ws2.Cells[r2, 1].Value = idx++;
                ws2.Cells[r2, 2].Value = f.Title;
                ws2.Cells[r2, 3].Value = f.DeptId?.ToString() ?? "-";
                ws2.Cells[r2, 4].Value = f.Severity;
                ws2.Cells[r2, 5].Value = f.Status;
                ws2.Cells[r2, 6].Value = f.Deadline?.ToString("yyyy-MM-dd");
                r2++;
            }

            ws.Cells.AutoFitColumns();
            ws2.Cells.AutoFitColumns();

            return pkg.GetAsByteArray();
        }

    }
}