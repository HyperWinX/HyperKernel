using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperKernel.Tools
{
    public static class Tools
    {
        public static bool TryParse(string str, out int num)
        {
            num = 0;
            try
            {
                num = Convert.ToInt32(str);
            }
            catch
            {
                return false;
            }
            return true;
        }
    }
}
