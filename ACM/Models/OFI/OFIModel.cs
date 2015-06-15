using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Configuration;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using System.Text;

namespace ACM.Models
{
    public class OFIModel : BaseModel
    {
        [Display(Name = "OFI #")]
        public int? OFIID { get; set; }

        [Display(Name = "Date Created")]
        public DateTime CreatedDate { get; set; }

        [Display(Name = "Created By")]
        public string CreatedUserId { get; set; }

        [Display(Name = "Category"), Required()]
        public int? OFICategoryID { get; set; }

        [Display(Name = "Origin"), Required()]
        public int? OFIOriginID { get; set; }

        [Display(Name = "Type"), Required()]
        public int? OFITypeID { get; set; }

        [Display(Name = "Subject"), Required()]
        public string Subject { get; set; }

        [Display(Name = "Target Date"), Required()]
        public DateTime? TargetDate { get; set; }

        [Display(Name = "Date Completed")]
        public DateTime? CompletedDate { get; set; }

        [Display(Name = "Priority"), Required()]
        public int? PriorityID { get; set; }

        [Display(Name = "Department")]
        public int? DepartmentID { get; set; }

        [Display(Name = "Estimated Time (hrs)")]
        public double? TimeEstimation { get; set; }

        [Display(Name = "Status")]
        public int StatusID { get; set; }

        [Display(Name = "OFI Assigned To"), Required()]
        public List<string> OFIAssignedTo { get; set; }

        public string MinutesLeftString
        {
            get
            {
                if (!LockMinutesLeft.HasValue)
                    return "";

                if (LockMinutesLeft.Value == 1)
                    return " - " + LockMinutesLeft.Value.ToString() + " minute remaining.";

                return " - " + LockMinutesLeft.Value.ToString() + " minutes remaining.";
            }
        }

        public bool CurrentLocked { get; set; }

        public string CurrentLockID { get; set; }

        public int? LockMinutesLeft { get; set; }

        public IEnumerable<HttpPostedFileBase> attachments { get; set; }

        public string Action { get; set; }

        public string FirstAction { get; set; }

        public List<ACM.OFIEntry> Entries { get; set; }

        public List<ACM.OFIFile> OFIFiles { get; set; }

        public void DeleteFile(int id)
        {
            var db = ACM.Helpers.DBHelper.GetDBContext();

            var fileResult = db.OFIFiles.FirstOrDefault(m => m.OFIFileID == id);

            var uploadFolder = ACM.Helpers.ConfigHelper.UploadFolder();

            if (!uploadFolder.EndsWith("\\"))
                uploadFolder = uploadFolder + "\\";

            var fullFile = uploadFolder + fileResult.FullFileName;

            if (System.IO.File.Exists(fullFile))
                System.IO.File.Delete(fullFile);

            db.OFIFiles.Remove(fileResult);
            db.SaveChanges();
        }

        public void ApproveOFI(int id)
        {
            var db = ACM.Helpers.DBHelper.GetDBContext();

            var result = db.OFIs.FirstOrDefault(m => m.OFIID == id);

            if (result == null)
                return;

            result.ApprovedUserID = ACM.Helpers.UserHelper.GetCurrentUserID();
            result.ApprovedDateTime = DateTime.Now;
            db.SaveChanges();
        }

        public MvcHtmlString PrintApprovalStatusString(int id)
        {
            var db = ACM.Helpers.DBHelper.GetDBContext();

            var result = db.OFIs.FirstOrDefault(m => m.OFIID == id);

            var sb = new StringBuilder();

            sb.AppendFormat("<p style=\"color:#19CF2E; font-weight:bold;\">{0}", "Approved");
            sb.AppendFormat(" - {0} by {1}</p>", result.ApprovedDateTime.Value.ToShortDateString(), ACM.Helpers.UserHelper.GetUserFullName(result.ApprovedUserID));

            return new MvcHtmlString(sb.ToString());
        }

        public MvcHtmlString PrintApprovalStatusString()
        {
            return PrintApprovalStatusString(this.OFIID.Value);
        }

