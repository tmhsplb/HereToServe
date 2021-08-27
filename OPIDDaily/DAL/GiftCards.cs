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

                    gcivm.VisaCards20 = 0;
                    gcivm.VisaCards30 = 0;
                    gcivm.VisaCards40 = 0;
                    gcivm.VisaCards50 = 0;
                }
                else
                {
                    gcivm.METROCards20 = inventory.METROCards20;
                    gcivm.METROCards30 = inventory.METROCards30;
                    gcivm.METROCards40 = inventory.METROCards40;
                    gcivm.METROCards50 = inventory.METROCards50;

                    gcivm.VisaCards20 = inventory.VisaCards20;
                    gcivm.VisaCards30 = inventory.VisaCards30;
                    gcivm.VisaCards40 = inventory.VisaCards40;
                    gcivm.VisaCards50 = inventory.VisaCards50;
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

        public static string FulfillRequest(int nowServing, GiftCardInventoryViewModel gcivm)
        {
            using (OpidDailyDB opiddailycontext = new OpidDailyDB())
            {
                DateTime today = Extras.DateTimeToday().AddHours(12);
                Client client = opiddailycontext.Clients.Find(nowServing);
                GiftCard metroCard = new GiftCard { HolderId = client.Id, RegistrationDate = today, BalanceDate = today, CardBalance = 0 };
                GiftCard visaCard = new GiftCard { HolderId = client.Id, RegistrationDate = today, BalanceDate = today,  CardBalance = 0 };

                if (client != null)
                {
                    int agencyId = client.AgencyId;
                    Agency agency = opiddailycontext.Agencies.Where(a => a.AgencyId == agencyId).SingleOrDefault();

                    if (agency != null)
                    {
                        int metroBudget = agency.METROBudget, visaBudget = agency.VisaBudget;
                        int metroRequest = 0, visaRequest = 0;
                        bool metroBudgetExceeded = false, visaBudgetExceeded = false;
                        
                        if (!string.IsNullOrEmpty(gcivm.METROCard))
                        {
                            switch (gcivm.METROCard)
                            {
                                case "METROCard20":
                                    metroRequest = 20;
                                    metroCard.CardBalance = 20;
                                    break;

                                case "METROCard30":
                                    metroRequest = 30;
                                    metroCard.CardBalance = 30;
                                    break;

                                case "METROCard40":
                                    metroRequest = 40;
                                    metroCard.CardBalance = 40;
                                    break;

                                case "METROCard50":
                                    metroRequest = 50;
                                    metroCard.CardBalance = 50;
                                    break;
                            }
                        }

                        if (!string.IsNullOrEmpty(gcivm.VisaCard))
                        {
                            switch (gcivm.VisaCard)
                            {
                                case "VisaCard20":
                                    visaRequest = 20;
                                    visaCard.CardBalance = 20;
                                    break;

                                case "VisaCard30":
                                    visaRequest = 30;
                                    visaCard.CardBalance = 30;
                                    break;

                                case "VisaCard40":
                                    visaRequest = 40;
                                    visaCard.CardBalance = 40;
                                    break;

                                case "VisaCard50":
                                    visaRequest = 50;
                                    visaCard.CardBalance = 50;
                                    break;
                            }
                        }

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
                            // Update Visa Budget
                            agency.VisaBudget -= visaRequest;
                          
                            if (visaCard.CardBalance > 0)
                            {
                                opiddailycontext.Entry(client).Collection(c => c.GiftCards).Load();
                                client.GiftCards.Add(visaCard);
                            }

                            opiddailycontext.SaveChanges();

                            return "METROBudgetExceeded";
                        }
                        else if (visaBudgetExceeded)
                        {
                            // Update METRO Budget
                            agency.METROBudget -= metroRequest;

                            if (metroCard.CardBalance > 0)
                            {
                                opiddailycontext.Entry(client).Collection(c => c.GiftCards).Load();
                                client.GiftCards.Add(metroCard);
                            }
                            opiddailycontext.SaveChanges();

                            return "VisaBudgetExceeded";
                        }
                        else
                        {
                            // Update both budgets
                            agency.METROBudget -= metroRequest;
                            agency.VisaBudget -= visaRequest;
                            opiddailycontext.Entry(client).Collection(c => c.GiftCards).Load();
                            if (metroCard.CardBalance > 0)
                            {
                                client.GiftCards.Add(metroCard);
                            }
                            if (visaCard.CardBalance > 0)
                            {
                                client.GiftCards.Add(visaCard);
                            }
                            opiddailycontext.SaveChanges();

                            return "RequestFulfilled";
                        }
                    }
                }

                return "BadAgency";
            }
        }
    }
}