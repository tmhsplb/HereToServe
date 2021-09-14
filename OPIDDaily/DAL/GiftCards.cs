using OPIDDaily.DataContexts;
using OPIDDaily.Models;
using OPIDDaily.Utils;
using OpidDailyEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;

namespace OPIDDaily.DAL
{
    public class GiftCards
    {
        public enum GiftCardsEnum
        {
            METROCard20 = 120,
            METROCard30 = 130,
            METROCard40 = 140,
            METROCard50 = 150,
            METROCardNone = 0,

            VisaCard20 = 220,
            VisaCard30 = 230,
            VisaCard40 = 240,
            VisaCard50 = 250,
            VisaCardNone = 0
        }

        public static GiftCardInventoryViewModel GetInventory()
        {
            using (OpidDailyDB opiddailycontext = new OpidDailyDB())
            {
                GiftCardInventoryViewModel gcivm = new GiftCardInventoryViewModel();

                GiftCardInventory inventory = opiddailycontext.GiftCardInventory.OrderByDescending(i => i.InventoryDate).FirstOrDefault();

                if (inventory == null)
                {
                    gcivm.METROCards20 = 0;
                    gcivm.METROCards30 = 0;
                    gcivm.METROCards40 = 0;
                    gcivm.METROCards50 = 0;
                    gcivm.METROCardNone = 0;

                    gcivm.VisaCards20 = 0;
                    gcivm.VisaCards30 = 0;
                    gcivm.VisaCards40 = 0;
                    gcivm.VisaCards50 = 0;
                    gcivm.VisaCardNone = 0;
                }
                else
                {
                    gcivm.METROCards20 = inventory.METROCards20;
                    gcivm.METROCards30 = inventory.METROCards30;
                    gcivm.METROCards40 = inventory.METROCards40;
                    gcivm.METROCards50 = inventory.METROCards50;
                    gcivm.METROCardNone = 0;

                    gcivm.VisaCards20 = inventory.VisaCards20;
                    gcivm.VisaCards30 = inventory.VisaCards30;
                    gcivm.VisaCards40 = inventory.VisaCards40;
                    gcivm.VisaCards50 = inventory.VisaCards50;
                    gcivm.VisaCardNone = 0;
                }

                return UpdateFunds(gcivm);
            }
        }

        public static void SetBudgets(int agencyId, GiftCardInventoryViewModel gcivm)
        {
            using (OpidDailyDB opiddailycontext = new OpidDailyDB())
            {
                Agency agency = opiddailycontext.Agencies.Where(a => a.AgencyId == agencyId).SingleOrDefault();

                if (agency != null)
                {
                    gcivm.METROBudget = agency.METROBudget;
                    gcivm.VisaBudget = agency.VisaBudget;
                }
            }
        }

        private static GiftCardInventory GiftCardInventoryViewModelToGiftCardInventoryEntity(GiftCardInventoryViewModel gcivm)
        {
            DateTime now = Extras.DateTimeNow();
            GiftCardInventory gci = new GiftCardInventory();
            gci.InventoryDate = now;

            gci.METROCards20 = gcivm.METROCards20;
            gci.METROCards30 = gcivm.METROCards30;
            gci.METROCards40 = gcivm.METROCards40;
            gci.METROCards50 = gcivm.METROCards50;

            gci.VisaCards20 = gcivm.VisaCards20;
            gci.VisaCards30 = gcivm.VisaCards30;
            gci.VisaCards40 = gcivm.VisaCards40;
            gci.VisaCards50 = gcivm.VisaCards50;

            return gci;
        }

        public static GiftCardInventoryViewModel UpdateInventory(GiftCardInventoryViewModel gcivm)
        {
            using (OpidDailyDB opiddailycontext = new OpidDailyDB())
            {
                GiftCardInventory gci = GiftCardInventoryViewModelToGiftCardInventoryEntity(gcivm);
                opiddailycontext.GiftCardInventory.Add(gci);
                opiddailycontext.SaveChanges();
                return UpdateFunds(gcivm);
            }
        }

