using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ACM.Controllers
{
    public abstract partial class BaseController : Controller
    {

        public ActionResult RedirectToPrevious(String defaultAction, String defaultController)
        {
            if (Session == null || Session["PrevUrl"] == null)
            {
                return RedirectToAction(defaultAction, defaultController);
            }

            String url = ((Uri)Session["PrevUrl"]).PathAndQuery;

            if (Request.Url != null && Request.Url.PathAndQuery != url)
            {
                return Redirect(url);
            }

            return RedirectToAction(defaultAction, defaultController);
        }


        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var httpContext = filterContext.HttpContext;

            if (httpContext.Request.RequestType == "GET"
                && !httpContext.Request.IsAjaxRequest()
                && filterContext.IsChildAction == false)    // do no overwrite if we do child action.
            {
                // stop overwriting previous page if we just reload the current page.
                if (Session["CurUrl"] != null
                    && ((Uri)Session["CurUrl"]).Equals(httpContext.Request.Url))
                    return;

                Session["PrevUrl"] = Session["CurUrl"] ?? httpContext.Request.Url;
                Session["CurUrl"] = httpContext.Request.Url;

                var splitStr = Session["PrevUrl"].ToString().Split('/');

                if (splitStr != null)
                {
                    if (splitStr.Count() >= 6)
                    {
                        if (splitStr[4] == "EditOFI")
                            RemoveOFILock(System.Convert.ToInt32(splitStr[5]));
                    }
                }
            }
        }

        private void RemoveOFILock(int id)
        {
            var db = ACM.Helpers.DBHelper.GetDBContext();

            var userId = ACM.Helpers.UserHelper.GetCurrentUserID();

            var result =
                (from OFIs in db.OFIs
                 where OFIs.LockUserID == userId && OFIs.OFIID == id
                 select OFIs);

            if (result != null)
            {
                foreach (var tempItem in result)
                {
                    tempItem.LockUserID = null;
                    tempItem.LockDateTime = null;
                }
            }

            db.SaveChanges();
        }
    }
}