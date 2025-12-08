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

        public async Task SendAuditPlanRejectedForCreatorAsync(string toEmail, string creatorFullName, string auditTitle, string directorFullName, string comment)
        {
            var sanitizedAuditTitle = string.IsNullOrWhiteSpace(auditTitle) ? "Audit Plan" : auditTitle;
            string subject = $"[Audit Plan Rejected] Kế hoạch '{sanitizedAuditTitle}' bị từ chối bởi Director";
            var commentSection = BuildCommentSection(comment, "Lý do từ chối");

            string body = $@"
<p>Xin chào <strong>{creatorFullName}</strong>,</p>

<p>Kế hoạch kiểm định <strong>{sanitizedAuditTitle}</strong> bạn đã tạo đã được Director <strong>{directorFullName}</strong> xem xét và <span style='color:#d9534f;'><strong>từ chối phê duyệt</strong></span>.</p>

{commentSection}

<p>Vui lòng:</p>
<ol>
    <li>Xem lại các yêu cầu và lý do từ chối từ Director.</li>
    <li>Chỉnh sửa lại Audit Plan theo phản hồi.</li>
    <li>Gửi lại để Lead Auditor và Director xem xét lại.</li>
</ol>

<p>Nếu có thắc mắc, vui lòng liên hệ với Director hoặc Lead Auditor để được hỗ trợ.</p>

<p>Cảm ơn bạn đã phối hợp.</p>
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

        public async Task SendRegistrationEmailAsync(string toEmail, string fullName, string email, string password, string roleName)
        {
            string subject = "[Account Registration] Thông tin tài khoản đăng nhập hệ thống Audit Management";

            string body = $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <style>
        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
        .header {{ background-color: #0b70ff; color: white; padding: 20px; text-align: center; border-radius: 5px 5px 0 0; }}
        .content {{ background-color: #f9f9f9; padding: 30px; border: 1px solid #ddd; }}
        .credentials {{ background-color: white; padding: 20px; margin: 20px 0; border-left: 4px solid #0b70ff; border-radius: 4px; }}
        .credential-item {{ margin: 10px 0; }}
        .label {{ font-weight: bold; color: #555; }}
        .value {{ color: #0b70ff; font-size: 16px; font-family: monospace; }}
        .warning {{ background-color: #fff3cd; border: 1px solid #ffc107; padding: 15px; margin: 20px 0; border-radius: 4px; }}
        .footer {{ text-align: center; margin-top: 20px; color: #777; font-size: 12px; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h2>Chào mừng đến với Hệ thống Audit Management</h2>
        </div>
        <div class='content'>
            <p>Xin chào <strong>{WebUtility.HtmlEncode(fullName)}</strong>,</p>

            <p>Tài khoản của bạn đã được tạo thành công trong hệ thống <strong>Audit Management System for Aviation Academy</strong>.</p>

            <p><strong>Vai trò của bạn:</strong> {WebUtility.HtmlEncode(roleName)}</p>

            <div class='credentials'>
                <h3 style='margin-top: 0; color: #0b70ff;'>Thông tin đăng nhập:</h3>
                <div class='credential-item'>
                    <span class='label'>Email:</span><br/>
                    <span class='value'>{WebUtility.HtmlEncode(email)}</span>
                </div>
                <div class='credential-item'>
                    <span class='label'>Mật khẩu:</span><br/>
                    <span class='value'>{WebUtility.HtmlEncode(password)}</span>
                </div>
            </div>

            <div class='warning'>
                <p><strong>⚠️ Lưu ý quan trọng:</strong></p>
                <ul style='margin: 10px 0; padding-left: 20px;'>
                    <li>Không chia sẻ thông tin đăng nhập với người khác.</li>
                    <li>Nếu bạn không yêu cầu tạo tài khoản này, vui lòng liên hệ quản trị viên ngay lập tức.</li>
                </ul>
            </div>

            <p><strong>Hướng dẫn đăng nhập:</strong></p>
            <ol>
                <li>Truy cập hệ thống Audit Management System</li>
                <li>Sử dụng Email và Mật khẩu được cung cấp ở trên để đăng nhập</li>

            </ol>

            <p>Nếu bạn có bất kỳ câu hỏi nào, vui lòng liên hệ với quản trị viên hệ thống.</p>

            <p>Chúc bạn làm việc hiệu quả!</p>
        </div>
        <div class='footer'>
            <p><em>Hệ thống Audit Management System for Aviation Academy</em></p>
            <p>Email này được gửi tự động, vui lòng không trả lời.</p>
        </div>
    </div>
</body>
</html>";

            await SendEmailAsync(toEmail, subject, body);
        }
    }
}
//< p >
//Bạn có thể truy cập trực tiếp bằng đường link sau:< br />
//< a href = '{reviewUrl}' style = 'color:#0b70ff;' > Mở Audit Plan</a>
//</p>