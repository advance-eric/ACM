using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;


namespace ACM.Helpers
{
    public static class EmailHelper
    {
        public static void SendNewEntryEmail(int ofiId, int ofiEntryId)
        {
            var db = ACM.Helpers.DBHelper.GetDBContext();

            var ofi = db.OFIs.FirstOrDefault(m => m.OFIID == ofiId);
            var ofiEntry =
                (from Entries in db.OFIEntries
                 join Users in db.AspNetUsers on Entries.CreateUserID equals Users.Id into join1
                 from Users in join1.DefaultIfEmpty()
                 where Entries.OFIEntryID == ofiEntryId
                 select new { Entries, Users }).FirstOrDefault();

            var ofiAssignedEmails =
                (from Users in db.AspNetUsers
                 join Assigned in db.OFIAssigneds on Users.Id equals Assigned.UserID into join1
                 from Assigned in join1.DefaultIfEmpty()
                 where Assigned.OFIID == ofiId
                 select Users.Email).ToList();

            var actionTextBuilder = new StringBuilder();

            actionTextBuilder.AppendFormat("OFI #{0} has been updated.", ofiId.ToString());
            actionTextBuilder.AppendLine("A new entry has been added.");
            actionTextBuilder.AppendLine("<br/>");
            actionTextBuilder.AppendLine("<br/>");
            actionTextBuilder.AppendLine("Subject: " + ofi.Subject + "<br/>");
            actionTextBuilder.AppendLine("Created By: " + ofiEntry.Users.FirstName + " " + ofiEntry.Users.LastName + "<br/>");
            actionTextBuilder.AppendLine("Entry:" + "<br/>");
            actionTextBuilder.AppendLine(ofiEntry.Entries.EntryText);
            actionTextBuilder.AppendLine("<br/>");
            actionTextBuilder.AppendLine("<br/>");
            SendEmail(ofiAssignedEmails, "OFI Register Notification", actionTextBuilder.ToString());
        }

        private static string GetSignature(string name, string title, string company, string homePhone, string mobile = null)
        {
            string appDataDir = ConfigHelper.SignatureFolder();
            string signature = string.Empty;
            var fs = File.ReadAllText(appDataDir + "\\AGH.htm");
            var text = fs.Replace("[*DisplayName*]", name).Replace("[*company*]", company);
            if (!string.IsNullOrEmpty(mobile))
                text = text.Replace("[*mobile*]", "<b>m:</b> " + mobile);
            else
                text = text.Replace("[*mobile*]", string.Empty);
            if (!string.IsNullOrEmpty(homePhone))
                text = text.Replace("[*phone*]", homePhone);
            else
                text = text.Replace("[*phone*]", "(07) 3868 4558");
            if (!string.IsNullOrEmpty(title))
                text = text.Replace("[*title*]", string.Format("| {0}", title));
            else
                text = text.Replace("[*title*]", string.Empty);
            signature = text;

            return signature;
        }

        public static void AddEmailLog(List<string> emailAddressesTo, string subject, string body, string errorMessage)
        {
            using (var db = ACM.Helpers.DBHelper.GetDBContext())
            {
                var newItem = db.AuditEntries.Create();
                newItem.AuditEntryType = "EMAIL";
                newItem.Description = string.Format("Email sent to: {0} Subject: {1} Body: {2} ErrorMessage: {3}", emailAddressesTo.Select(i => i).Aggregate((i, j) => i + ',' + j), subject, body, errorMessage);
                newItem.EntryDateTime = DateTime.Now;

                db.AuditEntries.Add(newItem);

                db.SaveChanges();

            }
        }

        public static void SendEmail(ACM.Email email)
        {
            using (var db = DBHelper.GetDBContext())
            {
                var emailTo = db.EmailToes.Where(m => m.EmailID == email.EmailID).Select(m => m.EmailAddress).ToList();
                EmailHelper.SendEmail(emailTo, email.Subject, email.EmailBody);
            }

        }

        public static string GetDateTimeString(DateTime? inputDate)
        {
            if (inputDate.HasValue)
                return inputDate.Value.ToShortDateString();

            return "";
        }

