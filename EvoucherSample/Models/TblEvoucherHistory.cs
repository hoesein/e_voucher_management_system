using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EvoucherSample.Models
{
    public class TblEvoucherHistory
    {
        public int Id { get; set; }
        public int EinfoId { get; set; }
        public string PromoCode { get; set; }
        public string BoughtProduct { get; set; }
        public DateTime Created_At { get; set; } = DateTime.Now;
        public DateTime Updated_At { get; set; } = DateTime.Now;
    }
}
