using EvoucherSample.DataService;
using EvoucherSample.Utilities;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EvoucherSample.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PromoCodeController : ControllerBase
    {

        private BackendDbContext _backendDbContext;

        public PromoCodeController(BackendDbContext backendDbContext)
        {
            _backendDbContext = backendDbContext;
        }

        [HttpPost("{phone}")]
        public async Task<ActionResult> UseEvoucher(string phone)
        {
            try
            {
                var eVoucher = _backendDbContext.eVouchers.Where(x => x.Phone.Equals(phone)).FirstOrDefault();

                if(Object.ReferenceEquals(null, eVoucher)) return NotFound(new { status = 400, message = "No Voucher" });

                if(Int32.Parse(eVoucher.MaxUse) == 0) return NotFound(new { status = 400, message = "Voucher has been used" });

                eVoucher.MaxUse = (Int32.Parse(eVoucher.MaxUse) - 1).ToString();
                
                var promoCode = AlphaNumericGenerator.AlphaNumericString();

                var duplicatePromoCode = _backendDbContext.eVoucherHistory.Where(x => x.PromoCode == promoCode).Count();

                while(duplicatePromoCode > 0)
                {
                    promoCode = AlphaNumericGenerator.AlphaNumericString();
                    duplicatePromoCode = _backendDbContext.eVoucherHistory.Where(x => x.PromoCode == promoCode).Count();
                }

                var eVoucherHis = _backendDbContext.eVoucherHistory.Where(x => x.EinfoId == eVoucher.Id).FirstOrDefault();

                eVoucherHis.PromoCode = promoCode;

                await _backendDbContext.SaveChangesAsync();

                return Ok(new
                {
                    status = 200,
                    message = "Voucher have been user"
                });

            }
            catch (Exception e)
            {

                return NotFound(new
                {
                    status = 500,
                    message = "Server Side Error Occure. " + e.Message
                });
            }
        }

    }
}
