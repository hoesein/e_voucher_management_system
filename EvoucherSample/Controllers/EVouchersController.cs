using EvoucherSample.DataService;
using EvoucherSample.Models;
using EvoucherSample.Models.EvoucherViewModel;
using EvoucherSample.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QRCoder;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace EvoucherSample.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EVouchersController : ControllerBase
    {
        private BackendDbContext _backendDbContext;

        public EVouchersController(BackendDbContext backendDbContext)
        {
            _backendDbContext = backendDbContext;
        }

        // GET: api/<EVouchersController>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<EvoucherResponseModel>>> GetEvoucher()
        {
            try
            {
                var getAllEvoucher = await _backendDbContext.eVouchers.ToListAsync();

                List<EvoucherResponseModel> eVoucherResponse = new List<EvoucherResponseModel>();

                getAllEvoucher.ForEach(async x =>
                {
                    eVoucherResponse.Add(new EvoucherResponseModel
                    {
                        Id = x.Id,
                        Name = x.Name,
                        Phone = x.Phone,
                        QrCode = x.QrCode,
                        IsUsed = x.IsUsed,
                        IsActive = x.IsActive,
                        EvoucherHistories = await _backendDbContext.eVoucherHistory.Where(h => h.Id.Equals(x.Id)).ToListAsync()
                    });
                });

                return Ok(eVoucherResponse);

            }
            catch (Exception e)
            {

                return NotFound(new
                {
                    status = 500,
                    message = "Server Side Error Occure"
                });
            }
        }

        // GET api/<EVouchersController>/5
        [HttpGet("{phone}")]
        public async Task<ActionResult<EvoucherResponseModel>> GetEvoucher(string phone)
        {
            try
            {
                var getVoucher = await _backendDbContext.eVouchers.Where(x => x.Phone.Equals(phone)).FirstOrDefaultAsync();

                return Ok(new EvoucherResponseModel
                {
                    Id = getVoucher.Id,
                    Name = getVoucher.Name,
                    Phone = getVoucher.Phone,
                    QrCode = getVoucher.QrCode,
                    IsUsed = getVoucher.IsUsed,
                    IsActive = getVoucher.IsActive,
                    EvoucherHistories = await _backendDbContext.eVoucherHistory.Where(h => h.Id.Equals(getVoucher.Id)).ToListAsync()
                });

            }
            catch (Exception e)
            {

                return NotFound(new
                {
                    status = 500,
                    message = "Server Side Error Occure"
                });
            }
        }

        // POST api/<EVouchersController>
        [HttpPost]
        public async Task<ActionResult> PostEVoucher([FromForm] TblEvoucherInfo eVoucher)
        {
            try
            {
                // qr code generate
                QRCodeGenerator QrGenerator = new QRCodeGenerator();

                // store image
                var fileName = ContentDispositionHeaderValue.Parse(eVoucher.VoucImage.ContentDisposition).FileName.Trim('"');

                for (var i = 0; i < eVoucher.Quantity; i++)
                {
                    var imageUrl = Convert.ToString(eVoucher.Name.Replace(' ', '_').ToLower() + "_" + i + "." + fileName.Split('.').Last());
                    var folderPath = Path.Combine("Resources", imageUrl);

                    using (var stream = new FileStream(folderPath, FileMode.Create, FileAccess.ReadWrite))
                    {
                        await eVoucher.VoucImage.CopyToAsync(stream);
                    }

                    // qr code generate
                    // TODO :  save generated qr code to physical storage

                    QRCodeData QrCodeInfo = QrGenerator.CreateQrCode(eVoucher.Phone, QRCodeGenerator.ECCLevel.Q);
                    QRCode generateQrCode = new QRCode(QrCodeInfo);
                    Bitmap QrBitmap = generateQrCode.GetGraphic(20);
                    byte[] BitmapArray = QrBitmap.BitmapToByteArray();
                    string QrUri = string.Format("data:image/png;base64,{0}", Convert.ToBase64String(BitmapArray));

                    TblEvoucherInfo tblEvoucher = new TblEvoucherInfo
                    {
                        Title = eVoucher.Title,
                        Description = eVoucher.Description,
                        ExpiryDate = eVoucher.ExpiryDate,
                        VoucImageUrl = imageUrl,
                        Amount = eVoucher.Amount,
                        Quantity = eVoucher.Quantity,
                        QrCode = QrUri,
                        PaymentCode = eVoucher.PaymentCode,
                        BuyingCode = eVoucher.BuyingCode,
                        Name = eVoucher.Name,
                        Phone = eVoucher.Phone,
                        MaxUse = eVoucher.MaxUse
                    };

                    await _backendDbContext.eVouchers.AddAsync(tblEvoucher);

                    await _backendDbContext.SaveChangesAsync();

                    await _backendDbContext.eVoucherHistory.AddAsync(new TblEvoucherHistory
                    {
                        EinfoId = tblEvoucher.Id
                    });

                    await _backendDbContext.SaveChangesAsync();
                }

                return Ok(new
                {
                    status = 200,
                    message = "Voucher Created Successfully"
                });
            }
            catch (Exception e)
            {
                return BadRequest(new
                {
                    status = 500,
                    message = e.Message
                });
            }
        }

        // PUT api/<EVouchersController>/5
        [HttpPut("{phone}")]
        public async Task<ActionResult> Put(string phone, [FromBody] TblEvoucherInfo eVoucher)
        {
            try
            {
                if (!string.IsNullOrEmpty(phone)) return NotFound(new { status = 404, message = "No Credential from Client" });

                var getEvoucher = await _backendDbContext.eVouchers.Where(x => x.Phone.Equals(phone)).FirstOrDefaultAsync();

                getEvoucher.Title = eVoucher.Title;
                getEvoucher.Description = eVoucher.Description;
                getEvoucher.Amount = eVoucher.Amount;
                getEvoucher.ExpiryDate = eVoucher.ExpiryDate;
                getEvoucher.IsUsed = eVoucher.IsUsed;
                getEvoucher.IsActive = eVoucher.IsActive;
                getEvoucher.MaxUse = eVoucher.MaxUse;

                _backendDbContext.eVouchers.Update(getEvoucher);
                await _backendDbContext.SaveChangesAsync();

                return Ok(new
                {
                    status = 200,
                    message = "Update Evoucher Successfully",
                });
            }
            catch (Exception e)
            {
                return NotFound(new
                {
                    status = 500,
                    message = "Server Side Error Occure"
                });
            }
        }

        // DELETE api/<EVouchersController>/5
        [HttpDelete("{phone}")]
        public void Delete(string phone)
        {
            // TODO: NEED TO BE DEVELOP DELETE FEATURE
        }
    }
}
