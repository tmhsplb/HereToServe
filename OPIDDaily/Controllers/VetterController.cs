using DataTables.Mvc;
using OPIDDaily.DAL;
using OPIDDaily.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Web;
using System.Web.Mvc;
using OPIDDaily.Utils;
using OPIDDaily.DataContexts;
using OpidDailyEntities;
using log4net;

namespace OPIDDaily.Controllers
{
    [Authorize(Roles = "Vetter")]
    public class VetterController : SharedController
    {
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(typeof(BackOfficeController));
       
        public ActionResult Home()
        {
            // Log.Debug("Return view Home");
            return View("Home");
        }


        public ActionResult VetServiceRequest()
        {
            int nowServing = NowServing();

            if (nowServing == 0)
            {
                ViewBag.Warning = "Please first select a client from the Dashboard.";
                return View("Warning");
            }

            Client client = Clients.GetClient(nowServing, null);

            if (client == null)
            {
                ViewBag.Warning = "Could not find selected client.";
                return View("Warning");
            }


            if (CheckManager.HasHistory(client.Id))
            {
                return RedirectToAction("ExistingClientServiceRequest");
            }

            return RedirectToAction("ExpressClientServiceRequest");
        }

        [HttpPost]
        public ActionResult StoreDemographicInfo(DemographicInfoViewModel civm)
        {
            int nowServing = NowServing();
            Clients.StoreDemographicInfo(nowServing, civm);
            return RedirectToAction("ManageDashboard");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult StoreExpressClientServiceRequest(RequestedServicesViewModel rsvm)
        {
            // Called when 
            //   ~/Views/BackOffice/ExpressClientServiceRequest.cshtml
            // posts to server. rsvm will contain both requested services
            // and supporting documents.
            int nowServing = NowServing();

            if (nowServing == 0)
            {
                ViewBag.Warning = "You waited too long before acting! Session timeout. Go back to Dashboard and try again.";
                return View("Warning");
            }

            Client client = Clients.GetClient(nowServing, null);  // pass null so the supporting documents won't be erased
            string serviceRequestError = ServiceRequestError(rsvm);

            if (!string.IsNullOrEmpty(serviceRequestError))
            {
                ViewBag.ClientName = Clients.ClientBeingServed(client);
                ViewBag.DOB = client.DOB.ToString("MM/dd/yyyy");
                ViewBag.Age = client.Age;
                ModelState.AddModelError("ServiceRequestError", serviceRequestError);
                rsvm.Agencies = Agencies.GetAgenciesSelectList(client.AgencyId);
                rsvm.MBVDS = MBVDS.GetMBVDSelectList();
                return View("ExpressClientServiceRequest", rsvm);
            }

            Clients.StoreRequestedServicesAndSupportingDocuments(client.Id, rsvm);
            PrepareClientNotes(client, rsvm);
            return RedirectToAction("ManageDashboard", "Vetter");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult StoreExistingClientServiceRequest(RequestedServicesViewModel rsvm)
        {
            // Called when          
            //    ~/Views/BackOffice/ExistingClientServiceRequest.cshtml
            // posts to server. rsvm will contain both requested services
            // and supporting documents.
            int nowServing = NowServing();

            if (nowServing == 0)
            {
                ViewBag.Warning = "You waited too long before acting! Session timeout. Go back to Clients list and try again.";
                return View("Warning");
            }

            Client client = Clients.GetClient(nowServing, null);  // pass null so the supporting documents won't be erased
            string serviceRequestError = ServiceRequestError(rsvm);

            if (!string.IsNullOrEmpty(serviceRequestError))
            {
                ViewBag.ClientName = Clients.ClientBeingServed(client);
                ViewBag.DOB = client.DOB.ToString("MM/dd/yyyy");
                ViewBag.Age = client.Age;
                ModelState.AddModelError("ServiceRequestError", serviceRequestError);
                rsvm.Agencies = Agencies.GetAgenciesSelectList(client.AgencyId);
                rsvm.MBVDS = MBVDS.GetMBVDSelectList();
                return View("ExistingClientServiceRequest", rsvm);
            }

            Clients.StoreRequestedServicesAndSupportingDocuments(client.Id, rsvm);
            PrepareClientNotes(client, rsvm);
            return RedirectToAction("ManageDashboard", "Vetter");
        }
    
        public ActionResult ChangePassword()
        {
            return View("ChangePassword");
        }
    }
}