        public static GiftCardInventoryViewModel UpdateFunds(GiftCardInventoryViewModel gcivm)
        { 
            int METROFunds = 0;
            int VisaFunds = 0;

            METROFunds += 20 * gcivm.METROCards20;
            METROFunds += 30 * gcivm.METROCards30;
            METROFunds += 40 * gcivm.METROCards40;
            METROFunds += 50 * gcivm.METROCards50;

            gcivm.METROFunds = METROFunds;

            VisaFunds += 20 * gcivm.VisaCards20;
            VisaFunds += 30 * gcivm.VisaCards30;
            VisaFunds += 40 * gcivm.VisaCards40;
            VisaFunds += 50 * gcivm.VisaCards50;

            gcivm.VisaFunds = VisaFunds;

            return gcivm;
        }

        public static void UpdateBudgets(int agencyId, int metroBudget, int visaBudget)
        {
            using (OpidDailyDB opiddailycontext = new OpidDailyDB())
            {
                Agency agency = opiddailycontext.Agencies.Where(a => a.AgencyId == agencyId).SingleOrDefault();

                if (agency != null)
                {
                    agency.METROBudget = metroBudget;
                    agency.VisaBudget = visaBudget;
                    opiddailycontext.SaveChanges();
                    // Avoid a race conditon by allowing the
                    // commit to the database time to complete.
                    Thread.Sleep(200);
                }
            }
        }

        private static bool IsMETROCard(GiftCard gcard)
        {
            return (100 < gcard.GiftCardType && gcard.GiftCardType < 200);
        }

        private static bool IsVisaCard(GiftCard gcard)
        {
            return (200 < gcard.GiftCardType && gcard.GiftCardType < 300);
        }

        public static GiftCardInventoryViewModel SetRadioButtons(Agency agency, int id)
        {
            using (OpidDailyDB opiddailycontext = new OpidDailyDB())
            {
                GiftCardInventoryViewModel gcivm = GiftCards.GetInventory();
                gcivm.AgencyId = agency.AgencyId.ToString();
                gcivm.METROBudget = agency.METROBudget;
                gcivm.VisaBudget = agency.VisaBudget;
                gcivm.CurrentMETRORequest = 0;
                gcivm.CurrentVisaRequest = 0;

                Client client = opiddailycontext.Clients.Find(id);
                if (client != null)
                {
                    opiddailycontext.Entry(client).Collection(c => c.GiftCards).Load();
                    List<GiftCard> gcards = client.GiftCards.ToList();

                    gcivm.METROCard = gcivm.METROCardNone.ToString();
                    gcivm.CurrentMETRORequest = 0;
                    gcivm.VisaCard = gcivm.VisaCardNone.ToString();
                    gcivm.CurrentVisaRequest = 0;

                    foreach (GiftCard gcard in gcards)
                    {
                        if (gcard.IsActive && gcard.IsCurrent)
                        {
                            if (IsMETROCard(gcard))
                            {
                                gcivm.METROCard = gcard.GiftCardType.ToString();
                                gcivm.CurrentMETRORequest = gcard.CardBalance;
                            }

                            if (IsVisaCard(gcard))
                            {
                                gcivm.VisaCard = gcard.GiftCardType.ToString();
                                gcivm.CurrentVisaRequest = gcard.CardBalance;
                            }
                        }
                    }
                }
            
                return gcivm;
            }
        }

        private static GiftCard FulfillMetroRequest(DateTime today, GiftCardInventoryViewModel gcivm)
        {
            GiftCard metroCard = new GiftCard { RegistrationDate = today, BalanceDate = today, CardBalance = 0, IsActive = true };
            int mcard = Convert.ToInt32(gcivm.METROCard);

            switch (mcard)
            {
                case (int)GiftCardsEnum.METROCard20:
                    metroCard.GiftCardType = (int)GiftCardsEnum.METROCard20;
                    metroCard.CardBalance = 20;
                    break;

                case (int)GiftCardsEnum.METROCard30:
                    metroCard.GiftCardType = (int)GiftCardsEnum.METROCard30;
                    metroCard.CardBalance = 30;
                    break;

                case (int)GiftCardsEnum.METROCard40:
                    metroCard.GiftCardType = (int)GiftCardsEnum.METROCard40;
                    metroCard.CardBalance = 40;
                    break;

                case (int)GiftCardsEnum.METROCard50:
                    metroCard.GiftCardType = (int)GiftCardsEnum.METROCard50;
                    metroCard.CardBalance = 50;
                    break;

                case (int)GiftCardsEnum.METROCardNone:
                    metroCard.GiftCardType = (int)GiftCardsEnum.METROCardNone;
                    metroCard.CardBalance = 0;
                    break;
            }
 
            return metroCard;
        }

