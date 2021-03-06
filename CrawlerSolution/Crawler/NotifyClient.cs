﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace Crawler
{
    class NotifyClient
    {
        
        public static void sendMessage(string email)
        {
            MailMessage message = new MailMessage();
            message = new MailMessage();
            message.To.Add(email);
            message.Subject = "Notification of completed crawl";
            message.From = new MailAddress("donotreply@wddinc.com");
            SmtpClient smtp = new SmtpClient("smtp.gmail.com",587);
            smtp.Credentials = new NetworkCredential(ConfigReader.ReadUserNameString(), ConfigReader.ReadPasswordString());//"fake.andrew.lewis@gmail.com","passwordpass");
            smtp.EnableSsl = true;
            message.Body = "A crawl has completed --Crawler";
            if (smtp.Equals(null))
            {
                Console.Out.WriteLine("Something is wrong with the smtp");
            }else
            {
                smtp.Send(message);
            }
        }


    }
}