        public static void SendNewOFIEmail(int ofiId, int ofiEntryId)
        {
            var db = ACM.Helpers.DBHelper.GetDBContext();

            var ofi = db.OFIs.FirstOrDefault(m => m.OFIID == ofiId);
            var ofiEntry =
                (from Entries in db.OFIEntries
                 join Users in db.AspNetUsers on Entries.CreateUserID equals Users.Id into join1
                 from Users in join1.DefaultIfEmpty()
                 where Entries.OFIEntryID == ofiEntryId
                 select new { Entries, Users }).FirstOrDefault();
            var actionTextBuilder = new StringBuilder();

            var ofiAssignedEmails =
                (from Users in db.AspNetUsers
                 join Assigned in db.OFIAssigneds on Users.Id equals Assigned.UserID into join1
                 from Assigned in join1.DefaultIfEmpty()
                 where Assigned.OFIID == ofiId
                 select Users.Email).ToList();

            actionTextBuilder.AppendFormat("OFI #{0} has been created and has been assigned to you", ofiId.ToString());
            actionTextBuilder.AppendLine("<br/>");
            actionTextBuilder.AppendLine("<br/>");
            actionTextBuilder.AppendLine("Subject: " + ofi.Subject + "<br/>");
            actionTextBuilder.AppendLine("Created By: " + ofiEntry.Users.FirstName + " " + ofiEntry.Users.LastName + "<br/>");
            actionTextBuilder.AppendLine("Agreed Target Date: " + GetDateTimeString(ofi.TargetDate) + "<br/>");
            actionTextBuilder.AppendLine("Entry:" + "<br/>");
            actionTextBuilder.AppendLine(ofiEntry.Entries.EntryText);
            actionTextBuilder.AppendLine("<br/>");
            actionTextBuilder.AppendLine("<br/>");

            SendEmail(ofiAssignedEmails, "OFI Register Notification", actionTextBuilder.ToString());


        }

        public static void SendNewAssignedOFIEmail(int ofiId, List<string> userID)
        {
            var db = ACM.Helpers.DBHelper.GetDBContext();

            var ofi = db.OFIs.FirstOrDefault(m => m.OFIID == ofiId);
            var userList = db.AspNetUsers.Where(item => userID.Contains(item.Id)).Select(m => m.Email).ToList();

            var actionTextBuilder = new StringBuilder();

            actionTextBuilder.AppendFormat("OFI #{0} has been assigned to you", ofiId.ToString());
            actionTextBuilder.AppendLine("<br/>");
            actionTextBuilder.AppendLine("Subject: " + ofi.Subject + "<br/>");
            actionTextBuilder.AppendLine("Agreed Target Date: " + GetDateTimeString(ofi.TargetDate) + "<br/>");
            actionTextBuilder.AppendLine("<br/>");

            SendEmail(userList, "OFI Register Notification", actionTextBuilder.ToString());


        }

        public static void SendEmail(List<string> emailAddressesTo, string subject, string body)
        {
            string fromAddress = "info@advancenational.com.au";
            string path = ConfigHelper.SignatureFolder() + "\\logo.png";

            var emailBody = body.Replace(System.Environment.NewLine, "<br />");
            emailBody = string.Format("<p style=\"font-size:10.0pt; font-family:Arial;\">{0}", emailBody);

            var name = string.Empty;
            var title = string.Empty;
            var phone = "(07) 3868 4558";
            var company = "Advance Group Holdings Pty Ltd";

            emailBody += GetSignature(name, title, company, phone);

            AlternateView avHtml = AlternateView.CreateAlternateViewFromString(emailBody, null, MediaTypeNames.Text.Html);

            LinkedResource pic1 = new LinkedResource(path, new ContentType("image/png"));
            pic1.ContentId = "companylogo";
            avHtml.LinkedResources.Add(pic1);

            MailMessage m = new MailMessage();
            m.AlternateViews.Add(avHtml);

            m.From = new MailAddress(fromAddress);

            if (ConfigHelper.OverrideOutgoingEmails())
                m.To.Add(new MailAddress(ConfigHelper.OverrideOutgoingEmailsTo()));
            else
                m.To.Add(emailAddressesTo.Select(i => i).Aggregate((i, j) => i + ',' + j));

            m.Subject = subject;

            SmtpClient client = new SmtpClient(ConfigHelper.SMTPHost());

            if (ConfigHelper.UseSMTPAuthentication())
                client.Credentials = new System.Net.NetworkCredential(ConfigHelper.SMTPAuthenticationUsername(), ConfigHelper.SMTPAuthenticationPassword());

            client.Port = ConfigHelper.SMTPPort();

            try
            {
                client.Send(m);
                AddEmailLog(emailAddressesTo, subject, body, "");
            }
            catch (Exception ex)
            {
                AddEmailLog(emailAddressesTo, subject, body, ex.Message);
            }
        }
    }
}
