using ASM_Repositories.Entities;
using ASM_Services.Interfaces;
using ASM_Services.Models.Email;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ASM_Services.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _mailSettings;

        public EmailService(IOptions<EmailSettings> mailSettings)
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

            using var smtp = new MailKit.Net.Smtp.SmtpClient();
            await smtp.ConnectAsync(_mailSettings.SmtpServer, _mailSettings.Port, true);
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
    }
}
//< p >
//Bạn có thể truy cập trực tiếp bằng đường link sau:< br />
//< a href = '{reviewUrl}' style = 'color:#0b70ff;' > Mở Audit Plan</a>
//</p>