using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ACM.Models.ITReports
{
    public class ITReportsModel
    {
        [Display(Name = "Filter by Report Type")]
        public int? ReportCategoryID { get; set; }
        [Display(Name = "Filter by Report Status")]
        public int? ReportViewID { get; set; }

        public string GenerateFileLink(int reportId)
        {
            var db = ACM.Helpers.DBHelper.GetDBContext();
            var result = db.Reports.FirstOrDefault(m => m.ReportID == reportId).ReportFileID;

            if (!result.HasValue)
                return "";

            var file = db.ReportFiles.FirstOrDefault(m => m.ReportFileID == result.Value);

            string faClass = "fa fa-file-text";
            switch (file.FullFileName.GetLast(3))
            {
                case "pdf":
                    faClass = "fa fa-file-pdf-o";
                    break;
                case "doc":
                case "docx":
                    faClass = "fa fa-file-word-o";
                    break;
                case "xls":
                case "xlsx":
                    faClass = "fa fa-file-excel-o";
                    break;
                case "jpg":
                case "bmp":
                case "png":
                    faClass = "fa fa-file-image-o";
                    break;
                case "zip":
                    faClass = "fa fa-file-archive-o";
                    break;
            }

            var retVal = "<a href=\"/OFI/GetFile/" + file.ReportFileID.ToString() + "\"><i class=\"" + faClass + "\"></i>&nbsp;&nbsp;&nbsp;" + file.FileDescription + "</a>";
                        
            return new MvcHtmlString(retVal).ToHtmlString();
        }
        
        public byte[] GetFileBuffer(int id)
        {
            var reportFolder = ACM.Helpers.ConfigHelper.ReportFolder();

            if (!reportFolder.EndsWith("\\"))
                reportFolder += "\\";

            using (var db = ACM.Helpers.DBHelper.GetDBContext())
            {
                var file = db.ReportFiles.FirstOrDefault(m => m.ReportFileID == id);
                var fullFileName = reportFolder + file.CreatedDateTime.Year.ToString() + "\\" + file.CreatedDateTime.Month.ToString() + "\\" + file.FullFileName;
                var fs = new System.IO.FileStream(fullFileName, System.IO.FileMode.Open, System.IO.FileAccess.Read);

                var buffer = new byte[fs.Length + 1];
                fs.Read(buffer, 0, (int)fs.Length);

                return buffer;
            }
        }
        
        public List<ITReportGridItem> GetGridData()
        {

            
            using (var db = ACM.Helpers.DBHelper.GetDBContext())
            {
                var result =
                    (from Reports in db.Reports
                     join ReportTypes in db.ReportCategories on Reports.ReportCategoryID equals ReportTypes.ReportCategoryID into join1
                     from ReportTypes in join1.DefaultIfEmpty()
                     join Users in db.AspNetUsers on Reports.ReviewUserID equals Users.Id into join2
                     from Users in join2.DefaultIfEmpty()
                     select new ITReportGridItem
                     {
                         ReportID = Reports.ReportID,
                         ReportType = ReportTypes.CategoryName,
                         ReportDate = Reports.ReportDate,
                         ReviewDate = Reports.ReviewDateTime,
                         ReviewedBy = Users.FirstName + " "  + Users.LastName,
                         Comments = Reports.Comments
                     }).ToList();

                return result;
            }
        }
    }

    public class ITReportItemModel
    {
        public string Action { get; set; }
        public ACM.Report reportItem { get; set; }
        public bool RaiseNC { get; set; }
        public int? ReportID { get; set; }
        
        public void LoadModel(int id)
        {
            this.ReportID = id;
            var db = ACM.Helpers.DBHelper.GetDBContext();
            this.reportItem = db.Reports.FirstOrDefault(m => m.ReportID == id);
            db.Dispose();
        }

    }

    public class ITReportGridItem 
    {
        public int ReportID { get; set; }
        public string ReportType { get; set; }
        public DateTime ReportDate { get; set; }
        public DateTime? ReviewDate { get; set; }
        public string ReviewedBy { get; set; }
        public string Comments { get; set; }

    }
}