        private static GiftCard FulfillVisaRequest(DateTime today, GiftCardInventoryViewModel gcivm)
        {
            GiftCard visaCard = new GiftCard { RegistrationDate = today, BalanceDate = today, CardBalance = 0, IsActive = true };
            int vcard = Convert.ToInt32(gcivm.VisaCard);

            switch (vcard)
            {
                case (int)GiftCardsEnum.VisaCard20:
                    visaCard.GiftCardType = (int)GiftCardsEnum.VisaCard20;
                    visaCard.CardBalance = 20;
                    break;

                case (int)GiftCardsEnum.VisaCard30:
                    visaCard.GiftCardType = (int)GiftCardsEnum.VisaCard30;
                    visaCard.CardBalance = 30;
                    break;

                case (int)GiftCardsEnum.VisaCard40:
                    visaCard.GiftCardType = (int)GiftCardsEnum.VisaCard40;
                    visaCard.CardBalance = 40;
                    break;

                case (int)GiftCardsEnum.VisaCard50:
                    visaCard.GiftCardType = (int)GiftCardsEnum.VisaCard50;
                    visaCard.CardBalance = 50;
                    break;

                case (int)GiftCardsEnum.VisaCardNone:
                    visaCard.GiftCardType = (int)GiftCardsEnum.VisaCardNone;
                    visaCard.CardBalance = 0;
                    break;
            }

            return visaCard;
        }

