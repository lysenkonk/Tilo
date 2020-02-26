﻿using MailKit.Net.Smtp;
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

                emailMessage.From.Add(new MailboxAddress("Администрация сайта", "lysenkonk@gmail.com"));
                emailMessage.To.Add(new MailboxAddress(email));
                emailMessage.Subject = subject;
                emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html)
                {
                    Text = message
                };


                using (SmtpClient  client = new SmtpClient())
                {
                    await client.ConnectAsync("smtp.gmail.com", 465, true);
                    await client.AuthenticateAsync("kostjja2019@gmail.com", "soniia2014");
                    await client.SendAsync(emailMessage);

                    await client.DisconnectAsync(true);
                }
            }
            catch(Exception ex)
            {
                new Exception(ex.Message.ToString());
            }
        }
    }
}
