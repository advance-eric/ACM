using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ACM.Helpers
{
    public class FormatHelper
    {

        public static string GetDateTimeString(DateTime? inputDate)
        {
            if (inputDate.HasValue)
                return inputDate.Value.ToShortDateString() + " " + inputDate.Value.ToShortTimeString();

            return "";
        }
    }
}