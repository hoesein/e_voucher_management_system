using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace EvoucherSample.Models
{
    public class TblEvoucherInfo
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime ExpiryDate { get; set; }

        [NotMapped]
        public IFormFile VoucImage { get; set; }
        
        public string VoucImageUrl { get; set; }
        public int Amount { get; set; }
        public int Quantity { get; set; }
        public string QrCode { get; set; }
        public string PaymentCode { get; set; }
        public string BuyingCode { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public Boolean IsUsed { get; set; } = true;
        public Boolean IsActive { get; set; } = true;
        public string MaxUse { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime UpdatedDate { get; set; } = DateTime.Now;
    }
}
