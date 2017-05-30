namespace Events.Data.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using Data;
    using Microsoft.AspNet.Identity.EntityFramework;
    using Microsoft.AspNet.Identity;
    using System.Collections.Generic;

    public sealed class DbMigrationsConfig : DbMigrationsConfiguration<Events.Data.ApplicationDbContext>
    {
        public DbMigrationsConfig()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = true;
        }

        protected override void Seed(ApplicationDbContext context)
        {
            if (!context.Users.Any())
            {
                var adminEmail = "admin@admin.com";
                var adminUserName = adminEmail;
                var adminFullName = "System Admin";
                var adminPassword = adminEmail;
                string adminRole = "Administrator";
                CreateAdminUser(context, adminEmail, adminUserName, adminFullName, adminPassword, adminRole);
                CreateSeveralEvents(context);
            }
        }

        private void CreateSeveralEvents(ApplicationDbContext context)
        {
            context.Events.Add(new Event()
            {
                Title = "First Event",
                StartDateTime = DateTime.Now.Date.AddDays(5).AddHours(21).AddMinutes(30),
                Duration = TimeSpan.FromHours(1.5)   
            });

            context.Events.Add(new Event()
            {
                Title = "Third Event",
                StartDateTime = DateTime.Now.Date.AddDays(-2).AddHours(17).AddMinutes(30),
                Duration = TimeSpan.FromHours(1.5),
                Comments = new HashSet<Comment>()
                {
                    new Comment() {Text = "<Anonymous> comment" },
                    new Comment() {Text = "User comment", Author = context.Users.First() }
                }
            });

            context.Events.Add(new Event()
            {
                Title = "Second Event",
                StartDateTime = DateTime.Now.Date.AddDays(15).AddHours(21).AddMinutes(30),
                Duration = TimeSpan.FromHours(1),
                Comments = new HashSet<Comment>()
                {
                    new Comment() {Text = "Test comment", Author = context.Users.First() }
                }
            });

            context.Events.Add(new Event()
            {
                Title = "Fourth Event",
                StartDateTime = DateTime.Now.Date.AddDays(-15).AddHours(16).AddMinutes(30),
                Duration = TimeSpan.FromHours(10),
                Comments = new HashSet<Comment>()
                {
                    new Comment() {Text = "<Anonymous> comment" }
                }
            });

        }

        private void CreateAdminUser(ApplicationDbContext context, string adminEmail, string adminUserName, string adminFullName, string adminPassword, string adminRole)
        {
            //Create the admin user
            var adminUser = new ApplicationUser
            {
                UserName = adminUserName,
                FullName = adminFullName,
                Email = adminEmail,
            };
            var userStore = new UserStore<ApplicationUser>(context);
            var userManager = new UserManager<ApplicationUser>(userStore);
            userManager.PasswordValidator = new PasswordValidator
            {
                RequireDigit = false,
                RequiredLength = 1,
                RequireNonLetterOrDigit = false,
                RequireLowercase = false,
                RequireUppercase = false,
            };
            var userCreateResult = userManager.Create(adminUser, adminPassword);
            if (!userCreateResult.Succeeded)
            {
                throw new Exception(string.Join("; ", userCreateResult.Errors));
            }

            //Create the admin role

            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            var roleCreateResult = roleManager.Create(new IdentityRole(adminRole));
            if (!roleCreateResult.Succeeded)
            {
                throw new Exception(string.Join("; ", roleCreateResult.Errors));
            }

            //Add the admin user to the admin role

            var adminRoleResult = userManager.AddToRole(adminUser.Id, adminRole);
            if (!adminRoleResult.Succeeded)
            {
                throw new Exception(string.Join("; ", adminRoleResult.Errors));
            }
        }
    }
}
