﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OPIDDaily.DAL;
using OPIDDaily.Models;

namespace OPIDDaily.Controllers
{
    public class SuperadminController : UsersController
    {
        public ActionResult Home()
        {
            string workingConnectionString = string.Empty;

            ViewBag.Release = Config.Release;

            switch (Config.Release)
            {
                case "Desktop":
                    workingConnectionString = Config.WorkingDesktopConnectionString;
                    break;
                case "Staging":
                    workingConnectionString = Config.WorkingStagingConnectionString;
                    break;
                case "Production":
                    workingConnectionString = Config.WorkingProductionConnectionString;
                    break;
            }

            ViewBag.ConnectionString = Config.ConnectionString;
            ViewBag.ChangedConnectionString = (Config.ConnectionString.Equals(workingConnectionString) ? "False" : "True");

            // Log.Info("Goto Superadmin home page");
            return View();
        }

        public ActionResult ManageRoles()
        {
            return RedirectToAction("Index", "Role");
        }

        
        public ActionResult ManageUsers()
        {
            return View("Users");
        }

        public string ExtendInvitation(InvitationViewModel invite)
        {
            if (InUse(invite.UserName))
            {
                string status = string.Format("The user name {0} is already in use. Please use a different user name.", invite.UserName);
                return status;
            }

            Identity.ExtendInvitation(invite);

            return "Success";
        }

        public JsonResult GetUsers(int page, int rows)
        {
            List<InvitationViewModel> invitations = Identity.GetUsers();

            var jsonData = new
            {
                total = 1,
                page,
                records = invitations.Count,
                rows = invitations
            };

            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }
    }
}