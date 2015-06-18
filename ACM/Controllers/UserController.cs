using System;
using System.Linq;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace ACM.Controllers
{
    public partial class AdminController : Controller
    {

        [HttpGet()]
        public ActionResult UserEdit(string id)
        {
            var model = new ACM.Models.Admin.UserModel();

            if (!string.IsNullOrWhiteSpace(id))
                model.LoadData(id);
            else
                model.ActiveUser = true;

            return View(model);
        }

        [HttpPost(), ValidateAntiForgeryToken()]
        public ActionResult UserEdit(ACM.Models.Admin.UserModel model)
        {
            if (model.Action == "CancelChanges")
                return RedirectToAction("Users");

            if (!ModelState.IsValid)
                return View(model);

            model.Validate(ModelState);

            if (!ModelState.IsValid)
                return View(model);

            if (!string.IsNullOrWhiteSpace(model.Id))
                model.SaveData(ModelState);

            ModelState.AddModelError("", "Successfully saved user.");

            return View(model);
        }

        [HttpGet()]
        public ActionResult UserEditPasswordPartial(string id)
        {
            return PartialView();
        }

        [HttpPost()]
        public ActionResult ResetPassword(string id, string password)
        {
            UserManager<IdentityUser> userManager = new UserManager<IdentityUser>(new UserStore<IdentityUser>());
            userManager.RemovePassword(id);
            userManager.AddPassword(id, password);
            return Content("");
        }

        public ActionResult Users()
        {
            return View();
        }

        public ActionResult UserGrid_Read([DataSourceRequest]DataSourceRequest request)
        {
            var model = new ACM.Models.Admin.UserModel();
            return Json(model.GetGridData().ToDataSourceResult(request));
        }
    }
}