        public static string FulfillRequest(int nowServing, GiftCardInventoryViewModel gcivm)
        {
            using (OpidDailyDB opiddailycontext = new OpidDailyDB())
            {
                DateTime today = Extras.DateTimeToday().AddHours(12);
                Client client = opiddailycontext.Clients.Find(nowServing);
                                
                if (client != null)
                {  
                    opiddailycontext.Entry(client).Collection(c => c.GiftCards).Load();
                    List<GiftCard> gcards = client.GiftCards.ToList();
                    GiftCard todaysMETROGiftCard = null, todaysVisaGiftCard = null;

                    foreach (GiftCard gcard in gcards)
                    {
                        if (gcard.IsCurrent && gcard.IsActive)
                        {
                            if (IsMETROCard(gcard))
                            {
                                todaysMETROGiftCard = gcard;
                            }

                            // Implict typing to determine Visa card
                            if (IsVisaCard(gcard))
                            {
                                todaysVisaGiftCard = gcard;
                            }
                        }
                    }

                    int agencyId = client.AgencyId;
                    Agency agency = opiddailycontext.Agencies.Where(a => a.AgencyId == agencyId).SingleOrDefault();

                    if (agency != null)
                    {
                        GiftCard metroCard = null, visaCard = null;
                        int metroBudget = agency.METROBudget, visaBudget = agency.VisaBudget;
                        int metroRequest, visaRequest;
                        bool metroBudgetExceeded = false, visaBudgetExceeded = false;
                        bool metroGiftCardRequested = true, visaGiftCardRequested = true;
                     
                        if (!gcivm.METROCard.Equals(gcivm.METROCardNone.ToString()))
                        {
                            metroCard = FulfillMetroRequest(today, gcivm);
                            metroRequest = metroCard.CardBalance;
                        }
                        else
                        {
                            // Radio button "No METRO Card" was selected. Deactivate a current METRO card (if any).
                            foreach (GiftCard gcard in gcards)
                            {
                                if (gcard.IsCurrent && IsMETROCard(gcard))
                                { 
                                    gcard.IsActive = false;
                                }
                            }

                            metroGiftCardRequested = false;
                            metroRequest = 0;
                        }

                        if (!gcivm.VisaCard.Equals(gcivm.VisaCardNone.ToString()))
                        {
                            visaCard = FulfillVisaRequest(today, gcivm);
                            visaRequest = visaCard.CardBalance;
                        }
                        else
                        {
                            // Radio button "No Visa Card" was selected. Deactivate a current Visa card (if any).
                            foreach (GiftCard gcard in gcards)
                            {
                                if (gcard.IsCurrent && IsVisaCard(gcard))
                                {
                                    gcard.IsActive = false;
                                }
                            }

                            visaGiftCardRequested = false;
                            visaRequest = 0;
                        }

                        // Add back the amount of the current request (possibly 0) to the Visa Budget
                        agency.VisaBudget += gcivm.CurrentVisaRequest;
                        // Deduct the amount of the new request (possibly 0) from the Visa Budget
                        agency.VisaBudget -= visaRequest;

                        // Add back the amount of the current request (possibly 0) to the METRO Budget
                        agency.METROBudget += gcivm.CurrentMETRORequest;
                        // Deducut the amount of the new request (possibly 0) from the METRO Budget
                        agency.METROBudget -= metroRequest;

                        if (metroRequest > metroBudget)
                        {
                            metroBudgetExceeded = true;
                        }

                        if (visaRequest > visaBudget)
                        {
                            visaBudgetExceeded = true;
                        }

                        if (metroBudgetExceeded && visaBudgetExceeded)
                        {
                            return "BothBudgetsExceeded";
                        }
                        else if (metroBudgetExceeded)
                        {
                            if (visaGiftCardRequested)
                            {
                                if (todaysVisaGiftCard != null)
                                {
                                    todaysVisaGiftCard.IsActive = true;
                                    todaysVisaGiftCard.CardBalance = visaCard.CardBalance;
                                    todaysVisaGiftCard.GiftCardType = visaCard.GiftCardType;
                                }
                                else
                                {
                                    // There is a new current Visa card. Unmark as current a previous one (if any).
                                    foreach (GiftCard gcard in gcards)
                                    {
                                        if (gcard.IsCurrent && IsVisaCard(gcard))
                                        {
                                            gcard.IsCurrent = false;
                                        }
                                    }

                                    visaCard.IsCurrent = true;
                                    visaCard.IsActive = true;
                                    client.GiftCards.Add(visaCard);
                                }

                                opiddailycontext.SaveChanges();
                            }

                            return "METROBudgetExceeded";
                        }
                        else if (visaBudgetExceeded)
                        {
                            if (metroGiftCardRequested)
                            {
                                if (todaysMETROGiftCard != null)
                                {
                                    todaysMETROGiftCard.IsActive = true;
                                    todaysMETROGiftCard.CardBalance = metroCard.CardBalance;
                                    todaysMETROGiftCard.GiftCardType = metroCard.GiftCardType;
                                }
                                else
                                {
                                    // There is a new current METRO card. Unmark as current a previous one (if any).
                                    foreach (GiftCard gcard in gcards)
                                    {
                                        if (gcard.IsCurrent && IsMETROCard(gcard))
                                        { 
                                            gcard.IsCurrent = false;
                                        }
                                    }

                                    metroCard.IsCurrent = true;
                                    metroCard.IsActive = true;
                                    client.GiftCards.Add(metroCard);
                                }

                                opiddailycontext.SaveChanges();
                            }

                            return "VisaBudgetExceeded";
                        }
                        else
                        {
                            if (visaGiftCardRequested)
                            {
                                if (todaysVisaGiftCard != null)
                                {
                                    todaysVisaGiftCard.IsActive = true;
                                    todaysVisaGiftCard.CardBalance = visaCard.CardBalance;
                                    todaysVisaGiftCard.GiftCardType = visaCard.GiftCardType;
                                }
                                else
                                {
                                    // There is a new current Visa card. Unmark as current a previous one (if any).
                                    foreach (GiftCard gcard in gcards)
                                    {
                                        if (gcard.IsCurrent && IsVisaCard(gcard))
                                        {
                                            gcard.IsCurrent = false;
                                        }
                                    }

                                    visaCard.IsCurrent = true;
                                    visaCard.IsActive = true;
                                    client.GiftCards.Add(visaCard);
                                }
                            }

                            if (metroGiftCardRequested)
                            {
                                if (todaysMETROGiftCard != null)
                                {
                                    todaysMETROGiftCard.IsActive = true;
                                    todaysMETROGiftCard.CardBalance = metroCard.CardBalance;
                                    todaysMETROGiftCard.GiftCardType = metroCard.GiftCardType;
                                }
                                else
                                {
                                    // There is a new current METRO card. Unmark as current a previous one (if any).
                                    foreach (GiftCard gcard in gcards)
                                    {
                                        // Implicit type check for METRO cards
                                        if (gcard.IsCurrent && IsMETROCard(gcard))
                                        {
                                            gcard.IsCurrent = false;
                                        }
                                    }

                                    metroCard.IsCurrent = true;
                                    metroCard.IsActive = true;
                                    client.GiftCards.Add(metroCard);
                                }
                            }

                            opiddailycontext.SaveChanges();

                            return "RequestFulfilled";
                        }
                    }
                }

                return "BadAgency";
            }
        }

