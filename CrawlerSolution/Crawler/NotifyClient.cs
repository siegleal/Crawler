using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace Crawler
{
    class NotifyClient
    {
        
        public static void sendMessage()
        {
            MailMessage message = new MailMessage();
            message = new MailMessage();
            message.To.Add("asiegle@gmail.com");
            message.Subject = "Notification of completed crawl";
            message.From = new MailAddress("donotreply@wddinc.com");
            SmtpClient smtp = new SmtpClient("smtp.gmail.com",587);
            smtp.Credentials = new NetworkCredential("fake.andrew.lewis@gmail.com","passwordpass");
            smtp.EnableSsl = true;
            message.Body = "This is a test";
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
