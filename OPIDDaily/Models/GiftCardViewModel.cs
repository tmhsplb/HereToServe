using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OPIDDaily.Models
{
    public class GiftCardViewModel
    {
        public int Id { get; set; }

        public string Label { get; set; }

        public int GiftCardType { get; set; }

        public DateTime RegistrationDate { get; set; }

        public string RegistrationID { get; set; }

        public DateTime BalanceDate { get; set; }

        public int CardBalance { get; set; }
    }
}