        public static bool RequestingMETROGiftCard(int id, RequestedServicesViewModel rsvm)
        {
            using (OpidDailyDB opiddailycontext = new OpidDailyDB())
            {
                Client client = opiddailycontext.Clients.Find(id);
             
                if (client != null)
                {
                    opiddailycontext.Entry(client).Collection(c => c.GiftCards).Load();
                    List<GiftCard> gcards = client.GiftCards.ToList();

                    foreach (GiftCard gcard in gcards)
                    {
                        if (gcard.IsActive && gcard.IsCurrent && IsMETROCard(gcard))
                        {
                            rsvm.METROGiftCardAmount = gcard.CardBalance;
                            return true;
                        }
                    }
                }

                return false;
            }
        }

        public static bool RequestingVisaGiftCard(int id, RequestedServicesViewModel rsvm)
        {
            using (OpidDailyDB opiddailycontext = new OpidDailyDB())
            {
                Client client = opiddailycontext.Clients.Find(id);
           
                if (client != null)
                {
                    opiddailycontext.Entry(client).Collection(c => c.GiftCards).Load();
                    List<GiftCard> gcards = client.GiftCards.ToList();

                    foreach (GiftCard gcard in gcards)
                    {
                        if (gcard.IsActive && gcard.IsCurrent && IsVisaCard(gcard))
                        {
                            rsvm.VisaGiftCardAmount = gcard.CardBalance;
                            return true;
                        }
                    }
                }

                return false;
            }
        }

        // A pocket check representing a gift card has been edited. If the status has been changed
        // to "Gift Card Delivered" and the card was marked as current, then
        // change the marking to not current (gcard.IsCurrent = false) based on the cardType.
        public static void DeliverGiftCard(int nowServing, VisitViewModel vvm)
        {
            string cardType = vvm.Item.Trim();
            string status = vvm.Status;
            
            using (OpidDailyDB opiddailycontext = new OpidDailyDB())
            {
                Client client = opiddailycontext.Clients.Find(nowServing);

                if (client != null && !string.IsNullOrEmpty(status) && status.Equals("Gift Card Delivered"))
                {
                    PocketCheck pcheck = opiddailycontext.PocketChecks.Find(vvm.Id);
                    if (pcheck != null)
                    {
                        pcheck.Disposition = status;
                    }

                    opiddailycontext.Entry(client).Collection(c => c.GiftCards).Load();
                    List<GiftCard> gcards = client.GiftCards.ToList();

                    foreach (GiftCard gcard in gcards)
                    {
                        if (gcard.IsCurrent && cardType.Equals("METRO Gift Card") && IsMETROCard(gcard))
                        {
                            gcard.IsCurrent = false;
                        }

                        if (gcard.IsCurrent && cardType.Equals("VISA Gift Card") && IsVisaCard(gcard))
                        {
                            gcard.IsCurrent = false;    
                        }
                    }

                    opiddailycontext.SaveChanges();
                }
            }
        }

