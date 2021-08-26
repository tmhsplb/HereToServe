using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OpidDailyEntities
{
    public class GiftCardInventory
    {
        [Key]
        public int Id { get; set; }
        public DateTime InventoryDate { get; set; }
        public int METROCards20 { get; set; }
        public int METROCards30 { get; set; }
        public int METROCards40 { get; set; }
        public int METROCards50 { get; set; }
        public int VisaCards20 { get; set; }
        public int VisaCards30 { get; set; }
        public int VisaCards40 { get; set; }
        public int VisaCards50 { get; set; }
    }
}