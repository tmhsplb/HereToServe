﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using OPIDDaily.Models;
using OPIDDaily.DataContexts;
using OpidDailyEntities;

namespace OPIDDaily.DAL
{
    public class PocketChecks
    {
        private static PocketCheckViewModel PocketCheckToPocketCheckViewModel(Client client, PocketCheck pc)
        {
            return new PocketCheckViewModel
            {
                Id = pc.Id,
                AgencyName = (!string.IsNullOrEmpty(client.AgencyName) ? client.AgencyName : Agencies.GetAgencyName(client.AgencyId)),
                Name = pc.Name,
                HeadOfHousehold = (pc.HeadOfHousehold ? "Y" : string.Empty),
                Date = pc.Date,
                Item = pc.Item,
                Check = pc.Num,
                Status = pc.Disposition,
                Notes = pc.Notes
            };
        }

        private static bool IsPocketCheck(PocketCheck pcheck)
        {
            if (pcheck.Item.Trim().StartsWith("METRO") || (pcheck.Item.Trim().StartsWith("VISA")))
            {
                // Gift Cards are implemented as pocket checks but should not
                // appear in the Pocket Checks Report.
                return false;
            }

            // Does not depend on Disposition, which might not be determined yet.
            // It's still a pocket check based on check number alone.
            // See CheckManager.PocketCheck which DOES depend on disposition.

            return 0 < pcheck.Num && pcheck.Num < 9999;
        }

        
        private static List<PocketCheckViewModel> GetFilteredPocketChecks(SearchParameters sps, List<PocketCheckViewModel> pocketChecks)
        {
            List<PocketCheckViewModel> filteredPocketChecks;

            if (!string.IsNullOrEmpty(sps.AgencyName))
            {
                filteredPocketChecks = pocketChecks.Where(pc => pc.AgencyName != null && pc.AgencyName.ToUpper().StartsWith(sps.AgencyName.ToUpper())).ToList();
            }
            else if (!string.IsNullOrEmpty(sps.Name))
            {
                filteredPocketChecks = pocketChecks.Where(pc => pc.Name != null && pc.Name.ToUpper().StartsWith(sps.Name.ToUpper())).ToList();
            }
            else if (!string.IsNullOrEmpty(sps.Check))
            {
                filteredPocketChecks = pocketChecks.Where(pc => pc.Check != 0 && Convert.ToString(pc.Check).Equals(sps.Check)).ToList();
            }
            else
            {
                filteredPocketChecks = pocketChecks;
            }

            return filteredPocketChecks;
        }

        public static List<PocketCheckViewModel> GetPocketChecks(SearchParameters sps)
        {
            using (OpidDailyDB opiddailycontext = new OpidDailyDB())
            {
                List<PocketCheck> pchecks = opiddailycontext.PocketChecks.Where(pc => pc.HH == 0 && pc.IsActive == true).ToList();
                List<PocketCheckViewModel> pocketChecks = new List<PocketCheckViewModel>();

                foreach (PocketCheck pcheck in pchecks)
                {
                    if (IsPocketCheck(pcheck))
                    {
                        Client client = opiddailycontext.Clients.Find(pcheck.ClientId);

                        if (client != null)
                        {
                            pocketChecks.Add(PocketCheckToPocketCheckViewModel(client, pcheck));
                        }
                    }
                }

                if (sps != null && sps._search == false)
                {
                    // Make sure that pocket checks are listed in alphabetical order
                    pocketChecks = pocketChecks.OrderBy(pc => pc.Name).ToList();
                    return pocketChecks;
                }

                return GetFilteredPocketChecks(sps, pocketChecks); 
            }
        }


        public static void EditPocketCheck(PocketCheckViewModel pcvm)
        {
            using (OpidDailyDB opiddailycontext = new OpidDailyDB())
            {
                PocketCheck pcheck = opiddailycontext.PocketChecks.Where(pc => pc.Id == pcvm.Id).SingleOrDefault();

                if (pcheck != null)
                {
                    pcheck.Notes = pcvm.Notes;
                    opiddailycontext.SaveChanges();
                }
            }
        }

        public static List<PocketCheckViewModel> GetDependentPocketChecks(int id)
        {
            using (OpidDailyDB opiddailycontext = new OpidDailyDB())
            {
                PocketCheck pcheck = opiddailycontext.PocketChecks.Find(id);

                if (pcheck != null)
                { 
                    if (pcheck.HeadOfHousehold == true)
                    {
                        List<PocketCheck> dependentPocketChecks = opiddailycontext.PocketChecks.Where(pc => pc.HH == pcheck.ClientId).ToList();
                        List<PocketCheckViewModel> pcvms = new List<PocketCheckViewModel>();

                        Client client = opiddailycontext.Clients.Find(pcheck.ClientId);

                        if (client != null)
                        {
                            foreach (PocketCheck dependentPocketCheck in dependentPocketChecks)
                            {
                                PocketCheckViewModel pcvm = PocketCheckToPocketCheckViewModel(client, dependentPocketCheck);
                                pcvms.Add(pcvm);
                            }

                            return pcvms;
                        }
                    }

                    return null;
                }

                return null;
            }
        }
    }
}