        private static string GiftCardLabel(GiftCard gcard)
        {
            int amount = 0;
            string suffix = " Card";

            if (IsMETROCard(gcard))
            {
                amount = gcard.GiftCardType % 100;
                suffix = "METRO card";
            }


            if (IsVisaCard(gcard))
            {
                amount = gcard.GiftCardType % 200;
                suffix = "VISA card";
            }

            return string.Format("${0} {1}:", amount, suffix);
        }

        private static GiftCardViewModel GiftCardEntityToGiftCardViewModel(GiftCard gcard)
        {
            return new GiftCardViewModel
            {
                Id = gcard.Id,
                Label = GiftCardLabel(gcard),
                GiftCardType = gcard.GiftCardType,
                RegistrationDate = gcard.RegistrationDate,
                RegistrationID = gcard.RegistrationID,
                BalanceDate = gcard.BalanceDate,
                CardBalance = gcard.CardBalance
            };
        }

        public static GiftCardViewModel GetCurrentMETRORequest(int nowServing)
        {
            using (OpidDailyDB opiddailycontext = new OpidDailyDB())
            {
                Client client = opiddailycontext.Clients.Find(nowServing);

                if (client != null)
                {
                    opiddailycontext.Entry(client).Collection(c => c.GiftCards).Load();
                    List<GiftCard> gcards = client.GiftCards.ToList();

                    foreach (GiftCard gcard in gcards)
                    {
                        if (gcard.IsActive && gcard.IsCurrent && IsMETROCard(gcard))
                        {
                            return GiftCardEntityToGiftCardViewModel(gcard);
                        }
                    }
                }

                return null;
            }
        }

        public static GiftCardViewModel GetCurrentVisaRequest(int nowServing)
        {
            using (OpidDailyDB opiddailycontext = new OpidDailyDB())
            {
                Client client = opiddailycontext.Clients.Find(nowServing);

                if (client != null)
                {
                    opiddailycontext.Entry(client).Collection(c => c.GiftCards).Load();
                    List<GiftCard> gcards = client.GiftCards.ToList();

                    foreach (GiftCard gcard in gcards)
                    {
                        if (gcard.IsActive && gcard.IsCurrent && IsVisaCard(gcard))
                        {
                            return GiftCardEntityToGiftCardViewModel(gcard);
                        }
                    }
                }

                return null;
            }
        }

        public static string GetCurrentMETRORegistrationId(int nowServing)
        {
            using (OpidDailyDB opiddailycontext = new OpidDailyDB())
            {
                Client client = opiddailycontext.Clients.Find(nowServing);

                if (client != null)
                {
                    opiddailycontext.Entry(client).Collection(c => c.GiftCards).Load();
                    List<GiftCard> gcards = client.GiftCards.ToList();

                    foreach (GiftCard gcard in gcards)
                    {
                        if (gcard.IsActive && gcard.IsCurrent && IsMETROCard(gcard))
                        {
                            return gcard.RegistrationID;
                        }
                    }
                }
            }

            return string.Empty;
        }

        public static void SetCurrentMETRORegistrationId(int nowServing, string metroRegistrationId)
        {
            using (OpidDailyDB opiddailycontext = new OpidDailyDB())
            {
                Client client = opiddailycontext.Clients.Find(nowServing);

                if (client != null)
                {
                    opiddailycontext.Entry(client).Collection(c => c.GiftCards).Load();
                    List<GiftCard> gcards = client.GiftCards.ToList();

                    foreach (GiftCard gcard in gcards)
                    {
                        if (gcard.IsActive && gcard.IsCurrent && IsMETROCard(gcard))
                        {
                            gcard.RegistrationID = metroRegistrationId;
                            opiddailycontext.SaveChanges();
                            // Avoid a race condition
                            Thread.Sleep(200);
                            return;
                        }
                    }
                }
            }
        }

