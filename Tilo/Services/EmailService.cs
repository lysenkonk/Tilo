using MailKit.Net.Smtp;
using MimeKit;
using System;
using System.Threading.Tasks;
using Tilo.Models.ViewModels;

namespace Tilo.Services
{
    public class EmailService
    {
        public async Task SendEmailAsync(string email, string subject, MailViewModel model)
        {
            try
            {
                var emailMessage = new MimeMessage();
                emailMessage.From.Add(new MailboxAddress("Tiloshowroom", "admin@tiloshowroom.com"));
                emailMessage.To.Add(new MailboxAddress(email));
                emailMessage.Subject = subject;

                var builder = new BodyBuilder();
                foreach(var pathForImage in model.HeaderImage)
                {
                    var image = builder.LinkedResources.Add(pathForImage.ContentPath);
                    image.ContentId = pathForImage.ContentId;
                }
                //var header = builder.LinkedResources.Add(ContentPath);
                //header.ContentId = model.HeaderImage.ContentId;

                builder.HtmlBody = model.Content;
                emailMessage.Body = builder.ToMessageBody();
                //emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html)
                //{
                //    Text = message
                //};

                var emailMessageForAdmin = new MimeMessage();

                emailMessageForAdmin.From.Add(new MailboxAddress("Tiloshowroom", "admin@tiloshowroom.com"));
                emailMessageForAdmin.To.Add(new MailboxAddress("lysenkonk@gmail.com"));
                emailMessageForAdmin.Subject = "New order";
                emailMessageForAdmin.Body = builder.ToMessageBody();

                //var emailMessageForAdmin2 = new MimeMessage();

                //emailMessageForAdmin2.From.Add(new MailboxAddress("Tiloshowroom", "admin@tiloshowroom.com"));
                //emailMessageForAdmin2.To.Add(new MailboxAddress("tilolingerie@gmail.com"));
                //emailMessageForAdmin2.Subject = "New order";
                //emailMessageForAdmin2.Body = new TextPart(MimeKit.Text.TextFormat.Html)
                //{
                //    Text = message
                //};

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