        public MvcHtmlString PrintNotApprovedStatusString()
        {
            var sb = new StringBuilder();

            sb.Append("<p style=\"color:#FF0000; font-weight:bold;\">This OFI has not yet been approved. Please contact your national manager for approval.</p>");

            return new MvcHtmlString(sb.ToString());
        }

        public bool HasOFIBeenApproved()
        {
            var db = ACM.Helpers.DBHelper.GetDBContext();

            var result = db.OFIs.FirstOrDefault(m => m.OFIID == this.OFIID);

            if (result == null)
                return false;

            if (result.ApprovedDateTime.HasValue)
                return true;

            return false;
        }

        public IEnumerable<OFIList> GetGridDataMyActive()
        {
            var closedOFIStatus = ACM.Helpers.ConfigHelper.ClosedStatus();

            var db = ACM.Helpers.DBHelper.GetDBContext();

            var currentUserID = ACM.Helpers.UserHelper.GetCurrentUserID();

            var result =
            (from i in db.OFIs
             let p = db.OFIAssigneds.Where(p2 => i.OFIID == p2.OFIID).FirstOrDefault()
             where i.StatusID != closedOFIStatus
             orderby i.OFIID ascending
             select new OFIList
             {
                 OFIID = i.OFIID,
                 DateCreated = i.CreatedDate,
                 Subject = i.Subject,
                 TargetDate = i.TargetDate
             }).ToList();

            return result;
        }

        public OFIApprovalText GetOFIApprovalClass(int id)
        {
            var db = ACM.Helpers.DBHelper.GetDBContext();

            var result = db.OFIs.FirstOrDefault(m => m.OFIID == this.OFIID);

            if (result == null)
                return new OFIApprovalText();

            var retVal = new OFIApprovalText();
            retVal.ApprovalDate = result.ApprovedDateTime.Value.ToShortDateString();
            retVal.ApprovalName = ACM.Helpers.UserHelper.GetUserFullName(result.ApprovedUserID);
            return retVal;
        }

        public IEnumerable<OFIList> GetGridDataAllActive()
        {
            var db = ACM.Helpers.DBHelper.GetDBContext();

            var currentUserID = ACM.Helpers.UserHelper.GetCurrentUserID();

            var result =
                (from OFIs in db.OFIs
                 where (OFIs.StatusID != 3)
                 select new OFIList
                 {
                     OFIID = OFIs.OFIID,
                     DateCreated = OFIs.CreatedDate,
                     Subject = OFIs.Subject,
                     TargetDate = OFIs.TargetDate
                 });



            return result;
        }

        public string GenerateFileLink(OFIFile file)
        {
            string faClass = "fa fa-file-text";
            switch (file.FullFileName.GetLast(3).ToLower())
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

            var retVal = "<a href=\"/OFI/GetFile/" + file.OFIFileID.ToString() + "\"><i class=\"" + faClass + "\"></i>&nbsp;&nbsp;&nbsp;" + file.FileDescription + "</a>";
            retVal += "&nbsp;&nbsp;&nbsp;<a href=\"#\" style=\"color:#ED8080\" onClick=\"removeFile(" + file.OFIFileID.ToString() + ")\">Remove</a>";
            retVal += "<br/>";

            return new MvcHtmlString(retVal).ToHtmlString();
        }

