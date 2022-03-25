using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EvoucherSample.Models.EvoucherViewModel
{
    public class EvoucherResponseModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string QrCode { get; set; }
        public Boolean IsUsed { get; set; }
        public Boolean IsActive { get; set; }
        public List<TblEvoucherHistory> EvoucherHistories { get; set; }
    }
}
