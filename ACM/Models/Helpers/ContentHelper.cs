using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ACM.Helpers
{
    public static class ContentHelper
    {
        public static string GetContentType(string ext)
        {
            string contentType = "application/octetstream";
            if (!string.IsNullOrWhiteSpace(ext))
            {
                var rk = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey("." + ext.ToLower());
                if (rk == null)
                {
                    if (ext.Contains("/"))
                    {
                        contentType = ext;
                    }
                }
                else
                {
                    object value = rk.GetValue("Content Type");
                    if (value != null)
                    {
                        contentType = value.ToString();
                    }
                }
            }
            return contentType;
        }
    }
}