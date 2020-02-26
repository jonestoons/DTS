using System.Linq;
using System.Security.Claims;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Threading.Tasks;
using System.Web;
using System.Net.Mail;
using System.Configuration;
using System.Net.Mime;
using System.IO;
using DocumentTracking.Models.DataModels;

namespace DocumentTracking.Models
{
    // Configure the application user manager used in this application. UserManager is defined in ASP.NET Identity and is used by the application.

    public class ApplicationUserManager : UserManager<ApplicationUser>
    {
        public ApplicationUserManager(IUserStore<ApplicationUser> store)
            : base(store)
        {
        }

        public static ApplicationUserManager Create(IdentityFactoryOptions<ApplicationUserManager> options,
            IOwinContext context)
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
                RequiredLength = 3,
                RequireNonLetterOrDigit = false,
                RequireDigit = false,
                RequireLowercase = false,
                RequireUppercase = false,
            };
            // Configure user lockout defaults
            manager.UserLockoutEnabledByDefault = true;
            manager.DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(5);
            manager.MaxFailedAccessAttemptsBeforeLockout = 5;
            // Register two factor authentication providers. This application uses Phone and Emails as a step of receiving a code for verifying the user
            // You can write your own provider and plug in here.
            manager.RegisterTwoFactorProvider("PhoneCode", new PhoneNumberTokenProvider<ApplicationUser>
            {
                MessageFormat = "Your security code is: {0}"
            });
            manager.RegisterTwoFactorProvider("EmailCode", new EmailTokenProvider<ApplicationUser>
            {
                Subject = "SecurityCode",
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

    // Configure the RoleManager used in the application. RoleManager is defined in the ASP.NET Identity core assembly
    public class ApplicationRoleManager : RoleManager<IdentityRole>
    {
        public ApplicationRoleManager(IRoleStore<IdentityRole,string> roleStore)
            : base(roleStore)
        {
        }

        public static ApplicationRoleManager Create(IdentityFactoryOptions<ApplicationRoleManager> options, IOwinContext context)
        {
            return new ApplicationRoleManager(new RoleStore<IdentityRole>(context.Get<ApplicationDbContext>()));
        }
    }

    public class EmailService : IIdentityMessageService
    {
        public Task SendAsync(IdentityMessage message)
        {
            // Plug in your email service here to send an email.
            //return Task.FromResult(0);
            return Task.Factory.StartNew(() =>
            {
                sendMail(message);
            });
        }
        void sendMail(IdentityMessage message)
        {

            MsgSetup setup = new MsgSetup();
            ApplicationDbContext db = new ApplicationDbContext();
            setup = db.MsgSetups.Where(x => x.Id == 1).FirstOrDefault();
            string line;
            //Pass the file path and file name to the StreamReader constructor

            using (StreamReader sr = new StreamReader(AppDomain.CurrentDomain.BaseDirectory + "Content/MailTemplates/Confirmation.txt"))
            {
                line = sr.ReadToEnd();
            }

            //Read the first line of text
            //line = sr.ReadLine();

            //var t = line.Split(';').ToList();

            //for (int i = 0; i < t.Count; i++)
            //{
            //    if (i == 0)
            //    {
            //        setup.Email = t[i].ToString();
            //    }
            //    else if (i == 1)
            //    {
            //        setup.Password = t[i].ToString();
            //    }
            //    else if (i == 2)
            //    {
            //        setup.Port = t[i].ToString();
            //    }
            //    else if (i == 3)
            //    {
            //        setup.Host = t[i].ToString();
            //    }
            //}
            //sr.Close();


            #region formatter
            string text = string.Format("Please click on this link to {0}: {1}", message.Subject, message.Body);
            //string html = "Please confirm your account by clicking this link: <a href='" + message.Body + "'>link</a><br/>";
            string html = line.Replace("{0}",message.Body);
            //html += HttpUtility.HtmlEncode(@"Or click on the copy the following link on the browser:" + message.Body);
            #endregion

            MailMessage msg = new MailMessage();
            msg.From = new MailAddress(setup.Email);
            msg.To.Add(new MailAddress(message.Destination));
            msg.Subject = message.Subject;
            msg.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(text, null, MediaTypeNames.Text.Plain));
            msg.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(html, null, MediaTypeNames.Text.Html));

            SmtpClient smtpClient = new SmtpClient();
            System.Net.NetworkCredential credentials = new System.Net.NetworkCredential(setup.Email, setup.Password);
            smtpClient.Host = setup.Host; //Or Your SMTP Server Address
            smtpClient.Credentials = new System.Net.NetworkCredential(setup.Email, setup.Password);
            smtpClient.Port = Convert.ToInt32(setup.Port);
            smtpClient.EnableSsl = true;
            smtpClient.Send(msg);

            //System.Net.NetworkCredential credentials = new System.Net.NetworkCredential(ConfigurationManager.AppSettings["Email"].ToString(), ConfigurationManager.AppSettings["Password"].ToString());
            //smtpClient.Host = "mail.staytuneonline.tv"; //Or Your SMTP Server Address
            //smtpClient.Credentials = new System.Net.NetworkCredential(ConfigurationManager.AppSettings["Email"].ToString(), ConfigurationManager.AppSettings["Password"].ToString());
            //smtpClient.Port = 8889;
            ////smtpClient.EnableSsl = true;
            //smtpClient.Send(msg);


            //===================================================================================================================================
            ////create the mail message 
            //MailMessage mail = new MailMessage();

            ////set the addresses 
            //mail.From = new MailAddress("noreply@staytuneonline.tv"); //IMPORTANT: This must be same as your smtp authentication address.
            //mail.To.Add("info@staytuneonline.tv");

            ////set the content 
            //mail.Subject = "This is an email";
            //mail.Body = "This is from system.net.mail using C sharp with smtp authentication.";
            ////send the message 
            //SmtpClient smtp = new SmtpClient("mail.staytuneonline.tv");

            ////IMPORANT:  Your smtp login email MUST be same as your FROM address. 
            //NetworkCredential Credentials = new NetworkCredential(ConfigurationManager.AppSettings["Email"].ToString(), ConfigurationManager.AppSettings["Password"].ToString());
            //smtp.Credentials = Credentials;
            //smtp.Send(mail);
            //lblMessage.Text = "Mail Sent";
        }
    }

