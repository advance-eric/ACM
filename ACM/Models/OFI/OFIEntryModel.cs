using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;

namespace ACM.Models
{
    public class OFIEntryModel
    {
        public int? OFIEntryID { get; set; }
        public int OFIID { get; set; }
        public DateTime CreateDate { get; set; }
        public string CreateUserID { get; set; }
        public bool Deleted { get; set; }
        public string EntryText { get; set; }

        public string Action { get; set; }

        public void LoadData(ModelStateDictionary modelState)
        {
            var db = ACM.Helpers.DBHelper.GetDBContext();

            if (!this.OFIEntryID.HasValue)
                return;

            var result = db.OFIEntries.FirstOrDefault(m => m.OFIEntryID == this.OFIEntryID.Value);

            if (result == null)
                return;

            this.OFIID = result.OFIID;
            this.CreateDate = result.CreateDate;
            this.CreateUserID = result.CreateUserID;
            this.Deleted = result.Deleted;
            this.EntryText = result.EntryText;
        }

        public void SaveData(ModelStateDictionary modelState)
        {
            var db = ACM.Helpers.DBHelper.GetDBContext();

            OFIEntry result;

            if (OFIEntryID.HasValue)
                result = db.OFIEntries.FirstOrDefault(m => m.OFIEntryID == this.OFIEntryID.Value);
            else
            {
                result = new OFIEntry();
                result.CreateDate = DateTime.Now;
                result.CreateUserID  = ACM.Helpers.UserHelper.GetCurrentUserID();
                result.OFIID = this.OFIID;
            }

            result.EntryText = HttpUtility.HtmlDecode(this.EntryText);
            result.UpdateDate = DateTime.Now;
            result.UpdateUserID = ACM.Helpers.UserHelper.GetCurrentUserID();

            bool addedOFI = false;

            if (result.OFIEntryID == 0)
            {
                addedOFI = true;
                
                db.OFIEntries.Add(result);
            }
            else
            {
                addedOFI = false;
                //EMAIL ALL FOR UPDATE
            }
            db.SaveChanges();

            if (addedOFI)
                ACM.Helpers.EmailHelper.SendNewEntryEmail(this.OFIID, result.OFIEntryID);
        }
    }
}