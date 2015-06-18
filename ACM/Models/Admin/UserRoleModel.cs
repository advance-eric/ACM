using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity.EntityFramework;

namespace ACM.Models.Admin
{
    public class UserRoleModel
    {

        public List<ApplicationUser> UsersRole { get; set; }

        public IEnumerable<IdentityRole> GetGridData()
        {
            var context = ApplicationDbContext.Create();
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            return roleManager.Roles.OrderBy(m => m.Name);
        }

        public void GetUserList(string roleId)
        {
            var context = ApplicationDbContext.Create();
            this.UsersRole = context.Users.Where(x => x.Roles.Select(y => y.RoleId).Contains(roleId)).ToList();
        }



    }
}