        public static string GetCurrentVisaRegistrationId(int nowServing)
        {
            using (OpidDailyDB opiddailycontext = new OpidDailyDB())
            {
                Client client = opiddailycontext.Clients.Find(nowServing);

                if (client != null)
                {
                    opiddailycontext.Entry(client).Collection(c => c.GiftCards).Load();
                    List<GiftCard> gcards = client.GiftCards.ToList();

                    foreach (GiftCard gcard in gcards)
                    {
                        if (gcard.IsActive && gcard.IsCurrent && IsVisaCard(gcard))
                        {
                            return gcard.RegistrationID;  
                        }
                    }
                }
            }

            return string.Empty;
        }

        private static PocketCheck NewGiftCardPocketCheck(Client client, string item, string amount, string registrationId)
        {
            DateTime today = Extras.DateTimeToday().AddHours(12);

            return new PocketCheck
            {
                ClientId = client.Id,
                HH = (client.HHId == null ? 0 : (int)client.HHId),
                Date = today,
                Name = Clients.ClientBeingServed(client, false),
                DOB = client.DOB,
                Item = string.Format(" {0}", item),
                Num = Convert.ToInt32(amount),
                Disposition = " Gift Card",
                Notes = string.Format(" {0}", registrationId),
                IsActive = true
            };
        }

        private static void AddPocketCheck(int nowServing, string item, string amount, string registrationId)
        {
            using (OpidDailyDB opiddailycontext = new OpidDailyDB())
            {
                Client client = opiddailycontext.Clients.Find(nowServing);

                if (client != null)
                {
                    // Check for existing pocket check using this registrationId
                    PocketCheck pc = opiddailycontext.PocketChecks.Where(p => p.Notes.Trim().Equals(registrationId)).SingleOrDefault();

                    if (pc == null)
                    {
                        opiddailycontext.PocketChecks.Add(NewGiftCardPocketCheck(client, item, amount, registrationId));
                        opiddailycontext.SaveChanges();
                    }
                }
            }
        }

        public static void UpdatePocketChecks(int nowServing, string item, string amount, string registrationId)
        {
            using (OpidDailyDB opiddailycontext = new OpidDailyDB())
            {
                Client client = opiddailycontext.Clients.Find(nowServing);

                if (client != null)
                {
                    // Check for existing pocket check using this registrationId
                    PocketCheck pc = opiddailycontext.PocketChecks.Where(p => p.Notes.Trim() == registrationId).SingleOrDefault();

                    if (pc == null)
                    {
                        int num = Convert.ToInt32(amount);
                        pc = opiddailycontext.PocketChecks.Where(p => p.Item.Trim() == item && p.Num == num).SingleOrDefault();

                        if (pc != null)
                        {
                            // We are just changing the registrationId of an existing pocket check
                            pc.Notes = string.Format(" {0}", registrationId);
                            opiddailycontext.SaveChanges();
                            return;
                        }

                        // There is no pocket check with this registrationId.
                        // Add a new pocket check.
                        AddPocketCheck(nowServing, item, amount, registrationId);
                    }
                }
            }
        }

        public static void SetCurrentVisaRegistrationId(int nowServing, string visaRegistrationId)
        {
            using (OpidDailyDB opiddailycontext = new OpidDailyDB())
            {
                Client client = opiddailycontext.Clients.Find(nowServing);

                if (client != null)
                {
                    opiddailycontext.Entry(client).Collection(c => c.GiftCards).Load();
                    List<GiftCard> gcards = client.GiftCards.ToList();

                    foreach (GiftCard gcard in gcards)
                    {
                        if (gcard.IsActive && gcard.IsCurrent && IsVisaCard(gcard))
                        {
                            gcard.RegistrationID = visaRegistrationId;
                            opiddailycontext.SaveChanges();
                            // Avoid a race condition
                            Thread.Sleep(200);
                            return;
                        }
                    }
                }
            }
        }
    }
}