        public void LoadData(ModelStateDictionary modelState)
        {
            var db = ACM.Helpers.DBHelper.GetDBContext();

            if (!this.OFIID.HasValue)
                return;

            var result = db.OFIs.FirstOrDefault(m => m.OFIID == this.OFIID.Value);

            this.CompletedDate = result.CompletedDate;
            this.CreatedDate = result.CreatedDate;
            this.CreatedUserId = result.CreatedUserId;
            this.DepartmentID = result.DepartmentID;
            this.OFICategoryID = result.OFICategoryID;
            this.OFIOriginID = result.OFIOriginID;
            this.OFITypeID = result.OFITypeID;
            this.PriorityID = result.PriorityID;
            this.StatusID = result.StatusID;
            this.Subject = result.Subject;
            this.TargetDate = result.TargetDate;
            this.TimeEstimation = result.TimeEstimation;

            this.OFIAssignedTo = db.OFIAssigneds.Where(m => m.OFIID == this.OFIID.Value).Select(select => select.UserID).ToList();
            this.Entries = db.OFIEntries.Where(m => m.OFIID == this.OFIID.Value).OrderBy(order => order.CreateDate).ToList();
            this.OFIFiles = db.OFIFiles.Where(m => m.OFIID == this.OFIID.Value).ToList();

            this.CurrentLocked = false;

            if (result.LockDateTime.HasValue)
            {
                var unlockMinutes = ACM.Helpers.ConfigHelper.UnlockMinutes();
                var unlockTime = result.LockDateTime.Value.AddMinutes(unlockMinutes);

                if (result.LockUserID == ACM.Helpers.UserHelper.GetCurrentUserID())
                {
                    this.CurrentLocked = false;
                    this.CurrentLockID = "";
                    result.LockDateTime = DateTime.Now;
                    result.LockUserID = ACM.Helpers.UserHelper.GetCurrentUserID();
                    db.SaveChanges();
                }
                else
                {
                    if (DateTime.Now >= unlockTime)
                    {
                        this.CurrentLocked = false;
                        this.CurrentLockID = "";
                        result.LockDateTime = DateTime.Now;
                        result.LockUserID = ACM.Helpers.UserHelper.GetCurrentUserID();
                        db.SaveChanges();
                    }
                    else
                    {
                        this.CurrentLocked = true;
                        this.CurrentLockID = result.LockUserID;
                        this.LockMinutesLeft = unlockTime.Subtract(DateTime.Now).Minutes;
                    }
                }
            }
            else
            {
                this.CurrentLocked = false;
                this.CurrentLockID = "";
                result.LockDateTime = DateTime.Now;
                result.LockUserID = ACM.Helpers.UserHelper.GetCurrentUserID();
                db.SaveChanges();
            }
        }

        public OFIFileReturn GetOFIFile(int id)
        {
            var retVal = new OFIFileReturn();
            var db = ACM.Helpers.DBHelper.GetDBContext();
            var result = db.OFIFiles.FirstOrDefault(m => m.OFIFileID == id);

            var uploadFolder = ACM.Helpers.ConfigHelper.UploadFolder();

            if (!uploadFolder.EndsWith("\\"))
                uploadFolder = uploadFolder + "\\";

            if (result != null)
            {
                retVal.FileBuffer = System.IO.File.ReadAllBytes(uploadFolder + result.FullFileName);
                retVal.ContentType = result.ContentType;
                retVal.FileName = result.FileDescription + "." + result.FullFileName.GetLast(3);
            }

            return retVal;
        }

        public void SaveFile(IEnumerable<HttpPostedFileBase> files, int id)
        {
            var uploadFolder = ACM.Helpers.ConfigHelper.UploadFolder();

            if (!uploadFolder.EndsWith("\\"))
                uploadFolder = uploadFolder + "\\";

            foreach (var tempFile in files)
            {
                var db = ACM.Helpers.DBHelper.GetDBContext();
                var newFileItem = db.OFIFiles.Create();
                newFileItem.OFIID = id;
                newFileItem.ContentType = tempFile.ContentType;
                newFileItem.CreatedDateTime = DateTime.Now;
                newFileItem.CreatedUserID = ACM.Helpers.UserHelper.GetCurrentUserID();
                newFileItem.FileDescription = System.IO.Path.GetFileNameWithoutExtension(tempFile.FileName);
                newFileItem.FileLength = tempFile.ContentLength;
                var appendPath = DateTime.Now.Year.ToString() + "\\" + DateTime.Now.Month.ToString() + "\\";
                var countItem = 1;
                string appendFileName;
                string fullNewPath;
                do
                {
                    appendFileName = id.ToString() + "_" + countItem.ToString() + "_" + System.IO.Path.GetFileName(tempFile.FileName);
                    fullNewPath = uploadFolder + appendPath + appendFileName;
                    countItem++;
                }
                while (System.IO.File.Exists(fullNewPath));
                newFileItem.FullFileName = appendPath + appendFileName;
                db.OFIFiles.Add(newFileItem);
                db.SaveChanges();

                if (!System.IO.Directory.Exists(uploadFolder + appendPath))
                    System.IO.Directory.CreateDirectory(uploadFolder + appendPath);

                tempFile.SaveAs(fullNewPath);
            }
        }

