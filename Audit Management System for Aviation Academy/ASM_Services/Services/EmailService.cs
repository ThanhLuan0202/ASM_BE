using ASM_Services.Interfaces;
using ASM_Services.Models.Email;
using MailKit.Net.Smtp;
using MimeKit;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ASM_Services.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _settings;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IOptions<EmailSettings> options, ILogger<EmailService> logger)
        {
            _settings = options?.Value ?? throw new ArgumentNullException(nameof(options));
            _logger = logger;
        }

        public async Task SendEmailAsync(EmailRequest request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));
            if (request.To == null || !request.To.Any())
                throw new ArgumentException("At least one recipient is required.", nameof(request.To));

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(
                string.IsNullOrWhiteSpace(_settings.FromDisplayName) ? _settings.FromEmail : _settings.FromDisplayName,
                _settings.FromEmail ?? _settings.UserName));

            foreach (var to in request.To.Where(x => !string.IsNullOrWhiteSpace(x)).Distinct(StringComparer.OrdinalIgnoreCase))
                message.To.Add(MailboxAddress.Parse(to.Trim()));

            if (request.Cc != null)
                foreach (var cc in request.Cc.Where(x => !string.IsNullOrWhiteSpace(x)))
                    message.Cc.Add(MailboxAddress.Parse(cc.Trim()));

            if (request.Bcc != null)
                foreach (var bcc in request.Bcc.Where(x => !string.IsNullOrWhiteSpace(x)))
                    message.Bcc.Add(MailboxAddress.Parse(bcc.Trim()));

            message.Subject = request.Subject ?? string.Empty;
            message.Body = new TextPart(string.IsNullOrWhiteSpace(request.HtmlBody) ? "plain" : "html")
            {
                Text = string.IsNullOrWhiteSpace(request.HtmlBody) ? request.PlainTextBody : request.HtmlBody
            };

            try
            {
                using var smtp = new SmtpClient();

                // Set timeout (milliseconds)
                try
                {
                    smtp.Timeout = _settings.TimeoutSeconds * 1000;
                }
                catch
                {
                    // Ignore if Timeout cannot be set for some reason
                }

                // Choose socket option based on port / settings
                var socketOption = MailKit.Security.SecureSocketOptions.Auto;
                if (_settings.Port == 465)
                {
                    socketOption = MailKit.Security.SecureSocketOptions.SslOnConnect;
                }
                else if (_settings.EnableSsl)
                {
                    socketOption = MailKit.Security.SecureSocketOptions.StartTls;
                }

                _logger.LogDebug("Connecting SMTP {Host}:{Port} using {Option}", _settings.Host, _settings.Port, socketOption);

                // Try connect with retries and fallback options
                var connected = false;
                var lastException = (Exception?)null;

                async Task<bool> TryConnectAsync(int port, MailKit.Security.SecureSocketOptions option)
                {
                    try
                    {
                        _logger.LogDebug("Attempting SMTP connect {Host}:{Port} option={Option}", _settings.Host, port, option);
                        await smtp.ConnectAsync(_settings.Host, port, option);
                        connected = true;
                        _logger.LogDebug("SMTP connected {Host}:{Port}", _settings.Host, port);
                        return true;
                    }
                    catch (Exception ex)
                    {
                        lastException = ex;
                        _logger.LogWarning(ex, "SMTP connect failed for {Host}:{Port} using {Option}", _settings.Host, port, option);
                        return false;
                    }
                }

                // Primary attempt
                if (!await TryConnectAsync(_settings.Port, socketOption))
                {
                    // Build fallback list
                    var fallbacks = new List<(int Port, MailKit.Security.SecureSocketOptions Option)>();
                    if (_settings.Port == 465)
                    {
                        fallbacks.Add((587, MailKit.Security.SecureSocketOptions.StartTls));
                    }
                    else if (_settings.Port == 587)
                    {
                        fallbacks.Add((465, MailKit.Security.SecureSocketOptions.SslOnConnect));
                    }
                    // Always try Auto as last resort
                    fallbacks.Add((_settings.Port, MailKit.Security.SecureSocketOptions.Auto));

                    foreach (var fb in fallbacks)
                    {
                        // small delay between attempts
                        await Task.Delay(500);
                        if (await TryConnectAsync(fb.Port, fb.Option))
                        {
                            break;
                        }
                    }
                }

                if (!connected)
                {
                    // If still not connected, throw last exception to be handled by caller
                    throw lastException ?? new Exception("Unable to connect to SMTP server");
                }

                if (!string.IsNullOrWhiteSpace(_settings.UserName))
                    await smtp.AuthenticateAsync(_settings.UserName, _settings.Password);

                await smtp.SendAsync(message);
                await smtp.DisconnectAsync(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email with subject {Subject}", request.Subject);
                throw;
            }
        }
    }
}
