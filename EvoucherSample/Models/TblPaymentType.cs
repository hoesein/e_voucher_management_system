using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EvoucherSample.Models
{
    public class TblPaymentType
    {
        public int Id { get; set; }
        public string PaymentCode { get; set; }
        public string PaymentType { get; set; }
        public DateTime Created_At { get; set; } = DateTime.Now;
        public DateTime Updated_At { get; set; } = DateTime.Now;
    }
}
