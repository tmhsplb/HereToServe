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
        public string Owner { get; set; }
    }
}