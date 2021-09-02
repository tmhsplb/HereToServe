using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OpidDailyEntities 
{
    public class GiftCard
    {
        [Key]
        public int Id { get; set; }

        public int GiftCardType { get; set; }

        public DateTime RegistrationDate { get; set; }

        public string RegistrationID { get; set; }

        public DateTime BalanceDate { get; set; }

        public int CardBalance { get; set; }

        public bool IsCurrent { get; set; }

        public bool IsActive { get; set;}
    }
}