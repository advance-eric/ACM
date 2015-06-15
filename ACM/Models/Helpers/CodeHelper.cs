using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACM.Helpers
{
    public static class CodeHelper
    {
        public static string GetReportCategory(int id)
        {
            using (var db = DBHelper.GetDBContext())
            {
                var result = db.ReportCategories.FirstOrDefault(m => m.ReportCategoryID == id).CategoryName;
                return result;
            }

        }

    }
}