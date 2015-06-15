using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.DirectoryServices;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using ACM.Models;

namespace ACM
{
    public class EmailService : IIdentityMessageService
    {
        public Task SendAsync(IdentityMessage message)
        {
            var toEmails = new List<string>();
            toEmails.Add(message.Destination);
            ACM.Helpers.EmailHelper.SendEmail(toEmails, message.Subject, message.Body);
            return Task.FromResult(0);
        }
    }

    public class SmsService : IIdentityMessageService
    {
        public Task SendAsync(IdentityMessage message)
        {
            // Plug in your SMS service here to send a text message.
            return Task.FromResult(0);
        }
    }

    public class ApplicationRoleManager : RoleManager<IdentityRole>
    {
        public ApplicationRoleManager(IRoleStore<IdentityRole, string> store)
            : base(store)
        {
        }
        public static ApplicationRoleManager Create(IdentityFactoryOptions<ApplicationRoleManager> options, IOwinContext context)
        {
            var roleStore = new RoleStore<IdentityRole>(context.Get<ApplicationDbContext>());
            return new ApplicationRoleManager(roleStore);
        }
    }

    // Configure the application user manager used in this application. UserManager is defined in ASP.NET Identity and is used by the application.
    public class ApplicationUserManager : UserManager<ApplicationUser>
    {
        public ApplicationUserManager(IUserStore<ApplicationUser> store)
            : base(store)
        {
        }

        public static ApplicationUserManager Create(IdentityFactoryOptions<ApplicationUserManager> options, IOwinContext context)
        {
            var manager = new ApplicationUserManager(new UserStore<ApplicationUser>(context.Get<ApplicationDbContext>()));
            // Configure validation logic for usernames
            manager.UserValidator = new UserValidator<ApplicationUser>(manager)
            {
                AllowOnlyAlphanumericUserNames = false,
                RequireUniqueEmail = true
            };

            // Configure validation logic for passwords
            manager.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 8,
                RequireNonLetterOrDigit = true,
                RequireDigit = true,
                RequireLowercase = true,
                RequireUppercase = true,
            };

            // Configure user lockout defaults
            manager.UserLockoutEnabledByDefault = true;
            manager.DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(5);
            manager.MaxFailedAccessAttemptsBeforeLockout = 5;

            // Register two factor authentication providers. This application uses Phone and Emails as a step of receiving a code for verifying the user
            // You can write your own provider and plug it in here.
            manager.RegisterTwoFactorProvider("Phone Code", new PhoneNumberTokenProvider<ApplicationUser>
            {
                MessageFormat = "Your security code is {0}"
            });
            manager.RegisterTwoFactorProvider("Email Code", new EmailTokenProvider<ApplicationUser>
            {
                Subject = "Security Code",
                BodyFormat = "Your security code is {0}"
            });
            manager.EmailService = new EmailService();
            manager.SmsService = new SmsService();
            var dataProtectionProvider = options.DataProtectionProvider;
            if (dataProtectionProvider != null)
            {
                manager.UserTokenProvider =
                    new DataProtectorTokenProvider<ApplicationUser>(dataProtectionProvider.Create("ASP.NET Identity"));
            }
            return manager;
        }
    }

    // Configure the application sign-in manager which is used in this application.
    public class ApplicationSignInManager : SignInManager<ApplicationUser, string>
    {
        public ApplicationSignInManager(ApplicationUserManager userManager, IAuthenticationManager authenticationManager)
            : base(userManager, authenticationManager)
        {
        }

        public override Task<ClaimsIdentity> CreateUserIdentityAsync(ApplicationUser user)
        {
            return user.GenerateUserIdentityAsync((ApplicationUserManager)UserManager);
        }

        public static ApplicationSignInManager Create(IdentityFactoryOptions<ApplicationSignInManager> options, IOwinContext context)
        {
            return new ApplicationSignInManager(context.GetUserManager<ApplicationUserManager>(), context.Authentication);
        }

        public async Task<SignInStatus> PasswordSignInAsyncCustom(string userName, string password, bool isPersistent, bool shouldLockout)
        {
            var baseResult = await base.PasswordSignInAsync(userName, password, isPersistent, shouldLockout);
            
            //var result = await Task.Run(FindAccountByEmail(userName));
            DirectoryEntry result = new DirectoryEntry();
            Task t = Task.Run(() =>
            {
                result = FindAccountByEmail(userName);
                var yeah = CheckAdsiLogin(GetPropertyValue(result, "sAMAccountName"), password);
            });

            if (result != null)
            {
                //var retVal = Task<SignInStatus.Success>();
                //retVal = 
                return SignInStatus.Success;
            }
            
            return baseResult;
        }

        private string GetPropertyValue(DirectoryEntry de, string propertyName)
        {
            foreach (PropertyValueCollection property in de.Properties)
            {
                if (property.PropertyName == propertyName)
                {
                    return property[0].ToString();
                }
            }
            return "";
        }

        public DirectoryEntry FindAccountByEmail(string email)
        {
            var oroot = new System.DirectoryServices.DirectoryEntry(ACM.Helpers.ConfigHelper.LDAPQuery());
            var osearcher = new System.DirectoryServices.DirectorySearcher(oroot);
            osearcher.Filter = string.Format("(&(mail={0}))", email);
            var oresult = osearcher.FindAll();

            if (oresult.Count > 1) throw new InvalidOperationException(string.Format("There are {0} items with mail {1} in LDAP.", oresult.Count, email));

            if (oresult.Count == 0)
                return null;

            return oresult[0].GetDirectoryEntry();
        }

        private bool CheckAdsiLogin(string accountName, string password)
        {
            try
            {
                /*var serverName = System.Configuration.ConfigurationManager.AppSettings["LDAPServerName"];
                var baseDN = System.Configuration.ConfigurationManager.AppSettings["LDAPBaseDN"];
                var userDN = System.Configuration.ConfigurationManager.AppSettings["LDAPUserDN"];
                //string _groupName = System.Configuration.ConfigurationManager.AppSettings["LDAPGroupName"];
                var accountFilter = System.Configuration.ConfigurationManager.AppSettings["LDAPAccountFilter"];*/

                // Search for user
                var de = new DirectoryEntry(ACM.Helpers.ConfigHelper.LDAPQuery());
                de.AuthenticationType = AuthenticationTypes.Secure;
                de.Username = accountName;
                de.Password = password;
                // Search for account name
                var search = "sAMAccountName=" + accountName;
                var ds = new DirectorySearcher(de, search);
                // Add properties to see if account is disabled
                ds.PropertiesToLoad.Add("userAccountControl");
                // Search subtree of UserDN
                ds.SearchScope = SearchScope.Subtree;

                // Find the user data - causes error if password is incorrect
                ds.FindOne();
                //SearchResult _sr = _ds.FindOne();

                // '' check to make sure they aren't disabled
                //Dim _userAC As Integer = Convert.ToInt32(_sr.Properties("userAccountControl")(0))
                //If _userAC <> 1 Then Return False

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
