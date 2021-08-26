using OPIDDaily.DataContexts;
using OPIDDaily.Models;
using OPIDDaily.Utils;
using OpidDailyEntities;
using System;
using System.Collections.Generic;
using System.Linq;
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
                }
            }
        }
    }
}