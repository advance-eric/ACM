using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ACM.Models.ITReports;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ACM.Controllers
{
    public class ITReportsController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ITReportAcknowledge(int id)
        {
            var model = new ITReportItemModel();
            model.LoadModel(id);
            return View(model);
        }

        [HttpPost]
        //public ActionResult ITReportAcknowledge(ITReportItemModel model)
        public ActionResult ITReportAcknowledge(FormCollection formCollection)
        {
            foreach (var key in formCollection.AllKeys)
            {
                var value = formCollection[key];
            }
            return PartialView("CloseWindow");
        }

        public ActionResult ITReportsGrid_Read([DataSourceRequest] DataSourceRequest request)
        {
            var model = new ACM.Models.ITReports.ITReportsModel();
            return Json(model.GetGridData().ToDataSourceResult(request));
        }

        public ActionResult DownloadReport(int id)
        {
            var db = ACM.Helpers.DBHelper.GetDBContext();

            var file = db.ReportFiles.FirstOrDefault(m => m.ReportFileID == id);
            var contentType = file.ContentType;
            var model = new ITReportsModel();
            var buffer = model.GetFileBuffer(id);
            var shortName = System.IO.Path.GetFileName(file.FullFileName);
            db.Dispose();
            return File(buffer, contentType, shortName);
        }
    }
}