using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OPIDDaily.Models
{
    public class GiftCardInventoryViewModel
    {
        [Display(Name = "METRO Funds")]
        public int METROFunds { get; set; }

        [Display(Name = "VISA Funds")]
        public int VisaFunds { get; set; }

        [Display(Name = "METRO Budget")]
        public int METROBudget { get; set; }

        [Display(Name = "VISA Budget")]
        public int VisaBudget { get; set; }

        public string METROCard { get; set; }

        [Display(Name = "$20 METRO Card")]
        public int METROCards20 { get; set; }
               
        [Display(Name = "$30 METRO Card")]
        public int METROCards30 { get; set; }

        [Display(Name = "$40 METRO Card")]
        public int METROCards40 { get; set; }

        [Display(Name = "$50 METRO Card")]
        public int METROCards50 { get; set; }

        public string VisaCard { get; set; }

        [Display(Name = "$20 VISA Card")]
        public int VisaCards20 { get; set; }

        [Display(Name = "$30 VISA Card")]
        public int VisaCards30 { get; set; }

        [Display(Name = "$40 VISA Card")]
        public int VisaCards40 { get; set; }

        [Display(Name = "$50 VISA Card")]
        public int VisaCards50 { get; set; }

        public string AgencyId { get; set; }

        public SelectList Agencies { get; set; }
    }
}