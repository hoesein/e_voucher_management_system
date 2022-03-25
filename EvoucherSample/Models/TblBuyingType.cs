using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EvoucherSample.Models
{
    public class TblBuyingType
    {
        public int Id { get; set; }
        public string BuyingCode { get; set; }
        public string BuyingType { get; set; }
        public DateTime Created_At { get; set; } = DateTime.Now;
        public DateTime Updated_At { get; set; } = DateTime.Now;
    }
}
