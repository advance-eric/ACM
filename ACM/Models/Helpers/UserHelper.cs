using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using ACM.Models;
using System.Text;

namespace ACM.Helpers
{
    public static class UserHelper
    {
        public static string GetCurrentUserID()
        {
            return HttpContext.Current.User.Identity.GetUserId();
        }

        public static string GetUserFullName(string userId)
        {
            using (var db = ACM.Helpers.DBHelper.GetDBContext())
            {
                var result = db.AspNetUsers.FirstOrDefault(m => m.Id == userId);

                if (result == null)
                    return "";

                return string.Format("{0} {1}", result.FirstName, result.LastName);
            }
        }
        public static bool IsCurrentUserAdministrator()
        {
            var roleID = GetUserRoleID(GetCurrentUserID());

            if (roleID == ConfigHelper.AdministratorRoleID())
                return true;

            return false;
        }

        public static bool IsCurrentUserNationalManager()
        {
            var roleID = GetUserRoleID(GetCurrentUserID());

            if (roleID == ConfigHelper.NationalManagerRoleID())
                return true;

            return false;
        }

        public static string GetUserRoleID(string userID)
        {
            var db = ACM.Helpers.DBHelper.GetDBContext();

            var result = db.AspNetUsers.FirstOrDefault(m => m.Id == userID).AspNetRoles.FirstOrDefault().Id;

            if (result == null)
                return "";

            return result;
        }
    }
}