    public class SmsService : IIdentityMessageService
    {
        public Task SendAsync(IdentityMessage message)
        {
            // Plug in your sms service here to send a text message.
            return Task.FromResult(0);
        }
    }

    // This is useful if you do not want to tear down the database each time you run the application.
    // public class ApplicationDbInitializer : DropCreateDatabaseAlways<ApplicationDbContext>
    // This example shows you how to create a new database if the Model changes
    public class ApplicationDbInitializer : DropCreateDatabaseIfModelChanges<ApplicationDbContext> 
    {
        protected override void Seed(ApplicationDbContext context) {
            InitializeIdentityForEF(context);
            base.Seed(context);
        }

        //Create User=Admin@Admin.com with password=Admin@123456 in the Admin role        
        public static void InitializeIdentityForEF(ApplicationDbContext db) {
            var userManager = HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var roleManager = HttpContext.Current.GetOwinContext().Get<ApplicationRoleManager>();
            const string name = "admin@example.com";
            const string password = "Admin@123456";
            const string roleName = "Admin";

            //Create Role Admin if it does not exist
            var role = roleManager.FindByName(roleName);
            if (role == null) {
                role = new IdentityRole(roleName);
                var roleresult = roleManager.Create(role);
            }

            var user = userManager.FindByName(name);
            if (user == null) {
                user = new ApplicationUser { UserName = name, Email = name };
                var result = userManager.Create(user, password);
                result = userManager.SetLockoutEnabled(user.Id, false);
            }

            // Add user admin to Role Admin if not already added
            var rolesForUser = userManager.GetRoles(user.Id);
            if (!rolesForUser.Contains(role.Name)) {
                var result = userManager.AddToRole(user.Id, role.Name);
            }
        }
    }

    public class ApplicationSignInManager : SignInManager<ApplicationUser, string>
    {
        public ApplicationSignInManager(ApplicationUserManager userManager, IAuthenticationManager authenticationManager) : 
            base(userManager, authenticationManager) { }

        public override Task<ClaimsIdentity> CreateUserIdentityAsync(ApplicationUser user)
        {
            return user.GenerateUserIdentityAsync((ApplicationUserManager)UserManager);
        }

        public static ApplicationSignInManager Create(IdentityFactoryOptions<ApplicationSignInManager> options, IOwinContext context)
        {
            return new ApplicationSignInManager(context.GetUserManager<ApplicationUserManager>(), context.Authentication);
        }
    }
}