        public void Validate(ModelStateDictionary modelState)
        {
        }

        public void UpdateOFI(ModelStateDictionary modelState)
        {
            var db = ACM.Helpers.DBHelper.GetDBContext();

            var result = db.OFIs.FirstOrDefault(m => m.OFIID == this.OFIID.Value);
            result.DepartmentID = this.DepartmentID;
            result.OFICategoryID = this.OFICategoryID ?? 0;
            result.OFIOriginID = this.OFIOriginID ?? 0;
            result.OFITypeID = this.OFITypeID ?? 0;
            result.PriorityID = this.PriorityID ?? 1;
            result.StatusID = ACM.Helpers.ConfigHelper.ActiveStatus();
            result.Subject = this.Subject;

            if (result.TargetDate != this.TargetDate)
            {
                var newDueDate = db.OFIDueDates.Create();
                newDueDate.DueDate = this.TargetDate.Value;
                newDueDate.DueDateCreatedDate = DateTime.Now;
                newDueDate.DueDateCreatedID = ACM.Helpers.UserHelper.GetCurrentUserID();
                newDueDate.OFIID = this.OFIID.Value;
                db.OFIDueDates.Add(newDueDate);
                db.SaveChanges();
            }

            result.TargetDate = this.TargetDate;
            result.TimeEstimation = this.TimeEstimation;

            var currentAssigned = db.OFIAssigneds.Where(m => m.OFIID == this.OFIID.Value).Select(select => select.UserID).ToList();

            var usersToRemove = currentAssigned.Except(this.OFIAssignedTo);
            var usersToAdd = this.OFIAssignedTo.Except(currentAssigned);

            foreach (var userID in currentAssigned)
            {
                if (usersToRemove.Contains(userID))
                {
                    var resultRemove = db.OFIAssigneds.Where(m => m.OFIID == this.OFIID.Value).Where(m => m.UserID == userID).FirstOrDefault();
                    if (resultRemove != null)
                    {
                        db.OFIAssigneds.Remove(resultRemove);
                        db.SaveChanges();
                    }
                }
            }

            foreach (var addUserId in usersToAdd)
            {
                var newItem = db.OFIAssigneds.Create();
                newItem.UserID = addUserId;
                newItem.OFIID = this.OFIID.Value;
                db.OFIAssigneds.Add(newItem);
                db.SaveChanges();
            }

            ACM.Helpers.EmailHelper.SendNewAssignedOFIEmail(this.OFIID.Value, usersToAdd.ToList());
        }

