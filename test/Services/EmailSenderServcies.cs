using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Client.Platforms.Features.DesktopOs.Kerberos;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Net;
using System.Net.Mail;
using test.Helpers;

namespace test.Services
{
    public class EmailSenderServcies : IEmailSender
    {
        private readonly IConfiguration _configuration;

        public EmailSenderServcies(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var emailSettings = _configuration.GetSection("EmailSettings");
            var host = "smtp.gmail.com";
            var port = 587;
            var fromEmail = "ahmedhossamahmed22@gmail.com";
            var password = "hecj hcmy kzud yptk";

            var client = new SmtpClient(host, port)
            {
                EnableSsl = true,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromEmail, password)
            };

            string styledMessage = $@"
                <!DOCTYPE html>
                <html>
                <head>
                    <style>
                        body {{ font-family: 'Arial', sans-serif; line-height: 1.6; color: #333; }}
                        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; border: 1px solid #eee; border-radius: 10px; background-color: #f9f9f9; }}
                        .header {{ text-align: center; padding-bottom: 20px; border-bottom: 2px solid #db98b7; margin-bottom: 20px; }}
                        .header h1 {{ color: #db98b7; margin: 0; font-size: 24px; }}
                        .content {{ background-color: #fff; padding: 20px; border-radius: 5px; }}
                        .footer {{ text-align: center; margin-top: 20px; font-size: 12px; color: #888; }}
                        .button {{ display: inline-block; padding: 10px 20px; background-color: #db98b7; color: #fff; text-decoration: none; border-radius: 5px; margin-top: 10px; }}
                    </style>
                </head>
                <body>
                    <div class='container'>
                        <div class='header'>
                            <h1>HappyPaws Haven</h1>
                        </div>
                        <div class='content'>
                            {htmlMessage}
                        </div>
                        <div class='footer'>
                            <p>&copy; {DateTime.Now.Year} HappyPaws Haven. All rights reserved.</p>
                            <p>This is an automated message, please do not reply.</p>
                        </div>
                    </div>
                </body>
                </html>";

            var mailMessage = new MailMessage
            {
                From = new MailAddress(fromEmail, "HappyPaws Haven"),
                Subject = subject,
                Body = styledMessage,
                IsBodyHtml = true
            };
            mailMessage.To.Add(email);

            return client.SendMailAsync(mailMessage);
        }
    }
}
