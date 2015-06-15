using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ACM.Helpers
{
    public static class ListHelper
    {
        public static List<SelectListItem> GetITReportStatus()
        {
            var retVal = new List<SelectListItem>();
            retVal.Add(new SelectListItem() { Text = "Outstanding", Value = "0", Selected = true });
            retVal.Add(new SelectListItem() { Text = "Archived", Value = "1", Selected = true });
            retVal.Add(new SelectListItem() { Text = "All", Value = "2", Selected = true });
            return retVal;
        }

        public static List<SelectListItem> GetITReportsList()
        {
            using (var db = ACM.Helpers.DBHelper.GetDBContext())
            {
                var results =
                    (from Reports in db.ReportCategories
                     select new SelectListItem
                     {
                         Text = Reports.CategoryName,
                         Value = Reports.ReportCategoryID.ToString()
                     }).ToList();

                results.Insert(0, new SelectListItem() { Value = "-1", Text = "Show All", Selected = true });
                

                return results;
            }
        }

        public static List<SelectListItem> GetUsers()
        {
            var db = ACM.Helpers.DBHelper.GetDBContext();

            var result =
                (from TableName in db.AspNetUsers
                 orderby TableName.LastName
                 select new SelectListItem
                 {
                     Text = TableName.FirstName + " " + TableName.LastName,
                     Value = TableName.Id
                 });

            return result.ToList();
        }

        public static List<SelectListItem> GetOFICategories()
        {
            var db = ACM.Helpers.DBHelper.GetDBContext();

            var result =
                (from TableName in db.OFICategories
                 where !TableName.Deleted
                 orderby TableName.OFICategoryName
                 select new SelectListItem
                 {
                     Text = TableName.OFICategoryName,
                     Value = TableName.OFICategoryID.ToString()
                 });

            return result.ToList();
        }

        public static List<SelectListItem> GetOFIOrigins()
        {
            var db = ACM.Helpers.DBHelper.GetDBContext();

            var result =
                (from TableName in db.OFIOrigins
                 where !TableName.Deleted
                 orderby TableName.OFIOriginName
                 select new SelectListItem
                 {
                     Text = TableName.OFIOriginName,
                     Value = TableName.OFIOriginID.ToString()
                 });

            return result.ToList();
        }

        public static List<SelectListItem> GetOFITypes()
        {
            var db = ACM.Helpers.DBHelper.GetDBContext();

            var result =
                (from TableName in db.OFITypes
                 where !TableName.Deleted
                 orderby TableName.OFITypeName
                 select new SelectListItem
                 {
                     Text = TableName.OFITypeName,
                     Value = TableName.OFITypeID.ToString()
                 });

            return result.ToList();
        }

        public static List<SelectListItem> GetOFIPriorities()
        {
            var db = ACM.Helpers.DBHelper.GetDBContext();

            var result =
                (from TableName in db.Priorities
                 where !TableName.Deleted
                 orderby TableName.PriorityName
                 select new SelectListItem
                 {
                     Text = TableName.PriorityName,
                     Value = TableName.PriorityID.ToString()
                 }).ToList();

            for (int i = 0; i <= result.Count - 1; i++)
            {
                if (result[i].Text == "Standard")
                    result[i].Selected = true;
            }

            return result;
        }

        public static List<SelectListItem> GetDepartments()
        {
            var db = ACM.Helpers.DBHelper.GetDBContext();

            var result =
                (from TableName in db.Departments
                 where !TableName.Deleted
                 orderby TableName.DepartmentName
                 select new SelectListItem
                 {
                     Text = TableName.DepartmentName,
                     Value = TableName.DepartmentID.ToString()
                 });

            return result.ToList();
        }

        public static List<SelectListItem> GetStatuses()
        {
            var db = ACM.Helpers.DBHelper.GetDBContext();

            var result =
                (from TableName in db.Statuses
                 where !TableName.Deleted
                 orderby TableName.StatusName
                 select new SelectListItem
                 {
                     Text = TableName.StatusName,
                     Value = TableName.StatusID.ToString()
                 });

            return result.ToList();
        }
    }
}