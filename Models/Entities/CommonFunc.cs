using AppleStore.Data;
using Microsoft.EntityFrameworkCore;
using System.Net.Mail;
using System.Net;
using AppleStore.Models.Entities.Email;

namespace AppleStore.Models.Entities
{
    public class CommonFunc
    {
        public static async Task<bool> IsEmailExistsAsync(ApplicationDbContext _context, string email)
        {
            return await _context.ApplicationUsers.AnyAsync(user => user.Email == email);
        }
        public static bool SendEmail(string to, string subject, string body, bool isHtml = true, string attachFile = "")
        {
            try
            {
                MailMessage msg = new MailMessage(ContactSender.emailSender, to, subject, body);
                msg.IsBodyHtml = isHtml;

                using (var client = new SmtpClient(ContactSender.hostEmail, ContactSender.portEmail))
                {
                    client.EnableSsl = true;
                    if (!string.IsNullOrEmpty(attachFile))
                    {
                        Attachment attachment = new Attachment(attachFile);
                        msg.Attachments.Add(attachment);
                    }
                    NetworkCredential credential = new NetworkCredential(ContactSender.emailSender, ContactSender.passwordSender);
                    client.UseDefaultCredentials = false;
                    client.Credentials = credential;
                    client.Send(msg);
                }
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }
    }
}
