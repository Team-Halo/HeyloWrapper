using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace HeyloWrapper
{
    static class Helper
    {
        public static uint ToArgb(this Color color)
        {
            return BitConverter.ToUInt32(new byte[] { color.B, color.G, color.R, color.A }, 0);
        }
    }
}
