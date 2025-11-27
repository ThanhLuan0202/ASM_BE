using ASM_Repositories.Entities;
using ASM_Services.Interfaces;
using ASM_Services.Models.Email;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace ASM_Services.Services
{
    public class EmailService : IEmailService
    {
        private readonly MailSettings _mailSettings;

        public EmailService(IOptions<MailSettings> mailSettings)
        {
            _mailSettings = mailSettings.Value;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            var email = new MimeMessage();
            email.Sender = MailboxAddress.Parse(_mailSettings.From);
            email.To.Add(MailboxAddress.Parse(toEmail));
            email.Subject = subject;

            var builder = new BodyBuilder
            {
                HtmlBody = body
            };
            email.Body = builder.ToMessageBody();

            using var smtp = new SmtpClient();

            smtp.AuthenticationMechanisms.Remove("XOAUTH2");

            await smtp.ConnectAsync(_mailSettings.SmtpServer, _mailSettings.Port,
                MailKit.Security.SecureSocketOptions.StartTls);

            await smtp.AuthenticateAsync(_mailSettings.From, _mailSettings.Password);

            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);
        }




        public async Task SendForLeadAuditor(string toEmail, string auditTitle, string leadFullName, DateTime? fieldworkStart)
        {
            string subject = $"[Audit Review] Kế hoạch kiểm định cần Lead Auditor xem xét – {auditTitle}";

            string body = $@"
<p>Xin chào <strong>{leadFullName}</strong>,</p>

<p>Bạn được chỉ định là <strong>Lead Auditor</strong> cho cuộc kiểm định:</p>

<p style='margin-left:20px'>
    <strong>Tên Audit:</strong> {auditTitle}<br/>
    <strong>Ngày bắt đầu Fieldwork:</strong> {fieldworkStart:dd/MM/yyyy}<br/>
</p>

<p>Auditor đã hoàn tất bản <strong>Audit Plan</strong> và gửi lên để bạn xem xét.</p>

<p>Vui lòng truy cập hệ thống và thực hiện các bước:</p>

<ol>
    <li>Kiểm tra phạm vi (Scope), tiêu chí (Criteria) và Objectives.</li>
    <li>Kiểm tra danh sách Checklist và tính phù hợp.</li>
    <li>Kiểm tra Schedule (Kickoff, Evidence Due, Fieldwork Start...).</li>
    <li>Phê duyệt hoặc yêu cầu chỉnh sửa Audit Plan.</li>
</ol>



<p>Xin cảm ơn vì sự hợp tác và hỗ trợ của bạn.</p>

<br/>
<p><em>Hệ thống Audit Management System</em></p>
";


            await SendEmailAsync(toEmail, subject, body);

        }


        public async Task SendRejectForAuditor(string toEmail, string auditorFullName, string auditTitle, string leadFullName, string reason)
        {
            string subject = $"[Audit Plan Rejected] Audit Plan bị Lead Auditor yêu cầu chỉnh sửa – {auditTitle}";

            string body = $@"
<p>Xin chào <strong>{auditorFullName}</strong>,</p>

<p>Bản <strong>Audit Plan</strong> bạn gửi cho cuộc kiểm định:</p>

<p style='margin-left:20px'>
    <strong>Tên Audit:</strong> {auditTitle}<br/>
</p>

<p>đã được <strong>Lead Auditor – {leadFullName}</strong> xem xét và <span style='color:red;'><strong>từ chối phê duyệt</strong></span>.</p>

<p>Lý do từ chối:</p>
<p style='margin-left:20px; color:#d9534f;'><em>“{reason}”</em></p>

<p>Vui lòng kiểm tra lại và thực hiện chỉnh sửa:</p>

<ol>
 
    <li>Gửi lại Audit Plan để Lead Auditor xem xét.</li>
</ol>



<br/>
<p>Cảm ơn bạn đã phối hợp.</p>
<p><em>Hệ thống Audit Management System</em></p>
";

            await SendEmailAsync(toEmail, subject, body);

        }

        public async Task SendAuditPlanForwardedToDirectorAsync(string toEmail, string directorFullName, string auditTitle, string forwardedByName, string forwardedByRole, string comment)
        {
            string subject = $"[Audit Review] Audit plan cần Director phê duyệt – {auditTitle}";

            var sanitizedAuditTitle = string.IsNullOrWhiteSpace(auditTitle) ? "Audit Plan" : auditTitle;
            var commentSection = BuildCommentSection(comment, $"Ghi chú từ {forwardedByName}");

            string body = $@"
<p>Xin chào <strong>{directorFullName}</strong>,</p>

<p>Audit plan <strong>{sanitizedAuditTitle}</strong> đã hoàn tất bước phê duyệt của Lead Auditor và vừa được chuyển tiếp tới bạn bởi <strong>{forwardedByName}</strong> ({forwardedByRole}).</p>

{commentSection}

<p>Vui lòng đăng nhập hệ thống để:</p>
<ol>
    <li>Xem lại nội dung Audit Plan, phạm vi và lịch trình.</li>
    <li>Đánh giá các nguồn lực, rủi ro và đề xuất hành động (nếu cần).</li>
    <li>Phê duyệt hoặc phản hồi thêm để nhóm chuẩn bị.</li>
</ol>

<p>Cảm ơn bạn đã dành thời gian hỗ trợ quy trình kiểm định.</p>
<p><em>Hệ thống Audit Management System</em></p>";

            await SendEmailAsync(toEmail, subject, body);
        }

        public async Task SendAuditPlanApprovedForCreatorAsync(string toEmail, string creatorFullName, string auditTitle, string directorFullName, DateTime? fieldworkStart, string comment)
        {
            var sanitizedAuditTitle = string.IsNullOrWhiteSpace(auditTitle) ? "Audit Plan" : auditTitle;
            string subject = $"[Audit Approved] Kế hoạch '{sanitizedAuditTitle}' đã được phê duyệt";
            var commentSection = BuildCommentSection(comment, "Ghi chú từ Director");

            string body = $@"
<p>Xin chào <strong>{creatorFullName}</strong>,</p>

<p>Kế hoạch kiểm định <strong>{sanitizedAuditTitle}</strong> đã được Director <strong>{directorFullName}</strong> phê duyệt.</p>
<p>Ngày fieldwork dự kiến: <strong>{FormatDate(fieldworkStart)}</strong>.</p>

<p>Bạn có thể bắt đầu phối hợp với Lead Auditor và các auditor để chuẩn bị nguồn lực, checklist và tài liệu hỗ trợ.</p>

{commentSection}

<p><em>Hệ thống Audit Management System</em></p>";

            await SendEmailAsync(toEmail, subject, body);
        }

        public async Task SendAuditPlanApprovedForLeadAsync(string toEmail, string leadFullName, string auditTitle, string directorFullName, DateTime? fieldworkStart, string comment)
        {
            var sanitizedAuditTitle = string.IsNullOrWhiteSpace(auditTitle) ? "Audit Plan" : auditTitle;
            string subject = $"[Audit Approved] Audit plan '{sanitizedAuditTitle}' đã được Director duyệt";
            var commentSection = BuildCommentSection(comment, "Ghi chú từ Director");

            string body = $@"
<p>Xin chào <strong>{leadFullName}</strong>,</p>

<p>Audit plan <strong>{sanitizedAuditTitle}</strong> đã được Director <strong>{directorFullName}</strong> phê duyệt.</p>
<p>Ngày fieldwork dự kiến: <strong>{FormatDate(fieldworkStart)}</strong>.</p>

<p>Vui lòng:</p>
<ol>
    <li>Thông báo cho các auditor trong team và phân công nhiệm vụ.</li>
    <li>Xác nhận lại các mốc lịch Fieldwork, Kick-off, Evidence collection.</li>
    <li>Phối hợp với các phòng ban được audit để sắp xếp nhân sự và tài liệu.</li>
</ol>

{commentSection}

<p><em>Hệ thống Audit Management System</em></p>";

            await SendEmailAsync(toEmail, subject, body);
        }

        public async Task SendAuditPlanApprovedForAuditorAsync(string toEmail, string auditorFullName, string auditTitle, string leadFullName, string directorFullName, DateTime? fieldworkStart, string comment)
        {
            var sanitizedAuditTitle = string.IsNullOrWhiteSpace(auditTitle) ? "Audit Plan" : auditTitle;
            string subject = $"[Audit Assignment] Bạn được phân công audit '{sanitizedAuditTitle}'";
            var commentSection = BuildCommentSection(comment, "Ghi chú từ Director");

            string body = $@"
<p>Xin chào <strong>{auditorFullName}</strong>,</p>

<p>Bạn được phân công tham gia cuộc kiểm định <strong>{sanitizedAuditTitle}</strong>.</p>
<ul>
    <li>Director: <strong>{directorFullName}</strong></li>
    <li>Lead Auditor: <strong>{leadFullName}</strong></li>
    <li>Ngày fieldwork dự kiến: <strong>{FormatDate(fieldworkStart)}</strong></li>
</ul>

<p>Vui lòng làm việc với Lead Auditor để rà soát checklist, thu thập tài liệu nền và thống nhất kế hoạch onsite.</p>

{commentSection}

<p><em>Hệ thống Audit Management System</em></p>";

            await SendEmailAsync(toEmail, subject, body);
        }

        public async Task SendAuditPlanApprovedForDepartmentHeadAsync(string toEmail, string deptHeadFullName, string deptName, string auditTitle, DateTime? fieldworkStart, string comment)
        {
            var sanitizedAuditTitle = string.IsNullOrWhiteSpace(auditTitle) ? "Audit Plan" : auditTitle;
            var sanitizedDeptName = string.IsNullOrWhiteSpace(deptName) ? "phòng ban của bạn" : deptName;
            string subject = $"[Audit Notification] Phòng {sanitizedDeptName} tham gia audit '{sanitizedAuditTitle}'";
            var commentSection = BuildCommentSection(comment, "Ghi chú thêm");

            string body = $@"
<p>Xin chào <strong>{deptHeadFullName}</strong>,</p>

<p>Cuộc kiểm định <strong>{sanitizedAuditTitle}</strong> đã được Director phê duyệt và phòng <strong>{sanitizedDeptName}</strong> nằm trong phạm vi audit.</p>
<p>Ngày fieldwork dự kiến: <strong>{FormatDate(fieldworkStart)}</strong>.</p>

<p>Để chuẩn bị tốt, vui lòng:</p>
<ul>
    <li>Chỉ định đầu mối phối hợp với đoàn audit.</li>
    <li>Rà soát quy trình, hồ sơ, bằng chứng liên quan.</li>
    <li>Thông báo cho các bên trong phòng ban về lịch làm việc với audit team.</li>
</ul>

{commentSection}

<p><em>Hệ thống Audit Management System</em></p>";

            await SendEmailAsync(toEmail, subject, body);
        }

        private static string FormatDate(DateTime? date) => date.HasValue ? date.Value.ToString("dd/MM/yyyy") : "Chưa ấn định";

        private static string BuildCommentSection(string comment, string title)
        {
            if (string.IsNullOrWhiteSpace(comment))
                return string.Empty;

            var encoded = WebUtility.HtmlEncode(comment);
            return $@"<p><strong>{title}:</strong></p>
<p style='margin-left:20px; color:#555;'>{encoded}</p>";
        }
    }
}
//< p >
//Bạn có thể truy cập trực tiếp bằng đường link sau:< br />
//< a href = '{reviewUrl}' style = 'color:#0b70ff;' > Mở Audit Plan</a>
//</p>