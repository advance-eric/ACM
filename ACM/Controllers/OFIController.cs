using System.Web.Script.Serialization;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ACM.Models;

namespace ACM.Controllers
{
    public class OFIController : BaseController
    {

        public ActionResult Index()
        {
            return View();
        }
        public ActionResult IndexActive()
        {
            return View();
        }

        public ActionResult _AjaxGetApprovalText(int id)
        {
//            var model = new ACM.Models.OFIModel();
            var model = new ACM.Models.OFIModel();
            return Content(model.PrintApprovalStatusString(id).ToHtmlString());
        }

        [HttpPost()]
        public ActionResult SaveFile(IEnumerable<HttpPostedFileBase> attachments, int? id)
        {
            var js = new JavaScriptSerializer();
            var model = new ACM.Models.OFIModel();
            model.SaveFile(attachments, id.Value);

            return Json("OK!", JsonRequestBehavior.AllowGet);
        }

        [HttpPost()]
        public ActionResult ApproveOFI(int id)
        {
            var model = new ACM.Models.OFIModel();
            model.ApproveOFI(id);
            return Json("", JsonRequestBehavior.AllowGet);
        }

        [HttpPost()]
        public ActionResult DeleteFile(int id)
        {
            var model = new ACM.Models.OFIModel();
            model.DeleteFile(id);
            return Content("");
        }

        /*public ActionResult OFIFileList(ACM.Models.OFIModel model)
        {
            return PartialView(model);
        }*/

        public ActionResult OFIFileList(int id)
        {
            var model = new ACM.Models.OFIModel() { OFIID = id };
            model.LoadData(ModelState);
            return PartialView(model);
        }

        public ActionResult GetFile(int id)
        {
            var model = new ACM.Models.OFIModel();
            var result = model.GetOFIFile(id);
            return File(result.FileBuffer, result.ContentType, result.FileName);
        }

        public ActionResult EntryEditor(int ofiId, int? id)
        {
            var model = new OFIEntryModel() { OFIID = ofiId };
            if (id.HasValue)
            {
                model.OFIEntryID = id.Value;
                model.LoadData(ModelState);
            }
            return View(model);
        }

        [HttpPost(), ValidateAntiForgeryToken()]
        public ActionResult EntryEditor(ACM.Models.OFIEntryModel model)
        {
            if (model.Action == "Cancel")
                return PartialView("CloseWindow");

            model.SaveData(ModelState);

            return PartialView("CloseWindow");
        }


        public ActionResult OFIGrid_Read([DataSourceRequest] DataSourceRequest request)
        {
            var model = new ACM.Models.OFIModel();
            var result = model.GetGridDataMyActive().ToDataSourceResult(request);
            return Json(result);
        }

        public ActionResult OFIGridAll_Read([DataSourceRequest] DataSourceRequest request)
        {
            var model = new ACM.Models.OFIModel();
            var result = model.GetGridDataAllActive().ToDataSourceResult(request);
            return Json(result);
        }

        public ActionResult EditOFI(int? id)
        {
            var model = new ACM.Models.OFIModel() { OFIID = id.Value };
            model.LoadData(ModelState);
            return View(model);
        }

        [HttpPost(), ValidateAntiForgeryToken()]
        public ActionResult EditOFI(ACM.Models.OFIModel model)
        {
            if (model.Action == "CancelChanges")
                return RedirectToAction("Index", "OFI");

            model.UpdateOFI(ModelState);

            return RedirectToAction("Index", "OFI");
        }

        [HttpGet()]
        public ActionResult CreateOFI()
        {
            var model = new ACM.Models.OFIModel();
            return View(model);
        }
        [HttpPost(), ValidateAntiForgeryToken()]
        public ActionResult CreateOFI(ACM.Models.OFIModel model)
        {
            if (model.Action == "Cancel")
                return RedirectToAction("Index");

            model.Validate(ModelState);
            if (!ModelState.IsValid)
                return View(model);

            var newOFINum = model.CreateNewOFI(ModelState);

            if (!ModelState.IsValid)
                return View(model);

            TempData["OFINum"] = newOFINum.Value.ToString();
            return RedirectToAction("OFISuccess");
        }
        [HttpGet()]
        public ActionResult OFISuccess()
        {
            return View();
        }
    }
}