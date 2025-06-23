using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Akquisition.Models;


namespace Akquisition.Security
{
    internal class UserStuff
    {
        internal static void What()
        {
            Models.ApplicationDbContext context = new Models.ApplicationDbContext();
            var roles = context.Roles;

            if (context.Users.Count() == 0)
                return;

            var users = context.Users.ToList();
            foreach (var user in users)
            {
                var uroles = user.Roles.ToList();
                var bla = from ur in user.Roles
                          join r in roles on ur.RoleId equals r.Id
                          select new { RollenName = r.Name };

                foreach (var urole in bla)
                {
                    System.Diagnostics.Debug.Print(String.Format("{0} : {1}", user.UserName, urole.RollenName));
                }
            }
        }

        internal static void AddMicha()
        {
            Models.ApplicationDbContext context = new Models.ApplicationDbContext();

            Microsoft.AspNet.Identity.PasswordHasher hasher = new Microsoft.AspNet.Identity.PasswordHasher();
            string password = hasher.HashPassword("hvu234");


            Models.ApplicationUser user = new Models.ApplicationUser();
            user.UserName = "michael.glander";
            user.PasswordHash = password;
            context.Users.Add(user);
            context.SaveChanges();
        }

        internal static void AddRoles()
        {
            Models.ApplicationDbContext context = new Models.ApplicationDbContext();
            context.Roles.Add(new Microsoft.AspNet.Identity.EntityFramework.IdentityRole { Name = "Admins" });
            //context.Roles.Add(new Microsoft.AspNet.Identity.EntityFramework.IdentityRole { Name = "DataWriter" });
            context.SaveChanges();
        }

        internal static void AddRoleToUser()
        {
            Models.ApplicationDbContext context = new Models.ApplicationDbContext();
            var users = context.Users.ToList();

            foreach (Models.ApplicationUser user in users)
            {
                if (!user.Email.Contains("micha"))
                {
                    var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
                    userManager.AddToRole(user.Id, "DataReader");
                }
            }

            //Models.ApplicationUser user = context.Users.Where(x => x.UserName == "michael.glander@tobis.de").First();
            

            //Controllers.AccountController controller = new Controllers.AccountController();
            //controller.UserManager.AddToRole(user.Id, "DataReader");
            context.SaveChanges();
        }

        internal static void RemoveUserFromRole()
        {
            Models.ApplicationDbContext context = new Models.ApplicationDbContext();
            Models.ApplicationUser user = context.Users.Where(x => x.Email.Contains("peter")).First();
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
            userManager.RemoveFromRole(user.Id, "DataWriter");

            context.SaveChanges();
        }

        internal static void AddUserToRole(string vorname, string rolle)
        {
            Models.ApplicationDbContext context = new Models.ApplicationDbContext();
            Models.ApplicationUser user = context.Users.Where(x => x.Email.Contains(vorname)).First();
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
            userManager.AddToRole(user.Id, rolle);

            context.SaveChanges();
        }

        internal static void CreateUser()
        {
            //string bla;
            //System.Web.Security.MembershipCreateStatus status;
            //System.Web.Security.MembershipUser user = System.Web.Security.Membership.CreateUser("tobias.seiffert@tobis.de", "2016#Studien", "tobias.seiffert@tobis.de", "Methode", "LineareRegression", true, out status);
            //bla = status.ToString();
            //if (status == System.Web.Security.MembershipCreateStatus.Success)
            //{
            //    bla = "yeah!";
            //}
            //else
            //{
            //    bla = "mpf!";
            //}
            //bla = bla + "!";
        }

    }
}