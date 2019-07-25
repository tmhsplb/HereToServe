﻿using OpidDaily.Models;
using OPIDDaily.DAL;
using OPIDDaily.Models;
using OPIDDaily.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OPIDDaily.Controllers
{
    public class InterviewerController : UsersController
    {
        public ActionResult Home()
        {
            return View();
        }

        public ActionResult ManageClients()
        {
            DateTime today = Extras.DateTimeToday();
            ViewBag.ServiceDate = today.ToString("ddd  MMM d");
            return View("Clients");
        }

        public JsonResult GetClients(int page, int? rows = 25)
        {
            DateTime today = Extras.DateTimeToday();
            ViewBag.ServiceDate = today.ToString("ddd  MMM d");

            List<ClientViewModel> clients = Clients.GetClients(Extras.DateTimeToday());

            int pageIndex = page - 1;
            int pageSize = (int)rows;
            int totalRecords = clients.Count;
            int totalPages = (int)Math.Ceiling((float)totalRecords / (float)rows);

            clients = clients.Skip(pageIndex * pageSize).Take(pageSize).ToList();

            var jsonData = new
            {
                total = totalPages,
                page,  
                records = totalRecords,
                rows = clients
            };

            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        public string EditClient(ClientViewModel cvm)
        {
            string status = Clients.EditClient(cvm);

            if (status.Equals("Success"))
            {
                DailyHub.Refresh();
            }
            return status;
        }

        private static int NowServing()
        {
            return Convert.ToInt32(SessionHelper.Get("NowServing"));
        }

        public void NowServing(int? nowServing = 0)
        {
            SessionHelper.Set("NowServing", nowServing.ToString());
        }

        public ActionResult History()
        {
            int nowServing = NowServing();

            if (nowServing == 0)
            {
                ViewBag.Warning = "Please select a client from the Clients Table before viewing History.";
                return View("Warning");
            }

            ViewBag.ClientName = Clients.ClientBeingServed(nowServing);

            return View();
        }

        public JsonResult GetHistory(int page, int rows)
        {
            int nowServing = NowServing();

            ViewBag.ClientName = Clients.ClientBeingServed(nowServing);

            List<VisitViewModel> visits = Visits.GetVisits(nowServing);

            int pageIndex = page - 1;
            int pageSize = rows;
            int totalRecords = visits.Count;
            int totalPages = (int)Math.Ceiling((float)totalRecords / (float)rows);

            visits = visits.Skip(pageIndex * pageSize).Take(pageSize).ToList();
            visits = visits.OrderBy(v => v.Date).ToList();

            var jsonData = new
            {
                total = totalPages,
                page,
                records = totalRecords,
                rows = visits
            };

            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        public string AddVisit(VisitViewModel vvm)
        {
            int nowServing = NowServing();
            Visits.AddVisit(nowServing, vvm);

            return "Success";
        }

        public string EditVisit(VisitViewModel vvm)
        {
            int nowServing = NowServing();
            Visits.EditVisit(nowServing, vvm);

            return "Success";
        }

        public string DeleteVisit(int id)
        {
            int nowServing = NowServing();
            Visits.DeleteVisit(nowServing, id);

            return "Success";
        }
    }
}