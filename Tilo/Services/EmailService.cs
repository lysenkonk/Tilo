using MailKit.Net.Smtp;
using MimeKit;
using System;
using System.Threading.Tasks;

namespace Tilo.Services
{
    public class EmailService
    {
        public async Task SendEmailAsync(string email, string subject, string message)
        {
            try
            {
                var emailMessage = new MimeMessage();
                emailMessage.From.Add(new MailboxAddress("Tiloshowroom", "admin@tiloshowroom.com"));
                emailMessage.To.Add(new MailboxAddress(email));
                emailMessage.Subject = subject;
                emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html)
                {
                    Text = message
                };

                var emailMessageForAdmin = new MimeMessage();

                emailMessageForAdmin.From.Add(new MailboxAddress("Tiloshowroom", "admin@tiloshowroom.com"));
                emailMessageForAdmin.To.Add(new MailboxAddress("lysenkonk@gmail.com"));
                emailMessageForAdmin.Subject = "New order";
                emailMessageForAdmin.Body = new TextPart(MimeKit.Text.TextFormat.Html)
                {
                    Text = message
                };

                using (SmtpClient client = new SmtpClient())
                {
                    await client.ConnectAsync("mail.tiloshowroom.com", 25, false);
                    await client.AuthenticateAsync("admin@tiloshowroom.com", "winteR$2021");

                    await client.SendAsync(emailMessage);
                    await client.SendAsync(emailMessageForAdmin);
                    await client.DisconnectAsync(true);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message.ToString());
            }
        }
    }
}