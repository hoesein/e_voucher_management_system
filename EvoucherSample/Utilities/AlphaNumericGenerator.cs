using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EvoucherSample.Utilities
{
    public class AlphaNumericGenerator
    {
        public static string AlphaNumericString()
        {
            // generate 6 digit numeric
            Random rndDigit = new Random();
            string digit = rndDigit.Next(1, 100000).ToString("D6");

            // generate 6 digit number and 5 alphabets string
            Random random = new Random();
            for(var i = 0; i < 5; i++)
            {
                int alphabet = random.Next(65, 91);
                digit += Convert.ToChar(alphabet);
            }
            return digit;
        }
    }
}
