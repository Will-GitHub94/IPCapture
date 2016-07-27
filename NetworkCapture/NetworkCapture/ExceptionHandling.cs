using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkCapture
{
    public class ExceptionHandling
    {
        public string getExceptionMessage(Exception ex)
        {
            return "----- " + ex.Message + " -----";
        }
    }
}
