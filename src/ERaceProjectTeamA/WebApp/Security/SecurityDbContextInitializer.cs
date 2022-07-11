using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

#region Additional Namespaces
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Configuration;
using System.Data.Entity;
using WebApp.Models;
#endregion

namespace WebApp.Security
{
    public class SecurityDbContextInitializer : CreateDatabaseIfNotExists<ApplicationDbContext>
    {

        protected override void Seed(ApplicationDbContext context)
        {
            #region Phase A - Set up our Security Roles
            // 1. Instantiate a Controller class from ASP.Net Identity to add roles
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            // 2. Grab our list of security roles from the web.config
            var startupRoles = ConfigurationManager.AppSettings["startupRoles"].Split(';');
            // 3. Loop through and create the security roles
            foreach (var role in startupRoles)
                roleManager.Create(new IdentityRole { Name = role });
            #endregion

            #region Phase B - Add a Website Administrator
            // 1. Get the values from the <appSettings>
            string adminUser = ConfigurationManager.AppSettings["adminUserName"];
            string adminRole = ConfigurationManager.AppSettings["adminRole"];
            string adminEmail = ConfigurationManager.AppSettings["adminEmail"];
            string adminPassword = ConfigurationManager.AppSettings["adminPassword"];

            // 2. Instantiate my Controller to manage Users
            var userManager = new ApplicationUserManager(new UserStore<ApplicationUser>(context));
            //                \   IdentityConfig.cs    /             \IdentityModels.cs/
            // 3. Add the web admin to the database
            var result = userManager.Create(new ApplicationUser
            {
                UserName = adminUser,
                Email = adminEmail,
                CustomerId = null,
                EmployeeId = null,
                PositionId = null
            }, adminPassword);
            if (result.Succeeded)
                userManager.AddToRole(userManager.FindByName(adminUser).Id, adminRole);
            #endregion

            #region Phase C - Add a Customer
            // 1. Get the values from the <appSettings>
            int customerId = int.Parse(ConfigurationManager.AppSettings["customerId"]);
            string customerUser = ConfigurationManager.AppSettings["customerUserName"];
            string customerRole = ConfigurationManager.AppSettings["customerRole"];
            string customerEmail = ConfigurationManager.AppSettings["customerEmail"];
            string customerPassword = ConfigurationManager.AppSettings["customerPassword"];
            result = userManager.Create(new ApplicationUser
            {
                CustomerId = customerId,
                UserName = customerUser,
                Email = customerEmail,
                EmployeeId = null,
                PositionId = null
            }, customerPassword);
            if (result.Succeeded)
                userManager.AddToRole(userManager.FindByName(customerUser).Id, customerRole);
            #endregion

            //#region Phase D - Add a Employee
            //// 1. Get the values from the <appSettings>
            //int employeeId = int.Parse(ConfigurationManager.AppSettings["employeeId"]);
            //string employeeUser = ConfigurationManager.AppSettings["employeeUserName"];
            //string employeeRole = ConfigurationManager.AppSettings["employeeRole"];
            //string employeeEmail = ConfigurationManager.AppSettings["employeeEmail"];
            //string employeePassword = ConfigurationManager.AppSettings["employeePassword"];
            //result = userManager.Create(new ApplicationUser
            //{
            //    EmployeeId = employeeId,
            //    UserName = employeeUser,
            //    Email = employeeEmail,
            //    CustomerId = null
            //}, employeePassword);
            //if (result.Succeeded)
            //    userManager.AddToRole(userManager.FindByName(employeeUser).Id, employeeRole);
            //#endregion
            #region Phase D - Add a Clerk
            // 1. Get the values from the <appSettings>
            int clerkId = int.Parse(ConfigurationManager.AppSettings["clerkId"]);
            string clerkUser = ConfigurationManager.AppSettings["clerkUserName"];
            string clerkRole = ConfigurationManager.AppSettings["clerkRole"];
            string clerkEmail = ConfigurationManager.AppSettings["clerkEmail"];
            string clerkPassword = ConfigurationManager.AppSettings["clerkPassword"];
            result = userManager.Create(new ApplicationUser
            {
                EmployeeId = clerkId,
                UserName = clerkUser,
                Email = clerkEmail,
                CustomerId = null,
                PositionId = 9
                
            }, clerkPassword);
            if (result.Succeeded)
                userManager.AddToRole(userManager.FindByName(clerkUser).Id, clerkRole);
            #endregion
            #region Phase D - Add a Director
            // 1. Get the values from the <appSettings>
            int directorId = int.Parse(ConfigurationManager.AppSettings["directorId"]);
            string directorUser = ConfigurationManager.AppSettings["directorUserName"];
            string directorRole = ConfigurationManager.AppSettings["directorRole"];
            string directorEmail = ConfigurationManager.AppSettings["directorEmail"];
            string directorPassword = ConfigurationManager.AppSettings["directorPassword"];
            result = userManager.Create(new ApplicationUser
            {
                EmployeeId = directorId,
                UserName = directorUser,
                Email = directorEmail,
                CustomerId = null,
                PositionId = 1

            }, directorPassword);
            if (result.Succeeded)
                userManager.AddToRole(userManager.FindByName(directorUser).Id, directorRole);
            #endregion
            #region Phase D - Add a Office Manager
            // 1. Get the values from the <appSettings>
            int officeManagerId = int.Parse(ConfigurationManager.AppSettings["officeManagerId"]);
            string officeManagerUser = ConfigurationManager.AppSettings["officeManagerUserName"];
            string officeManagerRole = ConfigurationManager.AppSettings["officeManagerRole"];
            string officeManagerEmail = ConfigurationManager.AppSettings["officeManagerEmail"];
            string officeManagerPassword = ConfigurationManager.AppSettings["officeManagerPassword"];
            result = userManager.Create(new ApplicationUser
            {
                EmployeeId = officeManagerId,
                UserName = officeManagerUser,
                Email = officeManagerEmail,
                CustomerId = null,
                PositionId = 10

            }, officeManagerPassword);
            if (result.Succeeded)
                userManager.AddToRole(userManager.FindByName(officeManagerUser).Id, officeManagerRole);
            #endregion
            #region Phase D - Add a Race Coordinator
            // 1. Get the values from the <appSettings>
            int raceCoordinatorId = int.Parse(ConfigurationManager.AppSettings["raceCoordinatorId"]);
            string raceCoordinatorUser = ConfigurationManager.AppSettings["raceCoordinatorUserName"];
            string raceCoordinatorRole = ConfigurationManager.AppSettings["raceCoordinatorRole"];
            string raceCoordinatorEmail = ConfigurationManager.AppSettings["raceCoordinatorEmail"];
            string raceCoordinatorPassword = ConfigurationManager.AppSettings["raceCoordinatorPassword"];
            result = userManager.Create(new ApplicationUser
            {
                EmployeeId = raceCoordinatorId,
                UserName = raceCoordinatorUser,
                Email = raceCoordinatorEmail,
                CustomerId = null,
                PositionId = 2

            }, raceCoordinatorPassword);
            if (result.Succeeded)
                userManager.AddToRole(userManager.FindByName(raceCoordinatorUser).Id, raceCoordinatorRole);
            #endregion
            #region Phase D - Add a Food Service
            // 1. Get the values from the <appSettings>
            int foodServiceId = int.Parse(ConfigurationManager.AppSettings["foodServiceId"]);
            string foodServiceUser = ConfigurationManager.AppSettings["foodServiceUserName"];
            string foodServiceRole = ConfigurationManager.AppSettings["foodServiceRole"];
            string foodServiceEmail = ConfigurationManager.AppSettings["foodServiceEmail"];
            string foodServicePassword = ConfigurationManager.AppSettings["foodServicePassword"];
            result = userManager.Create(new ApplicationUser
            {
                EmployeeId = foodServiceId,
                UserName = foodServiceUser,
                Email = foodServiceEmail,
                CustomerId = null,
                PositionId = 7

            }, foodServicePassword);
            if (result.Succeeded)
                userManager.AddToRole(userManager.FindByName(foodServiceUser).Id, foodServiceRole);
            #endregion


            base.Seed(context);
        }
    }
}