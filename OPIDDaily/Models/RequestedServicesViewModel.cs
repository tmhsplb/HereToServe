﻿using OpidDailyEntities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OPIDDaily.Models
{
    public class RequestedServicesViewModel
    {
        public string XID { get; set; }

        public string XBC { get; set; }

        [Display(Name = "Client will return with requested documents")]
        public bool NeedsDocs { get; set; }

        [Display(Name = "Client has returned with requested documents")]
        public bool HasDocs { get; set; }
 
        public string AgencyId { get; set; }

        public string AgencyName { get; set; }

        public SelectList Agencies { get; set; }

        public string MBVDId { get; set; }

        public SelectList MBVDS { get; set; }

        public bool OtherAgency { get; set; }

        public string OtherAgencyName { get; set; }

        [Display(Name = "Texas BC")]
        public bool BC { get; set; }

        public string BCNotes { get; set; }

        [Display(Name = "Harris County Clerk")]
        public bool HCC { get; set; }

        [Display(Name = "Pre-approved")]
        public bool PreApprovedBC { get; set; }

        [Display(Name = "Out-of-state BC")]
        public bool MBVD { get; set; }

        [Display(Name = "Pre-approved")]
        public bool PreApprovedMBVD { get; set; }

        public string MBVDNotes { get; set; }

        public string State { get; set; }

        public string TIDNotes { get; set; }

        [Display(Name = "New/Renewal ID")]
        public bool NewTID { get; set; }

        [Display(Name = "Pre-approved")]
        public bool PreApprovedNewTID { get; set; }
 

        [Display(Name = "Replacement ID")]
        public bool ReplacementTID { get; set; }

        [Display(Name = "Pre-approved")]
        public bool PreApprovedReplacementTID { get; set; }
        
        [Display(Name = "New/Renewal DL")]
        public bool NewTDL { get; set; }
 
        public string TDLNotes { get; set; }

        [Display(Name = "Replacement DL")]
        public bool ReplacementTDL { get; set; }

        [Display(Name = "Pre-approved")]
        public bool PreApprovedNewTDL { get; set; }

        [Display(Name = "Pre-approved")]
        public bool PreApprovedReplacementTDL { get; set; }

        public bool METROGiftCard { get; set; }

        public int METROGiftCardAmount { get; set; }

        public string METROGiftCardNotes { get; set; }

        public bool VisaGiftCard { get; set; }

        public int VisaGiftCardAmount { get; set; }

        public string VisaGiftCardNotes { get; set; }

        /*
        [Display(Name = "Eligible?")]
        public bool ReplacementTDLEligible { get; set; }
        */

        [Display(Name = "Vitals Check")]
        public bool Numident { get; set; }

        public string RequestedDocument { get; set; }
                
        public bool TrackingOnly { get; set; }

        public string Notes { get; set; }

        // Supporting Documents
        [Display(Name = "Birth Certificate")]
        public bool SDBC { get; set; }

        [Display(Name = "SS Card")]
        public bool SDSSC { get; set; }

        [Display(Name = "Texas ID")]
        public bool SDTID { get; set; }

        [Display(Name = "Texas DL")]
        public bool SDTDL { get; set; }

        [Display(Name = "TDCJ Card")]
        public bool SDTDCJ { get; set; }

        [Display(Name = "Voter's Registration Card")]
        public bool SDVREG { get; set; }

        [Display(Name = "Marriage License")]
        public bool SDML { get; set; }

        [Display(Name = "Divorce Decree")]
        public bool SDDD { get; set; }

        [Display(Name = "Service Letter")]
        public bool SDSL { get; set; }

        [Display(Name = "DD-214")]
        public bool SDDD214 { get; set; }

        [Display(Name = "Gold Card")]
        public bool SDGC { get; set; }

        [Display(Name = "EBT Card")]
        public bool SDEBT { get; set; }

        [Display(Name = "H.O.T. ID")]
        public bool SDHOTID { get; set; }

        [Display(Name = "School Records")]
        public bool SDSchoolRecords { get; set; }

        [Display(Name = "Passport")]
        public bool SDPassport { get; set; }

        [Display(Name = "Job Offer Letter")]
        public bool SDJobOffer { get; set; }

        [Display(Name= "Other")]
        public bool SDOther { get; set; }

        public string SDOthersd { get; set; }
    }
}