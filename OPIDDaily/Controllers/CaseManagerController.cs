using OPIDDaily.DAL;
using OPIDDaily.Models;
using OPIDDaily.Utils;
using OpidDailyEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Web;
using System.Web.Mvc;
 

namespace OPIDDaily.Controllers
{
    [Authorize(Roles = "CaseManager")]
    public class CaseManagerController : SharedController
    {
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(typeof(CaseManagerController));

        public ActionResult Home()
        {
            // VoucherBackButtonHelper("Reset", null);
            return View();
        }

        public ActionResult ManageMyClients()
        {
            DateTime today = Extras.DateTimeNoonToday();
            int msgCnt = Convert.ToInt32(SessionHelper.Get("MsgCnt"));

            ViewBag.ServiceDate = today.ToString("ddd  MMM d");
            ViewBag.MsgCnt = msgCnt;

            return View("Clients");
        }

        public JsonResult GetMyClients(int page, int? rows = 15)
        {
            int referringAgency = ReferringAgency();
            List<ClientViewModel> clients = Clients.GetMyClients(referringAgency);

            // VoucherBackButtonHelper("Reset", null);

            int pageIndex = page - 1;
            int pageSize = (int)rows;

            int totalRecords = clients.Count;
            int totalPages = (int)Math.Ceiling((float)totalRecords / (float)pageSize);

            clients = clients.Skip(pageIndex * pageSize).Take(pageSize).ToList();

            var jsonData = new
            {
                total = totalPages,
                page = page,
                records = totalRecords,
                rows = clients
            };

            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        public string AddMyClient(ClientViewModel cvm)
        {
            cvm.ReferringAgentId = ReferringAgentId();
            int referringAgency = ReferringAgency();
            int id = Clients.AddMyClient(cvm, referringAgency);

            if (id == -1)
            {
                return "Failure";
            }

            // Newly added client becomes the client being served.
            // Entity Framework will set client.Id to the Id of the inserted client.
            // See: https://stackoverflow.com/questions/5212751/how-can-i-get-id-of-inserted-entity-in-entity-framework
            NowServing(id);

            DailyHub.Refresh();
            return "Success";
        }

        public string EditMyClient(ClientViewModel cvm)
        {
            cvm.ReferringAgentId = ReferringAgentId();
            int id = Clients.EditMyClient(cvm);

            // Edited client becomes the client being served.
            NowServing(id);

            DailyHub.Refresh();

            if (!string.IsNullOrEmpty(cvm.Conversation) && cvm.Conversation.Equals("Y"))
            {
                return "OpenConversation";
            }

            return "Success";
        }

        public string DeleteMyClient(int id)
        {
            string trainingClients = Config.TrainingClients;

            if (!string.IsNullOrEmpty(trainingClients) && trainingClients.Contains(id.ToString()))
            {
                return "Failure";
            }

            Clients.DeleteClient(id);
            DailyHub.Refresh();
            return "Success";
        }

        [HttpPost]
        public ActionResult StoreDemographicInfo(DemographicInfoViewModel civm)
        {
            int nowServing = NowServing();
            Clients.StoreDemographicInfo(nowServing, civm);
            return RedirectToAction("ManageClients");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult StoreExpressClientServiceRequest(RequestedServicesViewModel rsvm)
        {
            // Called when 
            //   ~/Views/CaseManager/ExpressClientServiceRequest.cshtml
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
                rsvm.MBVDS = MBVDS.GetMBVDSelectList();
                return View("ExpressClientServiceRequest", rsvm);
            }

            Clients.StoreRequestedServicesAndSupportingDocuments(client.Id, rsvm);
            PrepareClientNotes(client, rsvm);
            return RedirectToAction("ManageMyClients", "CaseManager");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult StoreExistingClientServiceRequest(RequestedServicesViewModel rsvm)
        {
            // Called when          
            //    ~/Views/CaseManager/ExistingClientServiceRequest.cshtml
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
                ModelState.AddModelError("ServiceRequesstError", serviceRequestError);
                return View("ExistingClientServiceRequest", rsvm);
            }

            Clients.StoreRequestedServicesAndSupportingDocuments(client.Id, rsvm);
            PrepareClientNotes(client, rsvm);
            return RedirectToAction("ManageMyClients", "CaseManager");
        }

        public ActionResult GiftCardsRequest()
        {
            int nowServing = NowServing();

            if (nowServing == 0)
            {
                ViewBag.Warning = "Please first select a client from the Clients Table.";
                return View("Warning");
            }

            Client client = Clients.GetClient(nowServing, null);

            if (client == null)
            {
                ViewBag.Warning = "Could not find selected client.";
                return View("Warning");
            }

            if (client.LCK)
            {
                ViewBag.Warning = "Operation ID has currently locked gift card requests for this client.";
                return View("Warning");
            }

            int thisAgency = ReferringAgency();
            Agency agency = Agencies.GetAgency(thisAgency);

            if (agency != null && !agency.AllowGiftCards)
            {
                ViewBag.Warning = "Operation ID has currently disallowed gift card requests from your agency.";
                return View("Warning");
            }

            return RedirectToAction("FulfillGiftCardsRequest");
        }

        public ActionResult FulfillGiftCardsRequest()
        {
            int nowServing = NowServing();
            Client client = Clients.GetClient(nowServing, null);

            if (client != null)
            {
                Agency agency = Agencies.GetAgency(client.AgencyId);

                if (agency != null)
                {
                    GiftCardInventoryViewModel gcivm = GiftCards.SetRadioButtons(agency, client.Id);
                    ViewBag.ClientName = Clients.ClientBeingServed(client, false);
                    return View("GiftCardsRequest", gcivm);
                }

                ViewBag.Warning = string.Format("Could not find agency {0}", agency.AgencyName);
                return View("Warning");
            }

            ViewBag.Warning = "Could not find referenced client";
            return View("Warning");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult FulfillGiftCardsRequest(GiftCardInventoryViewModel gcivm)
        {
            int nowServing = NowServing();
            string fulfillableRequest = GiftCards.FulfillRequest(nowServing, gcivm);

            if (fulfillableRequest.Equals("BadAgency"))
            {
                ModelState.AddModelError("ExceedsMETROBudgetError", "Could not find agency.");
                return View("GiftCardsRequest", gcivm);
            }

            if (fulfillableRequest.Equals("METROBudgetExceeded"))
            {
                ModelState.AddModelError("ExceedsMETROBudgetError", "METRO gift card request exceeds budget.");
                return View("GiftCardsRequest", gcivm);
            }

            if (fulfillableRequest.Equals("VisaBudgetExceeded"))
            {
                ModelState.AddModelError("ExceedsVisaBudgetError", "Visa gift card request exceeds budget.");
                return View("GiftCardsRequest", gcivm);
            }

            if (fulfillableRequest.Equals("BothBudgetsExceeded"))
            {
                ModelState.AddModelError("ExceedsMETROBudgetError", "METRO gift card request exceeds budget.");
                ModelState.AddModelError("ExceedsVisaBudgetError", "Visa gift card request exceeds budget.");
                return View("GiftCardsRequest", gcivm);
            }

            return RedirectToAction("PrepareCaseManagerServiceTicket");
        }

        public ActionResult PrepareCaseManagerServiceTicket()
        {
            int nowServing = NowServing();

            if (nowServing == 0)
            {
                ViewBag.Warning = "Please first select a client from the Clients Table.";
                return View("Warning");
            }

            RequestedServicesViewModel rsvm = new RequestedServicesViewModel();
            Client client = Clients.GetClient(nowServing, rsvm);

            if (client.HHId != null)
            {
                ViewBag.Warning = "Cannot prepare a Service Ticket for a dependent of another client.";
                return View("Warning");
            }

            PrepareClientNotes(client, rsvm);

            DateTime today = Extras.DateTimeNoonToday();
            ViewBag.VoucherDate = today.ToString("MM/dd/yyyy");
            ViewBag.Expiry = client.Expiry.ToString("ddd MMM d, yyyy");

            ViewBag.ClientName = Clients.ClientBeingServed(client, false);
            ViewBag.BirthName = client.BirthName;


            // ViewBag.CurrentAddress = Clients.ClientAddress(client);
            ViewBag.Phone = (!string.IsNullOrEmpty(client.Phone) ? client.Phone : "N/A");
            ViewBag.Email = (!string.IsNullOrEmpty(client.Email) ? client.Email : "N/A");

            ViewBag.DOB = client.DOB.ToString("MM/dd/yyyy");
            // ViewBag.BirthPlace = Clients.GetBirthplace(client);
            ViewBag.Age = client.Age;
            ViewBag.Agency = Agencies.GetAgencyName(client.AgencyId);  // rsvm.Agency will be the Id of an Agency as a string

            List<ClientViewModel> dependents = Clients.GetDependents(nowServing);
            
            var objTuple = new Tuple<List<ClientViewModel>, RequestedServicesViewModel>(dependents, rsvm);

            //  VoucherBackButtonHelper("Set", rsvm);
            // return View("PrintVoucher", objTuple);
            return View("PrintServiceTicket", objTuple);
        }

        public ActionResult ChangePassword()
        {
            return View("ChangePassword");
        }
    }
}