        public int? CreateNewOFI(ModelStateDictionary modelState)
        {
            try
            {
                var db = ACM.Helpers.DBHelper.GetDBContext();

                var newOFIItem = db.OFIs.Create();
                newOFIItem.CreatedDate = DateTime.Now;
                newOFIItem.CreatedUserId = ACM.Helpers.UserHelper.GetCurrentUserID();
                newOFIItem.DepartmentID = this.DepartmentID;
                newOFIItem.OFICategoryID = this.OFICategoryID ?? 0;
                newOFIItem.OFIOriginID = this.OFIOriginID ?? 0;
                newOFIItem.OFITypeID = this.OFITypeID ?? 0;
                newOFIItem.PriorityID = this.PriorityID ?? 1;
                newOFIItem.StatusID = ACM.Helpers.ConfigHelper.ActiveStatus();
                newOFIItem.Subject = this.Subject;
                newOFIItem.TargetDate = this.TargetDate;
                newOFIItem.TimeEstimation = this.TimeEstimation;
                db.OFIs.Add(newOFIItem);
                db.SaveChanges();

                var newDueDate = db.OFIDueDates.Create();
                newDueDate.OFIID = newOFIItem.OFIID;
                newDueDate.DueDateCreatedDate = DateTime.Now;
                newDueDate.DueDateCreatedID = ACM.Helpers.UserHelper.GetCurrentUserID();
                newDueDate.DueDate = this.TargetDate.Value;
                db.OFIDueDates.Add(newDueDate);
                db.SaveChanges();

                var newOFIEntryItem = db.OFIEntries.Create();
                newOFIEntryItem.CreateDate = DateTime.Now;
                newOFIEntryItem.CreateUserID = ACM.Helpers.UserHelper.GetCurrentUserID();
                newOFIEntryItem.EntryText = HttpUtility.HtmlDecode(this.FirstAction);
                newOFIEntryItem.OFIID = newOFIItem.OFIID;
                db.OFIEntries.Add(newOFIEntryItem);
                db.SaveChanges();

                foreach (var userID in this.OFIAssignedTo)
                {
                    var newUserAssigned = db.OFIAssigneds.Create();
                    newUserAssigned.UserID = userID;
                    newUserAssigned.OFIID = newOFIItem.OFIID;
                    db.OFIAssigneds.Add(newUserAssigned);
                    db.SaveChanges();
                }
                ACM.Helpers.EmailHelper.SendNewOFIEmail(newOFIItem.OFIID, newOFIEntryItem.OFIEntryID);

                if (this.attachments != null)
                {
                    var uploadFolder = ACM.Helpers.ConfigHelper.UploadFolder();

                    if (!uploadFolder.EndsWith("\\"))
                        uploadFolder = uploadFolder + "\\";

                    foreach (var tempFile in attachments)
                    {
                        var newFileItem = new OFIFile();
                        newFileItem.OFIID = newOFIItem.OFIID;
                        newFileItem.ContentType = tempFile.ContentType;
                        newFileItem.CreatedDateTime = DateTime.Now;
                        newFileItem.CreatedUserID = ACM.Helpers.UserHelper.GetCurrentUserID();
                        newFileItem.FileDescription = System.IO.Path.GetFileNameWithoutExtension(tempFile.FileName);
                        newFileItem.FileLength = tempFile.ContentLength;
                        var appendPath = DateTime.Now.Year.ToString() + "\\" + DateTime.Now.Month.ToString() + "\\";
                        var countItem = 1;
                        string appendFileName;
                        string fullNewPath;
                        do
                        {
                            appendFileName = newOFIItem.OFIID.ToString() + "_" + countItem.ToString() + "_" + System.IO.Path.GetFileName(tempFile.FileName);
                            fullNewPath = uploadFolder + appendPath + appendFileName;
                            countItem++;
                        }
                        while (System.IO.File.Exists(fullNewPath));
                        newFileItem.FullFileName = appendPath + appendFileName;
                        db.OFIFiles.Add(newFileItem);
                        db.SaveChanges();

                        if (!System.IO.Directory.Exists(uploadFolder + appendPath))
                            System.IO.Directory.CreateDirectory(uploadFolder + appendPath);

                        tempFile.SaveAs(fullNewPath);
                    }
                }

                return newOFIItem.OFIID;
            }
            catch
            {
                modelState.AddModelError("", "An error has occurred.");
            }
            return null;
        }
    }

    public class OFIList
    {
        public int OFIID { get; set; }

        public string Subject { get; set; }

        public DateTime? DateCreated { get; set; }

        public DateTime? TargetDate { get; set; }
    }

    public class OFIFileReturn
    {
        public byte[] FileBuffer { get; set; }

        public string FileName { get; set; }

        public string ContentType { get; set; }
    }

    public class OFIApprovalText
    {
        public string ApprovalName { get; set; }

        public string ApprovalDate { get; set; }
    }
}