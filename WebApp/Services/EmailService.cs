//using System.Net;
//using System.Net.Mail;
//using SendGrid;
//using SendGrid.Helpers.Mail;
using MailKit.Net.Smtp;
using MimeKit;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using WebApp.Configuration;

namespace WebApp.Services
{
    public class EmailService : IEmailSender
    {
        //private readonly SendGridSettings _settings;
        private readonly SmtpClientSettings _settings;

        //public SendGridEmailService(IOptions<SendGridSettings> settings)
        public EmailService(IOptions<SmtpClientSettings> settings)
        {
            _settings = settings.Value;
        }

        //metodo para teste de envio de email local usando apisendgrid
        //public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        //{
        //    try
        //    {
        //        MimeMessage message = new MimeMessage();
        //        message.From.Add(new MailboxAddress("DnaBrasil", "nao_responder@idecace.org.br"));
        //        message.To.Add(MailboxAddress.Parse(email));
        //        message.Subject = subject;
        //        message.Body = new TextPart("html") { Text = htmlMessage };
        //        var mail = "nao_responder@idecace.org.br";
        //        var senha = "Idecace@2023";

        //        var client = new SmtpClient();

        //        try
        //        {
        //            client.Connect("email-ssl.com.br", 465, true);
        //            client.Authenticate(mail, senha);
        //            client.Send(message);
        //        }
        //        catch (Exception e)
        //        {
        //            Console.WriteLine(e);
        //            throw;
        //        }
        //        finally
        //        {
        //            client.Disconnect(true);
        //            client.Dispose();
        //        }

        //        //var apiKey = "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";
        //        //var client = new SendGridClient(apiKey);
        //        //var from = new EmailAddress("test@example.com", "Example User");
        //        //var subject = "Sending with SendGrid is Fun";
        //        //var to = new EmailAddress("test@example.com", "Example User");
        //        //var plainTextContent = "and easy to do anywhere with C#.";
        //        //var htmlContent = "<strong>and easy to do anywhere with C#.</strong>";
        //        //var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
        //        //var response = await client.SendEmailAsync(msg);


        //        //var apiKey = _settings.ApiKey;

        //        //var myMessage = new SendGridMessage();

        //        //myMessage.AddTo(email);
        //        //myMessage.From = new EmailAddress(_settings.FromEmail, _settings.FromName);
        //        //myMessage.Subject = subject;
        //        //myMessage.HtmlContent = htmlMessage;

        //        //var transportWeb = new SendGridClient(apiKey);
        //        //var response = await transportWeb.SendEmailAsync(myMessage);
        //    }
        //    catch (Exception e)
        //    {
        //        Console.WriteLine(e);
        //        throw;
        //    }
        //}



        //metodo para teste de envio de email local usando MimeMessage
        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            try
            {
                MimeMessage message = new MimeMessage();
                message.From.Add(new MailboxAddress(_settings.FromName, _settings.FromEmail));
                message.To.Add(MailboxAddress.Parse(email));
                message.Subject = subject;
                message.Body = new TextPart("html") { Text = htmlMessage };
                var mail = _settings.Usr;
                var senha = _settings.Pwd;

                var client = new SmtpClient();

                try
                {
                    client.Connect(_settings.Host, Convert.ToInt32(_settings.Port), true);
                    client.Authenticate(mail, senha);
                    var result = client.Send(message);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
                finally
                {
                    client.Disconnect(true);
                    client.Dispose();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        //metodo para teste de envio de email ambiente dna
        //public async Task SendEmailAsync(string toAddress, string subject, string message)
        //{
        //    using (var client = new System.Net.Mail.SmtpClient(_settings.Host, Convert.ToInt32(_settings.Port)))
        //    using (var mailMessage = new MailMessage(_settings.FromEmail, toAddress, subject, message))
        //    {
        //        client.Credentials = new NetworkCredential(_settings.FromEmail, _settings.Pwd);
        //        client.EnableSsl = true;

        //        mailMessage.IsBodyHtml = true;

        //        try
        //        {
        //            await client.SendMailAsync(mailMessage);
        //        }
        //        catch (SmtpException e)
        //        {
        //            Console.WriteLine(e);
        //            throw;
        //        }
        //        finally
        //        {
        //            client.Dispose();
        //        }
        //    }
        //}
    }
}
