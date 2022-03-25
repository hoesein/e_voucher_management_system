using EvoucherSample.DataService;
using EvoucherSample.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EvoucherSample
{
    public class DbInitilizer
    {
        public static void Initilizer(BackendDbContext context)
        {
            context.Database.EnsureCreated();

            if (context.tblPaymentType.Any() || context.tblBuyingType.Any())
            {
                return;
            }

            List<TblPaymentType> pTypes = new List<TblPaymentType>
            {
                new TblPaymentType{ PaymentCode = "mpu", PaymentType = "MPU"},
                new TblPaymentType{ PaymentCode = "master", PaymentType = "Master"},
                new TblPaymentType{ PaymentCode = "visa", PaymentType = "VISA"}
            };

            pTypes.ForEach( p => context.tblPaymentType.Add(p));
            context.SaveChanges();

            List<TblBuyingType> bTypes = new List<TblBuyingType>
            {
                new TblBuyingType{ BuyingCode = "001", BuyingType = "For Personal User"},
                new TblBuyingType{ BuyingCode = "002", BuyingType = "For Gift"},
            };

            bTypes.ForEach(x => context.tblBuyingType.Add(x));

            context.SaveChanges();
        }
    }
}
