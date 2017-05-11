using SportsStore.Domain.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SportsStore.Domain.Entities;
using System.Net.Mail;
using System.Net;

namespace SportsStore.Domain.Concrete
{

     //WebMail.SmtpServer = "smtp.qq.com";
     //       WebMail.SmtpPort = 25;
     //       WebMail.EnableSsl = true;
     //       WebMail.UserName = "553545725";//账号名
     //       WebMail.From = "553545725@qq.com";
     //       WebMail.Password = "fyfexeedmvrabaid";
     //       WebMail.SmtpUseDefaultCredentials = true;
     //       WebMail.Send("493955352@qq.com","rsvp notification",Model.Name+" is "+((Model.WillAttend??false)?"":"not")+" attending");
    //邮件设置
    public class EmailSettings
    {
        public string MailToAddress = "493955352@qq.com";// "order@example.com";
        public string MailFromAddress = "553545725@qq.com";//"Sportsstore@example.com";
        public bool UseSsl = true;
        public string Username = "553545725";// "MySmtpUsername";
        public string Password = "fyfexeedmvrabaid";//"MySmtpPassword";
        public string ServerName = "smtp.qq.com";//"smtp.example.com";
        public int ServerPort = 25;// 587;
        public bool WriteAsFile = true;//没有服务器可以用true
        public string FileLocation = @"c:\sports_store_emails";
    }

    public class EmailOrderProcessor : IOderProcessor
    {
        private EmailSettings emailSettings;

        public EmailOrderProcessor(EmailSettings settings)
        {
            emailSettings = settings;
        }

        public void ProcessOrder(Cart cart, ShippingDetails shippingDetails)
        {
            using (var smtpClient = new SmtpClient())
            {
                smtpClient.EnableSsl = emailSettings.UseSsl;
                smtpClient.Host = emailSettings.ServerName;
                smtpClient.Port = emailSettings.ServerPort;
                smtpClient.UseDefaultCredentials = false;
                smtpClient.Credentials = new NetworkCredential(emailSettings.Username, emailSettings.Password);

                if (emailSettings.WriteAsFile)
                {
                    smtpClient.DeliveryMethod = SmtpDeliveryMethod.SpecifiedPickupDirectory;
                    smtpClient.PickupDirectoryLocation = emailSettings.FileLocation;
                    smtpClient.EnableSsl = false;
                }

                StringBuilder body = new StringBuilder();
                body.AppendLine("A new order has been submitted")
                    .AppendLine("----")
                    .AppendLine("Items:");

                foreach (var line in cart.Lines)
                {
                    var subtotal = line.Quentity * line.Product.Price;
                    body.AppendFormat("{0} X {1}(subtotal:{2:c})", line.Quentity, line.Product.Name, subtotal);
                }

                body.AppendFormat("Total order value:{0:c}", cart.ComputeTotalValue())
                    .AppendLine("---")
                    .AppendLine("Ship to:")
                    .AppendLine(shippingDetails.Name)
                    .AppendLine(shippingDetails.Line1)
                    .AppendLine(shippingDetails.Line2 ?? "")
                    .AppendLine(shippingDetails.Line3 ?? "")
                    .AppendLine(shippingDetails.City)
                    .AppendLine(shippingDetails.State ?? "")
                    .AppendLine(shippingDetails.Country)
                    .AppendLine(shippingDetails.Zip)
                    .AppendLine("----")
                    .AppendFormat("Gift wrap:{0}", shippingDetails.GifWrap ? "Yes" : "No");

                MailMessage mailMessage = new MailMessage
                (
                 emailSettings.MailFromAddress,//from
                 emailSettings.MailToAddress,//to
                 "New oreder submited!",//subject
                 body.ToString()//body
                    );
                if (emailSettings.WriteAsFile) {
                    mailMessage.BodyEncoding = Encoding.ASCII;
                }
                smtpClient.Send(mailMessage);///发送 
            }
        }
    }
}
