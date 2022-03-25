using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace EvoucherSample.Utilities
{
    public static class BitmapExtension
    {
        public static byte[] BitmapToByteArray(this Bitmap bitMap)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                bitMap.Save(ms, ImageFormat.Png);
                return ms.ToArray();
            }
        }
    }
}
