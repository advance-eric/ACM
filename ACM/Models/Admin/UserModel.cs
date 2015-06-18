using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNet.Identity.EntityFramework;
using System.ComponentModel.DataAnnotations;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity; 

namespace ACM.Models.Admin
{
    public class UserModel 
    {
        [Display(Name = "ID #")]
        public string Id { get; set; }
        [Display(Name = "First Name")]
        public string FirstName { get; set; }
        [Display(Name = "Last Name")]
        public string LastName { get; set; }
        [Display(Name = "Email Address"), EmailAddress]
        public string Email { get; set; }
        [Display(Name = "Email Address Confirmed?")]
        public bool EmailConfirmed { get; set; }
        [Display(Name = "Password Hash")]
        public string PasswordHash { get; set; }
        public string SecurityStamp { get; set; }
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }
        [Display(Name = "Phone Number Confirmed?")]
        public bool PhoneNumberConfirmed { get; set; }
        public bool TwoFactorEnabled { get; set; }
        public DateTime? LockoutEndDateUtc { get; set; }
        public bool LockoutEnabled { get; set; }
        public int AccessFailedCount { get; set; }
        [Display(Name = "Login Name")]
        public string UserName { get; set; }
        [Display(Name = "Active User?")]
        public bool ActiveUser { get; set; }
        [Display(Name = "User Role")]
        public string UserRoleID { get; set; }
        [Display(Name = "Department")]
        public int? DepartmentID { get; set; }

        [Display(Name = "Enter Password")]
        public string EnterPassword { get; set; }
        [Display(Name = "Re-enter Password")]
        public string ReenterPassword { get; set; }

        [Display(Name = "Action")]
        public string Action { get; set; }

        public void Validate(ModelStateDictionary modelState)
        {
            var db = ACM.Helpers.DBHelper.GetDBContext();
            if (this.DepartmentID.HasValue)
            {
                var department = db.Departments.FirstOrDefault(m => m.DepartmentID == this.DepartmentID);

                if (!string.IsNullOrWhiteSpace(department.NationalManagerUserID) && this.UserRoleID == ACM.Helpers.ConfigHelper.NationalManagerRoleID() && (department.NationalManagerUserID != this.Id))
                    modelState.AddModelError("DepartmentID", "This department already has a national manager set.");
            }

        }

        public List<UserGridItem> GetGridData()
        {
            var db = ACM.Helpers.DBHelper.GetDBContext();

            var context = ApplicationDbContext.Create();
            

            var result =
                (from Users in context.Users
                 orderby Users.Email
                 select new UserGridItem
                 {
                     Id = Users.Id,
                     FullName = Users.FirstName + " "+ Users.LastName,
                     Role = Users.Roles.FirstOrDefault().RoleId
                 }).ToList();

            /*var result =
                (from Users in db.AspNetUsers
                 let p = db.AspNetUserRoles.Where(p2 => Users.Id == p2.UserId).FirstOrDefault()
                 join Roles in db.AspNetRoles on p.RoleId equals Roles.Id into join1
                 from Roles in join1.DefaultIfEmpty()
                 join Departments in db.Departments on Users.DepartmentID equals Departments.DepartmentID into join2
                 from Departments in join2.DefaultIfEmpty()
                 select new UserGridItem
                 {
                     Id = Users.Id,
                     FullName = Users.FirstName + " " + Users.LastName,
                     Role = Roles.Name,
                     Department = Departments.DepartmentName
                 }).ToList();*/




            return result;
        }

        public void LoadData(string id)
        {
            this.Id = id;

            var db = ACM.Helpers.DBHelper.GetDBContext();

            var result = db.AspNetUsers.FirstOrDefault(m => m.Id == id);

            if (result == null)
                return;

            this.FirstName = result.FirstName;
            this.LastName = result.LastName;
            this.Email = result.Email;
            this.EmailConfirmed = result.EmailConfirmed;
            this.PasswordHash = result.PasswordHash;
            this.SecurityStamp = result.SecurityStamp;
            this.PhoneNumber = result.PhoneNumber;
            this.PhoneNumberConfirmed = result.PhoneNumberConfirmed;
            this.TwoFactorEnabled = result.TwoFactorEnabled;
            this.LockoutEndDateUtc = result.LockoutEndDateUtc;
            this.LockoutEnabled = result.LockoutEnabled;
            this.AccessFailedCount = result.AccessFailedCount;
            this.UserName = result.UserName;
            //this.ActiveUser = result.ActiveUser;
            //this.UserRoleID = db.AspNetUserRoles.FirstOrDefault(m => m.UserId == id) == null ? "" : db.AspNetUserRoles.FirstOrDefault(m => m.UserId == id).RoleId;
            //this.DepartmentID = result.DepartmentID;

        }

        public void SaveData(ModelStateDictionary modelState)
        {
            var db = ACM.Helpers.DBHelper.GetDBContext();

            var result = db.AspNetUsers.FirstOrDefault(m => m.Id == this.Id);

            result.FirstName = this.FirstName;
            result.LastName = this.LastName;
            result.Email = this.Email;
            result.UserName = this.Email;
            result.PhoneNumber = this.PhoneNumber;
            /*result.DepartmentID = this.DepartmentID;
            result.ActiveUser = this.ActiveUser;*/

            db.SaveChanges();

            /*var roleItems = db.AspNetUserRoles.Where(m => m.UserId == this.Id);

            if (roleItems.FirstOrDefault() != null)
            {
                db.AspNetUserRoles.DeleteAllOnSubmit(roleItems);
                db.SubmitChanges();
            }

            var newRoleItem = new AspNetUserRole();
            newRoleItem.UserId = this.Id;
            newRoleItem.RoleId = this.UserRoleID;
            db.AspNetUserRoles.InsertOnSubmit(newRoleItem);
            db.SubmitChanges();*/
        }

        public List<SelectListItem> GetDepartments()
        {
            var db = ACM.Helpers.DBHelper.GetDBContext();

            var result =
                (from Departments in db.Departments
                 orderby Departments.DepartmentName
                 select new SelectListItem
                 {
                     Text = Departments.DepartmentName,
                     Value = Departments.DepartmentID.ToString()
                 }).ToList();

            result.Insert(0, new SelectListItem() { Text = "--Select Item--", Value = "" });

            return result;
        }

        public IEnumerable<SelectListItem> GetUserRoles()
        {

            var context = ApplicationDbContext.Create();
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            var result =
                (from Roles in roleManager.Roles
                 orderby Roles.Name
                 select new SelectListItem
                 {
                     Text = Roles.Name,
                     Value = Roles.Id
                 });

            return result;
        }


    }

    public class UserPasswordModel
    {
        [Display(Name = "ID #")]
        public string Id { get; set; }
        [Display(Name = "Enter Password")]
        public string EnterPassword { get; set; }
        [Display(Name = "Re-enter Password")]
        public string ReenterPassword { get; set; }
    }

    public class UserGridItem
    {
        [Display(Name = "ID #")]
        public string Id { get; set; }
        [Display(Name = "Full Name")]
        public string FullName { get; set; }
        [Display(Name = "Department")]
        public string Department { get; set; }
        [Display(Name = "Role")]
        public string Role { get; set; }
    }
}