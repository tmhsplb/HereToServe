using OPIDDaily.DAL;
using OPIDDaily.Models;
using OpidDailyEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OPIDDaily.Controllers
{
    [Authorize(Roles = "GiftCardAdmin")]
    public class GiftCardsController : SharedController
    {
        public ActionResult Home()
        {
            // Log.Debug("Return view Home");
            return View("Home");
        }

        public ActionResult ManageInventory()
        {
            GiftCardInventoryViewModel gcivm = GiftCards.GetInventory();
            int selectedAgency = Convert.ToInt32(SessionHelper.Get("SelectedAgency"));
            gcivm.Agencies = Agencies.GetAgenciesSelectList(selectedAgency);

            if (selectedAgency != 0)
            {
                GiftCards.SetBudgets(selectedAgency, gcivm);
            }

            return View("GiftCardsInventory", gcivm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateInventory(GiftCardInventoryViewModel gcivm)
        {
            GiftCards.UpdateInventory(gcivm);
            return RedirectToAction("ManageInventory");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult HandleBudgetRequest(string retrieveBudget, string updateBudget, GiftCardInventoryViewModel gcivm)
        {
            int agencyId = Convert.ToInt32(gcivm.AgencyId);

            if (agencyId == 0)
            {
                ModelState.AddModelError("BudgetError", "Please select an agency other than OpID");
                gcivm = GiftCards.GetInventory();
                gcivm.Agencies = Agencies.GetAgenciesSelectList(0);
                return View("GiftCardsInventory", gcivm);
            }

            if (!string.IsNullOrEmpty(retrieveBudget) && retrieveBudget.Equals("Retrieve Budget"))
            {
                SessionHelper.Set("SelectedAgency", agencyId.ToString());
                return RedirectToAction("ManageInventory");
            }
            else if (!string.IsNullOrEmpty(updateBudget) && updateBudget.Equals("Update Budget"))
            {
                SessionHelper.Set("SelectedAgency", agencyId.ToString());
                GiftCards.UpdateBudgets(agencyId, gcivm.METROBudget, gcivm.VisaBudget);
                return RedirectToAction("ManageInventory");
            }

            ModelState.AddModelError("BudgetError", "Unknown budget command");
            gcivm = GiftCards.GetInventory();
            gcivm.Agencies = Agencies.GetAgenciesSelectList(0);
            return View("GiftCardsInventory", gcivm);
        }

        public ActionResult GiftCardsServiceTicket()
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
                return RedirectToAction("ExistingClientServiceTicket");
            }

            return RedirectToAction("ExpressClientServiceTicket");
        }

        public ActionResult AssignCards()
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

            return RedirectToAction("AssignRequestedCards");
        }

        public ActionResult AssignRequestedCards()
        {
            int nowServing = NowServing();
             
            if (nowServing != 0)
            {
                GiftCardViewModel metroGiftCard = GiftCards.GetCurrentMETRORequest(nowServing);
                GiftCardViewModel visaGiftCard = GiftCards.GetCurrentVisaRequest(nowServing);

                if (metroGiftCard == null && visaGiftCard == null)
                {
                    ViewBag.Warning = "No gift cards requested";
                    return View("Warning");
                }

                var objTuple = new Tuple<GiftCardViewModel, GiftCardViewModel>(metroGiftCard, visaGiftCard);
                return View("AssignRequestedCards", objTuple);
            }

            ViewBag.Warning = "Could not find referenced client";
            return View("Warning");
        }

        [HttpPost]
        public ActionResult AssignRequestedCards(string metroRegistrationId, string metroAmount, string visaRegistrationId, string visaAmount)
        {
            int nowServing = NowServing();
             
            if (!string.IsNullOrEmpty(metroRegistrationId))
            {
                GiftCards.SetCurrentMETRORegistrationId(nowServing, metroRegistrationId);
                GiftCards.UpdatePocketChecks(nowServing, "METRO Gift Card", metroAmount, metroRegistrationId);
            }

            if (!string.IsNullOrEmpty(visaRegistrationId))
            {
                GiftCards.SetCurrentVisaRegistrationId(nowServing, visaRegistrationId);
                GiftCards.UpdatePocketChecks(nowServing, "VISA Gift Card", visaAmount, visaRegistrationId);
            }

            return RedirectToAction("GiftCardsServiceTicket");
        }

        public string EditGiftCardPocketCheck(VisitViewModel vvm)
        {
            int nowServing = NowServing();
            GiftCards.DeliverGiftCard(nowServing, vvm);
            DailyHub.Refresh();
            return "Success";
        }

        public ActionResult ChangePassword()
        {
            return View("ChangePassword");
        }
    }
}