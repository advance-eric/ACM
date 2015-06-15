using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACM.Helpers
{
    public static class ConfigHelper
    {

        private static string GetSetting(string settingCode)
        {
            using (var db = ACM.Helpers.DBHelper.GetDBContext())
            {
                try
                {
                    return db.Settings.FirstOrDefault(m => m.SettingCode == settingCode).SettingValue;
                }
                catch
                {
                    return "";
                }
            }
        }
        public static string UploadFolder()
        {
            return GetSetting("UploadFolder");
        }

        public static string LDAPQuery()
        {
            return GetSetting("LDAPQuery");
        }

        public static int ClosedStatus()
        {
            int retVal = 3;

            if (int.TryParse(GetSetting("ClosedStatus"), out retVal))
                return retVal;

            return 3;
        }

        public static int ActiveStatus()
        {
            int retVal = 1;

            if (int.TryParse(GetSetting("ActiveStatus"), out retVal))
                return retVal;

            return 1;
        }

        public static string SignatureFolder()
        {
            return GetSetting("SignatureFolder");
        }

        public static string SMTPHost()
        {
            return GetSetting("SMTPHost");
        }

        public static string ReportFolder()
        {
            return GetSetting("ReportFolder");
        }

        public static bool OverrideOutgoingEmails()
        {
            bool retVal = false;
            if (bool.TryParse(GetSetting("OverrideOutgoingEmails"), out retVal))
                return retVal;

            return false;
        }

        public static string OverrideOutgoingEmailsTo()
        {
            return GetSetting("OverrideOutgoingEmailsTo");
        }

        public static string SMTPAuthenticationUsername()
        {
            return GetSetting("SMTPAuthenticationUsername");
        }

        public static string SMTPAuthenticationPassword()
        {
            return GetSetting("SMTPAuthenticationPassword");
        }

        public static string NationalManagerRoleID()
        {
            return GetSetting("NationalManagerRoleID");
        }

        public static string StaffMemberRoleID()
        {
            return GetSetting("StaffMemberRoleID");
        }

        public static string ManagerRoleID()
        {
            return GetSetting("ManagerRoleID");
        }

        public static string GeneralManagerRoleID()
        {
            return GetSetting("GeneralManagerRoleID");
        }

        public static string ComplianceRoleID()
        {
            return GetSetting("ComplianceRoleID");
        }

        public static string AdministratorRoleID()
        {
            return GetSetting("AdministratorRoleID");
        }

        public static int UnlockMinutes()
        {
            int retVal = 60;

            if (int.TryParse(GetSetting("UnlockMinutes"), out retVal))
                return retVal;

            return 60;
        }

        public static int SMTPPort()
        {
            int retVal = 25;

            if (int.TryParse(GetSetting("SMTPPort"), out retVal))
                return retVal;

            return 25;
        }

        public static bool UseSMTPAuthentication()
        {
            bool retVal = false;
            if (bool.TryParse(GetSetting("UseSMTPAuthentication"), out retVal))
                return retVal;

            return true;
        }
    }
}
